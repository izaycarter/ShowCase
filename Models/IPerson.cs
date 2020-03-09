using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.UserModel;

namespace Kopis_Showcase.Models
{
    public interface IPerson
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
