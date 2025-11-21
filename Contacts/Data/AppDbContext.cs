using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Contacts.Application.Domain;

namespace Contacts.Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Email> Emails => Set<Email>();
    public DbSet<Phone> Phones => Set<Phone>();
    
    // --- NOWY DBSET DLA WAŻNYCH DAT ---
    public DbSet<ImportantDate> ImportantDates => Set<ImportantDate>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); 

        // --- Konfiguracja Contact ---
        modelBuilder.Entity<Contact>(contactBuilder =>
        {
            contactBuilder.HasKey(contact => contact.Id);

            contactBuilder.Property(contact => contact.FirstName)
                .HasConversion(vo => vo.Value, value => new FirstName(value))
                .IsRequired();

            contactBuilder.Property(contact => contact.LastName)
                .HasConversion(vo => vo.Value, value => new LastName(value))
                .IsRequired();

            contactBuilder.OwnsOne(contact => contact.Age, 
                ageBuilder => ageBuilder.Property(age => age.Value));

            // Relacja do Emails
            contactBuilder.HasMany(c => c.Emails)
                .WithOne(e => e.Contact) 
                .HasForeignKey(e => e.ContactId)
                .OnDelete(DeleteBehavior.Cascade); 

            // Relacja do Telefonów
            contactBuilder.HasMany(c => c.Phones)
                .WithOne(p => p.Contact) 
                .HasForeignKey(p => p.ContactId)
                .OnDelete(DeleteBehavior.Cascade); 

            // --- NOWA RELACJA DO WAŻNYCH DAT ---
            contactBuilder.HasMany(c => c.ImportantDates)
                .WithOne(d => d.Contact) // Data ma jeden Kontakt
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.Cascade); // Usunięcie Kontaktu usuwa jego Daty
        });

        // --- Konfiguracja Category (bez zmian) ---
        modelBuilder.Entity<Category>(categoryBuilder =>
        {
            categoryBuilder.HasKey(category => category.Id);
            categoryBuilder.Property(category => category.Name)
                .IsRequired();
        });

        // --- Konfiguracja Email (bez zmian) ---
        modelBuilder.Entity<Email>(emailBuilder =>
        {
            emailBuilder.HasKey(email => email.Id);
            emailBuilder.OwnsOne(email => email.Address, addressBuilder =>
            {
                addressBuilder.Property(a => a.Address).HasColumnName("EmailAddress");
            });
            emailBuilder.HasOne(email => email.Category)
                .WithMany() 
                .HasForeignKey(email => email.CategoryId)
                .IsRequired();
        });

        // --- Konfiguracja Phone (bez zmian) ---
        modelBuilder.Entity<Phone>(phoneBuilder =>
        {
            phoneBuilder.HasKey(phone => phone.Id);
            phoneBuilder.OwnsOne(phone => phone.Number, numberBuilder =>
            {
                numberBuilder.Property(n => n.Number).HasColumnName("PhoneNumber");
            });
            phoneBuilder.HasOne(phone => phone.Category)
                .WithMany() 
                .HasForeignKey(phone => phone.CategoryId)
                .IsRequired(); 
        });
        
        // --- NOWA KONFIGURACJA DLA WAŻNEJ DATY (IMPORTANTDATE) ---
        modelBuilder.Entity<ImportantDate>(dateBuilder =>
        {
            dateBuilder.HasKey(date => date.Id);
            
            // Konfiguracja daty
            dateBuilder.Property(d => d.Date).IsRequired();

            // Konfiguracja obiektu wartości (VO) dla opisu
            dateBuilder.OwnsOne(date => date.Description, descBuilder =>
            {
                descBuilder.Property(d => d.Value).HasColumnName("Description");
            });
        });
    }
}