using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Kopis_Showcase.Models;
using System.IO;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Kopis_Showcase.Data;
using NPOI.HSSF.UserModel;
using Microsoft.AspNetCore.Http;

namespace Kopis_Showcase.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly IUploadFile _upload;



        public HomeController(IWebHostEnvironment env, IUploadFile upload)
        {
            _env = env;

            _upload = upload;
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

                XSSFWorkbook xssfworkbook;

                using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite))
                {
                    await file.Upload.CopyToAsync(stream);
                    stream.Position = 0;
                    xssfworkbook = new XSSFWorkbook(stream);
                }
                ISheet sheet = xssfworkbook.GetSheetAt(0);

                _upload.ReadEachRowFromSheet(sheet);
            return RedirectToAction("Index", "Person");
        }

    }
}