namespace Application.Exceptions;

public class AlreadyLogoutException : Exception
{
    public AlreadyLogoutException()
        : base("You already logout.")
    {
    }

    public AlreadyLogoutException(string message)
        : base(message)
    {
    }
}