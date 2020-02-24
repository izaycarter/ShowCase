using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kopis_Showcase.Data;
using Kopis_Showcase.Models;
using Microsoft.AspNetCore.Http;

namespace Kopis_Showcase.Controllers
{
    public class PersonController : Controller
    {
        private readonly PersonContext _context;

        public PersonController(PersonContext context)
        {
            _context = context;
        }

        // GET: Person
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {

            ViewData["CurrentSort"] = sortOrder;

            ViewData["FirstNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "firstname_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["LastNameSortParm"] = sortOrder == "LastName" ? "LastName_desc" : "LastName";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var personContext = _context.Persons.Include(p => p.Gender).Include(p => p.MaritalStatus);

            var people = from p in personContext
                         select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                people = people.Where(p => p.LastName.Contains(searchString)
                                       || p.FirstName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "firstname_desc":
                    people = people.OrderByDescending(p => p.FirstName);
                    break;
                case "Date":
                    people = people.OrderBy(p => p.DateOfBirth);
                    break;
                case "date_desc":
                    people = people.OrderByDescending(p => p.DateOfBirth);
                    break;
                case "LastName_desc":
                    people = people.OrderByDescending(p => p.LastName);
                    break;
                case "LastName":
                    people = people.OrderBy(p => p.LastName);
                    break;
                default:
                    people = people.OrderBy(p => p.FirstName);
                    break;
            }



            int pageSize = 1;
            return View(await PaginatedList<Person>.CreateAsync(people, pageNumber ?? 1, pageSize));
       
        }

        // GET: Person/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .Include(p => p.Gender)
                .Include(p => p.MaritalStatus)
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: Person/Create
        public IActionResult Create()
        {
            ViewData["GenderID"] = new SelectList(_context.Gender, "GenderID", "GenderName");
            ViewData["MaritalStatusID"] = new SelectList(_context.MaritalStatus, "MaritalStatusID", "MaritalStatusName");
            return View();
        }

        // POST: Person/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,GenderID,DateOfBirth,MaritalStatusID,EmailAddress,StreetAddressLine1,StreetAddressLine2,PhoneNumber,City,State,Zip")] Person person)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(person);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            ViewData["GenderID"] = new SelectList(_context.Gender, "GenderID", "GenderName", person.GenderID);
            ViewData["MaritalStatusID"] = new SelectList(_context.MaritalStatus, "MaritalStatusID", "MaritalStatusName", person.MaritalStatusID);
            return View(person);
        }

        // GET: Person/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            ViewData["GenderID"] = new SelectList(_context.Gender, "GenderID", "GenderName", person.GenderID);
            ViewData["MaritalStatusID"] = new SelectList(_context.MaritalStatus, "MaritalStatusID", "MaritalStatusName", person.MaritalStatusID);
            return View(person);
        }

        // POST: Person/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonID,FirstName,LastName,GenderID,DateOfBirth,MaritalStatusID,EmailAddress,StreetAddressLine1,StreetAddressLine2,PhoneNumber,City,State,Zip")] Person person)
        {
            if (id != person.PersonID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.PersonID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenderID"] = new SelectList(_context.Gender, "GenderID", "GenderName", person.GenderID);
            ViewData["MaritalStatusID"] = new SelectList(_context.MaritalStatus, "MaritalStatusID", "MaritalStatusName", person.MaritalStatusID);
            return View(person);
        }

        // GET: Person/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .Include(p => p.Gender)
                .Include(p => p.MaritalStatus)
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (person == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(person);
        }

        // POST: Person/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Persons.Remove(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }

        }

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.PersonID == id);
        }


    }
}
