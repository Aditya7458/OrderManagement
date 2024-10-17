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
        if (user.Role == "Admin")
        {
            products.Add(product);
            Console.WriteLine($"Product {product.ProductName} added.");
        }
        else
        {
            throw new UnauthorizedAccessException("Only admin users can add products.");
        }
    }

    public void CreateUser(User user)
    {
        users.Add(user);
        Console.WriteLine($"User {user.Username} created.");
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
