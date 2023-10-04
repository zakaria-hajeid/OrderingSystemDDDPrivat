using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Sahred
{
    public class Error : IEquatable<Error>
    {
        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error NullValue = new("Erroe.NullValue", "The specific result is null");

        public Error(string code ,string message) {
            Code = code;
            Message = message;
        }
        public string Code { get; }
        public string Message { get; }

        public bool Equals(Error? other)
        {
            throw new NotImplementedException();
        }
    }
}
