using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Kopis_Showcase.Models;
using System.Data;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

using System.Globalization;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Kopis_Showcase.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public HomeController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
       
    }
}