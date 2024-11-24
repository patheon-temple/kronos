using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.SDK.UseCases
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

    public sealed class CreateTokenSetUseCase
    {
        public enum Result
        {
            Unknown = 0,
            Success = 1,
            Failure = 2,
        }
        public async Task<Result<Result>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return new Result<Result>(Result.Success);
        }
    }
}