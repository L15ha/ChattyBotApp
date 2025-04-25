var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();  // Serves wwwroot by default

// Serve index.html
app.MapGet("/", () => Results.Redirect("/index.html"));

// POST /chat
app.MapPost("/chat", async (HttpRequest request) =>
{
    var data = await request.ReadFromJsonAsync<ChatRequest>();
    var userMessage = data.Messages.Last().Content;

    foreach (var item in ChattyFAQ.Data)
    {
        if (userMessage.Contains(item.Key))
            return Results.Json(new { reply = item.Value });
    }

    return Results.Json(new { reply = "This is a mock GPT reply." });
});

// POST /upload
app.MapPost("/upload", async (IFormFile file) =>
{
    return Results.Json(new { message = $"Received file: {file.FileName}, size: {file.Length} bytes" });
});

app.Run();

record ChatRequest(List<Message> Messages);
record Message(string Role, string Content);
