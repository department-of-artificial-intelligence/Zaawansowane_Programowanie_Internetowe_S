using Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(contactBuilder =>
        {
            contactBuilder.HasKey(contact => contact.Id);
            contactBuilder.Property(contact => contact.Id).ValueGeneratedNever();

            contactBuilder.ComplexProperty(
                c => c.FirstName,
                f => f.Property(fn => fn.Value).HasColumnName("FirstName").IsRequired()
            );

            contactBuilder.ComplexProperty(
                c => c.LastName,
                l => l.Property(ln => ln.Value).HasColumnName("LastName").IsRequired()
            );
        });

        modelBuilder.Entity<Category>(categoryBuilder =>
        {
            categoryBuilder.HasKey(category => category.Id);
            categoryBuilder.Property(category => category.Id).ValueGeneratedNever();
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

                emailBuilder.HasOne<Category>()
                    .WithMany()
                    .HasForeignKey(email => email.CategoryId);

                emailBuilder.Property(email => email.Address)
                    .HasConversion(
                        address => address.Value,
                        value => ParseEmailAddress(value)
                    );
            }
        );

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
                phoneBuilder.Property<int>("Id");
                phoneBuilder.HasKey("Id");

                phoneBuilder.HasOne<Category>()
                    .WithMany()
                    .HasForeignKey(phone => phone.CategoryId);

                phoneBuilder.Property(phone => phone.Number)
                    .HasConversion(
                        number => number.Value,
                        value => ParsePhoneNumber(value)
                    );
            }
        );

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.Phones)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.ImportantDates,
            dateBuilder =>
            {
                dateBuilder.WithOwner().HasForeignKey("ContactId");
                dateBuilder.Property<int>("Id");
                dateBuilder.HasKey("Id");

                dateBuilder.Property(d => d.Description).IsRequired();
                dateBuilder.Property(d => d.Date).IsRequired();
            }
        );

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.ImportantDates)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private static EmailAddress ParseEmailAddress(string value) =>
        EmailAddress.Create(value).Match(
            address => address,
            error => throw new InvalidOperationException(error)
        );

    private static PhoneNumber ParsePhoneNumber(string value) =>
        PhoneNumber.Create(value).Match(
            number => number,
            error => throw new InvalidOperationException(error)
        );
}