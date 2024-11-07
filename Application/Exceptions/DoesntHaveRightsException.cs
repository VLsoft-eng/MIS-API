namespace Application.Exceptions;

public class DoesntHaveRightsException : Exception
{
    public DoesntHaveRightsException()
        : base("Doesn't have creating/editing rights in current resource. ")
    {
    }

    public DoesntHaveRightsException(string message)
        : base(message)
    {
    }
}