using LibraryManagement.DataAccess;
using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.BusinessLogic
{
    public class CititorService
    {
        private readonly CititorRepository _repo = new();

        public List<Cititor> GetAll() => _repo.GetAll();
        public List<Cititor> Search(string term) => _repo.Search(term);
        public Cititor? GetById(int id) => _repo.GetById(id);

        public void Add(Cititor c)
        {
            Validate(c);
            _repo.Insert(c);
        }

        public void Update(Cititor c)
        {
            Validate(c);
            _repo.Update(c);
        }

        public void Delete(int id) => _repo.Delete(id);

        private static void Validate(Cititor c)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(c.Nume))
                errors.Add("Numele este obligatoriu.");
            if (string.IsNullOrWhiteSpace(c.Prenume))
                errors.Add("Prenumele este obligatoriu.");
            if (string.IsNullOrWhiteSpace(c.IDNP) || c.IDNP.Length != 13 || !c.IDNP.All(char.IsDigit))
                errors.Add("IDNP-ul trebuie să conțină exact 13 cifre.");
            if (string.IsNullOrWhiteSpace(c.Telefon))
                errors.Add("Telefonul este obligatoriu.");
            if (!string.IsNullOrWhiteSpace(c.Email) && !c.Email.Contains('@'))
                errors.Add("Adresa de email nu este validă.");

            if (errors.Count > 0)
                throw new ValidationException(string.Join("\n", errors));
        }
    }
}