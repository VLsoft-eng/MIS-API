namespace Application.Exceptions;

public class SpecialityNotFoundException : Exception
{
    public SpecialityNotFoundException()
        : base("Speciality not found.")
    {
    }

    public SpecialityNotFoundException(string message)
        : base(message)
    {
    }
}