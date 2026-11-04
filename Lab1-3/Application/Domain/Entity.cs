namespace Application.Domain;
public abstract class Entity(Guid id) : IEquatable<Entity>
{
public Guid Id {get;} = id;
public bool Equals(Entity? other)
{
if (other is null) return false;
if (ReferenceEquals(this, other)) return true;
return Id == other.Id;
}
public override bool Equals(object? obj) => Equals(obj as Entity);
public override int GetHashCode() => Id.GetHashCode();
public static bool operator ==(Entity? left, Entity? right) =>
left is null ? right is null : left.Equals(right);
public static bool operator !=(Entity? left, Entity? right) =>
!(left == right);
}
