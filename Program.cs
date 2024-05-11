using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace lab_2_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=X1STANZZ;Database=lab_2_1;Trusted_Connection=True;";

            DatabaseManagement dbManagement = new DatabaseManagement(connectionString);

            while (true)
            {
                Console.WriteLine("Menu: ");
                Console.WriteLine("1. Show all tables");
                Console.WriteLine("2. Show table data");
                Console.WriteLine("3. Add data into table");
                Console.WriteLine("4. Queries");
                Console.WriteLine("5. Exit");
                Console.Write("Choose option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        dbManagement.DisplayTables();
                        break;
                    case "2":
                        Console.Write("Choose table: ");
                        string option = Console.ReadLine();
                        dbManagement.PrintTableByName(option);
                        break;
                    case "3":
                        Console.Write("Choose table: ");
                        string option_addData = Console.ReadLine();
                        dbManagement.AddDataToTable(option_addData);
                        break;
                    case "4":
                        Console.WriteLine("Additional Queries: ");
                        dbManagement.ExecuteComplexQuery();
                        break;
                    case "5":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Try again!");
                        break;
                }

            }

            Console.ReadLine();
        }
    }
}
