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
    {}

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
                .HasMaxLength(50)
                .IsRequired();
            userBuilder
                .Property(user => user.PasswordHash)
                .IsRequired();
            userBuilder
                .HasIndex(user => user.UserName)
                .IsUnique();
            userBuilder
                .HasIndex(user => user.RefreshTokenHash);
            userBuilder.ComplexProperty(
                user => user.Profile,
                profileBuilder =>
                {
                    profileBuilder.Property(profile => profile.FirstName).HasMaxLength(50);
                    profileBuilder.Property(profile => profile.LastName).HasMaxLength(50);
                    profileBuilder.Property(profile => profile.DateOfBirth);
                    profileBuilder
                        .Property(profile => profile.Address)
                        .HasMaxLength(UserProfile.AddressMaxLength);
                });
        });

        modelBuilder.Entity<Contact>(contactBuilder =>
        {
            // Zadanie 8 - usunięcie użytkownika usuwa również jego kontakty.
            contactBuilder
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(contact => contact.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            contactBuilder.HasKey(contact => contact.Id);
            contactBuilder
                .Property(contact => contact.Id)
                .ValueGeneratedNever();
            contactBuilder
                .Property(contact => contact.FirstName)
                .HasConversion(
                    firstName => firstName.Value,
                    value => Parse(FirstName.Create(value)))
                .HasMaxLength(50)
                .IsRequired();
            contactBuilder
                .Property(contact => contact.LastName)
                .HasConversion(
                    lastName => lastName.Value,
                    value => Parse(LastName.Create(value)))
                .HasMaxLength(50)
                .IsRequired();
        });

        modelBuilder.Entity<Category>(categoryBuilder =>
        {
            categoryBuilder.HasKey(category => category.Id);
            categoryBuilder
                .Property(category => category.Id)
                .ValueGeneratedNever();
            categoryBuilder.Property(category => category.Name).IsRequired();
            categoryBuilder
                .HasIndex(category => new {category.UserId, category.Name})
                .IsUnique();
            categoryBuilder
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(category => category.UserId)
                .OnDelete(DeleteBehavior.Cascade);
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
                    // NO ACTION (a nie RESTRICT) jest sprawdzane dopiero na końcu
                    // polecenia, więc kaskadowe usunięcie użytkownika wraz z jego
                    // kontaktami i kategoriami się powiedzie.
                    .OnDelete(DeleteBehavior.NoAction);
                emailBuilder
                    .Property(email => email.Address)
                    .HasConversion(
                        address => address.Value,
                        value => Parse(EmailAddress.Create(value)));
            });

        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.Phones,
            phoneBuilder =>
            {
                phoneBuilder.WithOwner().HasForeignKey("ContactId");
                phoneBuilder.Property<int>("Id");
                phoneBuilder.HasKey("Id");
                phoneBuilder
                    .Property(phone => phone.Value)
                    .HasColumnName("Number")
                    .IsRequired();
            });

        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.ImportantDates,
            dateBuilder =>
            {
                dateBuilder.WithOwner().HasForeignKey("ContactId");
                dateBuilder.Property<int>("Id");
                dateBuilder.HasKey("Id");
                dateBuilder.Property(date => date.Date).IsRequired();
                dateBuilder
                    .Property(date => date.Description)
                    .HasMaxLength(ImportantDate.DescriptionMaxLength)
                    .IsRequired();
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

    // Odczyt z bazy wartości, która nie przechodzi walidacji, oznacza
    // ręczną modyfikację danych - jest to sytuacja wyjątkowa.
    private static T Parse<T>(Application.Utilities.Result<T> result) =>
        result.Match(
            value => value,
            error => throw new InvalidOperationException(error));
}
