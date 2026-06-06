using LibraryManagement.DAL;
using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.BLL
{
    public class ImprumutService
    {
        private readonly ImprumutRepository _repo = new();

        public List<Imprumut> GetAll() => _repo.GetAll();
        public List<Imprumut> GetByCititor(int idCititor) => _repo.GetByCititor(idCititor);

        public void Add(Imprumut i)
        {
            Validate(i);
            if (_repo.ExistsDuplicate(i.IdCititor, i.IdCarte, i.DataImprumut))
                throw new ValidationException("Există deja un împrumut pentru această carte și cititor în luna selectată.");
            _repo.Insert(i);
        }

        public void Update(Imprumut i)
        {
            Validate(i);
            if (_repo.ExistsDuplicate(i.IdCititor, i.IdCarte, i.DataImprumut, i.IdImprumut))
                throw new ValidationException("Există deja un împrumut pentru această carte și cititor în luna selectată.");
            _repo.Update(i);
        }

        public void Delete(int id) => _repo.Delete(id);

        private static void Validate(Imprumut i)
        {
            var errors = new List<string>();

            if (i.IdCititor <= 0)
                errors.Add("Cititorul este obligatoriu.");
            if (i.IdCarte <= 0)
                errors.Add("Cartea este obligatorie.");
            if (i.ZileIntarziere < 0)
                errors.Add("Zilele de întârziere nu pot fi negative.");
            if (i.DataImprumut > DateTime.Today)
                errors.Add("Data împrumutului nu poate fi în viitor.");

            if (errors.Count > 0)
                throw new ValidationException(string.Join("\n", errors));
        }
    }
}