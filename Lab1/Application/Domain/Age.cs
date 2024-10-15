namespace Contacts.Application.Domain;

public record Age
{
    public int Value {get;}
    public Age(int value){
        if(value < 18 && value<120)
            throw new ArgumentOutOfRangeException(nameof(value));
            Value = value; 
    }
    public static implicit operator int(Age age) => age.Value;
    public static implicit operator Age(int value) => new(value);
}
