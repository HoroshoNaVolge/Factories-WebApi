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
using Factories.WebApi.BLL.Database;
using System.Security.Claims;

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

            var jwtOptionsSection = builder.Configuration.GetSection(JwtOptions.SectionName);

            var checkForValidJwtOptions = jwtOptionsSection.Get<JwtOptions>() ?? throw new InvalidOperationException($"Config section {jwtOptionsSection} must be set up");

            builder.Services.Configure<JwtOptions>(jwtOptionsSection);

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

            await EnsureDefaultAdminCreated(app);

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
        
        //Потому-что в OnModelCreating не добавить роль и клеймы через UserManager асинхронно
        private static async Task EnsureDefaultAdminCreated(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<UsersDbContext>();

            if (!dbContext.Database.CanConnect())
                throw new DbConnectException("Не удалось подключиться к базе данных, проверьте наличие базы данных и строку подключения");

            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            
            //Потому что UserName может измениться позже
            if (await userManager.FindByIdAsync("fe342990-c53a-4bb9-89b6-4b4482e956fb") == null)
            {
                var user = new IdentityUser
                {
                    Id = "fe342990-c53a-4bb9-89b6-4b4482e956fb",
                    UserName = "Admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    // Хэш соответствует паролю P@ssw0rd
                    string passwordHash = "AQAAAAIAAYagAAAAENHAMmgih8HUHvasMFLvvPqwmV/eEMdj8+d8hvvQ79SiWNGomApGcJe65AHTWwUFRQ==";

                    user.PasswordHash = passwordHash;

                    await userManager.UpdateAsync(user);

                    await userManager.AddToRoleAsync(user, "Admin");

                    await userManager.AddClaimAsync(user, new Claim("UnitOperator", "true"));
                    await userManager.AddClaimAsync(user, new Claim("TankOperator", "true"));
                }
            }
        }
    }
}
