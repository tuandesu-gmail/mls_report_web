using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Meliasoft.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Home", "Report");
            //return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Test()
        {
            var list = new List<TestModel>();
            for (int i = 50; i < 100; i++)
            {
                var j = (i + 1).ToString();
                list.Add(new TestModel { FirstName = j, LastName = j, Country = j, City = j, Title = j, Bold = (i % 5) == 0 });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        class TestModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
            public string Title { get; set; }
            public bool Bold { get; set; }
        }
    }
}