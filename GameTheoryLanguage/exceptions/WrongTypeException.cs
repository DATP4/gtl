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
    public WrongTypeException(string stmtType, string type1, string type2)
    {
        string message = $"{stmtType} expected type {type1} but recieved {type2}.";
        throw new WrongTypeException(message);
    }
}
