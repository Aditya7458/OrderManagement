// Base class Product
public class Product
{
    public int ProductId { get; set; } // This can remain optional when inserting
    public string ProductName { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int QuantityInStock { get; set; }
    public string Type { get; set; } // Electronics or Clothing

    // Constructor is no longer necessary for DB insertion, you can use setters instead
    public Product(string productName, string description, double price, int quantityInStock, string type)
    {
        ProductName = productName;
        Description = description;
        Price = price;
        QuantityInStock = quantityInStock;
        Type = type;
    }
}

// Subclass Electronics
public class Electronics : Product
{
    public string Brand { get; set; }
    public int WarrantyPeriod { get; set; }

    public Electronics(string productName, string description, double price, int quantityInStock, string brand, int warrantyPeriod)
        : base(productName, description, price, quantityInStock, "Electronics")
    {
        Brand = brand;
        WarrantyPeriod = warrantyPeriod;
    }
}

// Subclass Clothing
public class Clothing : Product
{
    public string Size { get; set; }
    public string Color { get; set; }

    public Clothing(string productName, string description, double price, int quantityInStock, string size, string color)
        : base(productName, description, price, quantityInStock, "Clothing")
    {
        Size = size;
        Color = color;
    }
}
