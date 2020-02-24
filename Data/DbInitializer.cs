using System;
using System.Linq;
using Kopis_Showcase.Models;

//namespace Kopis_Showcase.Data
//{
//    public static class DbInitializer
//    {
//        public static void Initialize(PersonContext context)
//        {
//            context.Database.EnsureCreated();

//            // Look for any students.
//            if (context.Persons.Any())
//            {
//                return;   // DB has been seeded
//            }

//            //var persons = new Person[]
//            //{
//            //    new Person{FirstName="Isaiah",LastName="Carter",GenderID=2,DateOfBirth=DateTime.Parse("05-09-1996"),MaritalStatusID=2,EmailAddress="izay.carter0509@gmail.com",StreetAddressLine1="19 cunningham circle",StreetAddressLine2="300 other street blvd", PhoneNumber="8649156152",City="Taylors",State="SC",Zip="29687"},
//            //    new Person{FirstName="Test",LastName="last",GenderID=1,DateOfBirth=DateTime.Parse("08-20-2000"),MaritalStatusID=1,EmailAddress="testperson@example.com",StreetAddressLine1="100 crazy street",StreetAddressLine2="543 other street blvd", PhoneNumber="7709065889",City="Atlanta",State="GA",Zip="30024"}
//            //};
//            foreach (Person p in persons)
//            {
//                context.Persons.Add(p);
//            }
//            context.SaveChanges();
//        }
//    }
//}
