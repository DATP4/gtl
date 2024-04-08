using System;

public class MemberAccessException : Exception
{
    public MemberAccessException()
    {
    }

    public MemberAccessException(string message)
        : base(message)
    {
    }

    public MemberAccessException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
