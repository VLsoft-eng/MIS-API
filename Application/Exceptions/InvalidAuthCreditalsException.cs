namespace Application.Exceptions;

public class InvalidAuthCredentialsException : Exception
{
    public InvalidAuthCredentialsException()
        : base("Invalid password or email.")
    {
    }

    public InvalidAuthCredentialsException(string message)
        : base(message)
    {
    }
}