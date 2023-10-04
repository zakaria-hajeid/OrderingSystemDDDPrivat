using Ordering.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Sahred
{
    public class Result
    {
        protected internal Result(bool isSuccess,Error error)
        {
            if(isSuccess && error != Error.None)
            {
                throw new InvalidOperationException();
            }
            if (!isSuccess && error == Error.None)
            {
                throw new InvalidOperationException();
            }
            IsSuccess = isSuccess;
            Error = error;
        }
        public bool IsSuccess { get; }
        public bool IsFailuer => !IsSuccess;
        public Error  Error { get; }
        public static Result success() => new Result(true, Error.None);
        public static Result<TValue> success<TValue>(TValue value) => new (value, true, Error.None);
        public static Result<TValue> Failure<TValue>(Error error) => new(false, error);
        public static Result<TValue> Create<TValue>(TValue? value) => value is not null ? success(value):
            Failure<TValue>(Error.NullValue);
       
    }

    public class Test
    {
        Result<object> x()
        {

            var ss= Result.Failure<object>(DomainErrors.order.createdOrderError);
            object re = ss.Value;
            return re;
            //or
            return re;
        }
    }
}
