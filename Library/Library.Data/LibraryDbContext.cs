using Library.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Library.Data;

public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Reader> Readers => Set<Reader>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Title).IsRequired();
            b.Property(x => x.Author).IsRequired();
            b.ComplexProperty(x => x.Isbn, isbn => isbn.Property(i => i.Value).HasColumnName("Isbn").IsRequired());
        });

        modelBuilder.Entity<Reader>(r =>
        {
            r.HasKey(x => x.Id);
            r.Property(x => x.Id).ValueGeneratedNever();
            r.Property(x => x.FullName).IsRequired();
            r.Property(x => x.Email).IsRequired();
        });

        modelBuilder.Entity<Loan>(l =>
        {
            l.HasKey(x => x.Id);
            l.Property(x => x.Id).ValueGeneratedNever();
        });
    }
}