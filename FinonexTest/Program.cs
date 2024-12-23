using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

const string EVENT_FILE_PATH = "server_events.jsonl";
const string CONNECTION_STRING = "Host=localhost;Database=postgres;Username=postgres;Password=1";

app.MapGet("/", async (HttpContext context) =>
{
    return "Hello World";
});

app.MapPost("/liveEvent", async (HttpContext context) =>
{
    // Check authorization
    if (context.Request.Headers.Authorization != "secret")
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Unauthorized");
        return;
    }

    // Read the event
    using var reader = new StreamReader(context.Request.Body);
    var eventJson = await reader.ReadToEndAsync();

    // Append to file
    await File.AppendAllTextAsync(EVENT_FILE_PATH, eventJson);

    context.Response.StatusCode = 200;
    await context.Response.WriteAsync("Event recorded");
});

app.MapGet("/userEvents/{userId}", async (string userId) =>
{
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    await conn.OpenAsync();

    using var cmd = new NpgsqlCommand(
        "SELECT revenue FROM users_revenue WHERE user_id = @userId",
        conn);
    cmd.Parameters.AddWithValue("userId", userId);

    var result = await cmd.ExecuteScalarAsync();

    return new
    {
        userId = userId,
        revenue = result != DBNull.Value ? Convert.ToInt32(result) : 0
    };
});

app.Run("http://localhost:8000");