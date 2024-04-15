using System;

public class BooleanExpressionException : Exception
{
    public BooleanExpressionException
()
    {
    }

    public BooleanExpressionException
(string message)
        : base(message)
    {
    }

    public BooleanExpressionException
(string message, Exception inner)
        : base(message, inner)
    {
    }
    public BooleanExpressionException(string type1, string type2)
    {
        string message = $"Boolean expression expected type {type1} but recieved {type2}.";
        throw new BooleanExpressionException(message);
    }
}
