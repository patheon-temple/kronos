﻿// <auto-generated />
using System;
using Kronos.WebAPI.Athena.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Kronos.WebAPI.Migrations
{
    [DbContext(typeof(AthenaDbContext))]
    [Migration("20241031084550_Scopes")]
    partial class Scopes
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("athena")
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Kronos.WebAPI.Athena.Data.ScopeDataModel", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Description")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasKey("Id")
                        .HasName("PK_scope_id");

                    b.ToTable("scopes", "athena");
                });

            modelBuilder.Entity("Kronos.WebAPI.Athena.Data.ServiceAccountDataModel", b =>
                {
                    b.Property<Guid>("ServiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("service_id");

                    b.Property<byte[]>("Secret")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("bytea")
                        .HasColumnName("secret");

                    b.HasKey("ServiceId")
                        .HasName("PK_service_account_id");

                    b.ToTable("service_accounts", "athena");
                });

            modelBuilder.Entity("Kronos.WebAPI.Athena.Data.UserAccountDataModel", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<string>("DeviceId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("device_id");

                    b.Property<byte[]>("PasswordHash")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bytea")
                        .HasColumnName("password_hash");

                    b.Property<string>("Username")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)")
                        .HasColumnName("username");

                    b.HasKey("UserId")
                        .HasName("PK_user_account_id");

                    b.ToTable("user_accounts", "athena");
                });

            modelBuilder.Entity("Kronos.WebAPI.Athena.Data.UserScopeDataModel", b =>
                {
                    b.Property<string>("ScopesId")
                        .HasColumnType("character varying(128)");

                    b.Property<Guid>("UserAccountsUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("ScopeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("ScopesId", "UserAccountsUserId");

                    b.HasIndex("UserAccountsUserId");

                    b.ToTable("users_scopes", "athena");
                });

            modelBuilder.Entity("Kronos.WebAPI.Athena.Data.UserScopeDataModel", b =>
                {
                    b.HasOne("Kronos.WebAPI.Athena.Data.ScopeDataModel", null)
                        .WithMany()
                        .HasForeignKey("ScopesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kronos.WebAPI.Athena.Data.UserAccountDataModel", null)
                        .WithMany()
                        .HasForeignKey("UserAccountsUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
