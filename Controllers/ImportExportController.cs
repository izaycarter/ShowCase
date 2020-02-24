using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kopis_Showcase.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;


namespace Kopis_Showcase.Controllers
{
    public class ImportExportController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public ImportExportController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public ActionResult Index()
        {
            return View();
        }

        private Person CreatePerson(IRow row, out Person person)
        {
            person = new Person();

            for (int i = row.FirstCellNum; i <= row.LastCellNum; i++)
            {
                ICell cell = row.GetCell(i);
                if(i == 0) { person.FirstName = cell.ToString(); }

                if (i == 1) { person.LastName = cell.ToString(); }

                if (i == 2) {
                    if (cell.ToString() == "Female")
                    {
                        person.GenderID = 1;

                    } if(cell.ToString() == "Male")
                    {
                        person.GenderID = 2;
                    }
                    else
                    {
                        person.GenderID = 3;
                    }
                    
                }

                if (i == 3) { person.DateOfBirth = cell.ToString(); }

                if (i == 4) { person.MaritalStatus.MaritalStatusName = cell.ToString(); }

                if (i == 5) { person.EmailAddress = cell.ToString(); }

                if (i == 6) { person.StreetAddressLine1 = cell.ToString(); }

                if (i == 7) { person.StreetAddressLine2 = cell.ToString(); }

                if (i == 8) { person.PhoneNumber = cell.ToString(); }

                if (i == 9) { person.City = cell.ToString(); }

                if (i == 10) { person.State = cell.ToString(); }

                if (i == 11) { person.Zip = cell.ToString(); }

            }

            return person;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OnPostImport(IFormFile upfile)
          {
            string folderName = "Upload";
            string webRootPath = _env.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string nameofFile = upfile.FileName;
            string fullPath = Path.Combine(newPath, nameofFile);


            XSSFWorkbook hssfworkbook;
             using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
             {
                upfile.CopyTo(stream);
                stream.Position = 0;
                hssfworkbook = new XSSFWorkbook(stream);
            }
             ISheet sheet = hssfworkbook.GetSheetAt(0);
             var people = new List<Person>();

            for(int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                CreatePerson(row, out Person person);
                people.Add(person);
        
            }

            return View("Index", people);
         }

        public async Task<IActionResult> OnPostExport()
        {
            string sWebRootFolder = _env.WebRootPath;
            string sFileName = @"demo.xlsx";
            _ = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
            _ = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Demo");
                IRow row = excelSheet.CreateRow(0);

                row.CreateCell(0).SetCellValue("ID");
                row.CreateCell(1).SetCellValue("Name");
                row.CreateCell(2).SetCellValue("Age");

                row = excelSheet.CreateRow(1);
                row.CreateCell(0).SetCellValue(1);
                row.CreateCell(1).SetCellValue("Kane Williamson");
                row.CreateCell(2).SetCellValue(29);

                row = excelSheet.CreateRow(2);
                row.CreateCell(0).SetCellValue(2);
                row.CreateCell(1).SetCellValue("Martin Guptil");
                row.CreateCell(2).SetCellValue(33);

                row = excelSheet.CreateRow(3);
                row.CreateCell(0).SetCellValue(3);
                row.CreateCell(1).SetCellValue("Colin Munro");
                row.CreateCell(2).SetCellValue(23);

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
