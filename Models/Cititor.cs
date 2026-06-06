namespace LibraryManagement.Models
{
    public class Cititor
    {
        public int IdCititor { get; set; }
        public string Nume { get; set; } = string.Empty;
        public string Prenume { get; set; } = string.Empty;
        public string IDNP { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime DataInregistrare { get; set; } = DateTime.Today;

        public string NumeComplet => $"{Nume} {Prenume}";

        public override string ToString() => NumeComplet;
    }
}