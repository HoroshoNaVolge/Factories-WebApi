using Microsoft.EntityFrameworkCore;
using Factories.WebApi.DAL.EF;
using Factories.WebApi.DAL.Interfaces;
using Factories.WebApi.DAL.Repositories;
using Serilog;
using Factories.WebApi.BLL.Services;
using Factories.WebApi.BLL.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.BLL.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Factories.WebApi.BLL.Database;

namespace Factories.WebApi.BLL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                             .AddEntityFrameworkStores<UsersDbContext>()
                             .AddDefaultTokenProviders();

            builder.Services.AddScoped<RoleManager<IdentityRole>>();

            var jwtOptionsSectionName = builder.Configuration.GetSection(JwtOptions.SectionName);
            var seedDataOptionsSectionName = builder.Configuration.GetSection(SeedDataOptions.SectionName);

            var checkForValidJwtOptions = jwtOptionsSectionName.Get<JwtOptions>() ?? throw new InvalidOperationException(nameof(JwtOptions));

            builder.Services.Configure<JwtOptions>(jwtOptionsSectionName);

            var checkForValidSeedDataOptions = seedDataOptionsSectionName.Get<SeedDataOptions>() ?? throw new InvalidOperationException(nameof(SeedDataOptions));
            builder.Services.Configure<SeedDataOptions>(seedDataOptionsSectionName);

            builder.Services.AddScoped<IJwtService, JwtService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
                };
            });

            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("AdminOrUnitOperatorPolicy", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        return context.User.IsInRole("Admin") || context.User.HasClaim(c => c.Type == "UnitOperator");
                    });
                })
                .AddPolicy("AdminOrTankOperatorPolicy", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        return context.User.IsInRole("Admin") || context.User.HasClaim(c => c.Type == "TankOperator");
                    });
                });

            builder.Services.AddDbContext<FacilitiesDbContext>(options =>
                          options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddDbContext<UsersDbContext>(options =>
                                      options.UseNpgsql(builder.Configuration.GetConnectionString("UsersConnection")));

            builder.Services.AddScoped<IRepository<Tank>, TankRepository>()
                             .AddScoped<IRepository<Unit>, UnitRepository>()
                             .AddScoped<IRepository<Factory>, FactoryRepository>();

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddSingleton<IRandomService, RandomService>();

            builder.Services.AddHostedService<WorkerService>();

            Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
             .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning).WriteTo.Console()
             .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
             .CreateLogger();

            builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            await EnsureDatabaseCreatedAsync(app);

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }

        // Для автоматической миграции БД при её отсутствии при запуске приложения. Добавляем админа с ролью и клеймами 
        private static async Task EnsureDatabaseCreatedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<UsersDbContext>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var options = services.GetRequiredService<IOptions<SeedDataOptions>>();

            await dbContext.Database.MigrateAsync();

            await UsersDbInitializer.SeedData(userManager, roleManager, options);
        }
    }
}
