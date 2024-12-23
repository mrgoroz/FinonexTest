using System.Text.Json;
using Npgsql;

class Program
{
    private const string CONNECTION_STRING = "Host=localhost;Database=postgres;Username=postgres;Password=1";
    private const string EVENT_FILE = "C:\\Users\\Goisr\\source\\repos\\FinonexTest\\FinonexTest\\server_events.jsonl";

    class Event
    {
        public string userId { get; set; }
        public string name { get; set; }
        public int value { get; set; }
    }

    static async Task Main(string[] args)
    {
        try
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            await conn.OpenAsync();

            // Use transaction for atomicity
            using var transaction = await conn.BeginTransactionAsync();

            string[] events = await File.ReadAllLinesAsync(EVENT_FILE);

            foreach (var eventJson in events)
            {
                if (string.IsNullOrWhiteSpace(eventJson)) continue;

                var evt = JsonSerializer.Deserialize<Event>(eventJson);

                // Update revenue in database
                using var cmd = new NpgsqlCommand(
                    @"INSERT INTO users_revenue (user_id, revenue) 
                      VALUES (@userId, @value)
                      ON CONFLICT (user_id) 
                      DO UPDATE SET revenue = users_revenue.revenue + @delta",
                    conn);

                cmd.Parameters.AddWithValue("userId", evt.userId);
                cmd.Parameters.AddWithValue("value", evt.value);
                cmd.Parameters.AddWithValue("delta",
                    evt.name == "add_revenue" ? evt.value : -evt.value);

                await cmd.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            Console.WriteLine("Data processing completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}