using Application.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Email> Emails => Set<Email>();

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    { 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(contactBuilder => {
            contactBuilder
            .HasKey(contact => contact.Id);
            contactBuilder
            .Property(contact => contact.FirstName)
            .IsRequired();
            contactBuilder
            .Property(contact => contact.LastName)
            .IsRequired();
        });

        modelBuilder.Entity<Category>(categoryBuilder => {
            categoryBuilder
            .HasKey(category => category.Id);
            categoryBuilder
            .Property(category => category.Name)
            .IsRequired();
        });

        modelBuilder.Entity<Email>(emailBuilder => {
            emailBuilder
            .HasKey(email => email.Id);
        });

        modelBuilder.Entity<Email>().OwnsOne(email => email.Address);

        modelBuilder.Entity<Contact>()
            .HasMany(c => c.Emails)
            .WithOne(e => e.Contact)
            .HasForeignKey(e => e.ContactId);

        modelBuilder.Entity<Contact>().OwnsOne(
            contact => contact.Age,
            ageBuilder => ageBuilder.Property(age => age.Value)
        );

        modelBuilder.Entity<User>(userBuilder =>
        {
            userBuilder
                .HasKey(user => user.Id);

            userBuilder
                .Property(user => user.UserName)
                .IsRequired();

            userBuilder
                .Property(user => user.Password)
                .IsRequired();
        });
        
        modelBuilder.Entity<Category>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId);
    }
}
