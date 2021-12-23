﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    [DbContext(typeof(HaSpManContext))]
    [Migration("20210826220934_AddMembershipAndAuditProperties")]
    partial class AddMembershipAndAuditProperties
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
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("MembershipExpiryDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<double>("MembershipFee")
                        .HasColumnType("float");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("Domain.Member", b =>
                {
                    b.OwnsOne("Types.Address", "Address", b1 =>
                        {
                            b1.Property<Guid>("MemberId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("varchar(50)");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("varchar(50)");

                            b1.Property<string>("HouseNumber")
                                .IsRequired()
                                .HasMaxLength(5)
                                .HasColumnType("varchar(5)");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("varchar(200)");

                            b1.Property<string>("ZipCode")
                                .IsRequired()
                                .HasMaxLength(10)
                                .HasColumnType("varchar(10)");

                            b1.HasKey("MemberId");

                            b1.ToTable("Members");

                            b1.WithOwner()
                                .HasForeignKey("MemberId");
                        });

                    b.OwnsMany("Types.AuditEvent", "AuditEvents", b1 =>
                        {
                            b1.Property<Guid>("MemberId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasMaxLength(1000)
                                .HasColumnType("varchar(1000)");

                            b1.Property<string>("PerformedBy")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("varchar(100)");

                            b1.Property<DateTimeOffset>("Timestamp")
                                .HasColumnType("datetimeoffset");

                            b1.HasKey("MemberId", "Id");

                            b1.ToTable("Member_AuditEvents");

                            b1.WithOwner()
                                .HasForeignKey("MemberId");
                        });

                    b.Navigation("Address")
                        .IsRequired();

                    b.Navigation("AuditEvents");
                });
#pragma warning restore 612, 618
        }
    }
}
