using Microsoft.Data.SqlClient;
using LibraryManagement.Models;

namespace LibraryManagement.DAL
{
    public class ImprumutRepository
    {
        private readonly DatabaseConnection _db;

        public ImprumutRepository()
        {
            _db = new DatabaseConnection();
        }

        public List<Imprumut> GetAll()
        {
            var list = new List<Imprumut>();
            const string sql = @"
                SELECT i.IdImprumut, i.IdCititor, i.IdCarte,
                       i.DataImprumut, i.ZileIntarziere,
                       c.Nume + ' ' + c.Prenume AS NumeCititor,
                       ca.Titlu AS TitluCarte,
                       ca.PretAmenda
                FROM Imprumut i
                INNER JOIN Cititor c  ON i.IdCititor = c.IdCititor
                INNER JOIN Carte   ca ON i.IdCarte   = ca.IdCarte
                ORDER BY i.DataImprumut DESC";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                list.Add(MapFromReader(reader));

            return list;
        }

        public List<Imprumut> GetByCititor(int idCititor)
        {
            var list = new List<Imprumut>();
            const string sql = @"
                SELECT i.IdImprumut, i.IdCititor, i.IdCarte,
                       i.DataImprumut, i.ZileIntarziere,
                       c.Nume + ' ' + c.Prenume AS NumeCititor,
                       ca.Titlu AS TitluCarte,
                       ca.PretAmenda
                FROM Imprumut i
                INNER JOIN Cititor c  ON i.IdCititor = c.IdCititor
                INNER JOIN Carte   ca ON i.IdCarte   = ca.IdCarte
                WHERE i.IdCititor = @IdCititor
                ORDER BY i.DataImprumut DESC";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdCititor", idCititor);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
                list.Add(MapFromReader(reader));

            return list;
        }

        public bool ExistsDuplicate(int idCititor, int idCarte, DateTime dataImprumut, int excludeId = 0)
        {
            const string sql = @"
                SELECT COUNT(*) FROM Imprumut
                WHERE IdCititor = @IdCititor
                  AND IdCarte = @IdCarte
                  AND MONTH(DataImprumut) = MONTH(@Data)
                  AND YEAR(DataImprumut)  = YEAR(@Data)
                  AND IdImprumut <> @ExcludeId";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IdCititor", idCititor);
            cmd.Parameters.AddWithValue("@IdCarte", idCarte);
            cmd.Parameters.AddWithValue("@Data", dataImprumut);
            cmd.Parameters.AddWithValue("@ExcludeId", excludeId);

            return (int)cmd.ExecuteScalar()! > 0;
        }

        public void Insert(Imprumut i)
        {
            const string sql = @"INSERT INTO Imprumut (IdCititor, IdCarte, DataImprumut, ZileIntarziere)
                                  VALUES (@IdCititor, @IdCarte, @DataImprumut, @ZileIntarziere)";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            AddParameters(cmd, i);
            cmd.ExecuteNonQuery();
        }

        public void Update(Imprumut i)
        {
            const string sql = @"UPDATE Imprumut SET
                                    IdCititor      = @IdCititor,
                                    IdCarte        = @IdCarte,
                                    DataImprumut   = @DataImprumut,
                                    ZileIntarziere = @ZileIntarziere
                                  WHERE IdImprumut = @Id";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            AddParameters(cmd, i);
            cmd.Parameters.AddWithValue("@Id", i.IdImprumut);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            const string sql = "DELETE FROM Imprumut WHERE IdImprumut = @Id";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        private static void AddParameters(SqlCommand cmd, Imprumut i)
        {
            cmd.Parameters.AddWithValue("@IdCititor", i.IdCititor);
            cmd.Parameters.AddWithValue("@IdCarte", i.IdCarte);
            cmd.Parameters.AddWithValue("@DataImprumut", i.DataImprumut);
            cmd.Parameters.AddWithValue("@ZileIntarziere", i.ZileIntarziere);
        }

        private static Imprumut MapFromReader(SqlDataReader r) => new()
        {
            IdImprumut = r.GetInt32(0),
            IdCititor = r.GetInt32(1),
            IdCarte = r.GetInt32(2),
            DataImprumut = r.GetDateTime(3),
            ZileIntarziere = r.GetInt32(4),
            NumeCititor = r.GetString(5),
            TitluCarte = r.GetString(6),
            PretAmenda = r.GetDecimal(7)
        };
    }
}