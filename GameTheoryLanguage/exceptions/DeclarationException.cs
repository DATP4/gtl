using System;

public class DeclarationException : Exception
{
    public DeclarationException()
    {
    }

    public DeclarationException(string message)
        : base(message)
    {
    }

    public DeclarationException(string message, Exception inner)
        : base(message, inner)
    {
    }
    public DeclarationException(string type1, string type2)
    {
        string message = $"Declaration expected type {type1} but recieved {type2}.";
        throw new DeclarationException(message);
    }
}
