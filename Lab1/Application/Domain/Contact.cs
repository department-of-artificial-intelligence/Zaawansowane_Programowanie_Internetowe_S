using Application.Utilities;

namespace Application.Domain;

public class Contact: Entity{
	private readonly List<Email> _emails = [];
	private readonly List<Phone> _phones = [];
	private readonly List<ImportantDate> _importantDates = [];

	public FirstName FirstName { get; private set; }
	public LastName LastName { get; private set; }
	public Sex Sex { get; private set; }
	public Age Age { get; private set; }
	public IReadOnlyCollection<Email> Emails => _emails;
	public IReadOnlyCollection<Phone> Phones => _phones;
	public IReadOnlyCollection<ImportantDate> ImportantDates => _importantDates;

	private Contact(Guid id, FirstName firstName, LastName lastName, Sex sex, Age age): this(id, sex){
		FirstName = firstName;
		LastName = lastName;
		Age = age;
	}

	private Contact(Guid id, Sex sex): base(id){
		FirstName = null!;
		LastName = null!;
		Sex = sex;
		Age = null!;
	}

	public bool HasEmail(EmailAddress address) => _emails.Any(e => e.Address == address);
	public bool HasPhone(PhoneNumber number) => _phones.Any(p => p.Number == number);
	public bool HasImportantDate(DateOnly date) => _importantDates.Any(d => d.Date == date);

	public Result<Unit> Update(FirstName firstName, LastName lastName, Sex sex, Age age){
    	FirstName = firstName;
    	LastName = lastName;
    	Sex = sex;
    	Age = age;
    
		return Result.Success(Unit.Instance);
	}

	public Result<Unit> AddEmail(EmailAddress address, Guid categoryId){
		if(HasEmail(address)){
			return Result.Failure<Unit>("Email już istnieje w kontakcie");
		}

		return Email.Create(address, categoryId).Tap(_emails.Add).Map(_ => Unit.Instance);
	}

	public Result<Unit> RemoveEmail(EmailAddress address){
    	var emailToRemove = _emails.SingleOrDefault(e => e.Address == address);
   		if(emailToRemove is null){
        	return Result.Failure<Unit>("Kontakt nie posiada podanego adresu email.");
    	}

    	_emails.Remove(emailToRemove);
    	return Result.Success(Unit.Instance);
	}

	public static Result<Contact> Create(FirstName firstName, LastName lastName, Sex sex, Age age){
		return Result.Success(new Contact(Guid.NewGuid(), firstName, lastName, sex, age));
	}

	public Result<Unit> AddPhone(PhoneNumber number, Guid categoryId){
    	if(HasPhone(number)){
        	return Result.Failure<Unit>("Numer telefonu już istnieje w kontakcie.");
    	}

    	return Phone.Create(number, categoryId).Tap(_phones.Add).Map(_ => Unit.Instance);
	}

	public Result<Unit> RemovePhone(PhoneNumber number){
    	var phoneToRemove = _phones.SingleOrDefault(p => p.Number == number);
    	if(phoneToRemove is null){
        	return Result.Failure<Unit>("Kontakt nie posiada podanego numeru telefonu.");
    	}

    	_phones.Remove(phoneToRemove);
    	return Result.Success(Unit.Instance);
	}
	
	public Result<Unit> AddImportantDate(string description, DateOnly date){
    	if(HasImportantDate(date)){
        	return Result.Failure<Unit>("Taka ważna data już istnieje w kontakcie.");
    	}

		return ImportantDate.Create(description, date).Tap(_importantDates.Add).Map(_ => Unit.Instance);
	}

	public Result<Unit> RemoveImportantDate(string description, DateOnly date){
		var importantDateToRemove = _importantDates.SingleOrDefault(d => d.Date == date);	
		if(importantDateToRemove is null){
			return Result.Failure<Unit>("Kontakt nie posiada podanej ważnej daty.");
		}

		_importantDates.Remove(importantDateToRemove);
		return Result.Success(Unit.Instance);
	}
}
