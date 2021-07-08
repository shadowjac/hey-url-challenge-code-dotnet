﻿// <auto-generated />
using System;
using HeyUrlChallengeCodeDotnet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace hey_url_challenge_code_dotnet.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20210707222042_AddingHistorical")]
    partial class AddingHistorical
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("hey_url_challenge_code_dotnet.Models.Historical", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BrowserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("OS")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid?>("UrlId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UrlId");

                    b.ToTable("Historicals");
                });

            modelBuilder.Entity("hey_url_challenge_code_dotnet.Models.Url", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("ShortUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Urls");
                });

            modelBuilder.Entity("hey_url_challenge_code_dotnet.Models.Historical", b =>
                {
                    b.HasOne("hey_url_challenge_code_dotnet.Models.Url", null)
                        .WithMany("Historical")
                        .HasForeignKey("UrlId");
                });

            modelBuilder.Entity("hey_url_challenge_code_dotnet.Models.Url", b =>
                {
                    b.Navigation("Historical");
                });
#pragma warning restore 612, 618
        }
    }
}
