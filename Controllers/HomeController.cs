using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Kopis_Showcase.Models;
using System.IO;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Kopis_Showcase.Interface;
using Kopis_Showcase.Repositories;
using Kopis_Showcase.Data;

namespace Kopis_Showcase.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly IPersonRepository _personRepository;


        public HomeController(IWebHostEnvironment env, IPersonRepository personRepository)
        {
            _env = env;

            _personRepository = personRepository;
        }

        public IActionResult Index()
        {

            ViewData["Title"] = "Home";
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UploadFile file)
        {
           
                var filepath = Path.Combine(_env.ContentRootPath, "wwwroot/uploads", file.Upload.FileName);
                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    await file.Upload.CopyToAsync(fileStream);
                }
                //change to IWorkBook for .xlsx and .xls compatiblitity
                IWorkbook xssfworkbook;

                using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite))
                {
                    await file.Upload.CopyToAsync(stream);
                    stream.Position = 0;
                    xssfworkbook = new XSSFWorkbook(stream);
                }
                ISheet sheet = xssfworkbook.GetSheetAt(0);
                _personRepository.ReadEachRowFromSheet(sheet);
                
            return RedirectToAction("Index", "Person");
        }

    }
}