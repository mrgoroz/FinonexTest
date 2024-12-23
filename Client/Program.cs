class Program
{
    private const string SERVER_URL = "http://localhost:8000/liveEvent";
    private const string EVENT_FILE = "C:\\Users\\Goisr\\source\\repos\\FinonexTest\\FinonexTest\\server_events.jsonl";

    static async Task Main(string[] args)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "secret");

        try
        {
            string[] events = await File.ReadAllLinesAsync(EVENT_FILE);

            foreach (var eventJson in events)
            {
                if (string.IsNullOrWhiteSpace(eventJson)) continue;

                var content = new StringContent(eventJson, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(SERVER_URL, content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to send event: {eventJson}");
                    Console.WriteLine($"Status code: {response.StatusCode}");
                }
                else
                {
                    Console.WriteLine($"Successfully sent event: {eventJson}");
                }

                // Add a small delay to prevent overwhelming the server
                await Task.Delay(100);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}