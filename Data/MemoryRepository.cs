using Microsoft.Data.SqlClient;
using UltronAPI.Models;

public class MemoryRepository
{
    private string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Ultron;Trusted_Connection=True;";

    // 🔹 Guardar memoria
    public void GuardarMemoria(Memoria memoria)
    {
        using var conn = new SqlConnection(connectionString);
        conn.Open();

        var query = @"INSERT INTO Memoria
        (UserId, Tipo, Contenido, Categoria, Relevancia, FechaCreacion, UltimoUso)
        VALUES (@UserId, @Tipo, @Contenido, @Categoria, @Relevancia, GETDATE(), GETDATE())";

        var cmd = new SqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@UserId", memoria.UserId);
        cmd.Parameters.AddWithValue("@Tipo", memoria.Tipo);
        cmd.Parameters.AddWithValue("@Contenido", memoria.Contenido);
        cmd.Parameters.AddWithValue("@Categoria", memoria.Categoria);
        cmd.Parameters.AddWithValue("@Relevancia", memoria.Relevancia);

        cmd.ExecuteNonQuery();
    }

    // 🔹 Evitar duplicados
    public bool ExisteMemoria(string contenido)
    {
        using var conn = new SqlConnection(connectionString);
        conn.Open();

        var query = "SELECT COUNT(*) FROM Memoria WHERE Contenido = @Contenido";

        var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Contenido", contenido);

        int count = (int)cmd.ExecuteScalar();

        return count > 0;
    }

    // 🔹 Memoria general (fallback)
    public List<Memoria> ObtenerMemoriaRelevante(int userId)
    {
        var lista = new List<Memoria>();

        using var conn = new SqlConnection(connectionString);
        conn.Open();

        var query = @"SELECT TOP 5 * FROM Memoria
                      WHERE UserId = @UserId
                      ORDER BY Relevancia DESC, UltimoUso DESC";

        var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@UserId", userId);

        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            lista.Add(new Memoria
            {
                Id = (int)reader["Id"],
                UserId = (int)reader["UserId"],
                Tipo = reader["Tipo"].ToString(),
                Contenido = reader["Contenido"].ToString(),
                Categoria = reader["Categoria"].ToString(),
                Relevancia = (int)reader["Relevancia"],
                FechaCreacion = (DateTime)reader["FechaCreacion"],
                UltimoUso = (DateTime)reader["UltimoUso"]
            });
        }

        return lista;
    }

    // 🔥 NUEVO: memoria por contexto
    public List<Memoria> ObtenerMemoriaPorContexto(int userId, string mensaje)
    {
        var lista = new List<Memoria>();

        using var conn = new SqlConnection(connectionString);
        conn.Open();

        var query = @"
        SELECT TOP 5 *
        FROM Memoria
        WHERE UserId = @UserId
        AND Contenido LIKE @Filtro
        ORDER BY Relevancia DESC, UltimoUso DESC";

        var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@Filtro", "%" + mensaje + "%");

        var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            lista.Add(new Memoria
            {
                Id = (int)reader["Id"],
                UserId = (int)reader["UserId"],
                Tipo = reader["Tipo"].ToString(),
                Contenido = reader["Contenido"].ToString(),
                Categoria = reader["Categoria"].ToString(),
                Relevancia = (int)reader["Relevancia"],
                FechaCreacion = (DateTime)reader["FechaCreacion"],
                UltimoUso = (DateTime)reader["UltimoUso"]
            });
        }

        return lista;
    }

    // 🔹 Actualizar uso
    public void ActualizarUso(int id)
    {
        using var conn = new SqlConnection(connectionString);
        conn.Open();

        var query = @"UPDATE Memoria
                      SET UltimoUso = GETDATE(),
                          Relevancia = Relevancia + 1
                      WHERE Id = @Id";

        var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Id", id);

        cmd.ExecuteNonQuery();
    }
}