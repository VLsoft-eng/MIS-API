namespace Application.Exceptions;

public class InvalidPaginationParamsException : Exception
{ 
    public InvalidPaginationParamsException()
        : base("Pagination parameters must be more than 0. ")
    {
    }

    public InvalidPaginationParamsException(string message)
        : base(message)
    {
    }
}