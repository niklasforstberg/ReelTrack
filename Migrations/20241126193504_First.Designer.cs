﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReelTrack.Helpers;

#nullable disable

namespace ReelTrack.Migrations
{
    [DbContext(typeof(ReelTrackDbContext))]
    [Migration("20241126193504_First")]
    partial class First
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("ReelTrack.Models.Family", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("InviteCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Families");
                });

            modelBuilder.Entity("ReelTrack.Models.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImdbId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("ReelTrack.Models.MovieWatch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChoosenById")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MovieId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WatchListId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("WatchedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ChoosenById");

                    b.HasIndex("MovieId");

                    b.HasIndex("WatchListId");

                    b.ToTable("MovieWatches");
                });

            modelBuilder.Entity("ReelTrack.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("FamilyId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FamilyId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ReelTrack.Models.WatchList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("FamilyId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FamilyId");

                    b.ToTable("WatchLists");
                });

            modelBuilder.Entity("ReelTrack.Models.WatchOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WatchListId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("WatchListId");

                    b.ToTable("WatchOrders");
                });

            modelBuilder.Entity("UserWatchList", b =>
                {
                    b.Property<int>("MembersId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WatchListsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("MembersId", "WatchListsId");

                    b.HasIndex("WatchListsId");

                    b.ToTable("UserWatchList");
                });

            modelBuilder.Entity("ReelTrack.Models.MovieWatch", b =>
                {
                    b.HasOne("ReelTrack.Models.User", "ChoosenBy")
                        .WithMany()
                        .HasForeignKey("ChoosenById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ReelTrack.Models.Movie", "Movie")
                        .WithMany("Watches")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ReelTrack.Models.WatchList", "WatchList")
                        .WithMany("MovieWatches")
                        .HasForeignKey("WatchListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChoosenBy");

                    b.Navigation("Movie");

                    b.Navigation("WatchList");
                });

            modelBuilder.Entity("ReelTrack.Models.User", b =>
                {
                    b.HasOne("ReelTrack.Models.Family", "Family")
                        .WithMany("Members")
                        .HasForeignKey("FamilyId");

                    b.Navigation("Family");
                });

            modelBuilder.Entity("ReelTrack.Models.WatchList", b =>
                {
                    b.HasOne("ReelTrack.Models.Family", "Family")
                        .WithMany("WatchLists")
                        .HasForeignKey("FamilyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Family");
                });

            modelBuilder.Entity("ReelTrack.Models.WatchOrder", b =>
                {
                    b.HasOne("ReelTrack.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ReelTrack.Models.WatchList", "WatchList")
                        .WithMany("WatchOrder")
                        .HasForeignKey("WatchListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("WatchList");
                });

            modelBuilder.Entity("UserWatchList", b =>
                {
                    b.HasOne("ReelTrack.Models.User", null)
                        .WithMany()
                        .HasForeignKey("MembersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ReelTrack.Models.WatchList", null)
                        .WithMany()
                        .HasForeignKey("WatchListsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ReelTrack.Models.Family", b =>
                {
                    b.Navigation("Members");

                    b.Navigation("WatchLists");
                });

            modelBuilder.Entity("ReelTrack.Models.Movie", b =>
                {
                    b.Navigation("Watches");
                });

            modelBuilder.Entity("ReelTrack.Models.WatchList", b =>
                {
                    b.Navigation("MovieWatches");

                    b.Navigation("WatchOrder");
                });
#pragma warning restore 612, 618
        }
    }
}
