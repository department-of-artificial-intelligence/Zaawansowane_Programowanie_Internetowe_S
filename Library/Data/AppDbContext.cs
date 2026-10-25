using Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Reader> Readers => Set<Reader>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(bookBuilder =>
        {
            bookBuilder.HasKey(b => b.Id);
            bookBuilder.Property(b => b.Id).ValueGeneratedNever();
            
            bookBuilder.ComplexProperty(
                b => b.BookTitle,
                titleBuilder => titleBuilder.Property(t => t.Value).HasColumnName("Title")
            );

            bookBuilder.ComplexProperty(
                b => b.BookCopies,
                copiesBuilder => copiesBuilder.Property(c => c.Value).HasColumnName("Copies")
            );

            bookBuilder.ComplexProperty(
                b => b.Author,
                authorBuilder =>
                {
                    authorBuilder.ComplexProperty(
                        a => a.Name,
                        nameBuilder =>
                        {
                            nameBuilder.Property(n => n.FirstName).HasColumnName("AuthorFirstName");
                            nameBuilder.Property(n => n.LastName).HasColumnName("AuthorLastName");
                        }
                    );
                }
            );
        });

        modelBuilder.Entity<Reader>(readerBuilder =>
        {
            readerBuilder.HasKey(r => r.Id);
            readerBuilder.Property(r => r.Id).ValueGeneratedNever();
            
            readerBuilder.ComplexProperty(
                r => r.Name,
                nameBuilder =>
                {
                    nameBuilder.Property(n => n.FirstName).HasColumnName("FirstName");
                    nameBuilder.Property(n => n.LastName).HasColumnName("LastName");
                }
            );
            
            readerBuilder.OwnsOne(
                r => r.Email,
                emailBuilder =>
                {
                    emailBuilder.Property(e => e.Value)
                        .HasColumnName("Email")
                        .IsRequired();

                    emailBuilder.HasIndex(e => e.Value).IsUnique();
                }
            );
        });
    }
}