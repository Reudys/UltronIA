namespace UltronAPI.Models
{
    public class Memoria
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Tipo { get; set; }
        public string Contenido { get; set; }
        public string Categoria { get; set; }
        public int Relevancia { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime UltimoUso { get; set; }
    }
}
