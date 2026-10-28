using Application.Domain;
using Application.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(userBuilder =>
        {
            userBuilder
                .HasKey(user => user.Id);
            userBuilder
                .Property(user => user.Id).ValueGeneratedNever();
            userBuilder
                .Property(user => user.UserName)
                .IsRequired();
            userBuilder
                .Property(user => user.Password)
                .IsRequired();
            userBuilder
                .HasIndex(user => user.UserName)
                .IsUnique();
        });

        modelBuilder.Entity<Contact>(contactBuilder =>
        {
            contactBuilder.HasKey(contact => contact.Id);
            contactBuilder
                .Property(contact => contact.Id)
                .ValueGeneratedNever();

            contactBuilder.Property(contact => contact.FirstName)
                .HasConversion(
                    firstName => firstName.Value,
                    value => Unwrap(FirstName.Create(value)))
                .IsRequired();

            contactBuilder.Property(contact => contact.LastName)
                .HasConversion(
                    lastName => lastName.Value,
                    value => Unwrap(LastName.Create(value)))
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

        // E-maile
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

        // Telefony (zadanie 7)
        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.Phones,
            phoneBuilder =>
            {
                phoneBuilder.WithOwner().HasForeignKey("ContactId");
                phoneBuilder.Property<int>("Id");
                phoneBuilder.HasKey("Id");

                phoneBuilder
                    .Property(phone => phone.Number)
                    .HasConversion(
                        number => number.Value,
                        value => Unwrap(PhoneNumber.Create(value)));

                phoneBuilder
                    .Property(phone => phone.Type)
                    .HasConversion<string>();
            });

        // Ważne daty (zadanie 8)
        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.ImportantDates,
            dateBuilder =>
            {
                dateBuilder.WithOwner().HasForeignKey("ContactId");
                dateBuilder.Property<int>("Id");
                dateBuilder.HasKey("Id");

                dateBuilder
                    .Property(importantDate => importantDate.Label)
                    .HasConversion(
                        label => label.Value,
                        value => Unwrap(DateLabel.Create(value)));
            });

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.Emails)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        modelBuilder.Entity<Contact>()
            .Navigation(c => c.Phones)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        modelBuilder.Entity<Contact>()
            .Navigation(c => c.ImportantDates)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Contact>().ComplexProperty(
            contact => contact.Age,
            ageBuilder => ageBuilder.Property(age => age.Value)
        );
    }

    private static EmailAddress ParseEmailAddress(string value) =>
        EmailAddress
            .Create(value)
            .Match(
                address => address,
                error => throw new InvalidOperationException(error));

    private static T Unwrap<T>(Result<T> result) =>
        result.Match(
            value => value,
            error => throw new InvalidOperationException(error));
}