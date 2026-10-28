using Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext: DbContext {
	public DbSet<Contact> Contacts => Set<Contact>();
	public DbSet<Category> Categories => Set<Category>();
	public DbSet<User> Users => Set<User>();

	public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

	protected override void OnModelCreating(ModelBuilder modelBuilder){
		modelBuilder.Entity<User>(userBuilder => {
			userBuilder.HasKey(user => user.Id);
			userBuilder.Property(user => user.Id).ValueGeneratedNever();
			userBuilder.Property(user => user.UserName).IsRequired();
			userBuilder.Property(user=>user.Password).IsRequired();
			userBuilder.HasIndex(user=>user.UserName).IsUnique();
		});

		modelBuilder.Entity<Contact>(contactBuilder => {
			contactBuilder.HasKey(contact => contact.Id);
			contactBuilder.Property(contact => contact.Id).ValueGeneratedNever();
		});

		modelBuilder.Entity<Category>(categoryBuilder => {
			categoryBuilder.HasKey(category => category.Id);
			categoryBuilder.Property(category => category.Id).ValueGeneratedNever();
			categoryBuilder.Property(category => category.Name).IsRequired();
			categoryBuilder.HasIndex(category => new {category.UserId, category.Name}).IsUnique();
			categoryBuilder.HasOne<User>().WithMany().HasForeignKey(category => category.UserId);
		});

		modelBuilder.Entity<Contact>().OwnsMany(
			contact => contact.Emails,
			emailBuilder => {
				emailBuilder.WithOwner().HasForeignKey("ContactId");
				emailBuilder.Property<int>("Id");
				emailBuilder.HasKey("Id");
				emailBuilder.HasOne<Category>().WithMany().HasForeignKey(email => email.CategoryId).OnDelete(DeleteBehavior.Restrict);
				emailBuilder.Property(email => email.Address).HasConversion(address => address.Value, value => ParseEmailAddress(value));
			}
		);

		modelBuilder.Entity<Contact>().OwnsMany(
    		contact => contact.Phones,
    		phoneBuilder => {
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
            	.HasConversion(number => number.Value, value => ParsePhoneNumber(value));
    	});

		modelBuilder.Entity<Contact>().OwnsMany(
    		contact => contact.ImportantDates,
    		dateBuilder => {
        		dateBuilder.WithOwner().HasForeignKey("ContactId");
        		dateBuilder.Property<int>("Id");
        		dateBuilder.HasKey("Id");
        		dateBuilder.Property(d => d.Description).IsRequired();
        		dateBuilder.Property(d => d.Date).IsRequired();
    	});

		modelBuilder.Entity<Contact>()
    		.Navigation(c => c.ImportantDates)
    		.UsePropertyAccessMode(PropertyAccessMode.Field);

		modelBuilder.Entity<Contact>()
    		.Navigation(c => c.Phones)
    		.UsePropertyAccessMode(PropertyAccessMode.Field);

		modelBuilder.Entity<Contact>().Navigation(c => c.Emails).UsePropertyAccessMode(PropertyAccessMode.Field);
		modelBuilder.Entity<Contact>().ComplexProperty(contact => contact.Age, ageBuilder => ageBuilder.Property(age => age.Value));
		modelBuilder.Entity<Contact>().ComplexProperty(
			contact => contact.FirstName,
			firstNameBuilder => firstNameBuilder.Property(fn => fn.Value).IsRequired()
		);
		modelBuilder.Entity<Contact>().ComplexProperty(
			contact => contact.LastName,
			lastNameBuilder => lastNameBuilder.Property(fn => fn.Value).IsRequired()
		);
	}

	private static EmailAddress ParseEmailAddress(string value) => EmailAddress.Create(value).Match(address => address, error => throw new InvalidOperationException(error));
	private static PhoneNumber ParsePhoneNumber(string value) => PhoneNumber.Create(value).Match(number => number, error => throw new InvalidOperationException(error));
}
