using System;
using System.Runtime.Serialization;

namespace Hermes.SDK
{
    public sealed class UnhandledBranchError : Exception
    {
        public UnhandledBranchError(Enum type)
        {
            Type = type;
        }

        public UnhandledBranchError(SerializationInfo info, StreamingContext context, Enum type) : base(info, context)
        {
            Type = type;
        }

        public UnhandledBranchError(string message, Enum type) : base(message)
        {
            Type = type;
        }

        public UnhandledBranchError(string message, Exception innerException, Enum type) : base(message, innerException)
        {
            Type = type;
        }

        public Enum Type { get; }
        
    }
    
    public class OperationError
    {
        public string Message { get; }
        public OperationError(string message)
        {
            Message = message;
        }
        public OperationError(Exception exception)
        {
            Message = exception.ToString();
        }
    }
    
    public  class OperationError<T> : OperationError where T : struct, Enum
    {
        public OperationError(string message, T? type) : base(message)
        {
            Type = type;
        }

        public OperationError(Exception exception, T? type) : base(exception)
        {
            Type = type;
        }

        public T? Type { get; }
    }
}