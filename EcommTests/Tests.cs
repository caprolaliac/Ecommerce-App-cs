using NUnit.Framework;
using Ecommerce_Application.Model;
using Ecommerce_Application.Repository;
using Ecommerce_Application.Exception;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Student_Information_System.Utility;

namespace EcommTests
{
    public class OrderProcessorTests
    {
        private IOrderProcessorRepository _repository;
        private Customer _testCustomer;
        private Product _testProduct;
        private int _testProductId;

        [SetUp]
        public void Setup()
        {
            _repository = new OrderProcessorRepositoryImpl();
            //CleanupTestData();

            _testCustomer = new Customer
            {
                Name = "varun",
                Email = "varun@13568.com",
                Password = "varun@13568"
            };

            _testProduct = new Product
            {
                Name = "Galaxy S4",
                Price = 100000,
                Description = "Latest Android",
                StockQuantity = 10
            };

            _repository.createCustomer(_testCustomer);
            _repository.createProduct(_testProduct);

            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                connection.Open();

                var customerCmd = new SqlCommand("select customer_id from customers where email = @Email", connection);
                customerCmd.Parameters.AddWithValue("@Email", _testCustomer.Email);
                _testCustomer.CustomerId = (int)customerCmd.ExecuteScalar();

                var productCmd = new SqlCommand("select product_id from products where name = @Name", connection);
                productCmd.Parameters.AddWithValue("@Name", _testProduct.Name);
                _testProductId = (int)productCmd.ExecuteScalar();
                _testProduct.ProductId = _testProductId;
            }
        }

        [TearDown]
        public void Cleanup()
        {
            //CleanupTestData();
        }

        //private void CleanupTestData()
        //{
        //    using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
        //    {
        //        connection.Open();
        //        using (var command = connection.CreateCommand())
        //        {
        //            command.CommandText = @"
        //                DELETE FROM order_items;
        //                DELETE FROM orders;
        //                DELETE FROM cart;
        //                DELETE FROM products;
        //                DELETE FROM customers;
        //                DBCC CHECKIDENT ('products', RESEED, 0);
        //                DBCC CHECKIDENT ('customers', RESEED, 0);
        //                DBCC CHECKIDENT ('orders', RESEED, 0);
        //                DBCC CHECKIDENT ('order_items', RESEED, 0);";
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        [Test]
        [Description("Create Product")]
        public void Test_CreateProduct()
        {
            Product newProduct = new Product
            {
                Name = "Iphone 16",
                Price = 150000,
                Description = "Newest Iphone",
                StockQuantity = 5
            };

            bool result = _repository.createProduct(newProduct);

            Assert.That(result, Is.True);

            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                connection.Open();
                var cmd = new SqlCommand("select * from products where name = @Name", connection);
                cmd.Parameters.AddWithValue("@Name", newProduct.Name);
                using (var reader = cmd.ExecuteReader())
                {
                    Assert.That(reader.Read(), Is.True);
                    Assert.That(reader["name"].ToString(), Is.EqualTo(newProduct.Name));
                }
            }
        }

        [Test]
        [Description("Add Product to Cart")]
        public void Test_AddToCart()
        {
            var product = _repository.GetProductById(_testProductId);
            Assert.That(product, Is.Not.Null, "product should exist");

            bool result = _repository.addToCart(_testCustomer, product, 1);

            Assert.That(result, Is.True);

            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                connection.Open();
                var cmd = new SqlCommand(
                    "select count(*) from cart where customer_id = @CustomerId and product_id = @ProductId",
                    connection);
                cmd.Parameters.AddWithValue("@CustomerId", _testCustomer.CustomerId);
                cmd.Parameters.AddWithValue("@ProductId", _testProductId);
                int count = (int)cmd.ExecuteScalar();
                Assert.That(count, Is.EqualTo(1));
            }
        }

        [Test]
        [Description("Place Order")]
        public void Test_PlaceOrder()
        {
            var product = _repository.GetProductById(_testProductId);
            Assert.That(product, Is.Not.Null, "product should exist");

            _repository.addToCart(_testCustomer, product, 1);
            string shipping_address = "123 church Street, Bangalore";
            var productList = new List<Dictionary<Product, int>>
            {
                new Dictionary<Product, int> { { product, 1 } }
            };

            bool result = _repository.placeOrder(_testCustomer, productList, shipping_address);

            Assert.That(result, Is.True);

            using (var connection = new SqlConnection(DbConnUtil.GetConnString()))
            {
                connection.Open();
                var cmd = new SqlCommand(
                    "select count(*) from orders where customer_id = @CustomerId",
                    connection);
                cmd.Parameters.AddWithValue("@CustomerId", _testCustomer.CustomerId);
                int orderCount = (int)cmd.ExecuteScalar();
                Assert.That(orderCount, Is.EqualTo(1));
            }
        }

        [Test]
        [Description("Product Not Found Exception")]
        public void Test_GetProductById()
        {
            int product_id = 9999;

            var ex = Assert.Throws<ProductNotFoundException>(() =>
                _repository.GetProductById(product_id));

            Assert.That(ex.Message, Does.Contain(product_id.ToString()));
        }

        [Test]
        [Description("Customer Not Found Exception")]
        public void Test_GetOrdersByCustomer()
        {
            int customer_id = 9999;

            var ex = Assert.Throws<CustomerNotFoundException>(() =>
                _repository.GetOrdersByCustomer(customer_id));

            Assert.That(ex.Message, Does.Contain(customer_id.ToString()));
        }
    }
}




