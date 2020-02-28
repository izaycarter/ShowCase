using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kopis_Showcase.Data;
using Kopis_Showcase.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Kopis_Showcase.Controllers
{
    public class PersonController : Controller
    {
        private readonly PersonContext _context;

        private readonly IWebHostEnvironment _env;

        public PersonController(PersonContext context, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
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

            var personContext = _context.Persons.Include(p => p.Gender).Include(p => p.MaritalStatus);

            var people = from p in personContext
                         select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                people = people.Where(p => p.LastName.Contains(searchString)
                                       || p.FirstName.Contains(searchString) );
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
                
                return RedirectToAction(nameof(Index));
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


        public async Task<IActionResult> OnPostExport()
        {
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"UpdatedPersonlist.xlsx";
            _ = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            _ = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            var personContext = _context.Persons.Include(p => p.Gender).Include(p => p.MaritalStatus);
            var people = from p in personContext
                         select p;
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
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
                foreach (var p in people)
                {
                    row = excelSheet.CreateRow(index);
                    row.CreateCell(0).SetCellValue(p.FirstName);
                    row.CreateCell(1).SetCellValue(p.LastName);
                    row.CreateCell(2).SetCellValue(p.Gender.GenderName);
                    row.CreateCell(3).SetCellValue(p.DateOfBirth.ToString("MM/dd/yyyy"));
                    row.CreateCell(4).SetCellValue(p.MaritalStatus.MaritalStatusName);
                    row.CreateCell(5).SetCellValue(p.EmailAddress);
                    row.CreateCell(6).SetCellValue(p.PhoneNumber);
                    row.CreateCell(7).SetCellValue(p.StreetAddressLine1);
                    row.CreateCell(8).SetCellValue(p.StreetAddressLine2);
                    row.CreateCell(9).SetCellValue(p.City);
                    row.CreateCell(10).SetCellValue(p.State);
                    row.CreateCell(11).SetCellValue(p.Zip);
                    index++;
                }
                excelSheet.AutoSizeColumn(0);
                excelSheet.AutoSizeColumn(1);
                excelSheet.AutoSizeColumn(2);
                excelSheet.AutoSizeColumn(3);
                excelSheet.AutoSizeColumn(7);
                excelSheet.AutoSizeColumn(8);
                workbook.Write(fs);
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
