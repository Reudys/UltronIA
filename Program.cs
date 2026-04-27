using System;
using System.Threading.Tasks;
using UltronAPI.Models;
using UltronAPI.Services;

class Program
{
    static async Task Main(string[] args)
    {
        var memoryManager = new MemoryManager();
        var ollama = new OllamaService();
        var extractor = new MemoryExtractor(ollama);
        var repo = new MemoryRepository();

        int userId = 1;

        Console.WriteLine("Ultron ha despertado (escribe 'salir' para terminar)\n");

        while (true)
        {
            Console.Write("Tú: ");
            string mensaje = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(mensaje))
                continue;

            if (mensaje.ToLower() == "salir")
                break;

            try
            {
                // 🔹 1. Obtener memoria inteligente
                var memorias = memoryManager.ObtenerMemoriaInteligente(userId, mensaje);

                // 🔹 2. Convertir a texto
                var memoriaTexto = memoryManager.ConstruirMemoriaTexto(memorias);

                // 🔹 3. Prompt limpio y optimizado
                var prompt = $@"
Eres un asistente personal llamado Ultron.

Tu estilo:
- Elegante y profesional
- Claro y directo
- Cortés pero sin exagerar

Reglas IMPORTANTES:
- SOLO usa la memoria proporcionada
- NO inventes información
- Si no sabes algo, dilo claramente
- Responde en Español
- Sé preciso y evita respuestas innecesariamente largas

Memoria del usuario:
{memoriaTexto}

Usuario: {mensaje}
";

                // 🔹 4. Obtener respuesta IA
                var respuesta = await ollama.GenerarRespuesta(prompt);
                respuesta = respuesta?.Trim();

                Console.WriteLine("\nUltron: " + respuesta + "\n");

                // 🔹 5. Marcar memorias como usadas
                memoryManager.MarcarUso(memorias);

                // 🔥 6. Extraer memoria automáticamente (MEJORADO)
                var nuevasMemorias = await extractor.ExtraerMemoria(mensaje);

                foreach (var memoria in nuevasMemorias)
                {
                    if (!repo.ExisteMemoria(memoria))
                    {
                        repo.GuardarMemoria(new Memoria
                        {
                            UserId = userId,
                            Tipo = "hecho",
                            Contenido = memoria,
                            Categoria = "general",
                            Relevancia = 7
                        });

                        Console.WriteLine("💾 (Memoria guardada: " + memoria + ")");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error: " + ex.Message + "\n");
            }
        }

        Console.WriteLine("👋 Ultron finalizado");
    }
}