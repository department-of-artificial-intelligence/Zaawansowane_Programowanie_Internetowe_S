using Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<User> Users => Set<User>();
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(userBuilder => {
            userBuilder
                .HasKey(user=>user.Id);
            userBuilder
                .Property(user => user.Id).ValueGeneratedNever();
            userBuilder
                .Property(user => user.UserName)
                .IsRequired();
            userBuilder
                .Property(user=>user.Password)
                .IsRequired();
            userBuilder
                .HasIndex(user=>user.UserName)
                .IsUnique();
            userBuilder.ComplexProperty(
                user => user.FirstName,
                fnBuilder => fnBuilder.Property(fn => fn.Value).HasColumnName("FirstName")
            );

            userBuilder.ComplexProperty(
                user => user.LastName,
                lnBuilder => lnBuilder.Property(ln => ln.Value).HasColumnName("LastName")
            );
            userBuilder.Property(user => user.DateOfBirth)
                .IsRequired(false);
            userBuilder.Property(user => user.Address)
                .IsRequired(false);
            });

        modelBuilder.Entity<Contact>(contactBuilder =>
        {
            contactBuilder.HasKey(contact => contact.Id);
            contactBuilder
                .Property(contact => contact.Id)
                .ValueGeneratedNever();

            contactBuilder
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(contact => contact.UserId);

            contactBuilder.ComplexProperty(
                contact => contact.FirstName,
                fnBuilder => fnBuilder.Property(fn => fn.Value).HasColumnName("FirstName")
            );

            contactBuilder.ComplexProperty(
                contact => contact.LastName,
                lnBuilder => lnBuilder.Property(ln => ln.Value).HasColumnName("LastName")
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
            categoryBuilder
                 .HasIndex(category => new {category.UserId, category.Name})
                 .IsUnique();
             categoryBuilder
                 .HasOne<User>()
                 .WithMany()
                 .HasForeignKey(category => category.UserId);
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

        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.Numbers,
            numberBuilder =>
            {
                numberBuilder.WithOwner().HasForeignKey("ContactId");
                numberBuilder.Property<int>("Id");
                numberBuilder.HasKey("Id");
                numberBuilder.Property(n => n.Value).HasColumnName("PhoneNumber");
            });

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.Numbers)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.ImportantDates,
            dateBuilder =>
            {
                dateBuilder.WithOwner().HasForeignKey("ContactId");
                dateBuilder.Property<int>("Id");
                dateBuilder.HasKey("Id");
            });

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.ImportantDates)
            .HasField("_dates")
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
}