using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        var orchestrator = new Orchestrator(
            Constants.CONFIG_FILE_PATH,
            Constants.CONTRACTS_FILE_PATH
        );

        Console.WriteLine("Tool for expiring contracts renewal.");

        while (true)
        {
            Console.Write("Enter your command (\"s\" - start, \"q\" - quit or \"c\" - clean notification log): ");
            string cmd = Console.ReadLine()?.Trim().ToLower();

            switch (cmd)
            {
                case "s":
                    orchestrator.LoadNotifications();
                    orchestrator.ClearCurrentNotifications();

                    Console.Write("Enter the current date in format YYYY-MM-DD (press Enter for today): ");
                    string currentDateStr = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(currentDateStr))
                        currentDateStr = DateTime.Now.ToString(Constants.DATE_FORMAT);

                    try
                    {
                        Console.WriteLine($"Date entered: {currentDateStr}");
                        orchestrator.AddCurrentDatetimeStr(currentDateStr);
                        orchestrator.CheckContracts();
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine("Invalid date format. Please use YYYY-MM-DD format (e.g., 2025-08-04)");
                    }
                    break;

                case "q":
                    Console.WriteLine("Exiting the tool");
                    return;

                case "c":
                    Console.WriteLine("Clearing notification log: " + Constants.NOTIFICATION_LOG_FILE_PATH);
                    EmptyFile(Constants.NOTIFICATION_LOG_FILE_PATH);
                    break;

                default:
                    Console.WriteLine("Invalid command");
                    break;
            }
        }
    }

    private static void EmptyFile(string filePath)
    {
        File.WriteAllText(filePath, "{}"); // overwrite with empty JSON object
    }
}
