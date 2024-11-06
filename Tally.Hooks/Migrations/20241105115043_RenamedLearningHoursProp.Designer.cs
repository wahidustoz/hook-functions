﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tally.Hooks.Data;

#nullable disable

namespace Tally.Hooks.Migrations
{
    [DbContext(typeof(FunctionsDbContext))]
    [Migration("20241105115043_RenamedLearningHoursProp")]
    partial class RenamedLearningHoursProp
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Tally.Hooks.Entities.AptitudeResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CategoryScores")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<int>("Grade")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Timestamp");

                    b.ToTable("AptitudeResults");
                });

            modelBuilder.Entity("Tally.Hooks.Entities.Certificate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("Holder")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Number")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasMaxLength(5)
                        .HasColumnType("character varying(5)")
                        .HasComputedColumnSql("substring(md5(\"Id\"::text) from 0 for 6)", true);

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Number");

                    b.ToTable("Certificates", (string)null);

                    b.HasDiscriminator<int>("Type");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Tally.Hooks.Entities.ProfessionalEnhancementCertificate", b =>
                {
                    b.HasBaseType("Tally.Hooks.Entities.Certificate");

                    b.Property<DateOnly>("From")
                        .HasColumnType("date");

                    b.Property<int>("LearningHours")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("To")
                        .HasColumnType("date");

                    b.HasDiscriminator().HasValue(0);
                });
#pragma warning restore 612, 618
        }
    }
}
