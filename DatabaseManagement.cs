using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab_2_1
{
    internal class DatabaseManagement
    {
        private String connectionString;
        public DatabaseManagement(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void DisplayTables()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection open");

                    PrintTableData(connection, "Author");

                    PrintTableData(connection, "Publisher");

                    PrintTableData(connection, "Genre");

                    PrintTableData(connection, "Customer");

                    PrintTableData(connection, "Book");

                    PrintTableData(connection, "Review");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine("Connection close");
                }
            }
        }
        private void PrintTableData(SqlConnection connection, string tableName)
        {
            string sql = $"SELECT * FROM {tableName}";
            SqlCommand command = new SqlCommand(sql, connection);
            SqlDataReader reader = command.ExecuteReader();

            Console.WriteLine($"\nData from table {tableName}:\n");

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write($"{reader.GetName(i)}: {reader.GetValue(i)}\t");
                }
                Console.WriteLine();
            }
            reader.Close();
        }

        public void PrintTableByName(string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection open");

                    PrintTableData(connection, tableName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine("Connection close");
                }
            }
        }
        public void AddDataToTable(string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine($"Connection opened");

                    int currentMaxId = GetCurrentMaxId(tableName);

                    Console.Write($"Enter Id (current value: {currentMaxId}): ");
                    int newId = int.Parse(Console.ReadLine());

                    SqlCommand cmd = new SqlCommand($"SELECT * FROM {tableName}", connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    int columnCount = reader.FieldCount;
                    string[] columnNames = new string[columnCount];
                    for (int i = 0; i < columnCount; i++)
                    {
                        columnNames[i] = reader.GetName(i);
                    }
                    reader.Close();

                    string[] values = new string[columnCount];
                    for (int i = 0; i < columnCount; i++)
                    {
                        if (columnNames[i] == "Id")
                        {
                            values[i] = newId.ToString();
                            continue;
                        }

                        Console.Write($"Enter value for column '{columnNames[i]}': ");
                        string value = Console.ReadLine();
                        values[i] = $"'{value}'";
                    }

                    string columns = string.Join(", ", columnNames);
                    string columnValues = string.Join(", ", values);

                    string sql = $"INSERT INTO {tableName} ({columns}) VALUES ({columnValues});"; 
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();

                    Console.WriteLine("Data was successfully added.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                    Console.WriteLine($"Connection closed.");
                }
            }
        }

        private int GetCurrentMaxId(string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand($"SELECT MAX(Id) FROM {tableName}", connection);
                object result = cmd.ExecuteScalar();
                if (result == DBNull.Value)
                {
                    return 0; 
                }
                else
                {
                    return Convert.ToInt32(result);
                }
            }
        }

        public void ExecuteComplexQuery()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query1 = @"SELECT Book.Name AS BookName, Author.Name AS AuthorName, Publisher.Name AS PublisherName
                              FROM Book
                              INNER JOIN Author ON Book.AuthorId = Author.Id
                              INNER JOIN Publisher ON Book.PublisherId = Publisher.Id";

                DataTable result1 = ExecuteQuery(connection, query1);

                string query2 = @"SELECT Name, Email, Country, City, Address
                              FROM Customer
                              WHERE Country = @Country";

                SqlParameter countryParam = new SqlParameter("@Country", SqlDbType.NVarChar);
                countryParam.Value = "UK";

                DataTable result2 = ExecuteQuery(connection, query2, countryParam);

                string query3 = @"SELECT COUNT(Id) AS TotalAuthors
                              FROM Author";

                DataTable result3 = ExecuteQuery(connection, query3);

                Console.WriteLine("Join tables query:");
                PrintDataTable(result1);

                Console.WriteLine("\nFiltration query:");
                PrintDataTable(result2);

                Console.WriteLine("\nAggregate functions query:");
                PrintDataTable(result3);
            }
        }

        private DataTable ExecuteQuery(SqlConnection connection, string query, params SqlParameter[] parameters)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters);

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        private void PrintDataTable(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    Console.Write($"{column.ColumnName}: {row[column]} \t");
                }
                Console.WriteLine();
            }
        }
    }
}
