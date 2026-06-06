using Microsoft.Data.SqlClient;
using LibraryManagement.Models;

namespace LibraryManagement.DAL
{
    public class CititorRepository
    {
        private readonly DatabaseConnection _db;

        public CititorRepository()
        {
            _db = new DatabaseConnection();
        }

        public List<Cititor> GetAll()
        {
            var list = new List<Cititor>();
            const string sql = "SELECT * FROM Cititor ORDER BY Nume, Prenume";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                list.Add(MapFromReader(reader));

            return list;
        }

        public Cititor? GetById(int id)
        {
            const string sql = "SELECT * FROM Cititor WHERE IdCititor = @Id";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();

            return reader.Read() ? MapFromReader(reader) : null;
        }

        public List<Cititor> Search(string searchTerm)
        {
            var list = new List<Cititor>();
            const string sql = @"SELECT * FROM Cititor
                                  WHERE Nume LIKE @Term
                                     OR Prenume LIKE @Term
                                     OR IDNP LIKE @Term
                                  ORDER BY Nume, Prenume";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Term", $"%{searchTerm}%");
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                list.Add(MapFromReader(reader));

            return list;
        }

        public void Insert(Cititor c)
        {
            const string sql = @"INSERT INTO Cititor (Nume, Prenume, IDNP, Telefon, Email, DataInregistrare)
                                  VALUES (@Nume, @Prenume, @IDNP, @Telefon, @Email, @DataInregistrare)";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            AddParameters(cmd, c);
            cmd.ExecuteNonQuery();
        }

        public void Update(Cititor c)
        {
            const string sql = @"UPDATE Cititor SET
                                    Nume = @Nume,
                                    Prenume = @Prenume,
                                    IDNP = @IDNP,
                                    Telefon = @Telefon,
                                    Email = @Email,
                                    DataInregistrare = @DataInregistrare
                                  WHERE IdCititor = @Id";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            AddParameters(cmd, c);
            cmd.Parameters.AddWithValue("@Id", c.IdCititor);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            const string sql = "DELETE FROM Cititor WHERE IdCititor = @Id";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        private static void AddParameters(SqlCommand cmd, Cititor c)
        {
            cmd.Parameters.AddWithValue("@Nume", c.Nume);
            cmd.Parameters.AddWithValue("@Prenume", c.Prenume);
            cmd.Parameters.AddWithValue("@IDNP", c.IDNP);
            cmd.Parameters.AddWithValue("@Telefon", c.Telefon);
            cmd.Parameters.AddWithValue("@Email", (object?)c.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DataInregistrare", c.DataInregistrare);
        }

        private static Cititor MapFromReader(SqlDataReader r) => new()
        {
            IdCititor = r.GetInt32(0),
            Nume = r.GetString(1),
            Prenume = r.GetString(2),
            IDNP = r.GetString(3),
            Telefon = r.GetString(4),
            Email = r.IsDBNull(5) ? null : r.GetString(5),
            DataInregistrare = r.GetDateTime(6)
        };
    }
}