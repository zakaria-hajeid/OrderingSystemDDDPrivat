using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Sahred
{
    public  interface IValidationResult
    {
        public static readonly Error ValidationError = new("VlidationError", "Validation problem occured");
        public Error[] Errors { get; }
    }
}
