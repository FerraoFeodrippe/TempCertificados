using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TempCertificados.Models;
using TempCertificados.Process;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace TempCertificados.Controllers
{   
    public class HomeController : Controller
    {
       private readonly IConfiguration _configuration;

       public HomeController(IConfiguration configuration)
       {
         _configuration = configuration;
       }   

        public IActionResult Index()
        {
            return View();
        }

     
        [HttpPost]
        public async Task<ActionResult> CreatePDF(string sheet,string img, string html)
        {
            var se = new SendEmail(_configuration);

            try
            {
                var pdfGenerator = new GeneratorPDF(sheet, img, html, se);
                await pdfGenerator.CreateAndSendPDFs();
                return Content("Emails enviados.");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
          
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
