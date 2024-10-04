using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_Application.Exception
{
    internal class OrderNotFoundException: System.Exception
    {
        public int OrderId { get; }

        public OrderNotFoundException(int orderId)
            : base($"Order with ID {orderId} was not found.")
        {
            OrderId = orderId;
        }
    }
}
