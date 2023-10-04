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
        private readonly TValue? _value;
        protected internal Result(TValue? value,bool isSuccess,Error error ):base(isSuccess,error)=>
            _value = value;
        protected internal Result(bool isSuccess, Error error) : base(isSuccess, error) {
            
        }
        public TValue  Value=>IsSuccess?_value!:
            throw new NotImplementedException("The Value of a failuer can not be accessed");

        public static implicit operator Result<TValue>(TValue? value) => Create(value);
    }
}
