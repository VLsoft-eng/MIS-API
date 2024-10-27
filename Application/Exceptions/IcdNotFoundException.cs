namespace Application.Exceptions;

public class IcdNotFoundException : Exception
{
    public IcdNotFoundException()
        : base("Icd with current id not found.")
    {
    }

    public IcdNotFoundException(string message)
        : base(message)
    {
    }
}