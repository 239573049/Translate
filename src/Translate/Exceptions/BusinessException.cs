using System;

namespace Translate.Exceptions;

public class BusinessException : Exception
{
    public int Code { get; set; }

    public BusinessException()
    {
    }

    public BusinessException(string? message) : base(message)
    {
    }

    public BusinessException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}