using Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Reader> Readers => Set<Reader>();
    public DbSet<Loan> Loans => Set<Loan>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }

    public AppDbContext()
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=LocalDatabase.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // --- KONFIGURACJA WAŻNYCH DAT (ZADANIE 8) ---
        modelBuilder.Entity<Contact>().OwnsMany(
            contact => contact.ImportantDates,
            dateBuilder =>
            {
                dateBuilder.WithOwner().HasForeignKey("ContactId");
                dateBuilder.Property(d => d.Id).ValueGeneratedNever();
                dateBuilder.HasKey(d => d.Id);

                dateBuilder.OwnsOne(d => d.ImportantDate, idBuilder =>
                {
                    idBuilder.Property(i => i.Name).HasColumnName("DateName").IsRequired();
                    idBuilder.Property(i => i.Date).HasColumnName("DateValue").IsRequired();
                });
            });

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.ImportantDates)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        modelBuilder.Entity<Contact>(contactBuilder =>
        {
            contactBuilder.HasKey(contact => contact.Id);
            contactBuilder
                .Property(contact => contact.Id)
                .ValueGeneratedNever();
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

        // --- KONFIGURACJA EMAILS ---
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
    contact => contact.Phones,
    phoneBuilder =>
    {
        phoneBuilder.WithOwner().HasForeignKey("ContactId");

        // Zmieniamy typ na Guid, ponieważ Phone dziedziczy po Entity (posiada właściwość Guid Id)
        phoneBuilder.Property(phone => phone.Id).ValueGeneratedNever();
        phoneBuilder.HasKey(phone => phone.Id);

        phoneBuilder
            .HasOne<Category>()
            .WithMany()
            .HasForeignKey(phone => phone.CategoryId);

        phoneBuilder
            .Property(phone => phone.Number)
            .HasConversion(
                number => number.Value,
                value => ParsePhoneNumber(value));
    });

        modelBuilder.Entity<Contact>()
            .Navigation(c => c.Phones)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // --- KONFIGURACJA COMPLEX PROPERTY DLA OBIEKTÓW WARTOŚCI ---
        modelBuilder.Entity<Contact>().ComplexProperty(
            contact => contact.FirstName,
            fnBuilder => fnBuilder.Property(fn => fn.Value).HasColumnName("FirstName")
        );

        modelBuilder.Entity<Contact>().ComplexProperty(
            contact => contact.LastName,
            lnBuilder => lnBuilder.Property(ln => ln.Value).HasColumnName("LastName")
        );

        modelBuilder.Entity<Contact>().ComplexProperty(
            contact => contact.Age,
            ageBuilder => ageBuilder.Property(age => age.Value)
        );

        modelBuilder.Entity<Book>(b =>
        {
            b.HasKey(book => book.Id);
            b.Property(book => book.Id).ValueGeneratedNever();

            // Mapowanie obiektu wartości ISBN
            b.ComplexProperty(book => book.Isbn, isbnBuilder =>
            {
                isbnBuilder.Property(i => i.Value)
                           .HasColumnName("ISBN")
                           .IsRequired();
            });
        });

        modelBuilder.Entity<Reader>(r =>
        {
            r.HasKey(reader => reader.Id);
            r.Property(reader => reader.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Loan>(l =>
        {
            l.HasKey(loan => loan.Id);
            l.Property(loan => loan.Id).ValueGeneratedNever();
        });
    }
    private static PhoneNumber ParsePhoneNumber(string value) =>
    PhoneNumber
        .Create(value)
        .Match(
            number => number,
            error => throw new InvalidOperationException(error));
    private static EmailAddress ParseEmailAddress(string value) =>
        EmailAddress
            .Create(value)
            .Match(
                address => address,
                error => throw new InvalidOperationException(error));
}