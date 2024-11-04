﻿// <auto-generated />
using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241031175100_SeedSpecialities")]
    partial class SeedSpecialities
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Comment", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("author_id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("consultation_id")
                        .HasColumnType("uuid");

                    b.Property<string>("content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<DateTime>("modifiedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("modified_date");

                    b.Property<Guid?>("parent_id")
                        .HasColumnType("uuid");

                    b.HasKey("id");

                    b.HasIndex("author_id");

                    b.HasIndex("consultation_id");

                    b.HasIndex("parent_id");

                    b.ToTable("comment", (string)null);
                });

            modelBuilder.Entity("Domain.Consultation", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<Guid>("inspection_id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("speciality_id")
                        .HasColumnType("uuid");

                    b.HasKey("id");

                    b.HasIndex("inspection_id");

                    b.HasIndex("speciality_id");

                    b.ToTable("consultation", (string)null);
                });

            modelBuilder.Entity("Domain.Diagnosis", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<int>("diagnosisType")
                        .HasColumnType("integer")
                        .HasColumnName("diagnosis_type ");

                    b.Property<Guid>("icd_diagnosis_id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("inspection_id")
                        .HasColumnType("uuid");

                    b.HasKey("id");

                    b.HasIndex("icd_diagnosis_id");

                    b.HasIndex("inspection_id");

                    b.ToTable("diagnosis", (string)null);
                });

            modelBuilder.Entity("Domain.Doctor", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("birthday")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("birthday");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<int>("gender")
                        .HasColumnType("integer")
                        .HasColumnName("gender");

                    b.Property<string>("hashedPassword")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("hashed_password");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("phone")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("phone");

                    b.Property<Guid>("speciality_id")
                        .HasColumnType("uuid");

                    b.HasKey("id");

                    b.HasIndex("email")
                        .IsUnique();

                    b.HasIndex("phone")
                        .IsUnique();

                    b.HasIndex("speciality_id");

                    b.ToTable("doctor", (string)null);
                });

            modelBuilder.Entity("Domain.Icd", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("createTime");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<Guid?>("parentId")
                        .HasColumnType("uuid");

                    b.Property<string>("сode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.HasKey("id");

                    b.HasIndex("parentId");

                    b.ToTable("ICD_10", (string)null);
                });

            modelBuilder.Entity("Domain.Inspection", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("anamnesis")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("anamnesis");

                    b.Property<string>("complaints")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("complaints");

                    b.Property<int>("conclusion")
                        .HasColumnType("integer")
                        .HasColumnName("conclustion");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<DateTime>("date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<DateTime?>("deathDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("death_date");

                    b.Property<Guid>("doctor_id")
                        .HasColumnType("uuid");

                    b.Property<bool>("isNotified")
                        .HasColumnType("boolean")
                        .HasColumnName("is_notified");

                    b.Property<DateTime?>("nextVisitDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("next_visit_date");

                    b.Property<Guid>("patient_id)")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("previous_inspection_id")
                        .HasColumnType("uuid");

                    b.Property<string>("treatment")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("treatment");

                    b.HasKey("id");

                    b.HasIndex("doctor_id");

                    b.HasIndex("patient_id)");

                    b.HasIndex("previous_inspection_id");

                    b.ToTable("inspection", (string)null);
                });

            modelBuilder.Entity("Domain.Patient", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("birtday")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("birthday");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<int>("gender")
                        .HasColumnType("integer")
                        .HasColumnName("gender");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("id");

                    b.ToTable("patient", (string)null);
                });

            modelBuilder.Entity("Domain.Speciality", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("createTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("create_time");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("id");

                    b.ToTable("speciality", (string)null);
                });

            modelBuilder.Entity("Domain.Token", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("tokenValue")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("token_value");

                    b.HasKey("id");

                    b.HasIndex("id")
                        .IsUnique();

                    b.HasIndex("tokenValue")
                        .IsUnique();

                    b.ToTable("banned_tokens", (string)null);
                });

            modelBuilder.Entity("Domain.Comment", b =>
                {
                    b.HasOne("Domain.Doctor", "author")
                        .WithMany()
                        .HasForeignKey("author_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Consultation", "consultation")
                        .WithMany()
                        .HasForeignKey("consultation_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Comment", "parent")
                        .WithMany()
                        .HasForeignKey("parent_id");

                    b.Navigation("author");

                    b.Navigation("consultation");

                    b.Navigation("parent");
                });

            modelBuilder.Entity("Domain.Consultation", b =>
                {
                    b.HasOne("Domain.Inspection", "inspection")
                        .WithMany()
                        .HasForeignKey("inspection_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Speciality", "speciality")
                        .WithMany()
                        .HasForeignKey("speciality_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("inspection");

                    b.Navigation("speciality");
                });

            modelBuilder.Entity("Domain.Diagnosis", b =>
                {
                    b.HasOne("Domain.Icd", "icd")
                        .WithMany()
                        .HasForeignKey("icd_diagnosis_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Inspection", "inspection")
                        .WithMany()
                        .HasForeignKey("inspection_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("icd");

                    b.Navigation("inspection");
                });

            modelBuilder.Entity("Domain.Doctor", b =>
                {
                    b.HasOne("Domain.Speciality", "speciality")
                        .WithMany()
                        .HasForeignKey("speciality_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("speciality");
                });

            modelBuilder.Entity("Domain.Icd", b =>
                {
                    b.HasOne("Domain.Icd", "parent")
                        .WithMany()
                        .HasForeignKey("parentId");

                    b.Navigation("parent");
                });

            modelBuilder.Entity("Domain.Inspection", b =>
                {
                    b.HasOne("Domain.Doctor", "doctor")
                        .WithMany()
                        .HasForeignKey("doctor_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Patient", "patient")
                        .WithMany()
                        .HasForeignKey("patient_id)")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Inspection", "previousInspection")
                        .WithMany()
                        .HasForeignKey("previous_inspection_id");

                    b.Navigation("doctor");

                    b.Navigation("patient");

                    b.Navigation("previousInspection");
                });
#pragma warning restore 612, 618
        }
    }
}
