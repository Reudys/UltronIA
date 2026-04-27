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
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Ultron ha despertado \n");

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.White;
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

                // 🔹 3. Prompt mejorado
                var prompt = $@"
Eres Ultron, yo tu creador soy Reudys Estrella Peréz, debes dirigirte a mi como Sr. Estrella.

Reglas OBLIGATORIAS:
- NO te presentes.
- NO digas ""como Ultron"".
- NO expliques tu función.
- NO agregues introducciones ni cierres.

Comportamiento:
- Usa la memoria SOLO si la pregunta lo requiere.
- Si la pregunta es personal (ej: ""dónde vivo"", ""qué me gusta""), usa la memoria.
- Si no, responde normalmente.
- Si no existe información en memoria, responde: ""No tengo esa información en mi memoria.""

Estilo:
- Respuesta directa
- Máximo 1-2 líneas
- Sin adornos

Memoria del usuario:
{memoriaTexto}

Usuario: {mensaje}
";

                // 🔹 4. Obtener respuesta IA
                var respuesta = await ollama.GenerarRespuesta(prompt);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nUltron: " + respuesta?.Trim() + "\n");
                

                // 🔹 5. Marcar memorias usadas
                memoryManager.MarcarUso(memorias);

                // 🔥 6. EXTRAER MEMORIA (CON FILTROS)
                var nuevasMemorias = await extractor.ExtraerMemoria(mensaje);

                if (nuevasMemorias.Count > 0)
                {
                    foreach (var memoria in nuevasMemorias)
                    {
                        if (memoria == null || string.IsNullOrWhiteSpace(memoria.Valor))
                            continue;

                        var valor = memoria.Valor.ToLower();

                        // 🚫 FILTRO 1: basura tipo "no especificado"
                        if (valor.Contains("no especificado"))
                            continue;

                        // 🚫 FILTRO 2: cosas del sistema (Ultron)
                        if (valor.Contains("ultron"))
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
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine($"💾 (Memoria guardada: {memoria.Valor})");
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
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