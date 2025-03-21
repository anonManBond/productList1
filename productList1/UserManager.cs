using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class UserManager
{
    // File path for storing users in CSV format
    private const string UserFilePath = "users.csv";

    // List to hold all registered users
    private List<User> users = new List<User>();

    // Constructor to initialize UserManager and load existing users from file
    public UserManager()
    {
        LoadUsers(); // Load users from file 
    }

    // Login method to validate user credentials
    public bool Login(string username, string password)
    {
        // Hash the password and try to find the user
        string hashedPassword = HashPassword(password);
        // Find a user that matches both username and hashed password
        var user = users.Find(u => u.Username == username && u.Password == hashedPassword);
        // Return true if user is found, otherwise false
        return user != null;
    }

    // Method to load users from the File (CSV File in this case)
    private void LoadUsers()
    {
        if (File.Exists(UserFilePath))
        {
            foreach (var line in File.ReadAllLines(UserFilePath))
            {
                var parts = line.Split(',');
                if (parts.Length == 2)
                {
                    users.Add(new User(parts[0], parts[1]));
                }
            }
        }
        else 
        {
            Console.WriteLine("User file not found. A new file will be created upon registration.");
        }
    }

    // Method to add a new user to the system
    public void AddUser(string username, string password)
    {
        string hashedPassword = HashPassword(password);
        users.Add(new User(username, hashedPassword));
        SaveUsers();
    }

    // Method to save the list of users to the file (the CSV file in this case)
    private void SaveUsers()
    {
        var lines = new List<string>();
        foreach (var user in users)
        {
            lines.Add($"{user.Username},{user.Password}");
        }
        File.WriteAllLines(UserFilePath, lines);
    }

    // Method to hash a password using SHA256 algorithm
    private string HashPassword(string password) 
    {
        using (SHA256 sha256 = SHA256.Create()) 
        {
            // Convert password to bytes and compute the hash
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            // Convert hash bytes to a string and return it in lowercase without hyphens
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
    // Get a user by username
    public User GetUserByUsername(string username)
    {
        return users.Find(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    // Delete a user by username
    public bool DeleteUser(string username)
    {
        var userToDelete = GetUserByUsername(username);
        if (userToDelete != null)
        {
            users.Remove(userToDelete);
            SaveUsers();
            return true;
        }
        return false;
    }

}

public class User
{
    // Properties to store the username and password
    public string Username { get; }
    public string Password { get; }

    // Constructor to initialize the User object
    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
