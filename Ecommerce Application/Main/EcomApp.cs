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
                Console.WriteLine("\nWelcome to E-commerce Application");
                Console.WriteLine("1. Sign Up");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit\n");
                orderProcessorRepository.PrintColored("Enter Your Choice (1-3):  ", ConsoleColor.Yellow);

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
                        orderProcessorRepository.PrintColored("Invalid choice.", ConsoleColor.Red);
                        Console.WriteLine();
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
                Console.WriteLine("7. Exit\n");

                orderProcessorRepository.PrintColored("Enter Your Choice (1-7):  ", ConsoleColor.Yellow);
                string choice = Console.ReadLine();
            menu:
                switch (choice)
                {
                    case "1":
                        Console.WriteLine("\nEnter Product Name:");
                        string productName = Console.ReadLine();
                        Console.WriteLine("Enter Product Price:");
                        decimal productPrice = decimal.Parse(Console.ReadLine());
                        Console.WriteLine("Enter Product Description:");
                        string productDescription = Console.ReadLine();
                        Console.WriteLine("Enter Stock Quantity:");
                        int stockQuantity = int.Parse(Console.ReadLine());

                        if (orderProcessorRepository.createProduct(new Product { Name = productName, Price = productPrice, Description = productDescription, StockQuantity = stockQuantity }))
                        {
                            orderProcessorRepository.PrintColored("Product created successfully.", ConsoleColor.Green);
                            Console.WriteLine();
                        }
                        else
                        {
                            orderProcessorRepository.PrintColored("Failed to create product.", ConsoleColor.Red);
                            goto menu;
                        }

                        break;

                    case "2":
                        orderProcessorRepository.DisplayAllProducts();
                        Console.WriteLine("\nEnter Product ID to delete:");
                        int productIdToDelete = int.Parse(Console.ReadLine());
                        if (orderProcessorRepository.deleteProduct(productIdToDelete))
                        {
                            orderProcessorRepository.PrintColored("Product deleted successfully.", ConsoleColor.Green);
                            Console.WriteLine();
                        }
                        else
                        {
                            orderProcessorRepository.PrintColored("Failed to delete product.", ConsoleColor.Red);
                            Console.WriteLine();
                            goto menu;
                        }

                        break;
                    case "3":
                    addProductsToCart:
                        orderProcessorRepository.DisplayAllProducts();
                        Console.WriteLine("\nEnter Product ID: ");

                        if (int.TryParse(Console.ReadLine(), out int productIdToAdd))
                        {
                            Console.WriteLine("Enter Quantity: ");

                            if (int.TryParse(Console.ReadLine(), out int quantityToAdd) && quantityToAdd > 0)
                            {
                                var productToAdd = orderProcessorRepository.GetProductById(productIdToAdd);

                                if (productToAdd != null && productToAdd.StockQuantity >= quantityToAdd)
                                {
                                    if (orderProcessorRepository.addToCart(currentCustomer, productToAdd, quantityToAdd))
                                    {
                                        orderProcessorRepository.PrintColored("Product added to cart.", ConsoleColor.Green);
                                    }
                                    else
                                    {
                                        orderProcessorRepository.PrintColored("Failed to add product to cart.", ConsoleColor.Red);
                                    }
                                }
                                else
                                {
                                    orderProcessorRepository.PrintColored("Insufficient stock available.", ConsoleColor.Red);
                                }
                            }
                            else
                            {
                                orderProcessorRepository.PrintColored("Invalid quantity entered.", ConsoleColor.Red);
                            }
                        }
                        else
                        {
                            orderProcessorRepository.PrintColored("Invalid Product ID.", ConsoleColor.Red);
                        }

                        Console.WriteLine("Do you want to add more products? (yes/no)");
                        if (Console.ReadLine()?.Trim().ToLower() == "yes")
                        {
                            goto addProductsToCart;
                        }

                        break;

                    case "4":
                        var cartItems = orderProcessorRepository.getAllFromCart(currentCustomer);
                        if (cartItems.Count == 0)
                        {
                            orderProcessorRepository.PrintColored("Your cart is empty.", ConsoleColor.Red);
                            break;
                        }

                        orderProcessorRepository.PrintColored("Items in your cart:", ConsoleColor.Green);
                        foreach (var item in cartItems)
                        {
                            int quantityInCart = orderProcessorRepository.GetQuantityInCart(currentCustomer, item.ProductId);
                            Console.WriteLine($"Product ID: {item.ProductId}, Name: {item.Name}, Quantity: {quantityInCart}");
                        }
                        break;

                    case "5":
                        Console.WriteLine("Enter Shipping Address:");
                        string shippingAddress = Console.ReadLine();

                        var productsInCart = orderProcessorRepository.getAllFromCart(currentCustomer);
                        if (productsInCart.Count == 0)
                        {
                            orderProcessorRepository.PrintColored("Your cart is empty", ConsoleColor.Red);
                            Console.WriteLine();
                            break;
                        }

                        var productsWithQuantity = new List<Dictionary<Product, int>>();
                        foreach (var product in productsInCart)
                        {
                            int quantityInCart = orderProcessorRepository.GetQuantityInCart(currentCustomer, product.ProductId);
                            productsWithQuantity.Add(new Dictionary<Product, int> { { product, quantityInCart } });
                        }

                        if (orderProcessorRepository.placeOrder(currentCustomer, productsWithQuantity, shippingAddress))
                        {
                            orderProcessorRepository.PrintColored("Order placed successfully!", ConsoleColor.Green);
                            Console.WriteLine();

                            var orderId = orderProcessorRepository.GetLatestOrderId(currentCustomer.CustomerId);
                            decimal totalPrice = orderProcessorRepository.CalculateTotalPrice(productsWithQuantity);
                            Console.WriteLine("Order Details:");

                            foreach (var productDict in productsWithQuantity)
                            {
                                foreach (var product in productDict)
                                {
                                    int quantityToDeduct = productDict[product.Key];
                                    Console.WriteLine($"Product Id: {product.Key.ProductId}, Quantity: {quantityToDeduct}, Total Price: {product.Key.Price * quantityToDeduct}"); // Correct total price calculation
                                    orderProcessorRepository.DeductStock(product.Key.ProductId, quantityToDeduct);
                                }
                            }

                            orderProcessorRepository.ClearCart(currentCustomer);
                        }
                        else
                        {
                            orderProcessorRepository.PrintColored("Failed to place the order. Please try again.", ConsoleColor.Red);
                            Console.WriteLine();
                        }
                        break;

                    case "6":
                        List<Order> orders = orderProcessorRepository.GetOrdersByCustomer(currentCustomer.CustomerId);
                        if (orders.Count == 0)
                        {
                            orderProcessorRepository.PrintColored("You have no orders.", ConsoleColor.Red);
                            Console.WriteLine();
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
                        orderProcessorRepository.PrintColored("Exiting...", ConsoleColor.Green);
                        Console.WriteLine();
                        return;

                    default:
                        orderProcessorRepository.PrintColored("Invalid choice. Please try again.", ConsoleColor.Red);
                        Console.WriteLine();
                        break;
                }
            }
        }

        private static bool Login()
        {
            Console.WriteLine("\nEnter Email:");
            string customerEmail = Console.ReadLine();
            if (!orderProcessorRepository.IsValidEmail(customerEmail))
            {
                orderProcessorRepository.PrintColored("Invalid email format.", ConsoleColor.Red);
                Console.WriteLine();
                return false;
            }
            Console.WriteLine("Enter Password:");
            string password = Console.ReadLine();
            try
            {
                currentCustomer = orderProcessorRepository.GetCustomerByEmail(customerEmail);
                if (currentCustomer != null && currentCustomer.Password == password)
                {
                    orderProcessorRepository.PrintColored("Login successful!", ConsoleColor.Green);
                    Console.WriteLine();
                    return true;
                }
                else
                {
                    orderProcessorRepository.PrintColored("Invalid Customer ID or Password. Please try again.", ConsoleColor.Red);
                    Console.WriteLine();
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                orderProcessorRepository.PrintColored(ex.Message, ConsoleColor.Red);
                return false;
            }
        }

        private static void SignUp()
        {
            Console.WriteLine("\nEnter Name:");
            string name = Console.ReadLine();

            Console.WriteLine("\nEnter Email:");
            string email = Console.ReadLine();
            if (!orderProcessorRepository.IsValidEmail(email))
            {
                orderProcessorRepository.PrintColored("Invalid email format.", ConsoleColor.Red);
                Console.WriteLine();
                return;
            }

            Console.WriteLine("\nEnter Password:");
            string password = Console.ReadLine();
            if (!orderProcessorRepository.IsValidPassword(password))
            {
                orderProcessorRepository.PrintColored("Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a digit, and a special character.", ConsoleColor.Red);
                Console.WriteLine();
                return;
            }

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
                    orderProcessorRepository.PrintColored("Sign up successful, Please log in.", ConsoleColor.Green);
                    Console.WriteLine();
                }
                else
                {
                    orderProcessorRepository.PrintColored("Sign up unsuccessful, please try again.", ConsoleColor.Red);
                    Console.WriteLine();
                }
            }
            catch (System.Exception ex)
            {
                orderProcessorRepository.PrintColored(ex.Message, ConsoleColor.Red);
                Console.WriteLine();
            }
        }
    }
}
