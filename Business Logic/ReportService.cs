using Microsoft.Data.SqlClient;
using LibraryManagement.DataAccess;
using LibraryManagement.Models;

namespace LibraryManagement.BusinessLogic
{
    public class ReportService
    {
        private readonly DatabaseConnection _db = new();

        public ReportSummary GenerateReport()
        {
            var summary = new ReportSummary
            {
                Rows = GetPerCititorRows()
            };

            summary.TotalGeneral = summary.Rows.Sum(r => r.TotalAmenda);
            summary.MediaPlati = summary.Rows.Count > 0
                                    ? summary.Rows.Average(r => r.TotalAmenda)
                                    : 0;

            (summary.CarteCeaMaiImprumutata, summary.NrImprumuturiCarte) = GetCarteCeaMaiImprumutata();

            return summary;
        }

        private List<ReportRow> GetPerCititorRows()
        {
            var list = new List<ReportRow>();
            const string sql = @"
                SELECT
                    c.Nume + ' ' + c.Prenume AS NumeCititor,
                    COUNT(i.IdImprumut)       AS NrImprumuturi,
                    ISNULL(SUM(i.ZileIntarziere), 0) AS TotalZile,
                    ISNULL(SUM(i.ZileIntarziere * ca.PretAmenda), 0) AS TotalAmenda
                FROM Cititor c
                LEFT JOIN Imprumut i  ON c.IdCititor = i.IdCititor
                LEFT JOIN Carte    ca ON i.IdCarte   = ca.IdCarte
                GROUP BY c.IdCititor, c.Nume, c.Prenume
                ORDER BY TotalAmenda DESC";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new ReportRow
                {
                    NumeCititor = reader.GetString(0),
                    NrImprumuturi = reader.GetInt32(1),
                    TotalZileIntarziere = reader.GetInt32(2),
                    TotalAmenda = reader.GetDecimal(3)
                });
            }

            return list;
        }

        private (string titlu, int count) GetCarteCeaMaiImprumutata()
        {
            const string sql = @"
                SELECT TOP 1
                    ca.Titlu,
                    COUNT(i.IdImprumut) AS NrImprumuturi
                FROM Carte ca
                LEFT JOIN Imprumut i ON ca.IdCarte = i.IdCarte
                GROUP BY ca.IdCarte, ca.Titlu
                ORDER BY NrImprumuturi DESC";

            using var conn = _db.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
                return (reader.GetString(0), reader.GetInt32(1));

            return ("N/A", 0);
        }
    }
}