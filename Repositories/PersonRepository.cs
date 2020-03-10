using System.Collections.Generic;
using System.Linq;
using Kopis_Showcase.Data;
using Microsoft.EntityFrameworkCore;
using Kopis_Showcase.Interface;
using Kopis_Showcase.Models;

namespace Kopis_Showcase.Repositories
{
    public class PersonRepository : IPersonRepository
    {

        private readonly SqlDbContext context;

        public PersonRepository(SqlDbContext context)
        {
            this.context = context;
        }

        //List of genders of Create form
        public IEnumerable<Gender> GetGenders()
        {
            var genders = context.Gender.ToList();
            return genders;
        }

        //List of maritalstatuses of Create form
        public IEnumerable<MaritalStatus> GetMaritalStatus()
        {
            var maritalStatus = context.MaritalStatus.ToList();

            return maritalStatus;
        }

        public void CreatePerson(Person person)
        {
            context.Persons.Add(person);
            context.SaveChangesAsync();
        }


        public void DeletePerson(int Id)
        {
            Person person = context.Persons.Find(Id);
            context.Persons.Remove(person);
            context.SaveChanges();
        }

        public IQueryable<Person> GetPeople()
        {
            var people = context.Persons;
            return people;
           
        }

        public Person GetPerson(int? Id)
        {
            var person = context.Persons.Where(person => person.PersonID == Id).Include(person => person.Gender)
                .Include(person => person.MaritalStatus)
                .FirstOrDefault();

            return person;
        }

        public void UpdatePerson(int Id)
        {
            Person person = context.Persons.Find(Id);
            context.Persons.Update(person);
            context.SaveChangesAsync();
        }
    }
}
