namespace Application.Exceptions;

public class InspectionNotFoundException : Exception
{
    public InspectionNotFoundException()
        : base("Inspection with current id not found.")
    {
    }

    public InspectionNotFoundException(string message)
        : base(message)
    {
    }
}