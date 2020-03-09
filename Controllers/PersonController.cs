using Microsoft.AspNetCore.Mvc;
using Kopis_Showcase.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kopis_Showcase.Controllers
{
    public class PersonController : Controller
    {
        private readonly IPerson _person;

        public PersonController(IPerson person)
        {
            _person = person;
        }


        // GET: Person
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber,
            string pageSizeFilter)
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["FirstNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "firstname_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["LastNameSortParm"] = sortOrder == "LastName" ? "LastName_desc" : "LastName";

            ViewData["10"] = pageSizeFilter == "25" ? "10" : pageSizeFilter == "50" ? "10" : "10";
            ViewData["25"] = String.IsNullOrEmpty(pageSizeFilter) ? "25" : pageSizeFilter == "50" ? "25" : pageSizeFilter == "10" ? "25" : "25";
            ViewData["50"] = String.IsNullOrEmpty(pageSizeFilter) ? "50" : pageSizeFilter == "25" ? "50" : pageSizeFilter == "10" ? "50" : "50";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;



            var people = from p in _person.GetPeople()
                         select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                people = people.Where(p => p.LastName.Contains(searchString)
                                       || p.FirstName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "firstname_desc":
                    people = people.OrderByDescending(person => person.FirstName);
                    break;
                case "Date":
                    people = people.OrderBy(person => person.DateOfBirth);
                    break;
                case "date_desc":
                    people = people.OrderByDescending(person => person.DateOfBirth);
                    break;
                case "LastName_desc":
                    people = people.OrderByDescending(person => person.LastName);
                    break;
                case "LastName":
                    people = people.OrderBy(person => person.LastName);
                    break;
                default:
                    people = people.OrderBy(person => person.FirstName);
                    break;
            }


            int pageSize;


            switch (pageSizeFilter)
            {

                case "10":
                    pageSize = 10;
                    break;
                case "25":
                    pageSize = 25;
                    break;
                case "50":
                    pageSize = 50;
                    break;
                default:
                    pageSize = 10;
                    break;

            }
            //This pulls all to memoery. refactor for efficiently   
            return View(await PaginatedList<Person>.CreateAsync(people, pageNumber ?? 1, pageSize));
        }
        // GET: Person/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thisPerson = _person.GetPerson(id);


            if (thisPerson == null)
            {
                return NotFound();
            }

            return View(thisPerson);
        }

        //GET: Person/Create
        public IActionResult Create()
        {
            var genders = _person.GetGenders();
            var maritalStatus = _person.GetMaritalStatus();

            ViewData["GenderID"] = new SelectList(genders, "GenderID", "GenderName");
            ViewData["MaritalStatusID"] = new SelectList(maritalStatus, "MaritalStatusID", "MaritalStatusName");
            return View();
        }

        //// POST: Person/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("PersonID,FirstName,LastName,GenderID,DateOfBirth,MaritalStatusID,EmailAddress,StreetAddressLine1,StreetAddressLine2,PhoneNumber,City,State,Zip")] Person person)
        {
            if (ModelState.IsValid)
            {
                _person.CreatePerson(person);
                return RedirectToAction("Index","Details",person.PersonID);
            }

            var genders = _person.GetGenders();
            var maritalStatus = _person.GetMaritalStatus();

            ViewData["GenderID"] = new SelectList(genders, "GenderID", "GenderName", person.GenderID);
            ViewData["MaritalStatusID"] = new SelectList(maritalStatus, "MaritalStatusID", "MaritalStatusName", person.MaritalStatusID);
            return View(person);
        }

        //// GET: Person/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var person = await personRepository.Persons.FindAsync(id);
        //    if (person == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["GenderID"] = new SelectList(personRepository.Gender, "GenderID", "GenderID", person.GenderID);
        //    ViewData["MaritalStatusID"] = new SelectList(personRepository.MaritalStatus, "MaritalStatusID", "MaritalStatusID", person.MaritalStatusID);
        //    return View(person);
        //}

        //// POST: Person/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("PersonID,FirstName,LastName,GenderID,DateOfBirth,MaritalStatusID,EmailAddress,StreetAddressLine1,StreetAddressLine2,PhoneNumber,City,State,Zip")] Person person)
        //{
        //    if (id != person.PersonID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            personRepository.Update(person);
        //            await personRepository.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!PersonExists(person.PersonID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["GenderID"] = new SelectList(personRepository.Gender, "GenderID", "GenderID", person.GenderID);
        //    ViewData["MaritalStatusID"] = new SelectList(personRepository.MaritalStatus, "MaritalStatusID", "MaritalStatusID", person.MaritalStatusID);
        //    return View(person);
        //}

        //// GET: Person/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var person = await personRepository.Persons
        //        .Include(p => p.Gender)
        //        .Include(p => p.MaritalStatus)
        //        .FirstOrDefaultAsync(m => m.PersonID == id);
        //    if (person == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(person);
        //}

        //// POST: Person/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var person = await personRepository.Persons.FindAsync(id);
        //    personRepository.Persons.Remove(person);
        //    await personRepository.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool PersonExists(int id)
        //{
        //    return personRepository.Persons.Any(e => e.PersonID == id);
        //}

    }
}
