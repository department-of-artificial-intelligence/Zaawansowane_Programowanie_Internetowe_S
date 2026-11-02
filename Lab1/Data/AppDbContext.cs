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
                .HasMaxLength(User.MaxUserNameChars)
                .IsRequired();
            userBuilder
                .HasIndex(user => user.UserName)
                .IsUnique();
            // Zadanie 7: tylko skrót hasła
            userBuilder
                .Property(user => user.PasswordHash)
                .IsRequired();
            // Rola jako liczba (domyślne mapowanie enuma, jak w PDF)
            userBuilder
                .Property(user => user.Role);
            // Zadanie 4: profil jako typ złożony (kolumny Profile_FirstName itd.)
            userBuilder.ComplexProperty(user => user.Profile, profileBuilder =>
            {
                profileBuilder.Property(p => p.FirstName).HasMaxLength(UserProfile.MaxNameChars);
                profileBuilder.Property(p => p.LastName).HasMaxLength(UserProfile.MaxNameChars);
                profileBuilder.Property(p => p.BirthDate);
                profileBuilder.Property(p => p.Address).HasMaxLength(UserProfile.MaxAddressChars);
            });
            // Zadanie 10
            userBuilder
                .HasIndex(user => user.RefreshTokenHash)
                .IsUnique();
        });

        modelBuilder.Entity<Contact>(contactBuilder =>
        {
            contactBuilder.HasKey(contact => contact.Id);
            contactBuilder
                .Property(contact => contact.Id)
                .ValueGeneratedNever();

            // Zadanie 1.4: konwersja obiektów wartości na tekst i z powrotem
            contactBuilder.Property(contact => contact.FirstName)
                .HasConversion(
                    firstName => firstName.Value,
                    value => FirstName.Create(value).GetValueOrThrow())
                .HasMaxLength(FirstName.MaxChars)
                .IsRequired();
            contactBuilder.Property(contact => contact.LastName)
                .HasConversion(
                    lastName => lastName.Value,
                    value => LastName.Create(value).GetValueOrThrow())
                .HasMaxLength(LastName.MaxChars)
                .IsRequired();

            // Zadanie 8: kontakt należy do użytkownika
            contactBuilder
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(contact => contact.UserId);
        });

        modelBuilder.Entity<Category>(categoryBuilder =>
        {
            categoryBuilder.HasKey(category => category.Id);
            categoryBuilder
                .Property(category => category.Id)
                .ValueGeneratedNever();
            categoryBuilder.Property(category => category.Name).IsRequired();
            // 3.24: nazwa unikalna w obrębie użytkownika + klucz obcy do Users
            categoryBuilder
                .HasIndex(category => new { category.UserId, category.Name })
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
                    .HasForeignKey(email => email.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
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
        phoneBuilder.Property(phone => phone.Number)
            .HasConversion(n => n.Value, v => PhoneNumber.Create(v).GetValueOrThrow())
            .HasMaxLength(PhoneNumber.MaxChars)
            .IsRequired();
        phoneBuilder.Property(phone => phone.Type)
            .HasConversion<string>();
    });
        modelBuilder.Entity<Contact>().OwnsMany(
        contact => contact.ImportantDates,
        dateBuilder =>
        {
            dateBuilder.WithOwner().HasForeignKey("ContactId");
            dateBuilder.HasKey(d => d.Id);
            dateBuilder.Property(d => d.Id).ValueGeneratedNever();
            dateBuilder.Property(d => d.Label)
                .HasConversion(l => l.Value, v => DateLabel.Create(v).GetValueOrThrow())
                .HasMaxLength(DateLabel.MaxChars)
                .IsRequired();
            dateBuilder.Property(d => d.Date).IsRequired();
        });
        modelBuilder.Entity<Contact>()
            .Navigation(c => c.ImportantDates)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        modelBuilder.Entity<Contact>()
            .Navigation(c => c.Phones)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.Emails)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Contact>().ComplexProperty(
            contact => contact.Age,
            ageBuilder => ageBuilder.Property(age => age.Value));
    }


    private static EmailAddress ParseEmailAddress(string value) =>
        EmailAddress
            .Create(value)
            .Match(
                address => address,
                error => throw new InvalidOperationException(error));
}