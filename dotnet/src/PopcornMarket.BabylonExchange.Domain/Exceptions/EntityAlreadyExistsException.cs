using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.BabylonExchange.Domain.Exceptions;

public sealed class EntityAlreadyExistsException<TEntity> : Exception
    where TEntity : Entity
{
    public string Identifier { get; }

    public EntityAlreadyExistsException(string identifier)
        : base($"Entity '{typeof(TEntity).Name}' already exists with identifier '{identifier}'.")
    {
        Identifier = identifier;
    }
}
