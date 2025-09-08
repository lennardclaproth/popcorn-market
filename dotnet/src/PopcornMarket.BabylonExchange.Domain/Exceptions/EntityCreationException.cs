using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.BabylonExchange.Domain.Exceptions;
public sealed class EntityCreationException<TEntity> : Exception 
    where TEntity : Entity
{
    public string ErrorMessage { get; }

    public EntityCreationException(string message) : base($"Failed to create entity of type '{typeof(TEntity).Name}', {message}") 
    {
        ErrorMessage = message;
    }
}
