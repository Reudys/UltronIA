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

        public async Task<List<MemoryItem>> ExtraerMemoria(string mensaje)
        {
            var prompt = $@"
Analiza el mensaje del usuario y extrae SOLO información útil para memoria.

REGLAS:
- NO guardes preguntas
- NO guardes respuestas del asistente
- NO inventes información
- NO completes información faltante
- SOLO usa lo que el usuario dijo literalmente
- Si no hay información útil: devuelve []

Clasifica automáticamente cada memoria en:

- tipo: (hecho | preferencia | habilidad | otro)
- valor: información importante resumida
- contexto: tema general (opcional)

REGLAS CRÍTICAS:
- NO describas al usuario
- NO interpretes
- NO clasifiques en lenguaje natural
- SOLO extrae datos literales
- PROHIBIDO generar frases como ""el usuario...""

RESPONDE SOLO EN JSON válido:

{{
  ""memorias"": [
    {{
      ""tipo"": """",
      ""valor"": """",
      ""contexto"": """"
    }}
  ]
}}

Mensaje:
{mensaje}
";

            var result = await _ollama.GenerarRespuesta(prompt);

            try
            {
                var json = JsonDocument.Parse(result);

                var list = new List<MemoryItem>();

                foreach (var item in json.RootElement.GetProperty("memorias").EnumerateArray())
                {
                    var tipo = item.GetProperty("tipo").GetString();
                    var valor = item.GetProperty("valor").GetString();
                    var contexto = item.TryGetProperty("contexto", out var ctx) ? ctx.GetString() : null;

                    if (string.IsNullOrWhiteSpace(valor))
                        continue;

                    list.Add(new MemoryItem
                    {
                        Tipo = tipo ?? "otro",
                        Valor = valor.Trim(),
                        Contexto = contexto
                    });
                }

                return list;
            }
            catch
            {
                return new List<MemoryItem>();
            }
        }
    }

    public class MemoryItem
    {
        public string Tipo { get; set; }
        public string Valor { get; set; }
        public string Contexto { get; set; }
    }
}
