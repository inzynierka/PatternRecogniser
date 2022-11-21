﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PatternRecogniser.Models;

namespace PatternRecogniser.Migrations
{
    [DbContext(typeof(PatternRecogniserDBContext))]
    partial class PatternRecogniserDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("ExperimentExperimentList", b =>
                {
                    b.Property<int>("experimentListsexperimentListId")
                        .HasColumnType("integer");

                    b.Property<int>("experimentsexperimentId")
                        .HasColumnType("integer");

                    b.HasKey("experimentListsexperimentListId", "experimentsexperimentId");

                    b.HasIndex("experimentsexperimentId");

                    b.ToTable("ExperimentExperimentList");
                });

            modelBuilder.Entity("PatternRecogniser.Models.Authentication", b =>
                {
                    b.Property<int>("userId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("hashedToken")
                        .HasColumnType("text");

                    b.Property<string>("lastSeed")
                        .HasColumnType("text");

                    b.Property<int?>("userId1")
                        .HasColumnType("integer");

                    b.HasKey("userId");

                    b.HasIndex("userId1");

                    b.ToTable("authentication");
                });

            modelBuilder.Entity("PatternRecogniser.Models.Experiment", b =>
                {
                    b.Property<int>("experimentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("extendedModelId")
                        .HasColumnType("integer");

                    b.HasKey("experimentId");

                    b.ToTable("experiment");
                });

            modelBuilder.Entity("PatternRecogniser.Models.ExperimentList", b =>
                {
                    b.Property<int>("experimentListId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("experimentType")
                        .HasColumnType("text");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<int>("userId")
                        .HasColumnType("integer");

                    b.HasKey("experimentListId");

                    b.HasIndex("userId", "name")
                        .IsUnique();

                    b.ToTable("experimentList");
                });

            modelBuilder.Entity("PatternRecogniser.Models.ExtendedModel", b =>
                {
                    b.Property<int>("extendedModelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("distribution")
                        .HasColumnType("integer");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<int>("userId")
                        .HasColumnType("integer");

                    b.HasKey("extendedModelId");

                    b.HasIndex("userId", "name")
                        .IsUnique();

                    b.ToTable("extendedModel");
                });

            modelBuilder.Entity("PatternRecogniser.Models.Pattern", b =>
                {
                    b.Property<int>("patternId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("extendedModelId")
                        .HasColumnType("integer");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<byte[]>("picture")
                        .HasColumnType("bytea");

                    b.HasKey("patternId");

                    b.HasIndex("extendedModelId");

                    b.ToTable("pattern");
                });

            modelBuilder.Entity("PatternRecogniser.Models.RecognisedPatterns", b =>
                {
                    b.Property<int>("recognisedPatternsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int?>("PatternRecognitionExperimentexperimentId")
                        .HasColumnType("integer");

                    b.Property<int>("patternId")
                        .HasColumnType("integer");

                    b.Property<double>("probability")
                        .HasColumnType("double precision");

                    b.HasKey("recognisedPatternsId");

                    b.HasIndex("PatternRecognitionExperimentexperimentId");

                    b.HasIndex("patternId");

                    b.ToTable("recognisedPatterns");
                });

            modelBuilder.Entity("PatternRecogniser.Models.User", b =>
                {
                    b.Property<int>("userId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<DateTime>("createDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("lastLog")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("login")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("userId");

                    b.HasIndex("email")
                        .IsUnique();

                    b.HasIndex("login")
                        .IsUnique();

                    b.ToTable("user");
                });

            modelBuilder.Entity("PatternRecogniser.Models.ValidationSet", b =>
                {
                    b.Property<int>("validationSetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int>("recognisedPatternId")
                        .HasColumnType("integer");

                    b.Property<byte[]>("testedPattern")
                        .HasColumnType("bytea");

                    b.Property<int>("truePatternId")
                        .HasColumnType("integer");

                    b.HasKey("validationSetId");

                    b.HasIndex("recognisedPatternId");

                    b.HasIndex("truePatternId");

                    b.ToTable("validationSet");
                });

            modelBuilder.Entity("PatternRecogniser.Models.ModelTrainingExperiment", b =>
                {
                    b.HasBaseType("PatternRecogniser.Models.Experiment");

                    b.Property<double>("accuracy")
                        .HasColumnType("double precision");

                    b.Property<int[]>("confusionMatrix")
                        .HasColumnType("integer[]");

                    b.Property<double>("missRate")
                        .HasColumnType("double precision");

                    b.Property<double>("precision")
                        .HasColumnType("double precision");

                    b.Property<double>("recall")
                        .HasColumnType("double precision");

                    b.Property<double>("specificity")
                        .HasColumnType("double precision");

                    b.Property<int?>("validationSetId1")
                        .HasColumnType("integer");

                    b.HasIndex("extendedModelId")
                        .IsUnique();

                    b.HasIndex("validationSetId1");

                    b.ToTable("ModelTrainingExperiment");
                });

            modelBuilder.Entity("PatternRecogniser.Models.PatternRecognitionExperiment", b =>
                {
                    b.HasBaseType("PatternRecogniser.Models.Experiment");

                    b.Property<byte[]>("testedPattern")
                        .HasColumnType("bytea");

                    b.HasIndex("extendedModelId");

                    b.ToTable("PatternRecognitionExperiment");
                });

            modelBuilder.Entity("ExperimentExperimentList", b =>
                {
                    b.HasOne("PatternRecogniser.Models.ExperimentList", null)
                        .WithMany()
                        .HasForeignKey("experimentListsexperimentListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PatternRecogniser.Models.Experiment", null)
                        .WithMany()
                        .HasForeignKey("experimentsexperimentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PatternRecogniser.Models.Authentication", b =>
                {
                    b.HasOne("PatternRecogniser.Models.User", "user")
                        .WithMany()
                        .HasForeignKey("userId1");

                    b.Navigation("user");
                });

            modelBuilder.Entity("PatternRecogniser.Models.ExperimentList", b =>
                {
                    b.HasOne("PatternRecogniser.Models.User", "user")
                        .WithMany("experimentLists")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("PatternRecogniser.Models.ExtendedModel", b =>
                {
                    b.HasOne("PatternRecogniser.Models.User", "user")
                        .WithMany("extendedModel")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("PatternRecogniser.Models.Pattern", b =>
                {
                    b.HasOne("PatternRecogniser.Models.ExtendedModel", "extendedModel")
                        .WithMany("patterns")
                        .HasForeignKey("extendedModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("extendedModel");
                });

            modelBuilder.Entity("PatternRecogniser.Models.RecognisedPatterns", b =>
                {
                    b.HasOne("PatternRecogniser.Models.PatternRecognitionExperiment", null)
                        .WithMany("recognisedPatterns")
                        .HasForeignKey("PatternRecognitionExperimentexperimentId");

                    b.HasOne("PatternRecogniser.Models.Pattern", "pattern")
                        .WithMany()
                        .HasForeignKey("patternId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("pattern");
                });

            modelBuilder.Entity("PatternRecogniser.Models.ValidationSet", b =>
                {
                    b.HasOne("PatternRecogniser.Models.Pattern", "recognisedPattern")
                        .WithMany()
                        .HasForeignKey("recognisedPatternId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PatternRecogniser.Models.Pattern", "truePattern")
                        .WithMany()
                        .HasForeignKey("truePatternId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("recognisedPattern");

                    b.Navigation("truePattern");
                });

            modelBuilder.Entity("PatternRecogniser.Models.ModelTrainingExperiment", b =>
                {
                    b.HasOne("PatternRecogniser.Models.Experiment", null)
                        .WithOne()
                        .HasForeignKey("PatternRecogniser.Models.ModelTrainingExperiment", "experimentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PatternRecogniser.Models.ExtendedModel", "extendedModel")
                        .WithOne("modelTrainingExperiment")
                        .HasForeignKey("PatternRecogniser.Models.ModelTrainingExperiment", "extendedModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PatternRecogniser.Models.ValidationSet", "validationSet")
                        .WithMany()
                        .HasForeignKey("validationSetId1");

                    b.Navigation("extendedModel");

                    b.Navigation("validationSet");
                });

            modelBuilder.Entity("PatternRecogniser.Models.PatternRecognitionExperiment", b =>
                {
                    b.HasOne("PatternRecogniser.Models.Experiment", null)
                        .WithOne()
                        .HasForeignKey("PatternRecogniser.Models.PatternRecognitionExperiment", "experimentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PatternRecogniser.Models.ExtendedModel", "extendedModel")
                        .WithMany()
                        .HasForeignKey("extendedModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("extendedModel");
                });

            modelBuilder.Entity("PatternRecogniser.Models.ExtendedModel", b =>
                {
                    b.Navigation("modelTrainingExperiment");

                    b.Navigation("patterns");
                });

            modelBuilder.Entity("PatternRecogniser.Models.User", b =>
                {
                    b.Navigation("experimentLists");

                    b.Navigation("extendedModel");
                });

            modelBuilder.Entity("PatternRecogniser.Models.PatternRecognitionExperiment", b =>
                {
                    b.Navigation("recognisedPatterns");
                });
#pragma warning restore 612, 618
        }
    }
}
