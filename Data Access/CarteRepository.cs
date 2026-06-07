using Microsoft.Data.SqlClient;
using LibraryManagement.Models;

namespace LibraryManagement.DataAccess
{
    public class CarteRepository
    {
        private readonly DatabaseConnection _db;

        public CarteRepository()
        {
            _db = new DatabaseConnection();
        }

        public List<Carte> GetAll()
        {
            var list = new List<Carte>();
            const string sql = "SELECT * FROM Carte ORDER BY Titlu";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                list.Add(MapFromReader(reader));

            return list;
        }

        public Carte? GetById(int id)
        {
            const string sql = "SELECT * FROM Carte WHERE IdCarte = @Id";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();

            return reader.Read() ? MapFromReader(reader) : null;
        }

        public List<Carte> Search(string searchTerm)
        {
            var list = new List<Carte>();
            const string sql = @"SELECT * FROM Carte
                                  WHERE Titlu LIKE @Term OR Autor LIKE @Term
                                  ORDER BY Titlu";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Term", $"%{searchTerm}%");
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                list.Add(MapFromReader(reader));

            return list;
        }

        public List<Carte> FilterByGen(string gen)
        {
            var list = new List<Carte>();
            const string sql = "SELECT * FROM Carte WHERE Gen = @Gen ORDER BY Titlu";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Gen", gen);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                list.Add(MapFromReader(reader));

            return list;
        }

        public List<string> GetGenuri()
        {
            var list = new List<string>();
            const string sql = "SELECT DISTINCT Gen FROM Carte ORDER BY Gen";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                list.Add(reader.GetString(0));

            return list;
        }

        public void Insert(Carte c)
        {
            const string sql = @"INSERT INTO Carte (Titlu, Autor, Gen, AnPublicare, PretAmenda)
                                  VALUES (@Titlu, @Autor, @Gen, @AnPublicare, @PretAmenda)";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            AddParameters(cmd, c);
            cmd.ExecuteNonQuery();
        }

        public void Update(Carte c)
        {
            const string sql = @"UPDATE Carte SET
                                    Titlu = @Titlu,
                                    Autor = @Autor,
                                    Gen = @Gen,
                                    AnPublicare = @AnPublicare,
                                    PretAmenda = @PretAmenda
                                  WHERE IdCarte = @Id";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            AddParameters(cmd, c);
            cmd.Parameters.AddWithValue("@Id", c.IdCarte);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            const string sql = "DELETE FROM Carte WHERE IdCarte = @Id";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        private static void AddParameters(SqlCommand cmd, Carte c)
        {
            cmd.Parameters.AddWithValue("@Titlu", c.Titlu);
            cmd.Parameters.AddWithValue("@Autor", c.Autor);
            cmd.Parameters.AddWithValue("@Gen", c.Gen);
            cmd.Parameters.AddWithValue("@AnPublicare", c.AnPublicare);
            cmd.Parameters.AddWithValue("@PretAmenda", c.PretAmenda);
        }

        private static Carte MapFromReader(SqlDataReader r) => new()
        {
            IdCarte = r.GetInt32(0),
            Titlu = r.GetString(1),
            Autor = r.GetString(2),
            Gen = r.GetString(3),
            AnPublicare = r.GetInt32(4),
            PretAmenda = r.GetDecimal(5)
        };
    }
}