namespace Application.Exceptions;

public class DoctorNotFoundException : Exception
{
    public DoctorNotFoundException()
        : base("Doctor not found.")
    {
    }

    public DoctorNotFoundException(string message)
        : base(message)
    {
    }
}