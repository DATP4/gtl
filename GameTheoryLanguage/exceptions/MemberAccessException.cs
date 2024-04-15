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
    public MemberAccessException(string type1, string type2)
    {
        string message = $"MemberAccess expected type {type1} but recieved {type2}.";
        throw new MemberAccessException(message);
    }
}
