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

namespace Kopis_Showcase.Controllers
{
    public class PersonController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly IPersonRepository _person;

        public PersonController(IPersonRepository person, IWebHostEnvironment env)
        {
            _env = env;
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
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = _person.GetPerson(id);
            if (person == null)
            {
                return NotFound();
            }

            var genders = _person.GetGenders();
            var maritalStatus = _person.GetMaritalStatus();

            ViewData["GenderID"] = new SelectList(genders, "GenderID", "GenderName", person.GenderID);
            ViewData["MaritalStatusID"] = new SelectList(maritalStatus, "MaritalStatusID", "MaritalStatusName", person.MaritalStatusID);
            return View(person);
        }

        // POST: Person/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("PersonID,FirstName,LastName,GenderID,DateOfBirth,MaritalStatusID,EmailAddress,StreetAddressLine1,StreetAddressLine2,PhoneNumber,City,State,Zip")] Person person)
        {
            if (id != person.PersonID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _person.UpdatePerson(id);
                return RedirectToAction("Index", "Details", id);
            }

            var genders = _person.GetGenders();
            var maritalStatus = _person.GetMaritalStatus();

            ViewData["GenderID"] = new SelectList(genders, "GenderID", "GenderName", person.GenderID);
            ViewData["MaritalStatusID"] = new SelectList(maritalStatus, "MaritalStatusID", "MaritalStatusName", person.MaritalStatusID);
            return View(person);
        }

        // GET: Person/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = _person.GetPerson(id);
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
            _person.DeletePerson(id);
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
            var peopleList = from p in _person.GetPeople()
            select p;

            using (var file = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {


                IWorkbook workbook;
                workbook = new XSSFWorkbook();
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

                int index = 1;
                foreach (var person in peopleList)
                {
                    row = excelSheet.CreateRow(index);
                    row.CreateCell(0).SetCellValue(person.FirstName);
                    row.CreateCell(1).SetCellValue(person.LastName);
                    row.CreateCell(2).SetCellValue(person.Gender.GenderName);
                    row.CreateCell(3).SetCellValue(person.DateOfBirth.ToString("MM/dd/yyyy"));
                    row.CreateCell(4).SetCellValue(person.MaritalStatus.MaritalStatusName);
                    row.CreateCell(5).SetCellValue(person.EmailAddress);
                    row.CreateCell(6).SetCellValue(person.PhoneNumber);
                    row.CreateCell(7).SetCellValue(person.StreetAddressLine1);
                    if (person.StreetAddressLine2 == null)
                    {
                        row.CreateCell(8).SetCellValue("N/A");
                    }
                    else
                    {
                        row.CreateCell(8).SetCellValue(person.StreetAddressLine2);
                    }
                    row.CreateCell(9).SetCellValue(person.City);
                    row.CreateCell(10).SetCellValue(person.State);
                    row.CreateCell(11).SetCellValue(person.Zip);
                    index++;
                }
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
