using Microsoft.EntityFrameworkCore;
using WebApiAuth.Models; // ⬅️ namespace z Twoim modelem User

namespace WebApiAuth.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>(); // tabela Users w bazie danych

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
