using Ecommerce_Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_Application.Repository
{
    internal interface IOrderProcessorRepository
    {
        Customer GetCustomerByName(string customerName);
        Customer GetCustomerByEmail(string customerEmail);
        Product GetProductById(int productId);
        bool ClearCart(Customer customer);
        bool createProduct(Product product);
        bool UpdateProduct(Product product);
        public void DisplayAllProducts();
        bool createCustomer(Customer customer);
        //int createCustomerInt(Customer customer);
        bool UpdateCustomer(Customer customer);
        bool deleteProduct(int productId);
        bool deleteCustomer(int customerId);
        bool addToCart(Customer customer, Product product, int quantity);
        bool removeFromCart(Customer customer, Product product);
        List<Product> getAllFromCart(Customer customer);
        bool placeOrder(Customer customer, List<Dictionary<Product, int>> products, string shippingAddress);
        List<Order> GetOrdersByCustomer(int customerId);
        int GetLatestOrderId(int customerId);

        List<OrderItem> GetOrderDetails(int orderId);

    }
}
