using YourNamespace.util;
public class Program
{
    static void Main(string[] args)
    {
        IOrderManagementRepository orderProcessor = new OrderProcessor();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("1. Create User\n2. Create Product\n3. Cancel Order\n4. Get All Products\n5. Get Orders by User\n6. Exit");
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

                    // Assuming the user is retrieved from some user repository
                    User admin = new User(adminId, "Admin", "password", "Admin"); // Adjust as necessary to get the real admin user

                    Console.WriteLine("Enter Product Name:");
                    string productName = Console.ReadLine();

                    Console.WriteLine("Enter Description:");
                    string description = Console.ReadLine();

                    Console.WriteLine("Enter Price:");
                    double price = Convert.ToDouble(Console.ReadLine());

                    Console.WriteLine("Enter Quantity in Stock:");
                    int quantity = Convert.ToInt32(Console.ReadLine());

                    // You can create the product as Electronics or Clothing based on type
                    // For example, let's assume it's Electronics; adjust accordingly
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

                    // Call CreateProduct method
                    orderProcessor.CreateProduct(admin, product);
                    break;

                case 3:
                    // Cancel order logic
                    break;
                case 4:
                    // Get all products
                    break;
                case 5:
                    // Get orders by user
                    break;
                case 6:
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
}