﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Project.Tech.Shop.Services.Products;

#nullable disable

namespace Project.Tech.Shop.Services.Products.Migrations
{
    [DbContext(typeof(ProductsContext))]
    [Migration("20240507130419_missingstuff")]
    partial class missingstuff
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Products")
                .HasAnnotation("ProductVersion", "7.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Project.Tech.Shop.Services.Products.Enitites.Basket", b =>
                {
                    b.Property<Guid>("BasketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("BasketId");

                    b.ToTable("Baskets", "Products");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.Products.Enitites.BasketItem", b =>
                {
                    b.Property<int>("BasketItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BasketItemId"));

                    b.Property<Guid>("BasketId")
                        .HasColumnType("uuid");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("BasketItemId");

                    b.HasIndex("BasketId");

                    b.HasIndex("ProductId");

                    b.ToTable("BasketItems", "Products");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.Products.Enitites.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ProductId"));

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Category")
                        .HasColumnType("integer");

                    b.Property<int>("Condition")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("GraphicsCard")
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("ProcessorType")
                        .HasColumnType("text");

                    b.Property<int?>("RAM")
                        .HasColumnType("integer");

                    b.Property<double?>("ScreenSize")
                        .HasColumnType("double precision");

                    b.Property<string>("Series")
                        .HasColumnType("text");

                    b.Property<int>("StockLevel")
                        .HasColumnType("integer");

                    b.Property<int?>("Storage")
                        .HasColumnType("integer");

                    b.Property<string>("StorageType")
                        .HasColumnType("text");

                    b.Property<bool?>("TouchScreen")
                        .HasColumnType("boolean");

                    b.HasKey("ProductId");

                    b.HasIndex("Category")
                        .HasDatabaseName("IX_Products_Category");

                    b.ToTable("Products", "Products");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.Products.Enitites.Sale", b =>
                {
                    b.Property<int>("SaleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SaleId"));

                    b.Property<Guid>("BasketId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<int?>("ProductId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("SaleDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<decimal>("TotalSaleAmount")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("SaleId");

                    b.HasIndex("BasketId")
                        .IsUnique();

                    b.HasIndex("ProductId");

                    b.ToTable("Sales", "Products");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.Products.Enitites.BasketItem", b =>
                {
                    b.HasOne("Project.Tech.Shop.Services.Products.Enitites.Basket", "Basket")
                        .WithMany("Items")
                        .HasForeignKey("BasketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Project.Tech.Shop.Services.Products.Enitites.Product", "Product")
                        .WithMany("BasketItems")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Basket");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.Products.Enitites.Sale", b =>
                {
                    b.HasOne("Project.Tech.Shop.Services.Products.Enitites.Basket", "Basket")
                        .WithOne("Sale")
                        .HasForeignKey("Project.Tech.Shop.Services.Products.Enitites.Sale", "BasketId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Project.Tech.Shop.Services.Products.Enitites.Product", null)
                        .WithMany("Sales")
                        .HasForeignKey("ProductId");

                    b.Navigation("Basket");
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.Products.Enitites.Basket", b =>
                {
                    b.Navigation("Items");

                    b.Navigation("Sale")
                        .IsRequired();
                });

            modelBuilder.Entity("Project.Tech.Shop.Services.Products.Enitites.Product", b =>
                {
                    b.Navigation("BasketItems");

                    b.Navigation("Sales");
                });
#pragma warning restore 612, 618
        }
    }
}
