using System;

public class FunctionCallException : Exception
{
    public FunctionCallException()
    {
    }

    public FunctionCallException(string message)
        : base(message)
    {
    }

    public FunctionCallException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