//using NUnit.Framework;
//using Ecommerce_Application.Model;
//using Ecommerce_Application.Repository;
//using Ecommerce_Application.Exception;
//using System;
//using System.Collections.Generic;

//namespace EcommTests
//{
//    [TestFixture]
//    public class OrderProcessorTests
//    {
//        private IOrderProcessorRepository _repository;
//        private Customer _testCustomer;
//        private Product _testProduct;

//        [SetUp]
//        public void Setup()
//        {
//            _repository = new OrderProcessorRepositoryImpl();

//            _testCustomer = new Customer
//            {
//                Name = "ram B",
//                Email = "ram.B@email.com",
//                Password = "Ramb@123456"
//            };

//            // Insert the customer into the database
//            _repository.createCustomer(_testCustomer);  // Assuming this method sets the ID in the customer object

//            // Create a test product
//            _testProduct = new Product
//            {
//                Name = "Samsung Galaxy S24 ultra",
//                Price = 999,
//                Description = "Latest Samsung phone",
//                StockQuantity = 5
//            };

//            // Insert the product into the database
//            _repository.createProduct(_testProduct);  // Assuming this method sets the ID in the product object

//            // Add the product to the cart
//            _repository.addToCart(_testCustomer, _testProduct, 1);
//        }


//        [Test]
//        [Description("Create Product")]
//        public void Test_CreateProduct_Success()
//        {
//            Product newProduct = new Product
//            {
//                Name = "Apple iPhone 16",
//                Price = 1099,
//                Description = "Latest iPhone",
//                StockQuantity = 3
//            };

//            bool result = _repository.createProduct(newProduct); // This should set newProduct.ProductId automatically
//            Assert.That(result, Is.True);
//        }

//        [Test]
//        [Description("Add Product to Cart")]
//        public void Test_AddToCart_Success()
//        {
//            int quantity = 1;
//            bool result = _repository.addToCart(_testCustomer, _testProduct, quantity);
//            Assert.That(result, Is.True);
//        }

//        [Test]
//        [Description("Place Order")]
//        public void Test_PlaceOrder_Success()
//        {
//            string shippingAddress = "124 Main St, Delhi";
//            var productList = new List<Dictionary<Product, int>>
//            {
//                new Dictionary<Product, int> { { _testProduct, 1 } }
//            };

//            bool result = _repository.placeOrder(_testCustomer, productList, shippingAddress);
//            Assert.That(result, Is.True);
//        }

//        [Test]
//        [Description("Product Not Found Exception")]
//        public void Test_GetProductById_ThrowsException()
//        {
//            int nonExistentProductId = 1005;

//            var ex = Assert.Throws<ProductNotFoundException>(() =>
//                _repository.GetProductById(nonExistentProductId));

//            Assert.That(ex.Message, Does.Contain(nonExistentProductId.ToString()));
//        }

//        [Test]
//        [Description("Customer Not Found Exception")]
//        public void Test_GetOrdersByCustomer_ThrowsException()
//        {
//            int nonExistentCustomerId = 9999;
//            var ex = Assert.Throws<CustomerNotFoundException>(() =>
//                _repository.GetOrdersByCustomer(nonExistentCustomerId));

//            Assert.That(ex.Message, Does.Contain(nonExistentCustomerId.ToString()));
//        }
//    }
//}

