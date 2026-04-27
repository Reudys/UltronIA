using UltronAPI.Models;

namespace UltronAPI.Services
{
    public class MemoryManager
    {
        private MemoryRepository _repo = new MemoryRepository();

        // 🔹 Construir texto
        public string ConstruirMemoriaTexto(List<Memoria> memorias)
        {
            return string.Join("\n", memorias.Select(m => "- " + m.Contenido));
        }

        // 🔥 NUEVO: memoria inteligente combinada
        public List<Memoria> ObtenerMemoriaInteligente(int userId, string mensaje)
        {
            var memoriaContextual = _repo.ObtenerMemoriaPorContexto(userId, mensaje);
            var memoriaGeneral = _repo.ObtenerMemoriaRelevante(userId);

            // Combinar y evitar duplicados
            var todas = memoriaContextual
                .Concat(memoriaGeneral)
                .GroupBy(m => m.Id)
                .Select(g => g.First())
                .Take(5)
                .ToList();

            return todas;
        }

        // 🔹 Marcar uso
        public void MarcarUso(List<Memoria> memorias)
        {
            foreach (var m in memorias)
            {
                _repo.ActualizarUso(m.Id);
            }
        }
    }
}
