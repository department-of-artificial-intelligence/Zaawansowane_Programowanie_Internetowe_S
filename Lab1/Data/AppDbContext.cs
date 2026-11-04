using Application.Domain;
using Microsoft.EntityFrameworkCore;
namespace Data;
public class AppDbContext : DbContext
{
public DbSet<Contact> Contacts => Set<Contact>();
public DbSet<Category> Categories => Set<Category>();
public DbSet<User> Users => Set<User>();
public AppDbContext(DbContextOptions<AppDbContext> options)
: base(options)
{}
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
modelBuilder.Entity<Contact>(contactBuilder =>
{
contactBuilder.HasKey(contact => contact.Id);
contactBuilder
.Property(contact => contact.Id)
.ValueGeneratedNever();
contactBuilder.Property(contact => contact.UserId).IsRequired();
contactBuilder.HasOne<User>().WithMany().HasForeignKey(contact => contact.UserId);
contactBuilder.ComplexProperty(contact => contact.FirstName,
builder => builder.Property(value => value.Value).HasColumnName("FirstName"));
contactBuilder.ComplexProperty(contact => contact.LastName,
builder => builder.Property(value => value.Value).HasColumnName("LastName"));
});
modelBuilder.Entity<Category>(categoryBuilder =>
{
categoryBuilder.HasKey(category => category.Id);
categoryBuilder
.Property(category => category.Id)
.ValueGeneratedNever();
categoryBuilder.Property(category => category.Name).IsRequired();
categoryBuilder.Property(category => category.UserId).IsRequired();
categoryBuilder
.HasIndex(category => new {category.UserId, category.Name})
.IsUnique();
categoryBuilder
.HasOne<User>()
.WithMany()
.HasForeignKey(category => category.UserId);


});
modelBuilder.Entity<User>(userBuilder =>
{
userBuilder.HasKey(user => user.Id);
userBuilder.Property(user => user.Id).ValueGeneratedNever();
userBuilder.Property(user => user.UserName).IsRequired();
userBuilder.Property(user => user.Password).IsRequired();
userBuilder.Property(user => user.FirstName).IsRequired();
userBuilder.Property(user => user.LastName).IsRequired();
userBuilder.Property(user => user.Address).IsRequired();
userBuilder.Property(user => user.RefreshToken);
userBuilder.Property(user => user.RefreshTokenExpiresAt);
userBuilder.HasIndex(user => user.UserName).IsUnique();
});
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
.HasForeignKey(email => email.CategoryId);
emailBuilder
.Property(email => email.Address)
.HasConversion(
address => address.Value,
value => ParseEmailAddress(value));
});
modelBuilder.Entity<Contact>()
.Navigation(c => c.Emails)
.UsePropertyAccessMode(PropertyAccessMode.Field);
modelBuilder.Entity<Contact>().OwnsMany(
contact => contact.PhoneNumbers,
phoneBuilder =>
{
phoneBuilder.WithOwner().HasForeignKey("ContactId");
phoneBuilder.Property<int>("Id");
phoneBuilder.HasKey("Id");
phoneBuilder.Property(phone => phone.Value).IsRequired();
});
modelBuilder.Entity<Contact>().OwnsMany(
contact => contact.ImportantDates,
dateBuilder =>
{
dateBuilder.WithOwner().HasForeignKey("ContactId");
dateBuilder.Property<int>("Id");
dateBuilder.HasKey("Id");
dateBuilder.Property(date => date.Name).IsRequired();
dateBuilder.Property(date => date.Date).IsRequired();
});
modelBuilder.Entity<Contact>()
.Navigation(c => c.PhoneNumbers)
.UsePropertyAccessMode(PropertyAccessMode.Field);
modelBuilder.Entity<Contact>()
.Navigation(c => c.ImportantDates)
.UsePropertyAccessMode(PropertyAccessMode.Field);
modelBuilder.Entity<Contact>().ComplexProperty(
contact => contact.Age,
ageBuilder => ageBuilder.Property(age => age.Value)
);
}
private static EmailAddress ParseEmailAddress(string value) =>
EmailAddress
.Create(value)
.Match(
address => address,
error => throw new InvalidOperationException(error));
}
