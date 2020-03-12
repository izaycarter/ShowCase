using Microsoft.AspNetCore.Mvc;
using Kopis_Showcase.Models;
using System;
using Kopis_Showcase.Interface;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Kopis_Showcase.Repositories;
using Kopis_Showcase.Data;

namespace Kopis_Showcase.Controllers
{
    public class PersonController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly IPersonRepository _personRepository;


        public PersonController(IWebHostEnvironment env, IPersonRepository personRepository)
        {
            _env = env;

            _personRepository = personRepository;
        }


        // GET: Person
        // Fix filter to keep pageSize when sortOrder and searchString Changes
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

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;



            var people = from person in _personRepository.GetPeople() select person;
 

            if (!String.IsNullOrEmpty(searchString))
            {
                people = people.Where(person => person.LastName.Contains(searchString)
                                       || person.FirstName.Contains(searchString) || person.PhoneNumber.Contains(searchString));
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

            ViewData["10"] = pageSizeFilter == "25" ? "10" : pageSizeFilter == "50" ? "10" : "10";
            ViewData["25"] = String.IsNullOrEmpty(pageSizeFilter) ? "25" : pageSizeFilter == "50" ? "25" : pageSizeFilter == "10" ? "25" : "25";
            ViewData["50"] = String.IsNullOrEmpty(pageSizeFilter) ? "50" : pageSizeFilter == "25" ? "50" : pageSizeFilter == "10" ? "50" : "50";

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
        //Figured out how to return to index with filters on pageNumber you left
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var thisPerson = await _personRepository.DetailViewPerson(id);


            if (thisPerson == null)
            {
                return NotFound();
            }

            return View(thisPerson);
        }

        //GET: Person/Create
        public IActionResult Create()
        {
            var genders = _personRepository.GetGenders();
            var maritalStatus = _personRepository.GetMaritalStatuses();

            ViewData["GenderID"] = new SelectList(genders, "GenderID", "GenderName");
            ViewData["MaritalStatusID"] = new SelectList(maritalStatus, "MaritalStatusID", "MaritalStatusName");
            return View();
        }

        //// POST: Person/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Create(Person person)
        {
            
            try
            {
                 _personRepository.CreatePerson(person);
                

            }
            catch 
            {
                var genders = _personRepository.GetGenders();
                var maritalStatus = _personRepository.GetMaritalStatuses();

                ViewData["GenderID"] = new SelectList(genders, "GenderID", "GenderName", person.GenderID);
                ViewData["MaritalStatusID"] = new SelectList(maritalStatus, "MaritalStatusID", "MaritalStatusName", person.MaritalStatusID);
                return View(person);

            }

            return RedirectToAction("Index", "Person");


        }

        //// GET: Person/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = _personRepository.GetPerson(id);
            if (person == null)
            {
                return NotFound();
            }

            var genders = _personRepository.GetGenders();
            var maritalStatus = _personRepository.GetMaritalStatuses();

            ViewData["GenderID"] = new SelectList(genders, "GenderID", "GenderName", person.GenderID);
            ViewData["MaritalStatusID"] = new SelectList(maritalStatus, "MaritalStatusID", "MaritalStatusName", person.MaritalStatusID);
            return View(person);
        }

        // POST: Person/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Person person)
        {
            
            try
            {
                 _personRepository.UpdatePerson(person);
                
            }
            catch
            {
                
                var genders = _personRepository.GetGenders();
                var maritalStatus = _personRepository.GetMaritalStatuses();

                ViewData["GenderID"] = new SelectList(genders, "GenderID", "GenderName", person.GenderID);
                ViewData["MaritalStatusID"] = new SelectList(maritalStatus, "MaritalStatusID", "MaritalStatusName", person.MaritalStatusID);
                return View(person);

            }
            Person updatedPerson = await _personRepository.DetailViewPerson(person.PersonID);
            return View("Details", updatedPerson);


        }

        // GET: Person/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = _personRepository.GetPerson(id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        //// POST: Person/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _personRepository.DeletePerson(id);
            return RedirectToAction(nameof(Index));
        }

        //change to use interface and repository
        public async Task<IActionResult> OnPostExport()
        {
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"UpdatedPersonlist.xlsx";
            string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            
            using (var file = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.ReadWrite))
            {



                IWorkbook workbook = _personRepository.CreateEachPersonRow();





                workbook.Write(file);
            }

            
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }

    }
}
