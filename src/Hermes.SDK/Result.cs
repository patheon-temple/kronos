using System;

namespace Hermes.SDK
{
    public class Result<T> where T : Enum
    {
        public Result(T status)
        {
            Status = status;
        }

        public T Status { get; }
    }

    public class Result<T, TU> : Result<T> where T : Enum
    {
        public Result(T status, TU data) : base(status)
        {
            Data = data;
        }

        public TU Data { get; }
    }
}