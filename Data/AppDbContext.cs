using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(contactBuilder =>
        {
            contactBuilder.HasKey(contact => contact.Id);
            contactBuilder
                .Property(contact => contact.Id)
                .ValueGeneratedNever();
        });

        modelBuilder.Entity<Category>(categoryBuilder =>
        {
            categoryBuilder.HasKey(category => category.Id);
            categoryBuilder
                .Property(category => category.Id)
                .ValueGeneratedNever();
            categoryBuilder.Property(category => category.Name).IsRequired();
            categoryBuilder.HasIndex(category => category.Name).IsUnique();
        });

        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.Emails,
            emailBuilder =>
            {
                emailBuilder.WithOwner().HasForeignKey("ContactId");
                emailBuilder.Property<int>("Id");
                emailBuilder.HasKey("Id");
                emailBuilder
                    .HasOne<Category>()
                    .WithMany()
                    .HasForeignKey(email => email.CategoryId);
                emailBuilder
                    .Property(email => email.Address)
                    .HasConversion(
                        address => address.Value,
                        value => ParseEmailAddress(value));
            });

        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.Phones,
            phoneBuilder =>
            {
                phoneBuilder.WithOwner().HasForeignKey("ContactId");
                phoneBuilder.Property<int>("Id");
                phoneBuilder.HasKey("Id");
                phoneBuilder
                    .Property(phone => phone.Value)
                    .IsRequired()
                    .HasColumnName("PhoneNumber");
            });

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.Emails)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.Phones)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Contact>().ComplexProperty(
            contact => contact.Age,
            ageBuilder => ageBuilder.Property(age => age.Value)
        );

        modelBuilder.Entity<Contact>().ComplexProperty(
            contact => contact.FirstName,
            nameBuilder => nameBuilder.Property(n => n.Value).IsRequired().HasColumnName("FirstName")
        );

        modelBuilder.Entity<Contact>().ComplexProperty(
            contact => contact.LastName,
            nameBuilder => nameBuilder.Property(n => n.Value).IsRequired().HasColumnName("LastName")
        );
    }
    private static EmailAddress ParseEmailAddress(string value) =>
        EmailAddress
            .Create(value)
            .Match(
                address => address,
                error => throw new InvalidOperationException(error));
}

