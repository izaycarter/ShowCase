using System.Collections.Generic;
using System.Linq;
using Kopis_Showcase.Data;
using Microsoft.EntityFrameworkCore;
using Kopis_Showcase.Interface;
using Kopis_Showcase.Models;
using System;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Threading.Tasks;

namespace Kopis_Showcase.Repositories
{
    public class PersonRepository : IPersonRepository
    {

        private SqlDbContext _context;

        public PersonRepository(SqlDbContext context)
        {
            _context = context;
        }





        //List of genders of Create form
        public IEnumerable<Gender> GetGenders()
        {
            var genders = _context.Gender.ToList();
            return genders;
        }



        //List of maritalstatuses of Create form
        public IEnumerable<MaritalStatus> GetMaritalStatuses()
        {
            var maritalStatus = _context.MaritalStatus.ToList();

            return maritalStatus;
        }




        //CRUD For Person Model___________________________________

        public Person CreatePerson(Person person)
        {
            _context.Persons.Add(person); 
            _context.SaveChanges();
            return person;

        }



        public Person UpdatePerson(Person UpdatedPerson)
        {

            var person = _context.Persons.Attach(UpdatedPerson);
            person.State = EntityState.Modified;
            _context.SaveChanges();
            return UpdatedPerson;

        }


        public Person DeletePerson(int Id)
        {
            Person person = _context.Persons.Find(Id);
            if(person != null)
            {
                _context.Persons.Remove(person);
                _context.SaveChanges();
            }
            return person;
        }



        //People list for index view
        public IQueryable<Person> GetPeople()
        {
            var people = _context.Persons;
            return people;

        }


        public Task<Person> DetailedViewPerson(int? Id)
        {
            var person = _context.Persons
                .Include(p => p.Gender)
                .Include(p => p.MaritalStatus)
                .FirstOrDefaultAsync(m => m.PersonID == Id);


            return person;
        }

        // Gets Person for Edit View
        public Person GetPerson(int? Id)
        {
            var person = _context.Persons.Find(Id);

            return person;
        }

        



        //Excel Methods__________________________________________

        
        public IEnumerable<Person> GetPeopleForExcel()
        {
            var people = _context.Persons;
            return people;
        }



        public Person CreatePersonFromRow(IRow row)
        {
            var person = new Person();

            for (int i = row.FirstCellNum; i <= row.LastCellNum; i++)
            {
                ICell cell = row.GetCell(i);
                switch (i)
                {
                    case 0:
                        person.FirstName = cell.ToString();
                        break;
                    case 1:
                        person.LastName = cell.ToString();
                        break;
                    case 2:
                        person.GenderID = GetGenderFromRow(cell.ToString().ToUpper());
                        break;
                    case 3:
                        DateTime date = DateTime.Parse(cell.ToString());
                        person.DateOfBirth = date;
                        break;
                    case 4:
                        person.MaritalStatusID = GetMaritalStatusFromRow(cell.ToString().ToUpper());
                        break;
                    case 5:
                        person.EmailAddress = cell.ToString();
                        break;
                    case 6:
                        person.PhoneNumber = cell.ToString();
                        break;
                    case 7:
                        person.StreetAddressLine1 = cell.ToString();
                        break;
                    case 8:
                        person.StreetAddressLine2 = cell.ToString();
                        break;
                    case 9:
                        person.City = cell.ToString();
                        break;
                    case 10:
                        person.State = cell.ToString();
                        break;
                    case 11:
                        person.Zip = cell.ToString();
                        break;

                }
                _context.Persons.Add(person);
                
            }
            return person;
        }



        public void ReadEachRowFromSheet(ISheet sheet)
        {
            for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                CreatePersonFromRow(row);
            }
            _context.SaveChanges();
        }



        public int GetGenderFromRow(string GenderName)
        {
            switch (GenderName)
            {
                case "FEMALE":
                    return 1;
                case "MALE":
                    return 2;
                default:
                    return 3;

            }
        }


