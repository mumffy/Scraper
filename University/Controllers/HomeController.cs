using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using University.DAL;
using University.ViewModels;

namespace University.Controllers
{
    public class HomeController : Controller
    {
        private SchoolContext db = new SchoolContext(); //TODO don't instantiate here; inject, instead

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            //IQueryable<EnrollmentDateGroup> data = db.Students.GroupBy(s => s.EnrollmentDate)
            //    .Select(g => new EnrollmentDateGroup { EnrollmentDate = g.Key, StudentCount = g.Count() });
            String query = 
                "SELECT EnrollmentDate, COUNT(*) AS StudentCount " +
                "FROM Person " +
                "WHERE Discriminator = 'Student' " +
                "GROUP BY EnrollmentDate ";
            IEnumerable<EnrollmentDateGroup> data = db.Database.SqlQuery<EnrollmentDateGroup>(query);

            return View(data.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}