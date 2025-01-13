using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Sahred
{
    public class Result<TValue> : Result
    {
        private readonly TValue? _payload;
        protected internal Result(TValue? payload,bool isSuccess,Error error,string SuccessMesssage) :base(isSuccess,error, SuccessMesssage) =>
            _payload = payload;
        protected internal Result(bool isSuccess, Error error, string SuccessMesssage) : base(isSuccess, error, SuccessMesssage) {
            
        }
        public TValue  Payload=>IsSuccess?_payload!:
            throw new NotImplementedException("The Value of a failuer can not be accessed");

        public static implicit operator Result<TValue>(TValue payloadObject) => Create(payloadObject);

    }
}
