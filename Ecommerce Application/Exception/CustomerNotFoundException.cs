using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_Application.Exception
{
    internal class CustomerNotFoundException : System.Exception
    {
        public int CustomerId { get; }

        public CustomerNotFoundException(int customerId)
            : base($"Customer with ID {customerId} was not found.")
        {
            CustomerId = customerId;
        }
    }
}
