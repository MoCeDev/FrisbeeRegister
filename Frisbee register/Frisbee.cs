using MySql.Data.MySqlClient;
namespace Exercise001

{
    public class Frisbee
    {
        public static List<Frisbee> AllFrisbees { get; } = new List<Frisbee>();
        public string Brand { get; }
        public string Name { get; }
        public string Type { get; }
        public (int, int, int, int) FN { get; }
        public int Weight { get; }
        public string color { get; }
        public int Wear { get; }

        public Frisbee(string brand, string name, string type, (int, int, int, int) FN, int Weight, string color, int Wear)
        {
            Brand = brand;
            Name = name;
            Type = type;
            this.FN = FN;
            this.Weight = Weight;
            this.color = color;
            this.Wear = Wear;


            AllFrisbees.Add(this);
        }

        // Method to save the Frisbee instance to the MySQL database
        public void SaveToDatabase(string? connectionString)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                    INSERT INTO frisbees (BrandID, TypeID, Name, SPEED, GLIDE, TURN, FADE, Weight, color, Wear)
                    VALUES (@BrandID, @TypeID, @Name, @SPEED, @GLIDE, @TURN, @FADE, @Weight, @color, @Wear)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BrandID", Brand);
                        command.Parameters.AddWithValue("@TypeID", Type);
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@SPEED", FN.Item1);
                        command.Parameters.AddWithValue("@GLIDE", FN.Item2); 
                        command.Parameters.AddWithValue("@TURN", FN.Item3); 
                        command.Parameters.AddWithValue("@FADE", FN.Item4); 
                        command.Parameters.AddWithValue("@Weight", Weight);
                        command.Parameters.AddWithValue("@color", color);
                        command.Parameters.AddWithValue("@Wear", Wear);

                        command.ExecuteNonQuery();
                        Console.WriteLine($"Frisbee '{Name}' saved to database.");
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"MySQL error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General error: {ex.Message}");
                }
            }
        }

        public static void AllDiscs()
        {
            Console.WriteLine("All frisbees:");
            foreach (var disc in AllFrisbees)
            {
                Console.WriteLine(disc);
            }
        }

        public override string ToString()
        {
            return $"Brand: {Brand}, Disc name: {Name}, Type: {Type}, Flightnumbers: ({FN.Item1}, {FN.Item2}, {FN.Item3}, {FN.Item4})";
        }
    }
}
