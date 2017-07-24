using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Domain;

namespace Domain.Migrations
{
    [DbContext(typeof(Database))]
    partial class DatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Domain.SpeedTestResult", b =>
                {
                    b.Property<int>("SpeedTestResultId")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Download")
                        .HasAnnotation("Sqlite:ColumnType", "decimal(8,3)");

                    b.Property<decimal>("Ping")
                        .HasAnnotation("Sqlite:ColumnType", "decimal(8,3)");

                    b.Property<DateTime>("Timestamp");

                    b.Property<decimal>("Upload")
                        .HasAnnotation("Sqlite:ColumnType", "decimal(8,3)");

                    b.HasKey("SpeedTestResultId");

                    b.ToTable("SpeedTestResults");
                });
        }
    }
}
