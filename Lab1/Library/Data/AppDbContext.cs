using Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext : DbContext
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Reader> Readers => Set<Reader>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(bookBuilder =>
        {
            bookBuilder.HasKey(book => book.Id);
            bookBuilder.Property(book => book.Id).ValueGeneratedNever();
            bookBuilder.Property(book => book.Title).IsRequired();
            bookBuilder.Property(book => book.Author).IsRequired();
            bookBuilder
                .Property(book => book.Isbn)
                .HasConversion(
                    isbn => isbn.Value,
                    value => ParseIsbn(value))
                .IsRequired();
            bookBuilder.HasIndex(book => book.Isbn).IsUnique();
        });

        modelBuilder.Entity<Reader>(readerBuilder =>
        {
            readerBuilder.HasKey(reader => reader.Id);
            readerBuilder.Property(reader => reader.Id).ValueGeneratedNever();
            readerBuilder.Property(reader => reader.FirstName).IsRequired();
            readerBuilder.Property(reader => reader.LastName).IsRequired();
            readerBuilder
                .Property(reader => reader.Email)
                .HasConversion(
                    email => email.Value,
                    value => ParseEmailAddress(value))
                .IsRequired();
            readerBuilder.HasIndex(reader => reader.Email).IsUnique();
        });

        modelBuilder.Entity<Loan>(loanBuilder =>
        {
            loanBuilder.HasKey(loan => loan.Id);
            loanBuilder.Property(loan => loan.Id).ValueGeneratedNever();
            loanBuilder
                .HasOne<Book>()
                .WithMany()
                .HasForeignKey(loan => loan.BookId)
                .OnDelete(DeleteBehavior.Restrict);
            loanBuilder
                .HasOne<Reader>()
                .WithMany()
                .HasForeignKey(loan => loan.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Reservation>(reservationBuilder =>
        {
            reservationBuilder.HasKey(reservation => reservation.Id);
            reservationBuilder
                .Property(reservation => reservation.Id)
                .ValueGeneratedNever();
            reservationBuilder
                .HasOne<Book>()
                .WithMany()
                .HasForeignKey(reservation => reservation.BookId)
                .OnDelete(DeleteBehavior.Restrict);
            reservationBuilder
                .HasOne<Reader>()
                .WithMany()
                .HasForeignKey(reservation => reservation.ReaderId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static Isbn ParseIsbn(string value) =>
        Isbn
            .Create(value)
            .Match(
                isbn => isbn,
                error => throw new InvalidOperationException(error));

    private static EmailAddress ParseEmailAddress(string value) =>
        EmailAddress
            .Create(value)
            .Match(
                address => address,
                error => throw new InvalidOperationException(error));
}
