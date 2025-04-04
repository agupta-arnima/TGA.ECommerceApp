﻿// <auto-generated />
using Capstone.ECommerceApp.Product.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Capstone.ECommerceApp.Product.Data.Migrations
{
    [DbContext(typeof(ProductDbContext))]
    [Migration("20250404122057_AddProductToDB")]
    partial class AddProductToDB
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Capstone.ECommerceApp.Product.Domain.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Fresh fruits",
                            Name = "Fruit"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Fresh vegetables",
                            Name = "Vegetable"
                        });
                });

            modelBuilder.Entity("Capstone.ECommerceApp.Product.Domain.Models.ProductInfo", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<double>("Price")
                        .HasColumnType("double");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<int>("SupplierId")
                        .HasColumnType("int");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("SupplierId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            CategoryId = 1,
                            Description = "Quisque vel lacus ac magna, vehicula sagittis ut non lacus.",
                            ImageUrl = "https://placehold.co/603x403",
                            Name = "Apple",
                            Price = 15.0,
                            Stock = 100,
                            SupplierId = 1
                        },
                        new
                        {
                            ProductId = 2,
                            CategoryId = 1,
                            Description = "Quisque vel lacus ac magna, vehicula sagittis ut non lacus.",
                            ImageUrl = "https://placehold.co/602x402",
                            Name = "Orange",
                            Price = 13.99,
                            Stock = 150,
                            SupplierId = 2
                        },
                        new
                        {
                            ProductId = 3,
                            CategoryId = 1,
                            Description = "Quisque vel lacus ac magna, vehicula sagittis ut non lacus.",
                            ImageUrl = "https://placehold.co/601x401",
                            Name = "Banana",
                            Price = 10.99,
                            Stock = 200,
                            SupplierId = 1
                        },
                        new
                        {
                            ProductId = 4,
                            CategoryId = 1,
                            Description = "Quisque vel lacus ac magna, vehicula sagittis ut non lacus.",
                            ImageUrl = "https://placehold.co/600x400",
                            Name = "Pineapple",
                            Price = 15.0,
                            Stock = 50,
                            SupplierId = 2
                        });
                });

            modelBuilder.Entity("Capstone.ECommerceApp.Product.Domain.Models.Supplier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ContactNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Suppliers");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Address = "123 Street, City",
                            ContactNumber = "1234567890",
                            Email = "supplierA@example.com",
                            Name = "Supplier A"
                        },
                        new
                        {
                            Id = 2,
                            Address = "456 Avenue, City",
                            ContactNumber = "0987654321",
                            Email = "supplierB@example.com",
                            Name = "Supplier B"
                        });
                });

            modelBuilder.Entity("Capstone.ECommerceApp.Product.Domain.Models.ProductInfo", b =>
                {
                    b.HasOne("Capstone.ECommerceApp.Product.Domain.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Capstone.ECommerceApp.Product.Domain.Models.Supplier", "Supplier")
                        .WithMany("Products")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("Capstone.ECommerceApp.Product.Domain.Models.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Capstone.ECommerceApp.Product.Domain.Models.Supplier", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
