namespace Application.Exceptions;

public class ConsultationNotFoundException : Exception
{
    public ConsultationNotFoundException()
        : base("Consultation not found.")
    {
    }

    public ConsultationNotFoundException(string message)
        : base(message)
    {
    }
}