namespace PopcornMarket.SharedKernel.Primitives;

public abstract class Entity : IEquatable<Entity>
{
    protected Entity() { }

    protected Entity(Guid id)
    {
        Id = id;
    }
    public Guid Id { get; private init; }

    public static bool operator ==(Entity? first, Entity? second)
    {
        // If either is null, do a reference check
        if (ReferenceEquals(first, null) && ReferenceEquals(second, null))
            return true;
        if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            return false;

        // Otherwise, both not null, fallback to first.Equals(second).
        return first.Equals(second);
    }

    public static bool operator !=(Entity? first, Entity? second)
    {
        return !(first == second);
    }

    public bool Equals(Entity? other)
    {
        if (other == null)
        {
            return false;
        }

        if (other.GetType() != typeof(Entity))
        {
            return false;
        }

        return other.Id == Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj.GetType() != typeof(Entity))
        {
            return false;
        }

        if (obj is not Entity)
        {
            return false;
        }
        
        return Equals(obj as Entity);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
