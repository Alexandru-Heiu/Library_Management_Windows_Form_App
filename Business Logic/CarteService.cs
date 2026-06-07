using LibraryManagement.DataAccess;
using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.BusinessLogic
{
    public class CarteService
    {
        private readonly CarteRepository _repo = new();

        public List<Carte> GetAll() => _repo.GetAll();
        public List<Carte> Search(string term) => _repo.Search(term);
        public List<Carte> FilterByGen(string gen) => _repo.FilterByGen(gen);
        public List<string> GetGenuri() => _repo.GetGenuri();
        public Carte? GetById(int id) => _repo.GetById(id);

        public void Add(Carte c)
        {
            Validate(c);
            _repo.Insert(c);
        }

        public void Update(Carte c)
        {
            Validate(c);
            _repo.Update(c);
        }

        public void Delete(int id) => _repo.Delete(id);

        private static void Validate(Carte c)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(c.Titlu))
                errors.Add("Titlul este obligatoriu.");
            if (string.IsNullOrWhiteSpace(c.Autor))
                errors.Add("Autorul este obligatoriu.");
            if (string.IsNullOrWhiteSpace(c.Gen))
                errors.Add("Genul este obligatoriu.");
            if (c.AnPublicare < 1000 || c.AnPublicare > DateTime.Now.Year)
                errors.Add($"Anul publicării trebuie să fie între 1000 și {DateTime.Now.Year}.");
            if (c.PretAmenda < 0)
                errors.Add("Prețul amenzii nu poate fi negativ.");

            if (errors.Count > 0)
                throw new ValidationException(string.Join("\n", errors));
        }
    }
}