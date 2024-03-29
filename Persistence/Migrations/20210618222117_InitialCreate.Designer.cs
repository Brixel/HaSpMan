﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    [DbContext(typeof(HaSpManContext))]
    [Migration("20210618222117_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("HaSpMan")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Domain.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("LastName")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("Domain.Member", b =>
                {
                    b.OwnsOne("Domain.ValueObjects.Address", "Address", b1 =>
                        {
                              b1.Property<Guid>("MemberId")
                                  .HasColumnType("uniqueidentifier");

                              b1.Property<string>("City")
                                  .HasMaxLength(50)
                                  .HasColumnType("varchar(50)");

                              b1.Property<string>("Country")
                                  .HasMaxLength(50)
                                  .HasColumnType("varchar(50)");

                              b1.Property<string>("HouseNumber")
                                  .HasColumnType("nvarchar(max)");

                              b1.Property<string>("Street")
                                  .HasMaxLength(200)
                                  .HasColumnType("varchar(200)");

                              b1.Property<string>("ZipCode")
                                  .HasMaxLength(10)
                                  .HasColumnType("varchar(10)");

                              b1.HasKey("MemberId");

                              b1.ToTable("Members");

                              b1.WithOwner()
                                  .HasForeignKey("MemberId");
                          });

                    b.Navigation("Address");
                });
#pragma warning restore 612, 618
        }
    }
}
