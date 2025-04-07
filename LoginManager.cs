using System;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

public class LoginManager
{
    private AccountManager accountManager = new AccountManager();

    public bool HandleLogin(out int customerId)
    {
        customerId = -1;
        bool validLogin = false;

        while (!validLogin)
        {
            Console.Clear();
            Console.WriteLine("=========================================");
            Console.WriteLine("            A-T-I-M MACHINE");
            Console.WriteLine("=========================================");

            Console.Write("Enter username: ");
            string username = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("\nUsername cannot be empty.");
                Pause();
                continue;
            }

            Console.Write("Enter password: ");
            string password = ReadPassword();
            string hashedPassword = ComputeSha256Hash(password);

            string connectionString = "Server=localhost;Database=ATMAppDB;Trusted_Connection=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT CustomerID FROM Customer WHERE Username = @username AND PasswordHash = @passwordHash";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@passwordHash", hashedPassword);

                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        customerId = Convert.ToInt32(result);
                        Console.WriteLine("\n✅ Login successful!");
                        validLogin = true;
                    }
                    else
                    {
                        Console.WriteLine("\n❌ Invalid username or password.");
                        Console.Write("\nWould you like to try again? Yes (Y) / No (N): ");
                        string response = Console.ReadLine()?.Trim().ToUpper();

                        if (response == "N")
                        {
                            Console.Write("\nWould you like to register a new account? Yes (Y) / No (N): ");
                            string registerResponse = Console.ReadLine()?.Trim().ToUpper();

                            if (registerResponse == "Y")
                            {
                                accountManager.RegisterAccount();
                                return HandleLogin(out customerId); // Try login again after registration
                            }
                            else
                            {
                                Console.WriteLine("\nGoodbye!");
                                return false;
                            }
                        }
                    }
                }
            }

            Pause();
        }

        return true;
    }

    private static void Pause()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static string ComputeSha256Hash(string rawData)
    {
        using (var sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var builder = new StringBuilder();
            foreach (var b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
    }

    private static string ReadPassword()
    {
        var input = new StringBuilder();
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
}