        public int GetMaritalStatusFromRow(string MaritalName)
        {
            switch (MaritalName)
            {
                case "SINGLE":
                    return 1;
                case "MARRIED":
                    return 2;
                case "DIVORCED":
                    return 3;
                case "WIDOWED":
                    return 4;
                default:
                    return 5;

            }
        }

        public string GetGenderNameFromId(int GenderID)
        {
            switch (GenderID)
            {
                case 1:
                    return "Female";
                case 2:
                    return "Male";
                default:
                    return "Not Specified";

            }
        }


        public string GetMaritalStatusNameFromId(int MaritalStatusID)
        {
            switch (MaritalStatusID)
            {
                case 1:
                    return "Single";
                case 2:
                    return "Married";
                case 3:
                    return "Divorced";
                case 4:
                    return "Widowed";
                default:
                    return "Not Specified";

            }
        }

        //Creates Excel From Person entities
        public IWorkbook CreateEachPersonRow()
        {
            var people = GetPeopleForExcel();

            IWorkbook workbook = new XSSFWorkbook();
            ISheet excelSheet = workbook.CreateSheet("Person List");

            IRow row = excelSheet.CreateRow(0);
            row.CreateCell(0).SetCellValue("FirstName");
            row.CreateCell(1).SetCellValue("LastName");
            row.CreateCell(2).SetCellValue("Gender");
            row.CreateCell(3).SetCellValue("DateOfBirth");
            row.CreateCell(4).SetCellValue("MaritalStatus");
            row.CreateCell(5).SetCellValue("EmailAddress");
            row.CreateCell(6).SetCellValue("PhoneNumber");
            row.CreateCell(7).SetCellValue("StreetAddressLine1");
            row.CreateCell(8).SetCellValue("StreetAddressLine2");
            row.CreateCell(9).SetCellValue("City");
            row.CreateCell(10).SetCellValue("State");
            row.CreateCell(11).SetCellValue("Zip");


            var personIndex = 1;
            foreach (var person in people)
            {
                row = excelSheet.CreateRow(personIndex);

                switch (person.StreetAddressLine2)
                {
                    case null:
                        person.StreetAddressLine2 = "N/A";
                        break;
                }

                var MaritalStatusName = GetMaritalStatusNameFromId(person.MaritalStatusID);
                var genderName = GetGenderNameFromId(person.GenderID);

                for (int CellNumber = 0; CellNumber <= 11; CellNumber++)
                {
                    switch (CellNumber)
                    {
                        case 0:
                            row.CreateCell(CellNumber).SetCellValue(person.FirstName);
                            break;
                        case 1:
                            row.CreateCell(CellNumber).SetCellValue(person.LastName);
                            break;
                        case 2:

                            row.CreateCell(CellNumber).SetCellValue(genderName);
                            break;
                        case 3:
                            row.CreateCell(CellNumber).SetCellValue(person.DateOfBirth.ToString("MM/dd/yyyy"));
                            break;
                        case 4:

                            row.CreateCell(CellNumber).SetCellValue(MaritalStatusName);
                            break;
                        case 5:
                            row.CreateCell(CellNumber).SetCellValue(person.EmailAddress);
                            break;
                        case 6:
                            row.CreateCell(CellNumber).SetCellValue(person.PhoneNumber);
                            break;
                        case 7:
                            row.CreateCell(CellNumber).SetCellValue(person.StreetAddressLine1);
                            break;
                        case 8:
                            row.CreateCell(CellNumber).SetCellValue(person.StreetAddressLine2);
                            break;
                        case 9:
                            row.CreateCell(CellNumber).SetCellValue(person.City);
                            break;
                        case 10:
                            row.CreateCell(CellNumber).SetCellValue(person.State);
                            break;
                        case 11:
                            row.CreateCell(CellNumber).SetCellValue(person.Zip);
                            break;
                    }
                }
                personIndex++;
            } 
            return workbook;
        }


    }
}
