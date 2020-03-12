using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kopis_Showcase.Models;
using NPOI.SS.UserModel;

namespace Kopis_Showcase.Interface
{
    public interface IPersonRepository 
    {
        IEnumerable<Gender> GetGenders();
        IEnumerable<MaritalStatus> GetMaritalStatuses();
        int GetGenderFromRow(string GenderName);
        int GetMaritalStatusFromRow(string MaritalName);
        string GetGenderNameFromId(int GenderID);
        string GetMaritalStatusNameFromId(int MaritalStatusID);

        IQueryable<Person> GetPeople();
        Task<Person> DetailedViewPerson(int? Id);
        Person GetPerson(int? Id);
        Person CreatePerson(Person person);
        Person UpdatePerson(Person UpdatedPerson);
        Person DeletePerson(int Id);

        IEnumerable<Person> GetPeopleForExcel();
        Person CreatePersonFromRow(IRow row);
        void ReadEachRowFromSheet(ISheet sheet);
        IWorkbook CreateEachPersonRow();
        


    }
}
