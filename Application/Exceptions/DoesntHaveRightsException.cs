namespace Application.Exceptions;

public class DoesntHaveRightsException : Exception
{
    public DoesntHaveRightsException()
        : base("User doesn't have editing rights (not the inspection author")
    {
    }

    public DoesntHaveRightsException(string message)
        : base(message)
    {
    }
}