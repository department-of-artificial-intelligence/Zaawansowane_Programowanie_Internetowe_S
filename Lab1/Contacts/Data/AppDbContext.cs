using Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(contactBuilder =>
        {
            contactBuilder.HasKey(contact => contact.Id);
            contactBuilder
                .Property(contact => contact.Id)
                .ValueGeneratedNever();
            contactBuilder
                .Property(contact => contact.FirstName)
                .HasConversion(
                    firstName => firstName.Value,
                    value => ParseFirstName(value))
                .IsRequired();
            contactBuilder
                .Property(contact => contact.LastName)
                .HasConversion(
                    lastName => lastName.Value,
                    value => ParseLastName(value))
                .IsRequired();
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
                    .HasForeignKey(email => email.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                emailBuilder
                    .Property(email => email.Address)
                    .HasConversion(
                        address => address.Value,
                        value => ParseEmailAddress(value));
            });

        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.PhoneNumbers,
            phoneBuilder =>
            {
                phoneBuilder.WithOwner().HasForeignKey("ContactId");
                phoneBuilder.Property<int>("Id");
                phoneBuilder.HasKey("Id");
                phoneBuilder.Property(phone => phone.Value);
            });

        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.ImportantDates,
            dateBuilder =>
            {
                dateBuilder.WithOwner().HasForeignKey("ContactId");
                dateBuilder.Property<int>("Id");
                dateBuilder.HasKey("Id");
                dateBuilder.Property(date => date.Description);
                dateBuilder.Property(date => date.Date);
            });

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.Emails)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.PhoneNumbers)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.ImportantDates)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Contact>().ComplexProperty(
            contact => contact.Age,
            ageBuilder => ageBuilder.Property(age => age.Value)
        );
    }

    private static FirstName ParseFirstName(string value) =>
        FirstName
            .Create(value)
            .Match(
                firstName => firstName,
                error => throw new InvalidOperationException(error));

    private static LastName ParseLastName(string value) =>
        LastName
            .Create(value)
            .Match(
                lastName => lastName,
                error => throw new InvalidOperationException(error));

    private static EmailAddress ParseEmailAddress(string value) =>
        EmailAddress
            .Create(value)
            .Match(
                address => address,
                error => throw new InvalidOperationException(error));
}
