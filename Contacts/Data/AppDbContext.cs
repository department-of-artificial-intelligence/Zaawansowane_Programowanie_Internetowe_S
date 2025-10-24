using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Contacts.Application.Domain;

namespace Contacts.Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Email> Emails => Set<Email>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 

        modelBuilder.Entity<Contact>(contactBuilder =>
        {
            contactBuilder.HasKey(contact => contact.Id);

            contactBuilder.Property(contact => contact.FirstName)
                .HasConversion(
                    vo => vo.Value,
                    value => new FirstName(value)) 
                .IsRequired();

            contactBuilder.Property(contact => contact.LastName)
                .HasConversion(
                    vo => vo.Value,
                    value => new LastName(value))
                .IsRequired();

            contactBuilder.OwnsOne(
                contact => contact.Age,
                ageBuilder => ageBuilder.Property(age => age.Value)
            );

            contactBuilder.HasMany(c => c.Emails)
                .WithOne(e => e.Contact) 
                .HasForeignKey(e => e.ContactId)
                .OnDelete(DeleteBehavior.Cascade); 
        });

        modelBuilder.Entity<Category>(categoryBuilder =>
        {
            categoryBuilder.HasKey(category => category.Id);
            categoryBuilder.Property(category => category.Name)
                .IsRequired();
        });

        modelBuilder.Entity<Email>(emailBuilder =>
        {
            emailBuilder.HasKey(email => email.Id);

            emailBuilder.OwnsOne(email => email.Address, addressBuilder =>
            {
                addressBuilder.Property(a => a.Address).HasColumnName("EmailAddress");
            });

            emailBuilder.HasOne(email => email.Category)
                .WithMany() 
                .HasForeignKey(email => email.CategoryId)
                .IsRequired();
        });
    }
}
