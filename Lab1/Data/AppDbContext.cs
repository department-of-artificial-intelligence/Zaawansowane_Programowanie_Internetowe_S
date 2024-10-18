using Microsoft.EntityFrameworkCore;
using Contacts.Application.Domain;
using System.Runtime.Serialization;

namespace Contacts.Data;

public class AppDbContext : DbContext {
    public DbSet<Contacts.Application.Domain.Contact> Contacts => Set<Contacts.Application.Domain.Contact>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Email> Emails => Set<Email>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contacts.Application.Domain.Contact>(contactBuilder=>{
            contactBuilder
                .HasKey(Contact=>Contact.Id);
            contactBuilder
                .Property(contact=>contact.FirstName)
                .IsRequired();
            contactBuilder
                .Property(contact=>contact.LastName)
                .IsRequired();
        });

        modelBuilder.Entity<Category>(categoryBuilder => {
            categoryBuilder
                .HasKey(category => category.Id);
            categoryBuilder
                .Property(category => category.Name)
                .IsRequired();
        });

        modelBuilder.Entity<Email>().OwnsOne(email => email);

        modelBuilder.Entity<Contacts.Application.Domain.Contact>()
            .HasMany(c => c.Emails)
            .WithOne(e => e.Contact)
            .HasForeignKey(e => e.ContactId);

        modelBuilder.Entity<Contacts.Application.Domain.Contact>().OwnsOne(
            contact => contact.Age,
            ageBuilder => ageBuilder.Property(age => age.Value)
        );
    }
}