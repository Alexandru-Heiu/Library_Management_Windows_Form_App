namespace LibraryManagement.Models
{
    public class Imprumut
    {
        public int IdImprumut { get; set; }
        public int IdCititor { get; set; }
        public int IdCarte { get; set; }
        public DateTime DataImprumut { get; set; } = DateTime.Today;
        public int ZileIntarziere { get; set; }


        public string NumeCititor { get; set; } = string.Empty;
        public string TitluCarte { get; set; } = string.Empty;
        public decimal PretAmenda { get; set; }

        public decimal AmendaTotal => ZileIntarziere * PretAmenda;
    }
}