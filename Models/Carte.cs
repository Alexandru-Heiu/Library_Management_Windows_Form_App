namespace LibraryManagement.Models
{
    public class Carte
    {
        public int IdCarte { get; set; }
        public string Titlu { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string Gen { get; set; } = string.Empty;
        public int AnPublicare { get; set; }
        public decimal PretAmenda { get; set; }

        public override string ToString() => $"{Titlu} — {Autor}";
    }
}