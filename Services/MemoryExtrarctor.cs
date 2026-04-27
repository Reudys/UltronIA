using System.Text.Json;

namespace UltronAPI.Services
{
    public class MemoryExtractor
    {
        private readonly OllamaService _ollama;

        public MemoryExtractor(OllamaService ollama)
        {
            _ollama = ollama;
        }

        public async Task<List<string>> ExtraerMemoria(string mensaje)
        {
            var prompt = $@"
Extrae información importante del usuario.

Reglas:
- Devuelve un JSON válido
- Formato: {{ ""memorias"": [""texto1"", ""texto2""] }}
- Cada memoria debe ser corta y clara
- No combines información
- Ignora preguntas y cosas temporales
- Si no hay nada útil, devuelve: {{ ""memorias"": [] }}

Mensaje:
{mensaje}
";

            var result = await _ollama.GenerarRespuesta(prompt);

            try
            {
                var json = JsonDocument.Parse(result);

                var memorias = json.RootElement
                    .GetProperty("memorias")
                    .EnumerateArray()
                    .Select(x => x.GetString())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();

                return memorias;
            }
            catch
            {
                // fallback por si la IA rompe el JSON
                return new List<string>();
            }
        }
    }
}