﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Project.Tech.Shop.Services.UsersAccounts;

#nullable disable

namespace Project.Tech.Shop.Services.UsersAccounts.Migrations
{
    [DbContext(typeof(UserAccountsContext))]
    [Migration("20240529092422_AddRowVersion")]
    partial class AddRowVersion
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("UserAccounts")
                .HasAnnotation("ProductVersion", "7.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Project.Tech.Shop.Services.UsersAccounts.Entities.AccountActivity", b =>
                {
                    b.Property<Guid>("ActivityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ActivityType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("ActivityId");

                    b.HasIndex("UserId");

                    b.ToTable("AccountActivities", "UserAccounts");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.UsersAccounts.Entities.Address", b =>
                {
                    b.Property<Guid>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AddressLine")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("County")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("IsBillingAddress")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsShippingAddress")
                        .HasColumnType("boolean");

                    b.Property<string>("PostCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("AddressId");

                    b.HasIndex("UserId");

                    b.ToTable("Addresses", "UserAccounts");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.UsersAccounts.Entities.SecuritySettings", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("SecurityAnswerHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SecurityQuestion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.HasKey("UserId");

                    b.ToTable("SecuritySettings", "UserAccounts");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.UsersAccounts.Entities.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users", "UserAccounts");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.UsersAccounts.Entities.UserPreferences", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("PreferredPaymentMethod")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("ReceiveNewsletter")
                        .HasColumnType("boolean");

                    b.HasKey("UserId");

                    b.ToTable("UserPreferences", "UserAccounts");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.UsersAccounts.Entities.AccountActivity", b =>
                {
                    b.HasOne("Project.Tech.Shop.Services.UsersAccounts.Entities.User", "User")
                        .WithMany("Activities")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.UsersAccounts.Entities.Address", b =>
                {
                    b.HasOne("Project.Tech.Shop.Services.UsersAccounts.Entities.User", "User")
                        .WithMany("Addresses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.UsersAccounts.Entities.SecuritySettings", b =>
                {
                    b.HasOne("Project.Tech.Shop.Services.UsersAccounts.Entities.User", "User")
                        .WithOne("SecuritySettings")
                        .HasForeignKey("Project.Tech.Shop.Services.UsersAccounts.Entities.SecuritySettings", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.UsersAccounts.Entities.UserPreferences", b =>
                {
                    b.HasOne("Project.Tech.Shop.Services.UsersAccounts.Entities.User", "User")
                        .WithOne("Preferences")
                        .HasForeignKey("Project.Tech.Shop.Services.UsersAccounts.Entities.UserPreferences", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.UsersAccounts.Entities.User", b =>
                {
                    b.Navigation("Activities");

                    b.Navigation("Addresses");

                    b.Navigation("Preferences")
                        .IsRequired();

                    b.Navigation("SecuritySettings")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
