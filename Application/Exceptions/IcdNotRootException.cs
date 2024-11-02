namespace Application.Exceptions;

public class IcdNotRootException : Exception
{
    public IcdNotRootException()
        : base("Icd in IcdRoot not root.")
    {
    }

    public IcdNotRootException(string message)
        : base(message)
    {
    }
}