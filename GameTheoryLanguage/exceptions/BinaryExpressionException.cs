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
    public BinaryExpressionException(string type1, string type2)
    {
        string message = $"Binary expression expected type {type1} but recieved {type2}.";
        throw new BinaryExpressionException(message);
    }
}
