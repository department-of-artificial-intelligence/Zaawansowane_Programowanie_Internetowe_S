using Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(contactBuilder =>
        {
            contactBuilder.HasKey(contact => contact.Id);
            contactBuilder
                .Property(contact => contact.Id)
                .ValueGeneratedNever();

            
                modelBuilder.Entity<Contact>().ComplexProperty(
                    contact => contact.FirstName,
                    firstNameBuilder => firstNameBuilder
                        .Property(firstName => firstName.Value)
                        .HasColumnName("FirstName")
                        .IsRequired()
                );

                modelBuilder.Entity<Contact>().ComplexProperty(
                    contact => contact.LastName,
                    lastNameBuilder => lastNameBuilder
                        .Property(lastName => lastName.Value)
                        .HasColumnName("LastName")
                        .IsRequired()
                );

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

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.Emails)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Contact>().ComplexProperty(
            contact => contact.Age,
            ageBuilder => ageBuilder.Property(age => age.Value)
        );


        
        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.Phones,
            phoneBuilder =>
            {
                phoneBuilder.WithOwner().HasForeignKey("ContactId");

                phoneBuilder.HasKey(phone => phone.Id);
                phoneBuilder.Property(phone => phone.Id)
                    .ValueGeneratedNever();

                phoneBuilder
                    .Property(phone => phone.Number)
                    .HasConversion(
                        number => number.Value,
                        value => ParsePhoneNumber(value))
                    .IsRequired();
            });

        modelBuilder.Entity<Contact>()
            .Navigation(contact => contact.Phones)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        
        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.ImportantDates,
            dateBuilder =>
            {
                dateBuilder.WithOwner().HasForeignKey("ContactId");

                dateBuilder.HasKey(date => date.Id);
                dateBuilder.Property(date => date.Id)
                    .ValueGeneratedNever();

                dateBuilder.Property(date => date.Date)
                    .IsRequired();

                dateBuilder.Property(date => date.Name)
                    .HasConversion(
                        name => name.Value,
                        value => ParseEventName(value))
                    .IsRequired();
            });

        modelBuilder.Entity<Contact>()
            .Navigation(contact => contact.ImportantDates)
            .UsePropertyAccessMode(PropertyAccessMode.Field);


    }

    private static EmailAddress ParseEmailAddress(string value) =>
        EmailAddress
            .Create(value)
            .Match(
                address => address,
                error => throw new InvalidOperationException(error));


    
    private static PhoneNumber ParsePhoneNumber(string value) =>
        PhoneNumber
            .Create(value)
            .Match(
                number => number,
                error => throw new InvalidOperationException(error));


    
    private static EventName ParseEventName(string value) =>
        EventName.Create(value)
            .Match(
                name => name,
                error => throw new InvalidOperationException(error));


}
