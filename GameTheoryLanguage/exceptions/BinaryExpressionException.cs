using System;

public class BinaryExpressionException : Exception
{
    public BinaryExpressionException()
    {
    }

    public BinaryExpressionException(string message)
        : base(message)
    {
    }

    public BinaryExpressionException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
