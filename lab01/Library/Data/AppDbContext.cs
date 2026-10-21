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
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(bookBuilder =>
        {
            bookBuilder.HasKey(book => book.Id);
            bookBuilder.Property(book => book.Id).ValueGeneratedNever();
            bookBuilder
                .Property(book => book.Isbn)
                .HasConversion(
                    isbn => isbn.Value,
                    value => ParseIsbn(value))
                .IsRequired();
            bookBuilder.HasIndex(book => book.Isbn).IsUnique();
            bookBuilder.Property(book => book.Title).IsRequired();
            bookBuilder.Property(book => book.Author).IsRequired();
        });

        modelBuilder.Entity<Reader>(readerBuilder =>
        {
            readerBuilder.HasKey(reader => reader.Id);
            readerBuilder.Property(reader => reader.Id).ValueGeneratedNever();
            readerBuilder.Property(reader => reader.FirstName).IsRequired();
            readerBuilder.Property(reader => reader.LastName).IsRequired();
        });

        // Wypożyczenia i rezerwacje odwołują się do książek i czytelników tylko przez
        // identyfikator (osobne agregaty), bez kluczy obcych - historia wypożyczeń
        // pozostaje po usunięciu książki lub czytelnika.
        modelBuilder.Entity<Loan>(loanBuilder =>
        {
            loanBuilder.HasKey(loan => loan.Id);
            loanBuilder.Property(loan => loan.Id).ValueGeneratedNever();
            loanBuilder.HasIndex(loan => loan.BookId);
            loanBuilder.HasIndex(loan => loan.ReaderId);
        });

        modelBuilder.Entity<Reservation>(reservationBuilder =>
        {
            reservationBuilder.HasKey(reservation => reservation.Id);
            reservationBuilder.Property(reservation => reservation.Id).ValueGeneratedNever();
            reservationBuilder.Property(reservation => reservation.Status).HasConversion<string>();
            reservationBuilder.HasIndex(reservation => reservation.BookId);
            reservationBuilder.HasIndex(reservation => reservation.ReaderId);
        });
    }

    private static Isbn ParseIsbn(string value) =>
        Isbn
            .Create(value)
            .Match(
                isbn => isbn,
                error => throw new InvalidOperationException(error));
}
