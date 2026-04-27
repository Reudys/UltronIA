using System.Text;
using System.Text.Json;

public class OllamaService
{
    private HttpClient _http = new HttpClient();

    public async Task<string> GenerarRespuesta(string prompt)
    {
        var data = new
        {
            model = "llama3",
            prompt = prompt,
            stream = false
        };

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync("http://localhost:11434/api/generate", content);
        var result = await response.Content.ReadAsStringAsync();

        // 🔥 Parsear JSON correctamente
        using var doc = JsonDocument.Parse(result);
        return doc.RootElement.GetProperty("response").GetString();
    }
}