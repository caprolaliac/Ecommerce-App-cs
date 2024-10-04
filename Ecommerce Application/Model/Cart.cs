using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_Application.Model
{
    internal class Cart
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; } 
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
