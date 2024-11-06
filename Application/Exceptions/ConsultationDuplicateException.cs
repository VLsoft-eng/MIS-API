namespace Application.Exceptions;

public class ConsultationDuplicateException : Exception
{
    public ConsultationDuplicateException()
        : base("Consultations must have unique speciality in inspection bounds.")
    {
    }

    public ConsultationDuplicateException(string message)
        : base(message)
    {
    }
}