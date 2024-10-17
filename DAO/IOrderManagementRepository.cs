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
        
        bool userExists = false;
        using (var conn = DBConnUtil.GetDBConn(_connectionString))
        {
            string userCheckQuery = "SELECT COUNT(*) FROM Users WHERE userId = @userId;";

            using (var cmd = new SqlCommand(userCheckQuery, conn))
            {
                cmd.Parameters.AddWithValue("@userId", user.UserId);
                userExists = (int)cmd.ExecuteScalar() > 0;
            }
        }

        if (userExists)
        {
            int newOrderId;
            using (var conn = DBConnUtil.GetDBConn(_connectionString))
            {
                string insertOrderQuery = "INSERT INTO Orders (userId) OUTPUT INSERTED.orderId VALUES (@userId);";

                using (var cmd = new SqlCommand(insertOrderQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", user.UserId);
                    newOrderId = (int)cmd.ExecuteScalar(); 
                }
            }

            foreach (var product in productsToOrder)
            {
                using (var conn = DBConnUtil.GetDBConn(_connectionString))
                {
                    string insertOrderDetailQuery = "INSERT INTO OrderDetails (orderId, productId, quantity) VALUES (@orderId, @productId, @quantity);";

                    using (var cmd = new SqlCommand(insertOrderDetailQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@orderId", newOrderId);
                        cmd.Parameters.AddWithValue("@productId", product.ProductId); 
                        cmd.Parameters.AddWithValue("@quantity", product.QuantityInStock);

                        cmd.ExecuteNonQuery(); 
                    }
                }
            }

            Console.WriteLine($"Order created for user {user.Username} with Order ID: {newOrderId}");
        }
        else
        {
            Console.WriteLine("User not found in the database. Please create the user first.");
        }
    }

    public void CancelOrder(int userId, int orderId)
    {
        using (var conn = DBConnUtil.GetDBConn(_connectionString))
        {
            // First, check if the order belongs to the user
            string checkOrderQuery = "SELECT COUNT(*) FROM Orders WHERE userId = @userId AND orderId = @orderId";

            using (var cmd = new SqlCommand(checkOrderQuery, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@orderId", orderId);

                int orderCount = (int)cmd.ExecuteScalar();

                if (orderCount == 0)
                {
                    throw new OrderNotFoundException("Order not found for the given user.");
                }
            }

            // Delete related entries from OrderDetails table
            string deleteOrderDetailsQuery = "DELETE FROM OrderDetails WHERE orderId = @orderId";

            using (var cmd = new SqlCommand(deleteOrderDetailsQuery, conn))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);
                cmd.ExecuteNonQuery();
            }

            // Delete the order from the Orders table
            string deleteOrderQuery = "DELETE FROM Orders WHERE orderId = @orderId AND userId = @userId";

            using (var cmd = new SqlCommand(deleteOrderQuery, conn))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Order canceled successfully.");
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
        var productList = new List<Product>();

        using (var conn = DBConnUtil.GetDBConn(_connectionString))
        {
            string query = "SELECT productId, productName, description, price, quantityInStock, type, brand, warrantyPeriod, size, color FROM Products;";

            using (var cmd = new SqlCommand(query, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Read the productId for reference, but it's not needed for the constructor
                        int productId = reader.GetInt32(0); // Index 0
                        string productName = reader.GetString(1); // Index 1
                        string description = reader.IsDBNull(2) ? null : reader.GetString(2); // Index 2
                        double price = reader.GetDouble(3); // Index 3
                        int quantityInStock = reader.GetInt32(4); // Index 4
                        string type = reader.GetString(5); // Index 5

                        // Create the product based on its type
                        Product product;

                        if (type.Equals("Electronics", StringComparison.OrdinalIgnoreCase))
                        {
                            string brand = reader.IsDBNull(6) ? null : reader.GetString(6); // Index 6
                            int? warrantyPeriod = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7); // Index 7
                            product = new Electronics(productName, description, price, quantityInStock, brand, warrantyPeriod ?? 0);
                            product.ProductId = productId; // Set ProductId after creation
                        }
                        else if (type.Equals("Clothing", StringComparison.OrdinalIgnoreCase))
                        {
                            string size = reader.IsDBNull(8) ? null : reader.GetString(8); // Index 8
                            string color = reader.IsDBNull(9) ? null : reader.GetString(9); // Index 9
                            product = new Clothing(productName, description, price, quantityInStock, size, color);
                            product.ProductId = productId; // Set ProductId after creation
                        }
                        else
                        {
                            // Skip products that do not match the expected types
                            continue;
                        }

                        productList.Add(product);
                    }
                }
            }
        }

        return productList;
    }

    public List<Product> GetOrderByUser(User user)
    {
        List<Product> orderedProducts = new List<Product>();

        // Check if the user exists in the database
        using (var conn = DBConnUtil.GetDBConn(_connectionString))
        {
            // Check if the user has any orders
            string userOrderQuery = @"
            SELECT p.productId, p.productName, p.description, p.price, p.quantityInStock, p.type,
                   p.brand, p.warrantyPeriod, p.size, p.color, od.quantity
            FROM Orders o
            JOIN OrderDetails od ON o.orderId = od.orderId
            JOIN Products p ON od.productId = p.productId
            WHERE o.userId = @userId";

            using (var cmd = new SqlCommand(userOrderQuery, conn))
            {
                cmd.Parameters.AddWithValue("@userId", user.UserId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string productType = reader["type"].ToString();

                            if (productType.Equals("Electronics", StringComparison.OrdinalIgnoreCase))
                            {
                                var electronics = new Electronics(
                                    reader["productName"].ToString(),
                                    reader["description"].ToString(),
                                    Convert.ToDouble(reader["price"]),
                                    Convert.ToInt32(reader["quantity"]),
                                    reader["brand"].ToString(),
                                    Convert.ToInt32(reader["warrantyPeriod"])
                                );
                                orderedProducts.Add(electronics);
                            }
                            else if (productType.Equals("Clothing", StringComparison.OrdinalIgnoreCase))
                            {
                                var clothing = new Clothing(
                                    reader["productName"].ToString(),
                                    reader["description"].ToString(),
                                    Convert.ToDouble(reader["price"]),
                                    Convert.ToInt32(reader["quantity"]),
                                    reader["size"].ToString(),
                                    reader["color"].ToString()
                                );
                                orderedProducts.Add(clothing);
                            }
                        }
                    }
                    else
                    {
                        throw new UserNotFoundException("No orders found for the specified user.");
                    }
                }
            }
        }

        return orderedProducts;
    }

}
