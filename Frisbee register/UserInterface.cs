namespace Exercise001;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using dotenv.net;
public class UserInterface
{
    public string? connectionString;
    public UserInterface()
    {
        DotEnv.Load();
        connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    }
    public void Start()
    {
        // Create a dictionary to map user input to actions
        var commands = new Dictionary<string, Action>
        {
            { "1", AddFrisbee },
            { "2", FetchData },
            { "3", UpdateRow },
            { "4", DeleteRow },
            { "5", StopProgram }

        };

        while (true)
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("1. Add discs");
            Console.WriteLine("2. Fetch Data");
            Console.WriteLine("3. Update row");
            Console.WriteLine("4. Delete row");
            Console.WriteLine("5. Stop program");
            Console.WriteLine();

            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                continue;
            }

            // Check if the input is a valid command and invoke the corresponding action
            if (commands.ContainsKey(input))
            {
                commands[input]();
            }
            else
            {
                Console.WriteLine("Invalid command. Please try again.");
            }
        }
    }

    public void AddFrisbee()
    {
        while (true)
        {
            //ASK for brand (innova, discmania etc..) check for empty response, check for x for stopping program, if all okay -> next question
            Console.WriteLine("Type 'x' to stop");
            Console.WriteLine("Give brand: 1. INNOVA, 2. DISCMANIA, 3. WESTSIDE DISC, 4. DYNAMIC DISC, 5. DISCRAFT, 6. LATITUDE 64, 7.PRODIGY");
            string? brand = Console.ReadLine();

            if (string.IsNullOrEmpty(brand))
            {
                Console.WriteLine("Invalid input. Try again!.");
                continue;
            }

            if (brand.Equals("X", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            //ASK disc name, check for empty
            Console.WriteLine("Give disc name: ");
            string? name = Console.ReadLine();
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Invalid input. Try again");
                continue;
            }

            //ASK for type, CHECK FOR empty
            Console.WriteLine("Give type (1. Putter, 2. Mid range, 3. Fairway, 4. Driver): ");
            string? type = Console.ReadLine();
            if (string.IsNullOrEmpty(type))
            {
                Console.WriteLine("Invalid type. Try again.");
                continue;
            }

            /*ASK for flight numbers (4 inputs separated by commas), check for empty string, then check that there are 4 inputs,  
            then check all 4 commas are integers, if they are convert them into 1 int (FN) -> continue
            */
            Console.WriteLine("Give flight numbers (comma-separated, e.g., 5,0,3,-2): ");
            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid input for flight numbers.");
                continue;
            }

            string[] flightNumbers = input.Split(',');

            if (flightNumbers.Length != 4)
            {
                Console.WriteLine("Invalid input for flight numbers. Please enter exactly four numbers separated by commas.");
                continue;
            }

            bool validInput = true;
            int[] parsedNumbers = new int[4];

            for (int i = 0; i < flightNumbers.Length; i++)
            {
                if (!int.TryParse(flightNumbers[i], out parsedNumbers[i]))
                {
                    validInput = false;
                    break;
                }
            }

            if (!validInput)
            {
                Console.WriteLine("Invalid input for flight numbers. Please enter four integers separated by commas.");
                continue;
            }

            // If we reach here, the input is valid
            (int, int, int, int) FN = (parsedNumbers[0], parsedNumbers[1], parsedNumbers[2], parsedNumbers[3]);

            //ASK for disc weight, check empty, check input is int
            Console.WriteLine("Give Weight: ");
            string? Weightinput = Console.ReadLine();
            if (string.IsNullOrEmpty(Weightinput))
            {
                continue;
            }
            int Weight;
            if (int.TryParse(Weightinput, out Weight))
            {
                //successfully parsed the weight
            }
            else
            {
                Console.WriteLine("Invalid input for Weight. Please enter a valid number.");
                continue;
            }

            //ASK for color, check empty
            Console.WriteLine("Give color: ");
            string? color = Console.ReadLine();
            if (string.IsNullOrEmpty(color))
            {
                continue;
            }
            //ASk for disc condition 1-5, check empty, check that input is int
            Console.WriteLine("Give Wear(1-5): ");
            string? Wearinput = Console.ReadLine();
            if (string.IsNullOrEmpty(Wearinput))
            {
                continue;
            }
            int Wear;
            if (int.TryParse(Wearinput, out Wear))
            {
                //successfully parsed Wear
            }
            else
            {
                Console.WriteLine("Invalid input for Wear. Please enter a valid number.");
                continue;
            }

            //MAKE OOP in Frisbee, save into database. confirm addition
            Frisbee frisbee = new Frisbee(brand.ToUpper(), name.ToUpper(), type.ToUpper(), FN, Weight, color.ToUpper(), Wear);

            // Save to database
            frisbee.SaveToDatabase(connectionString);

            Console.WriteLine($"Added {frisbee}\n");


            // Display all discs at the end


        }
    }


    void FetchData()
    {
        var commands = new Dictionary<string, Action>
        {
            { "1", FetchAll },
            { "2", FetchSpecific },


        };


        while (true)
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("Exit program: exit");
            Console.WriteLine("1. All data");
            Console.WriteLine("2. Specific data");

            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                return;
            }

            if (input.ToUpper() == "EXIT")
            {
                return;
            }

            // Check if the input is a valid command and invoke the corresponding action
            if (commands.ContainsKey(input))
            {
                commands[input]();
                return;
            }
            else
            {
                Console.WriteLine("Invalid command. Please try again.");
            }
        }
    }

    void FetchAll()
    {

        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query;
                MySqlCommand command = new MySqlCommand();


                // Query to fetch all records
                query = @"
                        SELECT ID, Name, BrandName, TypeName, SPEED, GLIDE, TURN, FADE, Weight, Color, Wear
                        FROM Frisbees f
                        JOIN Brands b ON f.BrandID = b.BrandID
                        JOIN Types t ON f.TypeID = t.TypeID
                        ORDER BY ID;
                    ";
                command = new MySqlCommand(query, connection);

                PrintTable(command);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    void FetchSpecific()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            List<string> filters = new List<string>();
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            string? input;
            int filterCount = 0;

            Console.WriteLine("");
            Console.WriteLine("For brand use: brandname = '(brand of disc)'");
            Console.WriteLine("For type use: typename = '(type of disc)'");
            Console.WriteLine("Add filters for fetching frisbees. Enter filters in the format `column = 'value'`.");
            Console.WriteLine("Type 'done' when you are finished adding filters.");

            while (true)
            {
                Console.Write("Enter filter (Example: brandid = '1'): ");
                input = Console.ReadLine();

                if (input?.Trim().ToLower() == "done") break;

                if (!string.IsNullOrWhiteSpace(input))
                {
                    string[] parts = input.Split('=');
                    if (parts.Length == 2)
                    {
                        string column = parts[0].Trim();
                        string value = parts[1].Trim().Trim('\'');

                        filters.Add($"{column} = @filter{filterCount}");
                        parameters.Add(new MySqlParameter($"@filter{filterCount}", value));
                        filterCount++;
                    }
                    else
                    {
                        Console.WriteLine("Invalid filter format. Use `column = 'value'`.");
                    }
                }
            }

            string query = @"
                SELECT ID, Name, BrandName, TypeName, SPEED, GLIDE, TURN, FADE, Weight, Color, Wear
                FROM Frisbees f
                JOIN Brands b ON f.BrandID = b.BrandID
                JOIN Types t ON f.TypeID = t.TypeID";

            if (filters.Count > 0)
            {
                query += " WHERE " + string.Join(" AND ", filters);
            }

            Console.WriteLine("");
            try
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddRange(parameters.ToArray());

                    PrintTable(command);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

    }

    static void PrintTable(MySqlCommand command)
    {
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            if (!reader.HasRows)
            {
                Console.WriteLine("No records found.");
                return;
            }

            Console.WriteLine("Fetching data...");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"{"ID",5} | {"TYPE",-20} | {"BRAND",-15} | {"TYPE",-10} | {"SPEED",5} | {"GLIDE",5} | {"TURN",5} | {"FADE",5} | {"WEIGHT",6} | {"COLOR",-15} | {"WEAR",4} |");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------");

            while (reader.Read())
            {
                int ID = reader.GetInt32("ID");
                string name = reader.GetString("Name");
                string brand = reader.GetString("BrandName");
                string type = reader.GetString("TypeName");
                int speed = reader.GetInt32("SPEED");
                int glide = reader.GetInt32("GLIDE");
                int turn = reader.GetInt32("TURN");
                int fade = reader.GetInt32("FADE");
                int weight = reader.GetInt32("Weight");
                string color = reader.GetString("Color");
                int wear = reader.GetInt32("Wear");

                Console.WriteLine($"{ID,5} | {name,-20} | {brand,-15} | {type,-10} | {speed,5} | {glide,5} | {turn,5} | {fade,5} | {weight,6} | {color,-15} | {wear,4} |");
            }

            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------------");
        }
    }

    void UpdateRow()
    {
        Console.WriteLine("Exit program: exit");
        Console.WriteLine("What row to update?");
        string? newID = Console.ReadLine();

        if (string.IsNullOrEmpty(newID))
        {
            return;
        }

        if (newID.ToUpper() == "EXIT")
        {
            return;
        }

        Console.WriteLine("What column to update?");
        string? column = Console.ReadLine();

        if (string.IsNullOrEmpty(column))
        {
            return;
        }

        Console.WriteLine("New value?");
        string? newValue = Console.ReadLine();

        if (string.IsNullOrEmpty(newValue))
        {
            return;
        }

        string[] allowedColumns = { "BrandID", "TypeID", "NAME", "SPEED", "GLIDE", "TURN", "FADE", "COLOR", "WEIGHT", "WEAR" };
        if (!allowedColumns.Contains(column, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine("Invalid column name.");
            return;
        }


        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = $@"
                    UPDATE frisbees 
                    SET {column} = @newValue
                    WHERE ID = @newID
                ";
                // UPDATE frisbees SET {column} = @newValue WHERE ID = @newID
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Bind parameters
                    command.Parameters.AddWithValue("@newValue", newValue.ToUpper());
                    command.Parameters.AddWithValue("@newID", newID);

                    // Execute query
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Row updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("No row found with the specified ID.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    void DeleteRow()
    {

        FetchAll();

        Console.WriteLine("Exit program: exit");
        Console.WriteLine("What ID to delete?");
        string? rowID = Console.ReadLine();

        if (string.IsNullOrEmpty(rowID))
        {
            return;
        }

        if (rowID.ToUpper() == "EXIT")
        {
            return;
        }

        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = $@"DELETE FROM frisbees 
                                WHERE ID = @rowID
                                ";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@rowID", rowID);


                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Row " + rowID + " deleted.");
                    }
                    else
                    {
                        Console.WriteLine("Delete has failed.");
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void StopProgram()
    {
        Console.WriteLine("Stopping the program...");
        // Add any cleanup code here if needed
        Environment.Exit(0); // Exits the program
    }
}