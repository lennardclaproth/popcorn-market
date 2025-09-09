namespace PopcornMarket.SharedKernel.Exceptions;

public class RequiredPropertyIsNullException : Exception
{
    public RequiredPropertyIsNullException(string message) : base(message) { }
}
