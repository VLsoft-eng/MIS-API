namespace Application.Exceptions;

public class PatientNotFoundException : Exception
{
    public PatientNotFoundException()
        : base("Speciality not found.")
    {
    }

    public PatientNotFoundException(string message)
        : base(message)
    {
    }
}