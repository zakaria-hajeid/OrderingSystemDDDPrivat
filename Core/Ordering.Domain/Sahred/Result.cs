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
        protected internal Result(bool isSuccess, Error error, string successMesssage)
        {
            if (isSuccess && error != Error.None)
            {
                throw new InvalidOperationException();
            }
            if (!isSuccess && error == Error.None)
            {
                throw new InvalidOperationException();
            }
            IsSuccess = isSuccess;
            Error = error;
            SuccessMesssage = successMesssage;
        }
        public bool IsSuccess { get; }
        public bool IsFailuer => !IsSuccess;
        public Error Error { get; }
        public string SuccessMesssage { get; set; }

        //static method  
        public static Result success(string message = "") => new Result(true, Error.None, message);
        public static Result<TValue> success<TValue>(TValue value, string message = "") => new(value, true, Error.None, message);
        public static Result<TValue> Failure<TValue>(Error error) => new(false, error, "");
         public static Result<TValue> Create<TValue>(TValue? value) => value is not null ? success(value, "Success Comand") :
             Failure<TValue>(Error.NullValue);

    }

    public class Test
    {
        /*Result<object> x()
        {

            var ss = Result.Failure<object>(DomainErrors.order.createdOrderError);
            object re = ss.Payload;
            //or
        }*/
    }
}
