using System;
using System.Collections.Generic;
using System.Linq;
using Kopis_Showcase.Data;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;

namespace Kopis_Showcase.Models
{
    public class IPersonRepository : IPerson
    {

        private readonly SqlDbContext context;

        public IPersonRepository(SqlDbContext context)
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
