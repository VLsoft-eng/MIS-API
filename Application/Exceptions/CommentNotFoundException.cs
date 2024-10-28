namespace Application.Exceptions;

public class CommentNotFoundException : Exception
{
    public CommentNotFoundException()
        : base("Comment not found.")
    {
    }

    public CommentNotFoundException(string message)
        : base(message)
    {
    }
}