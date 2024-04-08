using System;

public class VariableNotFoundException : Exception
{
    public VariableNotFoundException()
    {
    }

    public VariableNotFoundException(string message)
        : base(message)
    {
    }

    public VariableNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
