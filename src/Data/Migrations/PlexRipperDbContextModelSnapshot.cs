﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace PlexRipper.Data.Migrations
{
    [DbContext(typeof(PlexRipperDbContext))]
    class PlexRipperDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4");

            modelBuilder.Entity("PlexRipper.Domain.Entities.Account", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<string>("DisplayName")
                    .HasColumnType("TEXT");

                b.Property<bool>("IsEnabled")
                    .HasColumnType("INTEGER");

                b.Property<bool>("IsValidated")
                    .HasColumnType("INTEGER");

                b.Property<string>("Password")
                    .HasColumnType("TEXT");

                b.Property<string>("Username")
                    .HasColumnType("TEXT");

                b.Property<DateTime>("ValidatedAt")
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.ToTable("Accounts");
            });

            modelBuilder.Entity("PlexRipper.Domain.Entities.PlexAccount", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<int>("AccountId")
                    .HasColumnType("INTEGER");

                b.Property<string>("AuthToken")
                    .HasColumnType("TEXT");

                b.Property<string>("AuthenticationToken")
                    .HasColumnType("TEXT");

                b.Property<DateTime>("ConfirmedAt")
                    .HasColumnType("TEXT");

                b.Property<string>("Email")
                    .HasColumnType("TEXT");

                b.Property<int>("ForumId")
                    .HasColumnType("INTEGER");

                b.Property<bool>("HasPassword")
                    .HasColumnType("INTEGER");

                b.Property<DateTime>("JoinedAt")
                    .HasColumnType("TEXT");

                b.Property<long>("PlexId")
                    .HasColumnType("INTEGER");

                b.Property<string>("Title")
                    .HasColumnType("TEXT");

                b.Property<string>("Username")
                    .HasColumnType("TEXT");

                b.Property<string>("Uuid")
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.HasIndex("AccountId")
                    .IsUnique();

                b.ToTable("PlexAccounts");
            });

            modelBuilder.Entity("PlexRipper.Domain.Entities.PlexAccountServer", b =>
            {
                b.Property<int>("PlexAccountId")
                    .HasColumnType("INTEGER");

                b.Property<int>("PlexServerId")
                    .HasColumnType("INTEGER");

                b.HasKey("PlexAccountId", "PlexServerId");

                b.HasIndex("PlexServerId");

                b.ToTable("PlexAccountServers");
            });

            modelBuilder.Entity("PlexRipper.Domain.Entities.PlexLibrary", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<int>("Count")
                    .HasColumnType("INTEGER");

                b.Property<bool>("HasAccess")
                    .HasColumnType("INTEGER");

                b.Property<string>("Key")
                    .HasColumnType("TEXT");

                b.Property<int>("PlexServerId")
                    .HasColumnType("INTEGER");

                b.Property<int>("SectionId")
                    .HasColumnType("INTEGER");

                b.Property<string>("Title")
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.HasIndex("PlexServerId");

                b.ToTable("PlexLibraries");
            });

            modelBuilder.Entity("PlexRipper.Domain.Entities.PlexServer", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<string>("AccessToken")
                    .HasColumnType("TEXT");

                b.Property<string>("Address")
                    .HasColumnType("TEXT");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("TEXT");

                b.Property<bool>("Home")
                    .HasColumnType("INTEGER");

                b.Property<string>("Host")
                    .HasColumnType("TEXT");

                b.Property<string>("LocalAddresses")
                    .HasColumnType("TEXT");

                b.Property<string>("MachineIdentifier")
                    .HasColumnType("TEXT");

                b.Property<string>("Name")
                    .HasColumnType("TEXT");

                b.Property<bool>("Owned")
                    .HasColumnType("INTEGER");

                b.Property<long>("OwnerId")
                    .HasColumnType("INTEGER");

                b.Property<int>("Port")
                    .HasColumnType("INTEGER");

                b.Property<string>("Scheme")
                    .HasColumnType("TEXT");

                b.Property<string>("SourceTitle")
                    .HasColumnType("TEXT");

                b.Property<bool>("Synced")
                    .HasColumnType("INTEGER");

                b.Property<DateTime>("UpdatedAt")
                    .HasColumnType("TEXT");

                b.Property<string>("Version")
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.ToTable("PlexServers");
            });

            modelBuilder.Entity("PlexRipper.Domain.Entities.PlexAccount", b =>
            {
                b.HasOne("PlexRipper.Domain.Entities.Account", "Account")
                    .WithOne("PlexAccount")
                    .HasForeignKey("PlexRipper.Domain.Entities.PlexAccount", "AccountId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("PlexRipper.Domain.Entities.PlexAccountServer", b =>
            {
                b.HasOne("PlexRipper.Domain.Entities.PlexAccount", "PlexAccount")
                    .WithMany("PlexAccountServers")
                    .HasForeignKey("PlexAccountId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("PlexRipper.Domain.Entities.PlexServer", "PlexServer")
                    .WithMany("PlexAccountServers")
                    .HasForeignKey("PlexServerId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("PlexRipper.Domain.Entities.PlexLibrary", b =>
            {
                b.HasOne("PlexRipper.Domain.Entities.PlexServer", "PlexServer")
                    .WithMany("PlexLibraries")
                    .HasForeignKey("PlexServerId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
#pragma warning restore 612, 618
        }
    }
}