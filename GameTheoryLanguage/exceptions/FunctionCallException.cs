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
    public FunctionCallException(string type1, string type2)
    {
        string message = $"FunctionCall expected type {type1} but recieved {type2}.";
        throw new FunctionCallException(message);
    }
}
