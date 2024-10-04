using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_Application.Exception
{
    internal class ProductNotFoundException: System.Exception
    {
        public int ProductId { get; }

        public ProductNotFoundException(int productId)
            : base($"Product with ID {productId} was not found.")
        {
            ProductId = productId;
        }
    }
}
