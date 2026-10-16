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
        public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<Category> Categories => Set<Category>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfiguracja encji Contact
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

            // Konfiguracja encji Category
            modelBuilder.Entity<Category>(categoryBuilder =>
            {
                categoryBuilder.HasKey(category => category.Id);
                categoryBuilder
                    .Property(category => category.Id)
                    .ValueGeneratedNever();
                categoryBuilder.Property(category => category.Name).IsRequired();
                categoryBuilder.HasIndex(category => category.Name).IsUnique();
            });

            // Konfiguracja kolekcji adresów e-mail (OwnsMany)
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

            // Konfiguracja kolekcji numerów telefonów (Zadanie 7 - OwnsMany)
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

            // Konfiguracja kolekcji ważnych dat (Zadanie 8 - OwnsMany)
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

            // Wskazanie prywatnych pól bazowych dla kolekcji
            modelBuilder.Entity<Contact>()
                .Navigation(c => c.Emails)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<Contact>()
                .Navigation(c => c.Phones)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<Contact>()
                .Navigation(c => c.ImportantDates)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // Konfiguracja obiektu wartości Age (ComplexProperty)
            modelBuilder.Entity<Contact>().ComplexProperty(
                contact => contact.Age,
                ageBuilder => ageBuilder.Property(age => age.Value)
            );
        }

        private static FirstName ParseFirstName(string value) =>
            FirstName
                .Create(value)
                .Match(
                    name => name,
                    error => throw new InvalidOperationException(error));

        private static LastName ParseLastName(string value) =>
            LastName
                .Create(value)
                .Match(
                    name => name,
                    error => throw new InvalidOperationException(error));

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

        private static DateDescription ParseDateDescription(string value) =>
            DateDescription
                .Create(value)
                .Match(
                    desc => desc,
                    error => throw new InvalidOperationException(error));
    }
}