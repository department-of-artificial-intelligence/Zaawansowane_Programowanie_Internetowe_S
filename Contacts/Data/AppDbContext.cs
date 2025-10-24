using Microsoft.EntityFrameworkCore;
using Contacts.Application.Domain;
namespace Contacts.Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Email> Emails => Set<Email>();
    public DbSet<Phone> Phones => Set<Phone>();
    public DbSet<ImportantDate> ImportantDates => Set<ImportantDate>();
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(contactBuilder =>
        {
            contactBuilder
            .HasKey(contact => contact.Id);
            contactBuilder
            .Property(contact => contact.Firstname)
            .IsRequired();
            contactBuilder
            .Property(contact => contact.LastName)
            .IsRequired();
        });
        modelBuilder.Entity<Category>(categoryBuilder =>
        {
            categoryBuilder
            .HasKey(category => category.Id);
            categoryBuilder
            .Property(category => category.Name)
            .IsRequired();
        });
        modelBuilder.Entity<Email>(emailBuilder =>
        {
            emailBuilder
            .HasKey(email => email.Id);
        });
        modelBuilder.Entity<Email>().OwnsOne(email => email.Address);
        modelBuilder.Entity<Phone>(phoneBuilder =>
        {
            phoneBuilder.HasKey(phone => phone.Id);
        });
        modelBuilder.Entity<Phone>().OwnsOne(phone => phone.Number);
        modelBuilder.Entity<ImportantDate>(dateBuilder =>
        {
            dateBuilder.HasKey(d => d.Id);
        });
        modelBuilder.Entity<ImportantDate>().OwnsOne(d => d.Info);
        modelBuilder.Entity<Contact>()
        .HasMany(c => c.Emails)
        .WithOne(e => e.Contact)
        .HasForeignKey(e => e.ContactId);
        modelBuilder.Entity<Contact>()
        .HasMany(c => c.Phones)
        .WithOne(p => p.Contact)
        .HasForeignKey(p => p.ContactId);
        modelBuilder.Entity<Contact>()
        .HasMany(c => c.Dates)
        .WithOne(d => d.Contact)
        .HasForeignKey(d => d.ContactId);

        // Ensure Email must always have a Category and prevent cascading deletion of emails when deleting a category
        modelBuilder.Entity<Email>()
            .HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        // Ensure Phone must always have a Category, restrict deletion
        modelBuilder.Entity<Phone>()
            .HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        // Ensure ImportantDate must always have a Category, restrict deletion
        modelBuilder.Entity<ImportantDate>()
            .HasOne(d => d.Category)
            .WithMany()
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contact>().OwnsOne(
        contact => contact.Age,
        ageBuilder => ageBuilder.Property(age => age.Value)
        );
    }
}