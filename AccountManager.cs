using System;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

public class AccountManager
{
    private readonly string connectionString = "Server=localhost;Database=ATMAppDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public void RegisterAccount()
    {
        Console.WriteLine("\nRegistering a new account...");

        string username;
        do
        {
            Console.Write("Enter a new username: ");
            username = Console.ReadLine()?.Trim();

            if (UsernameExists(username))
            {
                Console.WriteLine("❌ Username already exists.");
                username = null;
            }
        } while (string.IsNullOrEmpty(username));

        Console.Write("Enter First Name: ");
        string firstName = Console.ReadLine();

        Console.Write("Enter Last Name: ");
        string lastName = Console.ReadLine();

        Console.Write("Enter Email: ");
        string email = Console.ReadLine();

        Console.Write("Enter Phone: ");
        string phone = Console.ReadLine();

        Console.Write("Enter Password: ");
        string password = ReadPassword();

        if (!SolveCaptcha())
        {
            Console.WriteLine("❌ CAPTCHA failed.");
            return;
        }

        string passwordHash = ComputeSha256Hash(password);

        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "INSERT INTO Customer (Username, PasswordHash, FirstName, LastName, Email, Phone) VALUES (@u, @p, @f, @l, @e, @ph)";
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", passwordHash);
                cmd.Parameters.AddWithValue("@f", firstName);
                cmd.Parameters.AddWithValue("@l", lastName);
                cmd.Parameters.AddWithValue("@e", email);
                cmd.Parameters.AddWithValue("@ph", phone);
                cmd.ExecuteNonQuery();
            }
        }

        Console.WriteLine("\n✅ Customer registered successfully.");
    }

    public bool UsernameExists(string username)
    {
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT COUNT(*) FROM Customer WHERE Username = @username";
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }

    public bool HasAccount(int customerId)
    {
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT COUNT(*) FROM Account WHERE CustomerID = @customerId";
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@customerId", customerId);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }

    public void CreateAccount(int customerId)
    {
        Console.Write("\nEnter account type (e.g., Savings, Checking): ");
        string accountType = Console.ReadLine()?.Trim();

        Console.Write("Enter initial deposit amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal balance))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            string sql = @"INSERT INTO Account (CustomerID, AccountType, Balance, CreatedAtDateTime) 
                           VALUES (@customerId, @accountType, @balance, GETDATE())";
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@customerId", customerId);
                cmd.Parameters.AddWithValue("@accountType", accountType);
                cmd.Parameters.AddWithValue("@balance", balance);
                cmd.ExecuteNonQuery();
            }
        }

        Console.WriteLine("✅ Account created successfully.");
    }

    public void CheckBalance()
    {
        Console.WriteLine("✅ Your current balance is: $1,000.00 [placeholder]");
    }

    public void DepositMoney()
    {
        Console.Write("\nEnter amount to deposit: $");
        string input = Console.ReadLine();
        Console.WriteLine($"\n[✔] ${input} deposited successfully!");
    }

    public void WithdrawMoney()
    {
        Console.Write("\nEnter amount to withdraw: $");
        string input = Console.ReadLine();
        Console.WriteLine($"\n[✔] Please collect your cash: ${input}");
    }

    public void ShowMiniStatement()
    {
        Console.WriteLine("\n[✔] Last 5 transactions:");
        Console.WriteLine(" - 04/05: +$500");
        Console.WriteLine(" - 03/30: -$100");
        Console.WriteLine(" - 03/27: +$200");
        Console.WriteLine(" - 03/25: -$50");
        Console.WriteLine(" - 03/20: -$20");
    }

    public void ChangePin()
    {
        Console.Write("\nEnter current PIN: ");
        string currentPin = Console.ReadLine();

        Console.Write("Enter new PIN: ");
        string newPin = Console.ReadLine();

        Console.Write("Confirm new PIN: ");
        string confirmPin = Console.ReadLine();

        if (newPin == confirmPin)
        {
            Console.WriteLine("\n[✔] PIN changed successfully!");
        }
        else
        {
            Console.WriteLine("\n[✘] PINs do not match. Try again.");
        }
    }

    private string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
    }

    private string ReadPassword()
    {
        StringBuilder input = new StringBuilder();
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                input.Append(key.KeyChar);
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input.Remove(input.Length - 1, 1);
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);
        return input.ToString();
    }

    private bool SolveCaptcha()
    {
        Random rand = new Random();
        int num1 = rand.Next(1, 10);
        int num2 = rand.Next(1, 10);
        int correct = num1 + num2;

        Console.WriteLine($"\nCAPTCHA: What is {num1} + {num2}?");
        Console.Write("Answer: ");
        string input = Console.ReadLine();

        return int.TryParse(input, out int result) && result == correct;
    }
}
