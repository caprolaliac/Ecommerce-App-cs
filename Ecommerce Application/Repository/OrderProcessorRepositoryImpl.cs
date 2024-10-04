using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce_Application.Model;
using Ecommerce_Application.Repository;
using static System.Exception;
using Ecommerce_Application.Exception;
using Student_Information_System.Utility;

namespace Ecommerce_Application.Repository
{
    internal class OrderProcessorRepositoryImpl : IOrderProcessorRepository
    {
        SqlCommand cmd = null;

        public OrderProcessorRepositoryImpl()
        {
            cmd = new SqlCommand();
        }
        public bool ClearCart(Customer customer)
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    string query = "delete from cart WHERE customer_id = @CustomerId";
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                        connection.Open();
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            Console.WriteLine("Cart cleared Successfully");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Please finish order first");
                            return false;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }


        public Customer GetCustomerByName(string customerName)
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    string query = "select * from customers where name = @CustomerName";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@CustomerName", customerName);
                        connection.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Customer
                                {
                                    CustomerId = (int)reader["customer_id"],
                                    Name = reader["name"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Password = reader["password"].ToString()
                                };
                            }
                            else
                            {
                                Console.WriteLine("Customer Not Found, Try Again");
                                return null;
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public Customer GetCustomerByEmail(string customerEmail)
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    string query = "select * from customers where email = @Email";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", customerEmail);
                        connection.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Customer
                                {
                                    CustomerId = (int)reader["customer_id"],
                                    Name = reader["name"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Password = reader["password"].ToString()
                                };
                            }
                            else
                            {
                                Console.WriteLine("Customer Not Found, Try Again");
                                return null;
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }



        public bool addToCart(Customer customer, Product product, int quantity)
        {
            if (product == null)
            {
                throw new ProductNotFoundException(-1);
            }
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    connection.Open();
                    cmd = new SqlCommand("insert into cart (customer_id, product_id, quantity) values (@CustomerId, @ProductId, @Quantity)", connection);
                    cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                    cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        Console.WriteLine("Product added successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Could not add the product.");
                        return false;
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public bool createCustomer(Customer customer)
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    connection.Open();
                    cmd = new SqlCommand("insert into customers (name, email, password) VALUES (@Name, @Email, @Password)", connection);
                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@Password", customer.Password);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        Console.WriteLine($"Customer {customer.Name} created successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Could not create the customer.");
                        return false;
                    }
                }

                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public bool createProduct(Product product)
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    connection.Open();
                    cmd = new SqlCommand("insert into products (name, price, description, stockQuantity) VALUES (@Name, @Price, @Description, @StockQuantity)", connection);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        Console.WriteLine($"Product {product.Name} created successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Could not create the product.");
                        return false;
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public bool deleteCustomer(int customerId)
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    connection.Open();
                    cmd = new SqlCommand("delete from customers WHERE customer_id = @CustomerId", connection);
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        Console.WriteLine("Customer deleted successfully.");
                        return true;
                    }
                    else
                    {
                        throw new CustomerNotFoundException(customerId);
                        //return false;
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public bool deleteProduct(int productId)
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    connection.Open();
                    cmd = new SqlCommand("DELETE FROM Products WHERE product_id = @ProductId", connection);
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        Console.WriteLine("Product deleted successfully.");
                        return true;
                    }
                    else
                    {
                        throw new ProductNotFoundException(productId);
                        //return false;
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public List<Product> getAllFromCart(Customer customer)
        {
            var cart_products = new List<Product>();
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    connection.Open();
                    cmd = new SqlCommand("select p.* from cart c join products p ON c.product_id = p.product_id where c.customer_id = @CustomerId", connection);
                    cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cart_products.Add(new Product
                            {
                                ProductId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                Description = reader.GetString(3),
                                StockQuantity = reader.GetInt32(4)
                            });
                        }
                    }
                    return cart_products;
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return cart_products;
                }


            }
        }

        public List<Order> GetOrdersByCustomer(int customerId)
        {
            List<Order> ordersList = new List<Order>();
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    connection.Open();
                    using (var cmd = new SqlCommand("select * from orders where customer_id = @CustomerId", connection))
                    {
                        cmd.Parameters.AddWithValue("@CustomerId", customerId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                throw new OrderNotFoundException(customerId);
                            }

                            while (reader.Read())
                            {
                                ordersList.Add(new Order
                                {
                                    OrderId = (int)reader["order_id"],
                                    CustomerId = (int)reader["customer_id"],
                                    OrderDate = (DateTime)reader["order_date"],
                                    TotalPrice = (decimal)reader["total_price"],
                                    ShippingAddress = reader["shipping_address"].ToString(),
                                    OrderItems = new List<OrderItem>() // Placeholder for order items
                                });
                            }
                        }
                    }
                }
                catch (OrderNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                    
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (ordersList.Count == 0)
            {
                throw new CustomerNotFoundException(customerId);
            }

            return ordersList;
        }


        public Product GetProductById(int productId)
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    connection.Open();
                    var cmd = new SqlCommand("select * from products where product_id = @ProductId", connection);
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Product
                            {
                                ProductId = (int)reader["product_id"],
                                Name = reader["name"].ToString(),
                                Price = (decimal)reader["price"],
                                Description = reader["description"].ToString(),
                                StockQuantity = (int)reader["stockQuantity"]
                            };
                        }
                        else
                        {
                            throw new ProductNotFoundException(productId);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                    
                }
            }
        }


        public bool placeOrder(Customer customer, List<Dictionary<Product, int>> products, string shippingAddress)
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    connection.Open();

                    var cmd = new SqlCommand("INSERT INTO orders (customer_id, order_date, total_price, shipping_address) VALUES (@CustomerId, @OrderDate, @TotalPrice, @ShippingAddress)", connection);

                    cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                    cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@TotalPrice", CalculateTotalPrice(products));
                    cmd.Parameters.AddWithValue("@ShippingAddress", shippingAddress);

                    cmd.ExecuteNonQuery();

                    var orderIdCmd = new SqlCommand("SELECT MAX(order_id) FROM orders", connection);
                    int orderId = (int)orderIdCmd.ExecuteScalar();

                    foreach (var productWithQuantity in products)
                    {
                        foreach (var product in productWithQuantity)
                        {
                            var orderItemCmd = new SqlCommand("INSERT INTO order_items (order_id, product_id, quantity) VALUES (@OrderId, @ProductId, @Quantity)", connection);
                            orderItemCmd.Parameters.AddWithValue("@OrderId", orderId);
                            orderItemCmd.Parameters.AddWithValue("@ProductId", product.Key.ProductId);
                            orderItemCmd.Parameters.AddWithValue("@Quantity", product.Value);
                            orderItemCmd.ExecuteNonQuery();
                        }
                    }

                    ClearCart(customer);
                    return true;
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        public int GetLatestOrderId(int customerId)
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                connection.Open();

                var cmd = new SqlCommand("select max(order_id) from orders WHERE customer_id = @CustomerId", connection);
                cmd.Parameters.AddWithValue("@CustomerId", customerId);

                return (int)cmd.ExecuteScalar();
            }
        }


        public bool removeFromCart(Customer customer, Product product)
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    connection.Open();
                    cmd = new SqlCommand("delete from cart where customer_id = @CustomerId and product_id = @ProductId", connection);
                    cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                    cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        Console.WriteLine($"Product with Product Id: {product.ProductId} removed from cart successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Could not remove the product from the cart.");
                        return false;
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public bool UpdateCustomer(Customer customer)
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    connection.Open();
                    cmd = new SqlCommand("UPDATE Customers SET name = @Name, email = @Email, password = @Password WHERE customer_id = @CustomerId", connection);
                    cmd.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@Password", customer.Password);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        Console.WriteLine($"Customer {customer.Name} updated successfully.");
                        return true;
                    }
                    else
                    {
                        throw new CustomerNotFoundException(customer.CustomerId);
                        //return false;
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public bool UpdateProduct(Product product)
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    connection.Open();
                    cmd = new SqlCommand("UPDATE Products SET name = @Name, price = @Price, description = @Description, stockQuantity = @StockQuantity WHERE product_id = @ProductId", connection);
                    cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        Console.WriteLine("Product updated successfully.");
                        return true;
                    }
                    else
                    {
                        throw new ProductNotFoundException(product.ProductId);
                        //return false;
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }

        }

        public void DisplayAllProducts()
        {
            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                try
                {
                    connection.Open();
                    var cmd = new SqlCommand("select * from products", connection);
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("Available Products:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Product ID: {reader["product_id"]}, Name: {reader["name"]}, Price: {reader["price"]}, Description: {reader["description"]}, Stock Quantity: {reader["stockQuantity"]}");
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public decimal CalculateTotalPrice(List<Dictionary<Product, int>> products)
        {
            decimal totalPrice = 0;

            foreach (var productWithQuantity in products)
            {
                foreach (var product in productWithQuantity)
                {
                    totalPrice += product.Key.Price * product.Value;
                }
            }

            return totalPrice;
        }

        public List<OrderItem> GetOrderDetails(int orderId)
        {
            var orderItems = new List<OrderItem>();

            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                connection.Open();

                var cmd = new SqlCommand("SELECT oi.order_item_id, oi.quantity, oi.product_id, p.product_name, p.price FROM order_items oi JOIN products p ON oi.product_id = p.product_id WHERE oi.order_id = @OrderId", connection);
                cmd.Parameters.AddWithValue("@OrderId", orderId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var orderItem = new OrderItem
                        {
                            OrderItemId = reader.GetInt32(0),
                            Quantity = reader.GetInt32(1),
                            ProductId = reader.GetInt32(2),
                        };

                        orderItems.Add(orderItem);
                    }
                }
            }

            return orderItems;
        }





        //{
        //    using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
        //    {
        //        try
        //        {
        //            string query = "insert into customers (name, email, password) OUTPUT INSERTED.CustomerId VALUES (@Name, @Email, @Password)";

        //            using (var cmd = new SqlCommand(query, connection))
        //            {
        //                cmd.Parameters.AddWithValue("@Name", customer.Name);
        //                cmd.Parameters.AddWithValue("@Email", customer.Email);
        //                cmd.Parameters.AddWithValue("@Password", customer.Password);

        //                connection.Open();
        //                int customerId = (int)cmd.ExecuteScalar();
        //                return customerId;
        //            }
        //        }
        //        catch (System.Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //            return -1;
        //        }
        //    }
        //}
    }

}
