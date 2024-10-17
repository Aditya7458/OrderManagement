// Base class Product
public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int QuantityInStock { get; set; }
    public string Type { get; set; } // Electronics or Clothing
    
    public Product(int productId, string productName, string description, double price, int quantityInStock, string type)
    {
        ProductId = productId;
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
    
    public Electronics(int productId, string productName, string description, double price, int quantityInStock, string brand, int warrantyPeriod)
        : base(productId, productName, description, price, quantityInStock, "Electronics")
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
    
    public Clothing(int productId, string productName, string description, double price, int quantityInStock, string size, string color)
        : base(productId, productName, description, price, quantityInStock, "Clothing")
    {
        Size = size;
        Color = color;
    }
}
