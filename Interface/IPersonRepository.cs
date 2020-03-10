using System.Collections.Generic;
using System.Linq;
using Kopis_Showcase.Models;

namespace Kopis_Showcase.Interface
{
    public interface IPersonRepository
    {
        IQueryable<Person> GetPeople();
        Person GetPerson(int? Id);
        void CreatePerson(Person person);
        void UpdatePerson(int Id);
        void DeletePerson(int Id);
        IEnumerable<Gender> GetGenders();
        IEnumerable<MaritalStatus> GetMaritalStatus();
    }
}
