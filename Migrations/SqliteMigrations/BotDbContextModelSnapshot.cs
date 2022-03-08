﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StickBot.Models;

#nullable disable

namespace StickBot.Migrations.SqliteMigrations
{
    [DbContext(typeof(BotDbContext))]
    partial class BotDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.3");

            modelBuilder.Entity("StickBot.Models.Settings", b =>
                {
                    b.Property<int>("SettingsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("BonkMax")
                        .HasColumnType("INTEGER");

                    b.Property<long>("BonkMin")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("BonkedRole")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("ServerId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("StealBonkChance")
                        .HasColumnType("REAL");

                    b.Property<long>("StealCooldown")
                        .HasColumnType("INTEGER");

                    b.Property<float>("StealSuccessChance")
                        .HasColumnType("REAL");

                    b.Property<ulong>("StickRole")
                        .HasColumnType("INTEGER");

                    b.HasKey("SettingsId");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("StickBot.Models.StealAttempt", b =>
                {
                    b.Property<int>("StealAttemptId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ServerId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("StealAttemptId");

                    b.ToTable("StealAttempts");
                });

            modelBuilder.Entity("StickBot.Models.Stick", b =>
                {
                    b.Property<int>("StickId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("BonkDate")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("HoldingUserId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("ServerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("StickId");

                    b.ToTable("Sticks");
                });
#pragma warning restore 612, 618
        }
    }
}
