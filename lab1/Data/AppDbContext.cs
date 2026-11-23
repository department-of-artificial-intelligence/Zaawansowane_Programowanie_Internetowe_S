using Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext : DbContext
{
  public DbSet<Contact> Contacts => Set<Contact>();
  public DbSet<Category> Categories => Set<Category>();
}
