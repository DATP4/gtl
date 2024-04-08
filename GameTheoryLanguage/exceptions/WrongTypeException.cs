using System;

public class WrongTypeException : Exception
{
    public WrongTypeException()
    {
    }

    public WrongTypeException(string message)
        : base(message)
    {
    }

    public WrongTypeException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
