var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();

// Serve index.html
app.MapGet("/", () => Results.Redirect("/index.html"));

// POST /chat
app.MapPost("/chat", async (HttpRequest request) =>
{
    var data = await request.ReadFromJsonAsync<ChatRequest>();
    var userMessage = data.Messages.Last().Content;

    // Exact match first
    foreach (var item in ChattyFAQ.Data)
    {
        if (userMessage.Contains(item.Key))
            return Results.Json(new { reply = item.Value });
    }

    // If no exact match, try partial suggestions
    var suggestions = ChattyFAQ.Data.Keys
        .Where(key => userMessage.Any(c => key.Contains(c)))
        .ToList();

    if (suggestions.Any())
    {
        string suggested = suggestions.First();
        return Results.Json(new { reply = $"คุณหมายถึง \"{suggested}\" หรือไม่?\nคำตอบ: {ChattyFAQ.Data[suggested]}" });
    }

    return Results.Json(new { reply = "ขออภัย ไม่พบข้อมูลที่เกี่ยวข้องครับ" });
});

// POST /upload
app.MapPost("/upload", async (IFormFile file) =>
{
    return Results.Json(new { message = $"Received file: {file.FileName}, size: {file.Length} bytes" });
});

app.Run();

record ChatRequest(List<Message> Messages);
record Message(string Role, string Content);
