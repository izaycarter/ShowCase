using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Kopis_Showcase.Models;
using System.IO;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Kopis_Showcase.Data;

namespace Kopis_Showcase.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly PersonContext _context;

        

        public HomeController(IWebHostEnvironment env, PersonContext context)
        {
            _env = env;

            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        private Person CreatePerson(IRow row)
        {
            var person = new Person();

            for (int i = row.FirstCellNum; i <= row.LastCellNum; i++)
            {
                ICell cell = row.GetCell(i);
                if (i == 0) { person.FirstName = cell.ToString(); }

                if (i == 1) { person.LastName = cell.ToString(); }

                if (i == 2)
                {
                    if (cell.ToString() == "Female")
                    {
                        person.GenderID = 1;

                    }
                    else if (cell.ToString() == "Male")
                    {
                        person.GenderID = 2;
                    }
                    else
                    {
                        person.GenderID = 3;
                    }

                }

                if (i == 3)
                {

                    DateTime date = DateTime.Parse(cell.ToString());


                    person.DateOfBirth = date;
                }

                if (i == 4)
                {
                    if (cell.ToString() == "Single")
                    {
                        person.MaritalStatusID = 1;
                    }
                     else if (cell.ToString() == "Married")
                    {
                        person.MaritalStatusID = 2;
                    }
                    else if (cell.ToString() == "Divorced")
                    {
                        person.MaritalStatusID = 3;
                    }
                    else if (cell.ToString() == "Widowed")
                    {
                        person.MaritalStatusID = 4;
                    }
                    else 
                    {
                        person.MaritalStatusID = 5;
                    }
                }

                if (i == 5) { person.EmailAddress = cell.ToString(); }

                if (i == 6) { person.PhoneNumber = cell.ToString(); }

                if (i == 7) { person.StreetAddressLine1 = cell.ToString(); }

                if (i == 8) { person.StreetAddressLine2 = cell.ToString(); }

                if (i == 9) { person.City = cell.ToString(); }

                if (i == 10) { person.State = cell.ToString(); }

                if (i == 11) { person.Zip = cell.ToString(); }

                
            }

            
            return person;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UploadFile file)
        {
            string extension = Path.GetExtension(file.Upload.FileName);
            try
            {
                if (extension == ".xlxs")
                {
                    var filepath = Path.Combine(_env.ContentRootPath, "wwwroot/uploads", file.Upload.FileName);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await file.Upload.CopyToAsync(fileStream);
                    }

                    XSSFWorkbook hssfworkbook;

                    using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        await file.Upload.CopyToAsync(stream);
                        stream.Position = 0;
                        hssfworkbook = new XSSFWorkbook(stream);
                    }
                    ISheet sheet = hssfworkbook.GetSheetAt(0);


                    for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        var person = CreatePerson(row);
                        _context.Add(person);
                    }
                    await _context.SaveChangesAsync();
                }
                
            }
            catch (Exception)
            {
                
            }

            
            return View();
        }

       

    }
}