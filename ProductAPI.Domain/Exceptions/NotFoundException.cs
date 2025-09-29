using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPI.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(Guid productId)
        : base($"Product with Id '{productId}' was not found.")
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
