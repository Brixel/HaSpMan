﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Persistence;

#nullable disable

namespace Persistence.Migrations
{
    [DbContext(typeof(HaSpManContext))]
    [Migration("20230728215040_RemoveFinancialConfiguration")]
    partial class RemoveFinancialConfiguration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("HaSpMan")
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.BankAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasMaxLength(34)
                        .HasColumnType("varchar");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar");

                    b.HasKey("Id");

                    b.ToTable("BankAccounts", "HaSpMan");
                });

            modelBuilder.Entity("Domain.FinancialYear", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("EndDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("StartDate")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.ToTable("FinancialYears", "HaSpMan");
                });

            modelBuilder.Entity("Domain.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("varchar");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar");

                    b.Property<DateTimeOffset?>("MembershipExpiryDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<double>("MembershipFee")
                        .HasColumnType("float");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(50)
                        .HasColumnType("varchar");

                    b.HasKey("Id");

                    b.ToTable("Members", "HaSpMan");
                });

            modelBuilder.Entity("Domain.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("BankAccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CounterPartyName")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("nvarchar(120)");

                    b.Property<DateTimeOffset>("DateFiled")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("FinancialYearId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("MemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("ReceivedDateTime")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("FinancialYearId");

                    b.ToTable("Transactions", "HaSpMan");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Transaction");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Persistence.Views.BankAccountsWithTotals", b =>
                {
                    b.Property<Guid>("BankAccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("NumberOfTransactions")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("BankAccountId");

                    b.ToTable((string)null);

                    b.ToView("vwBankAccountTotals", "HaSpMan");
                });

            modelBuilder.Entity("Domain.CreditTransaction", b =>
                {
                    b.HasBaseType("Domain.Transaction");

                    b.HasDiscriminator().HasValue("CreditTransaction");
                });

            modelBuilder.Entity("Domain.DebitTransaction", b =>
                {
                    b.HasBaseType("Domain.Transaction");

                    b.HasDiscriminator().HasValue("DebitTransaction");
                });

            modelBuilder.Entity("Domain.BankAccount", b =>
                {
                    b.OwnsMany("Types.AuditEvent", "AuditEvents", b1 =>
                        {
                            b1.Property<Guid>("BankAccountId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasMaxLength(1000)
                                .HasColumnType("varchar");

                            b1.Property<string>("PerformedBy")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("varchar");

                            b1.Property<DateTimeOffset>("Timestamp")
                                .HasColumnType("datetimeoffset");

                            b1.HasKey("BankAccountId", "Id");

                            b1.ToTable("BankAccount_AuditEvents", "HaSpMan");

                            b1.WithOwner()
                                .HasForeignKey("BankAccountId");
                        });

                    b.Navigation("AuditEvents");
                });

            modelBuilder.Entity("Domain.Member", b =>
                {
                    b.OwnsMany("Types.AuditEvent", "AuditEvents", b1 =>
                        {
                            b1.Property<Guid>("MemberId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasMaxLength(1000)
                                .HasColumnType("varchar");

                            b1.Property<string>("PerformedBy")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("varchar");

                            b1.Property<DateTimeOffset>("Timestamp")
                                .HasColumnType("datetimeoffset");

                            b1.HasKey("MemberId", "Id");

                            b1.ToTable("Member_AuditEvents", "HaSpMan");

                            b1.WithOwner()
                                .HasForeignKey("MemberId");
                        });

                    b.OwnsOne("Types.Address", "Address", b1 =>
                        {
                            b1.Property<Guid>("MemberId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("varchar");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("varchar");

                            b1.Property<string>("HouseNumber")
                                .IsRequired()
                                .HasMaxLength(15)
                                .HasColumnType("varchar");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("varchar");

                            b1.Property<string>("ZipCode")
                                .IsRequired()
                                .HasMaxLength(10)
                                .HasColumnType("varchar");

                            b1.HasKey("MemberId");

                            b1.ToTable("Members", "HaSpMan");

                            b1.WithOwner()
                                .HasForeignKey("MemberId");
                        });

                    b.Navigation("Address")
                        .IsRequired();

                    b.Navigation("AuditEvents");
                });

            modelBuilder.Entity("Domain.Transaction", b =>
                {
                    b.HasOne("Domain.FinancialYear", null)
                        .WithMany("Transactions")
                        .HasForeignKey("FinancialYearId");

                    b.OwnsMany("Domain.TransactionAttachment", "Attachments", b1 =>
                        {
                            b1.Property<Guid>("TransactionId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<string>("FullPath")
                                .IsRequired()
                                .HasMaxLength(1000)
                                .HasColumnType("nvarchar(1000)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("nvarchar(50)");

                            b1.HasKey("TransactionId", "Id");

                            b1.ToTable("Transaction_Attachments", "HaSpMan");

                            b1.WithOwner()
                                .HasForeignKey("TransactionId");
                        });

                    b.OwnsMany("Domain.TransactionTypeAmount", "TransactionTypeAmounts", b1 =>
                        {
                            b1.Property<Guid>("TransactionId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<decimal>("Amount")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<int>("TransactionType")
                                .HasColumnType("int");

                            b1.HasKey("TransactionId", "Id");

                            b1.ToTable("Transaction_TransactionTypeAmounts", "HaSpMan");

                            b1.WithOwner()
                                .HasForeignKey("TransactionId");
                        });

                    b.Navigation("Attachments");

                    b.Navigation("TransactionTypeAmounts");
                });

            modelBuilder.Entity("Persistence.Views.BankAccountsWithTotals", b =>
                {
                    b.HasOne("Domain.BankAccount", "Account")
                        .WithOne()
                        .HasForeignKey("Persistence.Views.BankAccountsWithTotals", "BankAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Domain.FinancialYear", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}