using Ecommerce_Application.Model;
using Ecommerce_Application.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_Application.Main
{
    internal class EcomApp
    {
        private static IOrderProcessorRepository orderProcessorRepository = new OrderProcessorRepositoryImpl();
        private static Customer currentCustomer;
        public static void Run()
        {
            while (true)
            {
                Console.WriteLine("Welcome to E-commerce Application");
                Console.WriteLine("1. Sign Up");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit\n");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        SignUp();
                        break;
                    case "2":
                        if (Login())
                        {
                            Menu();
                        }
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

            }
        }

        private static void Menu()
        {
            while (true)
            {
                Console.WriteLine("\nSelect an option:");
                Console.WriteLine("1. Create Product");
                Console.WriteLine("2. Delete Product");
                Console.WriteLine("3. Add to Cart");
                Console.WriteLine("4. View Cart");
                Console.WriteLine("5. Place Order");
                Console.WriteLine("6. View Customer Orders");
                Console.WriteLine("7. exit\n");
                Console.Write("Enter Your Choice (1-7):  ");
                string choice = Console.ReadLine();
            menu:
                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Enter Product Name:");
                        string productName = Console.ReadLine();
                        Console.WriteLine("Enter Product Price:");
                        decimal productPrice = decimal.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Product Description:");
                        string productDescription = Console.ReadLine();
                        Console.WriteLine("Enter Stock Quantity:");
                        int stockQuantity = int.Parse(Console.ReadLine());

                        if (orderProcessorRepository.createProduct(new Product { Name = productName, Price = productPrice, Description = productDescription, StockQuantity = stockQuantity }))
                        {
                            Console.WriteLine("Product created successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to create product.");
                            goto menu;
                        }

                        break;

                    case "2":
                        orderProcessorRepository.DisplayAllProducts();
                        Console.WriteLine("Enter Product ID to delete:");
                        int productIdToDelete = int.Parse(Console.ReadLine());
                        if (orderProcessorRepository.deleteProduct(productIdToDelete))
                        {
                            Console.WriteLine("Product deleted successfully.");

                        }
                        else
                        {
                            Console.WriteLine("Failed to delete product.");
                            goto menu;
                        }

                        break;
                    case "3":
                        orderProcessorRepository.DisplayAllProducts();
                        Console.WriteLine("Enter Product ID: ");
                        int productIdToAdd = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Quantity: ");
                        int quantityToAdd = int.Parse(Console.ReadLine());
                        if (orderProcessorRepository.addToCart(currentCustomer, new Product { ProductId = productIdToAdd }, quantityToAdd))
                        {
                            Console.WriteLine("Product added to cart.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to add product to cart.");
                        }

                        break;
                    case "4":
                        var cartItems = orderProcessorRepository.getAllFromCart(currentCustomer);
                        if (cartItems.Count == 0)
                        {
                            Console.WriteLine("Your cart is empty.");
                        }
                        else
                        {
                            Console.WriteLine("Items in your cart:");
                            foreach (var item in cartItems)
                            {
                                Console.WriteLine($"Product ID: {item.ProductId}, Name: {item.Name}, Quantity: {item.StockQuantity}\n");
                            }
                        }

                        break;
                    case "5":
                        Console.WriteLine("Enter Shipping Address:");
                        string shippingAddress = Console.ReadLine();

                        List<Product> cartProducts = orderProcessorRepository.getAllFromCart(currentCustomer);
                        if (cartProducts.Count == 0)
                        {
                            Console.WriteLine("Your cart is empty");
                            break;
                        }

                        List<Dictionary<Product, int>> productsWithQuantity = new List<Dictionary<Product, int>>();
                        foreach (var product in cartProducts)
                        {
                            productsWithQuantity.Add(new Dictionary<Product, int> { { product, 1 } });
                        }

                        if (orderProcessorRepository.placeOrder(currentCustomer, productsWithQuantity, shippingAddress))
                        {
                            Console.WriteLine("Order placed successfully!");

                            var orderId = orderProcessorRepository.GetLatestOrderId(currentCustomer.CustomerId);
                            var orderDetails = orderProcessorRepository.GetOrderDetails(orderId);
                            Console.WriteLine("Order Details:");
                            foreach (var item in orderDetails)
                            {
                                Console.WriteLine($"Product Id: {item.ProductId}, Quantity: {item.Quantity}");
                            }

                            orderProcessorRepository.ClearCart(currentCustomer);
                        }
                        else
                        {
                            Console.WriteLine("Failed to place the order. Please try again.");
                        }
                        break;


                    case "6":

                        List<Order> orders = orderProcessorRepository.GetOrdersByCustomer(currentCustomer.CustomerId);
                        if (orders.Count == 0)
                        {
                            Console.WriteLine("You have no orders.");
                        }
                        else
                        {
                            Console.WriteLine("Your Orders:");
                            foreach (var order in orders)
                            {
                                Console.WriteLine($"\nOrder ID: {order.OrderId}, Order Date: {order.OrderDate:d}, Total Price: {order.TotalPrice}, Shipping Address: {order.ShippingAddress}\n");
                            }
                        }

                        break;
                    case "7":
                        Console.WriteLine("Exitting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static bool Login()
        {
            Console.WriteLine("Enter Email:");
            string customerEmail = Console.ReadLine();
            Console.WriteLine("Enter Password:");
            string password = Console.ReadLine();
            try
            {
                currentCustomer = orderProcessorRepository.GetCustomerByEmail(customerEmail);
                if (currentCustomer != null && currentCustomer.Password == password)
                {
                    Console.WriteLine("Login successful!\n");
                    return true;
                }
                else
                {
                    Console.WriteLine("Invalid Customer ID or Password. Please try again.\n");
                    return false;
                }
            }

            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

        }

        private static void SignUp()
        {
            Console.WriteLine("Enter Name:");
            string name = Console.ReadLine();
            Console.WriteLine("Enter Email:");
            string email = Console.ReadLine();
            Console.WriteLine("Enter Password:");
            string password = Console.ReadLine();

            Customer newCustomer = new Customer
            {
                Name = name,
                Email = email,
                Password = password
            };
            try
            {
                if (orderProcessorRepository.createCustomer(newCustomer))
                {
                    Console.WriteLine("Sign up successful, Please log in.");
                }
                else
                {
                    Console.WriteLine("Sign up Unsuccessful please try again.");
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
