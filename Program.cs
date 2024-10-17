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
                    User admin = new User(adminId, "Admin", "password", "Admin");
                    Console.WriteLine("Enter Product ID:");
                    int productId = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter Product Name:");
                    string productName = Console.ReadLine();
                    Console.WriteLine("Enter Description:");
                    string description = Console.ReadLine();
                    Console.WriteLine("Enter Price:");
                    double price = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Enter Quantity in Stock:");
                    int quantity = Convert.ToInt32(Console.ReadLine());
                    Product product = new Product(productId, productName, description, price, quantity, "General");
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