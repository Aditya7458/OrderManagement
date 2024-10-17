namespace YourNamespace.util;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

public interface IOrderManagementRepository
{
    void CreateOrder(User user, List<Product> products);
    void CancelOrder(int userId, int orderId);
    void CreateProduct(User user, Product product);
    void CreateUser(User user);
    List<Product> GetAllProducts();
    List<Product> GetOrderByUser(User user);
}


public class OrderProcessor : IOrderManagementRepository
{
    private List<User> users = new List<User>();
    private List<Product> products = new List<Product>();
    private Dictionary<int, List<Product>> orders = new Dictionary<int, List<Product>>();
    private string _connectionString = DBPropertyUtil.GetConnectionString("dbProperties.txt");
    public void CreateOrder(User user, List<Product> productsToOrder)
    {
        if (users.Any(u => u.UserId == user.UserId))
        {
            if (!orders.ContainsKey(user.UserId))
            {
                orders[user.UserId] = new List<Product>();
            }
            orders[user.UserId].AddRange(productsToOrder);
            Console.WriteLine($"Order created for user {user.Username}");
        }
        else
        {
            Console.WriteLine("User not found. Please create the user first.");
        }
    }

    public void CancelOrder(int userId, int orderId)
    {
        if (orders.ContainsKey(userId) && orderId < orders[userId].Count)
        {
            orders[userId].RemoveAt(orderId);
            Console.WriteLine("Order canceled successfully.");
        }
        else
        {
            throw new OrderNotFoundException("Order not found for the given user.");
        }
    }

    public void CreateProduct(User user, Product product)
    {
        if (user.Role != "Admin")
        {
            throw new UnauthorizedAccessException("Only admin users can add products.");
        }

        using (SqlConnection conn = DBConnUtil.GetDBConn("dbProperties.txt"))
        {
            string query = "INSERT INTO Products (productName, description, price, quantityInStock, type, brand, warrantyPeriod, size, color) " +
                           "VALUES (@ProductName, @Description, @Price, @QuantityInStock, @Type, @Brand, @WarrantyPeriod, @Size, @Color)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                cmd.Parameters.AddWithValue("@Description", product.Description);
                cmd.Parameters.AddWithValue("@Price", product.Price);
                cmd.Parameters.AddWithValue("@QuantityInStock", product.QuantityInStock);
                cmd.Parameters.AddWithValue("@Type", product.Type);

                // Electronics specific fields
                if (product is Electronics electronics)
                {
                    cmd.Parameters.AddWithValue("@Brand", electronics.Brand);
                    cmd.Parameters.AddWithValue("@WarrantyPeriod", electronics.WarrantyPeriod);
                    cmd.Parameters.AddWithValue("@Size", DBNull.Value); // Set to null since not applicable
                    cmd.Parameters.AddWithValue("@Color", DBNull.Value); // Set to null since not applicable
                }
                // Clothing specific fields
                else if (product is Clothing clothing)
                {
                    cmd.Parameters.AddWithValue("@Brand", DBNull.Value); // Set to null since not applicable
                    cmd.Parameters.AddWithValue("@WarrantyPeriod", DBNull.Value); // Set to null since not applicable
                    cmd.Parameters.AddWithValue("@Size", clothing.Size);
                    cmd.Parameters.AddWithValue("@Color", clothing.Color);
                }
                else
                {
                    // If the product is neither Electronics nor Clothing, throw an error
                    throw new ArgumentException("Invalid product type.");
                }

                cmd.ExecuteNonQuery();
                Console.WriteLine($"Product {product.ProductName} added to the database.");
            }
        }
    }

    public void CreateUser(User user)
    {
        using (SqlConnection conn = DBConnUtil.GetDBConn(_connectionString))
        {
            // Check if the username already exists
            string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE username = @Username";
            using (SqlCommand checkUserCmd = new SqlCommand(checkUserQuery, conn))
            {
                checkUserCmd.Parameters.AddWithValue("@Username", user.Username);
                int userExists = (int)checkUserCmd.ExecuteScalar();

                if (userExists > 0)
                {
                    throw new Exception("Username already exists. Please choose a different username.");
                }
            }

            // Insert the new user
            string query = "INSERT INTO Users (username, password, role) VALUES (@Username, @Password, @Role)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Password", user.Password); // Make sure to hash passwords in a real application!
                cmd.Parameters.AddWithValue("@Role", user.Role);

                cmd.ExecuteNonQuery();
                Console.WriteLine($"User {user.Username} created.");
            }
        }
    }

    public List<Product> GetAllProducts()
    {
        return products;
    }

    public List<Product> GetOrderByUser(User user)
    {
        if (orders.ContainsKey(user.UserId))
        {
            return orders[user.UserId];
        }
        else
        {
            throw new UserNotFoundException("User not found.");
        }
    }
}
