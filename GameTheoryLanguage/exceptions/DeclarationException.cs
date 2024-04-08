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
}
