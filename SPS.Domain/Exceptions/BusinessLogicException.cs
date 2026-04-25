namespace SPS.Domain.Exceptions;

public class BusinessLogicException : Exception
{
    public string? ErrorCode { get; }

    public BusinessLogicException(string message, string? errorCode = null)
        : base(message) => ErrorCode = errorCode;

    public BusinessLogicException(string message, Exception innerException, string? errorCode = null)
        : base(message, innerException) => ErrorCode = errorCode;
}