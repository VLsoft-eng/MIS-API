namespace Application.Exceptions;

public class InspectionNotRootException : Exception
{
    public InspectionNotRootException()
        : base("Inspection not root.")
    {
    }

    public InspectionNotRootException(string message)
        : base(message)
    {
    }
}