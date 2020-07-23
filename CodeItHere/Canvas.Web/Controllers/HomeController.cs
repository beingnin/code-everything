using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Canvas.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Download()
        {
            var filestream = new FileStream(@"C:\Users\nithin.bc\Desktop\2.png", FileMode.Open);
            var filename = "MyFile -" + DateTime.Now.ToString("dd'\'MM'\'yyyy"+ ".pdf"); //which shows MyFile - 22%2f07%2f2020.pdf in debugger
            Response.AppendHeader("content-disposition", "attachment; filename*=UTF-8''" + Uri.EscapeDataString(filename));
            return File(filestream, "application/pdf");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}