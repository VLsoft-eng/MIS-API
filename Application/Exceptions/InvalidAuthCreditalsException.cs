namespace Application.Exceptions;

public class InvalidAuthCredentialsException : Exception
{
    public InvalidAuthCredentialsException()
        : base("Wrong password or email.")
    {
    }

    public InvalidAuthCredentialsException(string message)
        : base(message)
    {
    }
}