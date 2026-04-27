namespace UltronAPI.Services
{
    public static class MemoryValidator
    {
        public static bool EsValida(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            texto = texto.ToLower();

            // ❌ basura típica de IA
            if (texto.Contains("[insert") ||
                texto.Contains("[name") ||
                texto.Contains("[topic") ||
                texto.Contains("placeholder") ||
                texto.Contains("unknown") ||
                texto.Contains("?"))
                return false;

            // ❌ muy corto o inútil
            if (texto.Length < 5)
                return false;
            
            return true;
        }
    }
}
