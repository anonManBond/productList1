using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;

public class Product
{
    // Properties to store product information
    public string Category { get; set; }
    public string ProductName { get; set; }
    public float Price { get; set; }

    // Constructor to initialize a new product
    public Product(string category, string productName, float price)
    {
        Category = category;
        ProductName = productName;
        Price = price;
    }

    // Method to display product details
    public void Display(bool isHighlighted = false)
    {
        const int categoryWidth = 20;
        const int productWidth = 25;
        const int priceWidth = 15;

        // If product is highlighted, display in a different color
        if (isHighlighted)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{Category.PadRight(categoryWidth)}{ProductName.PadRight(productWidth)}{Price.ToString("C").PadLeft(priceWidth)}");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine($"{Category.PadRight(categoryWidth)}{ProductName.PadRight(productWidth)}{Price.ToString("C").PadLeft(priceWidth)}");
        }
    }

    // Converts a product to CSV format (Category, ProductName, Price)
    public static string ToCSV(Product product) => $"{product.Category},{product.ProductName},{product.Price}";

    // Converts a CSV line back into a Product object
    public static Product FromCSV(string csvLine)
    {
        var values = csvLine.Split(',');
        return new Product(values[0], values[1], float.Parse(values[2]));
    }
}

public class Program
{
    // List to store products and file path to save/load product data
    static List<Product> products = new List<Product>();
    const string filePath = "products.csv";

    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Program List Manager");

        // Check if user is registered or logs in
        if (!HandleUserRegistrationOrLogin())
        {
            Console.WriteLine("Incorrect Login... Please Try Again:");
            return;
        }

        // Load existing products from file if available
        LoadProducts();

