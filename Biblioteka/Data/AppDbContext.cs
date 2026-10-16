using Application.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AppDbContext : DbContext
    {
        // ------------------ ZESTAWY Z ZADAŃ 1-8 ------------------
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<Category> Categories => Set<Category>();

        // ------------------ NOWE ZESTAWY (BIBLIOTEKA) ------------
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Reader> Readers => Set<Reader>();
        public DbSet<Loan> Loans => Set<Loan>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =====================================================
            // 1. KONFIGURACJA KONTAKTÓW I KATEGORII (ZADANIA 1-8)
            // =====================================================

            // Encja Contact
            modelBuilder.Entity<Contact>(contactBuilder =>
            {
                contactBuilder.HasKey(contact => contact.Id);
                contactBuilder
                    .Property(contact => contact.Id)
                    .ValueGeneratedNever();

                contactBuilder
                    .Property(contact => contact.FirstName)
                    .HasConversion(
                        name => name.Value,
                        value => ParseFirstName(value))
                    .IsRequired();

                contactBuilder
                    .Property(contact => contact.LastName)
                    .HasConversion(
                        name => name.Value,
                        value => ParseLastName(value))
                    .IsRequired();
            });

            // Encja Category
            modelBuilder.Entity<Category>(categoryBuilder =>
            {
                categoryBuilder.HasKey(category => category.Id);
                categoryBuilder
                    .Property(category => category.Id)
                    .ValueGeneratedNever();
                categoryBuilder.Property(category => category.Name).IsRequired();
                categoryBuilder.HasIndex(category => category.Name).IsUnique();
            });

            // Kolekcja Emails (OwnsMany)
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

            // Kolekcja Phones (OwnsMany)
            modelBuilder.Entity<Contact>().OwnsMany(
                contact => contact.Phones,
                phoneBuilder =>
                {
                    phoneBuilder.WithOwner().HasForeignKey("ContactId");
                    phoneBuilder.Property<int>("Id");
                    phoneBuilder.HasKey("Id");
                    phoneBuilder
                        .HasOne<Category>()
                        .WithMany()
                        .HasForeignKey(phone => phone.CategoryId)
                        .OnDelete(DeleteBehavior.Restrict);

                    phoneBuilder
                        .Property(phone => phone.Number)
                        .HasConversion(
                            number => number.Value,
                            value => ParsePhoneNumber(value));
                });

            // Kolekcja ImportantDates (OwnsMany)
            modelBuilder.Entity<Contact>().OwnsMany(
                contact => contact.ImportantDates,
                dateBuilder =>
                {
                    dateBuilder.WithOwner().HasForeignKey("ContactId");
                    dateBuilder.Property<int>("Id");
                    dateBuilder.HasKey("Id");

                    dateBuilder.Property(d => d.Date).IsRequired();

                    dateBuilder
                        .Property(d => d.Description)
                        .HasConversion(
                            desc => desc.Value,
                            value => ParseDateDescription(value))
                        .IsRequired();
                });

            // Podpięcie prywatnych pól backing fields
            modelBuilder.Entity<Contact>()
                .Navigation(c => c.Emails)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<Contact>()
                .Navigation(c => c.Phones)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<Contact>()
                .Navigation(c => c.ImportantDates)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // Obiekt wartości Age
            modelBuilder.Entity<Contact>().ComplexProperty(
                contact => contact.Age,
                ageBuilder => ageBuilder.Property(age => age.Value)
            );

            // =====================================================
            // 2. KONFIGURACJA MODUŁU BIBLIOTEKI (ZADANIE 12)
            // =====================================================

            // Agregat Book i encja zależna BookCopy
            modelBuilder.Entity<Book>(bookBuilder =>
            {
                bookBuilder.HasKey(b => b.Id);
                bookBuilder.Property(b => b.Id).ValueGeneratedNever();

                bookBuilder.Property(b => b.Title)
                    .HasConversion(
                        t => t.Value,
                        v => ParseTitle(v))
                    .IsRequired();

                bookBuilder.Property(b => b.Author)
                    .HasConversion(
                        a => a.Value,
                        v => ParseAuthor(v))
                    .IsRequired();

                bookBuilder.Property(b => b.Isbn)
                    .HasConversion(
                        i => i.Value,
                        v => ParseIsbn(v))
                    .IsRequired();

                bookBuilder.OwnsMany(b => b.Copies, copyBuilder =>
                {
                    copyBuilder.WithOwner().HasForeignKey("BookId");
                    copyBuilder.Property(c => c.Id).ValueGeneratedNever();
                    copyBuilder.HasKey(c => c.Id);

                    copyBuilder.Property(c => c.Code)
                        .HasConversion(
                            code => code.Value,
                            v => ParseInventoryCode(v))
                        .IsRequired();

                    copyBuilder.Property(c => c.Status).IsRequired();
                });

                bookBuilder.Navigation(b => b.Copies)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            // Agregat Reader
            modelBuilder.Entity<Reader>(readerBuilder =>
            {
                readerBuilder.HasKey(r => r.Id);
                readerBuilder.Property(r => r.Id).ValueGeneratedNever();

                readerBuilder.Property(r => r.FirstName)
                    .HasConversion(
                        fn => fn.Value,
                        v => ParseFirstName(v))
                    .IsRequired();

                readerBuilder.Property(r => r.LastName)
                    .HasConversion(
                        ln => ln.Value,
                        v => ParseLastName(v))
                    .IsRequired();

                readerBuilder.Property(r => r.Email)
                    .HasConversion(
                        e => e.Value,
                        v => ParseEmailAddress(v))
                    .IsRequired();

                readerBuilder.Property(r => r.CardNumber)
                    .HasConversion(
                        c => c.Value,
                        v => ParseCardNumber(v))
                    .IsRequired();

                readerBuilder.Property(r => r.Status).IsRequired();
                readerBuilder.Property(r => r.MaxAllowedLoans).IsRequired();

                readerBuilder.HasIndex(r => r.CardNumber).IsUnique();
            });

            // Agregat Loan
            modelBuilder.Entity<Loan>(loanBuilder =>
            {
                loanBuilder.HasKey(l => l.Id);
                loanBuilder.Property(l => l.Id).ValueGeneratedNever();

                loanBuilder.Property(l => l.ReaderId).IsRequired();
                loanBuilder.Property(l => l.BookId).IsRequired();

                loanBuilder.Property(l => l.CopyCode)
                    .HasConversion(
                        c => c.Value,
                        v => ParseInventoryCode(v))
                    .IsRequired();

                loanBuilder.Property(l => l.BorrowDate).IsRequired();
                loanBuilder.Property(l => l.DueDate).IsRequired();
                loanBuilder.Property(l => l.ReturnDate).IsRequired(false);
                loanBuilder.Property(l => l.ExtensionCount).IsRequired();
            });
        }

        // =====================================================
        // 3. METODY PARSUJĄCE DLA OBIEKTÓW WARTOŚCI (VALUE OBJECTS)
        // =====================================================

        private static FirstName ParseFirstName(string value) =>
            FirstName.Create(value).Match(
                name => name,
                error => throw new InvalidOperationException(error));

        private static LastName ParseLastName(string value) =>
            LastName.Create(value).Match(
                name => name,
                error => throw new InvalidOperationException(error));

        private static EmailAddress ParseEmailAddress(string value) =>
            EmailAddress.Create(value).Match(
                address => address,
                error => throw new InvalidOperationException(error));

        private static PhoneNumber ParsePhoneNumber(string value) =>
            PhoneNumber.Create(value).Match(
                number => number,
                error => throw new InvalidOperationException(error));

        private static DateDescription ParseDateDescription(string value) =>
            DateDescription.Create(value).Match(
                desc => desc,
                error => throw new InvalidOperationException(error));

        private static Title ParseTitle(string value) =>
            Title.Create(value).Match(
                title => title,
                error => throw new InvalidOperationException(error));

        private static Author ParseAuthor(string value) =>
            Author.Create(value).Match(
                author => author,
                error => throw new InvalidOperationException(error));

        private static Isbn ParseIsbn(string value) =>
            Isbn.Create(value).Match(
                isbn => isbn,
                error => throw new InvalidOperationException(error));

        private static InventoryCode ParseInventoryCode(string value) =>
            InventoryCode.Create(value).Match(
                code => code,
                error => throw new InvalidOperationException(error));

        private static CardNumber ParseCardNumber(string value) =>
            CardNumber.Create(value).Match(
                card => card,
                error => throw new InvalidOperationException(error));
    }
}