namespace LibraryManagement.Models
{
    public class ReportRow
    {
        public string NumeCititor { get; set; } = string.Empty;
        public int NrImprumuturi { get; set; }
        public int TotalZileIntarziere { get; set; }
        public decimal TotalAmenda { get; set; }
    }

    public class ReportSummary
    {
        public List<ReportRow> Rows { get; set; } = new();
        public decimal TotalGeneral { get; set; }
        public decimal MediaPlati { get; set; }
        public string CarteCeaMaiImprumutata { get; set; } = string.Empty;
        public int NrImprumuturiCarte { get; set; }
    }
}