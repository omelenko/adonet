using System.Collections.Generic;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

internal class Program
{
    static void Main()
    {
        string connectionString = "Data source=:memory:";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                Console.WriteLine("Пiдключено до бази даних");
                CreateTable(connection);
                Console.WriteLine("Запит 1: Вивiд всiєї iнформацiї");
                DisplayAllData(connection);
                Console.WriteLine("Запит 2: Вивiд всiх назв");
                DisplayAllNames(connection);
                Console.WriteLine("Запит 3: Вивiд всiх кольорiв");
                DisplayAllColors(connection);
                Console.WriteLine("Запит 4-6: Показ калорiйностi");
                DisplayCaloricInfo(connection);
                DisplayStatistics(connection);
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка пiдключення до бази даних: " + ex.Message);
            }
        }
    }
    
    static void CreateTable(SqliteConnection connection)
    {
        string q = "CREATE TABLE VegetablesAndFruits (\r\n    Id INT PRIMARY KEY,\r\n    Name NVARCHAR(100),\r\n    Type NVARCHAR(50),\r\n    Color NVARCHAR(50),\r\n    Calories INT\r\n);\r\n";
        SqliteCommand c = connection.CreateCommand();
        c.CommandText = q;
        c.ExecuteNonQuery();
        string q1 = $"INSERT INTO VegetablesAndFruits(Id, Name, Type, Color, Calories) VALUES(0, 'Помiдор', 'Овоч', 'Червоний', 100);";
        SqliteCommand c1 = connection.CreateCommand();
        c1.CommandText = q1;
        c1.ExecuteNonQuery();
        string q2 = $"INSERT INTO VegetablesAndFruits(Id, Name, Type, Color, Calories) VALUES(1, 'Яблуко', 'Фрукт', 'Зелений', 200);";
        SqliteCommand c2 = connection.CreateCommand();
        c2.CommandText = q2;
        c2.ExecuteNonQuery();
        string q3 = $"INSERT INTO VegetablesAndFruits(Id, Name, Type, Color, Calories) VALUES(2, 'Огiрок', 'Овоч', 'Зелений',  120);";
        SqliteCommand c3 = connection.CreateCommand();
        c3.CommandText = q3;
        c3.ExecuteNonQuery();
        string q4 = $"INSERT INTO VegetablesAndFruits(Id, Name, Type, Color, Calories) VALUES(3, 'Груша', 'Фрукт', 'Жовтий',  110);";
        SqliteCommand c4 = connection.CreateCommand();
        c4.CommandText = q4;
        c4.ExecuteNonQuery();
    }

    static void DisplayAllData(SqliteConnection connection)
    {
        string query = "SELECT * FROM VegetablesAndFruits";
        DataTable dataTable = new DataTable();
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = query;
        SqliteDataReader reader = command.ExecuteReader();

        if (reader.HasRows)
            for (int i = 0; i < reader.FieldCount; i++)
                dataTable.Columns.Add(new DataColumn(reader.GetName(i)));

        int j = 0;
        while (reader.Read())
        {
            DataRow row = dataTable.NewRow();
            dataTable.Rows.Add(row);

            for (int i = 0; i < reader.FieldCount; i++)
                dataTable.Rows[j][i] = (reader.GetValue(i));

            j++;
        }

        foreach (DataRow row in dataTable.Rows)
        {
            Console.WriteLine($"{row["Name"]}, {row["Type"]}, {row["Color"]}, {row["Calories"]} kcal");
        }
    }

    static void DisplayAllNames(SqliteConnection connection)
    {
        string query = "SELECT Name FROM VegetablesAndFruits";
        SqliteCommand command = new SqliteCommand(query, connection);
        SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(reader["Name"]);
        }

        reader.Close();
    }

    static void DisplayAllColors(SqliteConnection connection)
    {
        string query = "SELECT DISTINCT Color FROM VegetablesAndFruits";
        SqliteCommand command = new SqliteCommand(query, connection);
        SqliteDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(reader["Color"]);
        }

        reader.Close();
    }

    static void DisplayCaloricInfo(SqliteConnection connection)
    {
        string maxCaloriesQuery = "SELECT MAX(Calories) FROM VegetablesAndFruits";
        SqliteCommand maxCaloriesCommand = new SqliteCommand(maxCaloriesQuery, connection);
        Int64 maxCalories = (Int64)maxCaloriesCommand.ExecuteScalar();
        Console.WriteLine($"Максимальна калорiйнiсть: {maxCalories} kcal");
        string minCaloriesQuery = "SELECT MIN(Calories) FROM VegetablesAndFruits";
        SqliteCommand minCaloriesCommand = new SqliteCommand(minCaloriesQuery, connection);
        Int64 minCalories = (Int64)minCaloriesCommand.ExecuteScalar();
        Console.WriteLine($"Мiнiмальна калорiйнiсть: {minCalories} kcal");
        string avgCaloriesQuery = "SELECT AVG(Calories) FROM VegetablesAndFruits";
        SqliteCommand avgCaloriesCommand = new SqliteCommand(avgCaloriesQuery, connection);
        double avgCalories = (double)avgCaloriesCommand.ExecuteScalar();
        Console.WriteLine($"Середня калорiйнiсть: {avgCalories:F2} kcal");
    }// ... попередній код ...

    static void DisplayStatistics(SqliteConnection connection)
    {
        Console.WriteLine("Запит 7-9: Показати кiлькiсть овочiв, фруктiв, та кiлькiсть за кольорами");
        string countVegetablesQuery = "SELECT COUNT(*) FROM VegetablesAndFruits WHERE Type = 'Овоч'";
        string countFruitsQuery = "SELECT COUNT(*) FROM VegetablesAndFruits WHERE Type = 'Фрукт'";
        string countByColorQuery = "SELECT Color, COUNT(*) FROM VegetablesAndFruits GROUP BY Color";

        SqliteCommand countVegetablesCommand = new SqliteCommand(countVegetablesQuery, connection);
        SqliteCommand countFruitsCommand = new SqliteCommand(countFruitsQuery, connection);
        SqliteCommand countByColorCommand = new SqliteCommand(countByColorQuery, connection);

        Console.WriteLine($"Кiлькiсть овочiв: {countVegetablesCommand.ExecuteScalar()}");
        Console.WriteLine($"Кiлькiсть фруктiв: {countFruitsCommand.ExecuteScalar()}");
        Console.WriteLine($"Кiлькiсть за кольором: {countByColorCommand.ExecuteScalar()}");

        SqliteDataReader countByColorReader = countByColorCommand.ExecuteReader();
        while (countByColorReader.Read())
        {
            Console.WriteLine($"Кiлькiсть {countByColorReader["Color"]} овочiв та фруктiв: {countByColorReader[1]}");
        }
        countByColorReader.Close();

        Console.WriteLine("Запит 10-12: Фiльтрація за калорiйнiстю");
        int specifiedCalories = 100;

        string belowCaloriesQuery = $"SELECT COUNT(*) FROM VegetablesAndFruits WHERE Calories < {specifiedCalories}";
        string aboveCaloriesQuery = $"SELECT COUNT(*) FROM VegetablesAndFruits WHERE Calories > {specifiedCalories}";
        string withinRangeCaloriesQuery = $"SELECT COUNT(*) FROM VegetablesAndFruits WHERE Calories BETWEEN {specifiedCalories} AND {specifiedCalories + 100}";

        SqliteCommand belowCaloriesCommand = new SqliteCommand(belowCaloriesQuery, connection);
        SqliteCommand aboveCaloriesCommand = new SqliteCommand(aboveCaloriesQuery, connection);
        SqliteCommand withinRangeCaloriesCommand = new SqliteCommand(withinRangeCaloriesQuery, connection);

        Console.WriteLine($"Кiлькiсть овочiв\\фруктiв де калорiйнiсть менша {specifiedCalories}: {belowCaloriesCommand.ExecuteScalar()}");
        Console.WriteLine($"Кiлькiсть овочiв\\фруктiв де калорiйнiсть бiльша {specifiedCalories}: {aboveCaloriesCommand.ExecuteScalar()}");
        Console.WriteLine($"Кiлькiсть овочiв\\фруктiв де калорiйнiсть в радiусi мiж {specifiedCalories} i {specifiedCalories + 100}: {withinRangeCaloriesCommand.ExecuteScalar()}");
    }
}
