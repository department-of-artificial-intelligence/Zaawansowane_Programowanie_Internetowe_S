using Application.Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace Data;

public class AppDbContext : DbContext
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Reader> Readers => Set<Reader>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(book =>
        {
            book.HasKey(b => b.Id);
            book.Property(b => b.Id).ValueGeneratedNever();
            
            book.Property(b => b.Isbn)
                .HasConversion(
                    isbn => isbn.Value,
                    value => ParseIsbn(value))
                .IsRequired();
        });

        modelBuilder.Entity<Reader>(reader =>
        {
            reader.HasKey(r => r.Id);
            reader.Property(r => r.Id).ValueGeneratedNever();
            reader.Property(r => r.BorrowedBooks); 
        });
    }

    private static Isbn ParseIsbn(string value) =>
        Isbn.Create(value).Match(
            isbn => isbn,
            error => throw new InvalidOperationException(error));
}