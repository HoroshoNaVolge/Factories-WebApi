﻿// <auto-generated />
using System;
using Factories.WebApi.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Factories.WebApi.DAL.Migrations
{
    [DbContext(typeof(FacilitiesDbContext))]
    [Migration("20240404120700_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Factories.WebApi.DAL.Entities.Factory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Factories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Первый нефтеперерабатывающий завод",
                            Name = "НПЗ№1"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Второй нефтеперерабатывающий завод",
                            Name = "НПЗ№2"
                        });
                });

            modelBuilder.Entity("Factories.WebApi.DAL.Entities.Tank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<double?>("MaxVolume")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UnitId")
                        .HasColumnType("integer");

                    b.Property<double?>("Volume")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("UnitId");

                    b.ToTable("Tanks");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Надземный-вертикальный",
                            MaxVolume = 2000.0,
                            Name = "Резервуар 1",
                            UnitId = 1,
                            Volume = 1500.0
                        },
                        new
                        {
                            Id = 2,
                            Description = "Надземный-горизонтальный",
                            MaxVolume = 3000.0,
                            Name = "Резервуар 2",
                            UnitId = 1,
                            Volume = 2500.0
                        },
                        new
                        {
                            Id = 3,
                            Description = "Надземный-горизонтальный",
                            MaxVolume = 3000.0,
                            Name = "Резервуар 3",
                            UnitId = 2,
                            Volume = 3000.0
                        },
                        new
                        {
                            Id = 4,
                            Description = "Надземный-вертикальный",
                            MaxVolume = 3000.0,
                            Name = "Резервуар 4",
                            UnitId = 2,
                            Volume = 3000.0
                        },
                        new
                        {
                            Id = 5,
                            Description = "Подземный-двустенный",
                            MaxVolume = 5000.0,
                            Name = "Резервуар 5",
                            UnitId = 2,
                            Volume = 4000.0
                        },
                        new
                        {
                            Id = 6,
                            Description = "Подводный",
                            MaxVolume = 500.0,
                            Name = "Резервуар 6",
                            UnitId = 2,
                            Volume = 500.0
                        });
                });

            modelBuilder.Entity("Factories.WebApi.DAL.Entities.Unit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("FactoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("FactoryId");

                    b.ToTable("Units");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Газофракционирующая установка",
                            FactoryId = 1,
                            Name = "ГФУ-2"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Атмосферно-вакуумная трубчатка",
                            FactoryId = 1,
                            Name = "АВТ-6"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Атмосферно - вакуумная трубчатка",
                            FactoryId = 2,
                            Name = "АВТ-10"
                        });
                });

            modelBuilder.Entity("Factories.WebApi.DAL.Entities.Tank", b =>
                {
                    b.HasOne("Factories.WebApi.DAL.Entities.Unit", "Unit")
                        .WithMany()
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("Factories.WebApi.DAL.Entities.Unit", b =>
                {
                    b.HasOne("Factories.WebApi.DAL.Entities.Factory", "Factory")
                        .WithMany()
                        .HasForeignKey("FactoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Factory");
                });
#pragma warning restore 612, 618
        }
    }
}