        // Start managing products
        ManageProducts();
    }

    // Handles user registration and login
    static bool HandleUserRegistrationOrLogin()
    {
        Console.WriteLine("To enter login - enter: 'L' | To register a new user - enter: 'R' | To delete an existing user - enter: 'D'");
        var key = Console.ReadKey(true).Key;

        // If user presses R to register, perform registration
        if (key == ConsoleKey.R)
        {
            RegisterNewUser();
            return HandleUserRegistrationOrLogin();
        }
        // If user presses 'L' for login, perform login
        if (key == ConsoleKey.L)
        {
            return Login(); 
        }
        // If user presses 'D' for deleting an existing user, delete the user
        if (key == ConsoleKey.D) 
        {
            DeleteUser();
            return HandleUserRegistrationOrLogin();
        }

        // If the user presses an invalid key, show an error message and return false
        ShowError("Invalid option. Please press 'L', 'R', or 'D'.");
        return HandleUserRegistrationOrLogin();
    }

    // Login process 
    static bool Login()
    {
        var userManager = new UserManager();

        Console.WriteLine("Enter Username: ");
        string username = Console.ReadLine();

        Console.WriteLine("Enter Password: ");
        string password = ReadPassword();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("Username or Password cannot be empty");
            return HandleUserRegistrationOrLogin();
        }

        return userManager.Login(username, password);
    }

    // User registration process
    static void RegisterNewUser()
    {
        var userManager = new UserManager();

        Console.WriteLine("Enter a new username: ");
        string username = Console.ReadLine();

        Console.WriteLine("Enter a new password: ");
        string password = ReadPassword();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("Username or Password cannot be empty");
            return;
        }

        userManager.AddUser(username, password);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("User registered successfully!");
        Console.ResetColor();

        Thread.Sleep(3000);
    }

    // Handels User password input without displaying it on the screen
    private static string ReadPassword()
    {
        StringBuilder password = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter) break;
            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Length--;
                Console.Write("\b \b");
            }
            else
            {
                password.Append(key.KeyChar);
                Console.Write('*');
            }
        }
        Console.WriteLine();
        return password.ToString();
    }

    // Manages adding products and displaying them
    static void ManageProducts()
    {
        // Show message if no products exist for the user
        if (!products.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Welcome! This is your first time using the Program, there are no products yet.");
            Console.ResetColor();
        }

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("To enter a new product - follow the steps | To quit - enter: 'Q'");
            Console.ResetColor();

            // Get product details from user
            var product = GetProductFromUser();
            if (product == null) break;

            // Add new product to the list
            products.Add(product);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("The product was successfully added!");
            Console.ResetColor();
            Console.WriteLine("---------------------------------------------------------------");
        }

        SaveProducts();
        DisplayProducts(products);
    }

    // Get product details from user input
    static Product GetProductFromUser()
    {
        Console.Write("Enter a Category: ");
        string category = Console.ReadLine();
        if (category.ToLower() == "q") return null;

        if (string.IsNullOrEmpty(category))
        {
            ShowError("Category cannot be empty");
            return null;
        }

        Console.Write("Enter a Product Name: ");
        string productName = Console.ReadLine();
        if (string.IsNullOrEmpty(productName))
        {
            ShowError("Product Name cannot be empty");
            return null;
        }

        // Get a valid price
        float price = GetValidPrice();
        return new Product(category, productName, price);
    }

    // Ensures that the entered price is a valid number
    static float GetValidPrice()
    {
        while (true)
        {
            Console.Write("Enter a Price: ");
            if (float.TryParse(Console.ReadLine(), out float price))
                return price;

            ShowError("Invalid Price. Please Enter a Valid Number.");
        }
    }

    // Displays a confirmation message and waits for user input hljjvhjvhhvjhvjhvj
    static bool ConfirmContinue(string message)
    {
        Console.WriteLine(message);
        var key = Console.ReadKey(true).Key;
        return key == ConsoleKey.Y;
    }

    // Displays a list of all products
    static void DisplayProducts(List<Product> products, string searchTerm = null)
    {
        if (!products.Any())
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("No products available.");
            Console.WriteLine("--------------------------------------------------");
        }
        else
        {
            Console.WriteLine("\n-------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.Green;

            const int categoryWidth = 20;
            const int productWidth = 25;
            const int priceWidth = 15;

            // Print headers
            Console.WriteLine($"{"Category".PadRight(categoryWidth)}{"Product".PadRight(productWidth)}{"Price".PadLeft(priceWidth)}");
            Console.ResetColor();

            // Sort products by price
            var sortedProducts = products.OrderBy(p => p.Price).ToList();
            float totalPrice = sortedProducts.Sum(p => p.Price);

            // Display each product and highlight if search matches
            foreach (var product in sortedProducts)
            {
                bool isMatched = searchTerm != null &&
                             (product.ProductName.ToLower().Contains(searchTerm.ToLower()) ||
                              product.Category.ToLower().Contains(searchTerm.ToLower()));
                product.Display(isHighlighted: isMatched);
            }

            // Display total amount of products
            Console.WriteLine();
            Console.WriteLine($"{"Total amount:",10} {totalPrice,25:C}");
            Console.WriteLine("---------------------------------------------------------------");
        }

        // Display options 
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("To enter a new product - enter: 'P' | To search for a product - enter: 'S' | To delete a product - enter: 'D' | To quit - enter: 'Q'");
        Console.ResetColor(); 
        HandleUserChoice();  // Handle user action from the displayed options
    }

    // Handles user choice, add product, search, delete, or quit
    static void HandleUserChoice()
    {
        ConsoleKey key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.P) 
        {
            ManageProducts(); // Add new product
        }
        else if (key == ConsoleKey.S)
        {
            SearchProducts(); // Search for a product
        }
        else if (key == ConsoleKey.Q)
        {
            Console.WriteLine("Goodbye!"); // Exit Program
        }
        else if (key == ConsoleKey.D) // Delete a Product
        {
            DeleteProduct();
        }
        else
        {
            ShowError("Invalid option. Please press 'P', 'S', 'D' or 'Q'.");
            HandleUserChoice();
        }
    }

    // Prompts the user to search for products
    static void SearchProducts()
    {
        Console.Write("Search: ");
        string searchTerm = Console.ReadLine();

        DisplayProducts(products, searchTerm);
    }

    // Deletes an existing user from the system
    static void DeleteUser()
    {
        var userManager = new UserManager();

        Console.WriteLine("Enter the username of the user you wish to delete: ");
        string username = Console.ReadLine();

        if (string.IsNullOrEmpty(username))
        {
            ShowError("Username cannot be empty.");
            return;
        }

        var user = userManager.GetUserByUsername(username);

        if (user == null)
        {
            ShowError("User not found.");
            return;
        }

        if (!ConfirmContinue($"Are you sure you want to delete the user '{username}'? (y/n)"))
        {
            Console.WriteLine("User deletion canceled.");
            return;
        }

        bool deleted = userManager.DeleteUser(username);

        if (deleted)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("User deleted successfully!");
            Console.ResetColor();
        }
        else
        {
            ShowError("Failed to delete user.");
        }
    }

    // Deletes a product from the list based on user choice
    static void DeleteProduct()
    {
        if (!products.Any())
        {
            ShowError("There are no products to delete.");
            return;
        }

        Console.WriteLine("Choose a product to delete by entering its number:");

        // Display products with numbers for user to select
        for (int i = 0; i < products.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {products[i].Category} - {products[i].ProductName} - {products[i].Price:C}");
        }

        int productNumber;
        while (true)
        {
            Console.Write("Enter the number of the product to delete: ");
            if (int.TryParse(Console.ReadLine(), out productNumber) && productNumber >= 1 && productNumber <= products.Count)
            {
                break;
            }
            else
            {
                ShowError("Invalid number, please try again.");
            }
        }

        // Remove the selected product
        products.RemoveAt(productNumber - 1);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Product deleted successfully!");
        Console.ResetColor();

        SaveProducts();
        DisplayProducts(products);
    }

    // Displays error message in red color
    static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    // Saves products to a CSV file
    static void SaveProducts()
    {
        File.WriteAllLines(filePath, products.Select(Product.ToCSV));
    }

    // Loads Products to a CSV file
    static void LoadProducts()
    {
        if (File.Exists(filePath))
        {
            products = File.ReadAllLines(filePath)
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                            .Select(Product.FromCSV)
                            .ToList();
        }
    }
}