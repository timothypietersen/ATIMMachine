using System;

class Program
{
    static AccountManager accountManager = new AccountManager();
    static int currentCustomerId;

    static void Main()
    {
        Console.Title = "A-T-I-M MACHINE";

        LoginManager loginManager = new LoginManager();
        bool isLoggedIn = loginManager.HandleLogin(out currentCustomerId);

        if (isLoggedIn)
        {
            ShowMainMenu();
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static void ShowMainMenu()
    {
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            DisplayMainMenu();

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    HandleAccountOption(CheckBalance);
                    break;
                case "2":
                    HandleAccountOption(DepositMoney);
                    break;
                case "3":
                    HandleAccountOption(WithdrawMoney);
                    break;
                case "4":
                    HandleAccountOption(ShowMiniStatement);
                    break;
                case "5":
                    HandleAccountOption(ChangePin);
                    break;
                case "6":
                    Console.WriteLine("\nThank you for using the ATM. Goodbye!");
                    exit = true;
                    break;
                default:
                    Console.WriteLine("\nInvalid option. Please try again.");
                    break;
            }

            if (!exit)
            {
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey();
            }
        }
    }

    static void DisplayMainMenu()
    {
        Console.WriteLine("=========================================");
        Console.WriteLine("            A-T-I-M MACHINE");
        Console.WriteLine("=========================================");
        Console.WriteLine("1. Check Balance");
        Console.WriteLine("2. Deposit");
        Console.WriteLine("3. Withdraw");
        Console.WriteLine("4. Mini Statement");
        Console.WriteLine("5. Change PIN");
        Console.WriteLine("6. Exit");
        Console.Write("\nSelect an option (1-6): ");
    }

    static void HandleAccountOption(Action accountAction)
    {
        // Pass currentCustomerId to the HasAccount and CreateAccount methods
        if (accountManager.HasAccount(currentCustomerId))
        {
            accountAction();
        }
        else
        {
            accountManager.CreateAccount(currentCustomerId);
        }
    }

    static void CheckBalance()
    {
        Console.WriteLine("\n[✔] Your current balance is: $1,000.00");
    }

    static void DepositMoney()
    {
        Console.Write("\nEnter amount to deposit: $");
        string input = Console.ReadLine();
        Console.WriteLine($"\n[✔] ${input} deposited successfully!");
    }

    static void WithdrawMoney()
    {
        Console.Write("\nEnter amount to withdraw: $");
        string input = Console.ReadLine();
        Console.WriteLine($"\n[✔] Please collect your cash: ${input}");
    }

    static void ShowMiniStatement()
    {
        Console.WriteLine("\n[✔] Last 5 transactions:");
        Console.WriteLine(" - 04/05: +$500");
        Console.WriteLine(" - 03/30: -$100");
        Console.WriteLine(" - 03/27: +$200");
        Console.WriteLine(" - 03/25: -$50");
        Console.WriteLine(" - 03/20: -$20");
    }

    static void ChangePin()
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
}
//comment
//comment 2
