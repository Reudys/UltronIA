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

                // 🔹 3. Prompt
                var prompt = $@"
Eres Ultron, un asistente personal.

Reglas:
- Usa SOLO la memoria proporcionada
- No inventes información
- Responde breve (2-4 líneas)
- Si no hay datos en memoria, di: ""No tengo esa información en mi memoria.""

Memoria del usuario:
{memoriaTexto}

Usuario: {mensaje}
";

                // 🔹 4. IA response
                var respuesta = await ollama.GenerarRespuesta(prompt);
                Console.WriteLine("\nUltron: " + respuesta?.Trim() + "\n");

                // 🔹 5. Marcar memorias usadas
                memoryManager.MarcarUso(memorias);

                // 🔥 6. EXTRAER MEMORIA (CORREGIDO PARA MemoryItem)
                var nuevasMemorias = await extractor.ExtraerMemoria(mensaje);

                if (nuevasMemorias.Count > 0)
                {
                    foreach (var memoria in nuevasMemorias)
                    {
                        if (memoria == null)
                            continue;

                        if (!repo.ExisteMemoria(memoria.Valor))
                        {
                            repo.GuardarMemoria(new Memoria
                            {
                                UserId = userId,
                                Tipo = memoria.Tipo ?? "otro",
                                Contenido = memoria.Valor,
                                Categoria = memoria.Contexto ?? "general",
                                Relevancia = 7
                            });

                            Console.WriteLine($"💾 (Memoria guardada: {memoria.Valor})");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("🧠 (No se extrajo memoria relevante)");
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