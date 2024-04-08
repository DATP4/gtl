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
}
