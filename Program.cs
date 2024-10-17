using Microsoft.Data.SqlClient;
namespace YourNamespace.util;
using System;
using System.Collections.Generic;

public class Program
{
    static void Main(string[] args)
    {
        IOrderManagementRepository orderProcessor = new OrderProcessor();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("1. Create User\n2. Create Product\n3. Create Order\n4. Cancel Order\n5. Get All Products\n6. Get Orders by User\n7. Exit");
            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Console.WriteLine("Enter user ID:");
                    int userId = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter username:");
                    string username = Console.ReadLine();
                    Console.WriteLine("Enter password:");
                    string password = Console.ReadLine();
                    Console.WriteLine("Enter role (Admin/User):");
                    string role = Console.ReadLine();
                    User user = new User(userId, username, password, role);
                    orderProcessor.CreateUser(user);
                    break;

                case 2:
                    Console.WriteLine("Enter Admin User ID:");
                    int adminId = Convert.ToInt32(Console.ReadLine());
                    User admin;
                    try
                    {
                        admin = GetUserById(adminId);
                        if (admin.Role.ToLower() != "admin")
                        {
                            Console.WriteLine("User is not an admin.");
                            break;
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        break;
                    }

                    Console.WriteLine("Enter Product Name:");
                    string productName = Console.ReadLine();
                    Console.WriteLine("Enter Description:");
                    string description = Console.ReadLine();
                    Console.WriteLine("Enter Price:");
                    double price = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Enter Quantity in Stock:");
                    int quantity = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter Product Type (Electronics/Clothing):");
                    string productType = Console.ReadLine();
                    Product product;

                    if (productType.Equals("Electronics", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Enter Brand:");
                        string brand = Console.ReadLine();
                        Console.WriteLine("Enter Warranty Period (in months):");
                        int warrantyPeriod = Convert.ToInt32(Console.ReadLine());
                        product = new Electronics(productName, description, price, quantity, brand, warrantyPeriod);
                    }
                    else if (productType.Equals("Clothing", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Enter Size:");
                        string size = Console.ReadLine();
                        Console.WriteLine("Enter Color:");
                        string color = Console.ReadLine();
                        product = new Clothing(productName, description, price, quantity, size, color);
                    }
                    else
                    {
                        Console.WriteLine("Invalid product type entered.");
                        break;
                    }

                    orderProcessor.CreateProduct(admin, product);
                    break;

                case 3:
                    Console.WriteLine("Enter User ID to place order:");
                    int orderUserId = Convert.ToInt32(Console.ReadLine());
                    User orderUser = new User(orderUserId, "User", "password", "User");

                    Console.WriteLine("Select products to order (by Product ID, separated by commas):");
                    var allProducts = orderProcessor.GetAllProducts();
                    foreach (var prod in allProducts)
                    {
                        Console.WriteLine($"{prod.ProductId}. {prod.ProductName} - {prod.Price}");
                    }

                    string productIds = Console.ReadLine();
                    List<Product> selectedProducts = new List<Product>();

                    foreach (var id in productIds.Split(','))
                    {
                        int prodId = Convert.ToInt32(id.Trim());
                        var selectedProduct = allProducts.Find(p => p.ProductId == prodId);
                        if (selectedProduct != null)
                        {
                            selectedProducts.Add(selectedProduct);
                        }
                    }

                    if (selectedProducts.Count > 0)
                    {
                        orderProcessor.CreateOrder(orderUser, selectedProducts);
                        Console.WriteLine("Order placed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("No valid products selected.");
                    }
                    break;

                case 4:
                    Console.WriteLine("Enter User ID:");
                    int cancelUserId = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter Order ID:");
                    int orderId = Convert.ToInt32(Console.ReadLine());
                    orderProcessor.CancelOrder(cancelUserId, orderId);
                    break;

                case 5:
                    var products = orderProcessor.GetAllProducts();
                    foreach (var prod in products)
                    {
                        Console.WriteLine($"{prod.ProductName} - {prod.Price}");
                    }
                    break;

                case 6:
                    Console.WriteLine("Enter User ID:");
                    int userOrderId = Convert.ToInt32(Console.ReadLine());
                    User userOrder = new User(userOrderId, "User", "password", "User");
                    var userOrders = orderProcessor.GetOrderByUser(userOrder);
                    foreach (var orderProd in userOrders)
                    {
                        Console.WriteLine($"{orderProd.ProductName} - {orderProd.Price}");
                    }
                    break;

                case 7:
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }

    public static User GetUserById(int userId)
    {
        using (SqlConnection conn = DBConnUtil.GetDBConn("dbProperties.txt"))
        {
            string query = "SELECT UserId, Username, Password, Role FROM Users WHERE UserId = @UserId";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string username = reader.GetString(1);
                        string password = reader.GetString(2);
                        string role = reader.GetString(3);

                        return new User(id, username, password, role);
                    }
                    else
                    {
                        throw new ArgumentException("User not found.");
                    }
                }
            }
        }
    }
}
