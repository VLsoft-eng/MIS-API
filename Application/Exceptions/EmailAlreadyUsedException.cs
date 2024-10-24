namespace Application.Exceptions;

public class EmailAlreadyUsedException : Exception
{
    public EmailAlreadyUsedException()
        : base("Email already used.")
    {
    }

    public EmailAlreadyUsedException(string message)
        : base(message)
    {
    }
}