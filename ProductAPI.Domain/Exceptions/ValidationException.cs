using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPI.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
     : base("One or more validation failures have occurred.")
        {
            // You can add a property to hold validation errors if needed
        }

        public ValidationException(string message)
            : base(message)
        {
        }
    }
}
