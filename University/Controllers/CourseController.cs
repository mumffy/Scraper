using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using University.DAL;
using University.Models;

namespace University.Controllers
{
    public class CourseController : Controller
    {
        private IUnitOfWork unitOfWork;

        public CourseController()
        {
            this.unitOfWork = new UnitOfWork();
        }

        public CourseController(IUnitOfWork uow) 
        {
            this.unitOfWork = uow;
        }

        // GET: Course
        public ActionResult Index(int? SelectedDepartment)
        {
            var departments = unitOfWork.DepartmentRepository.Get(
                orderBy: x => x.OrderBy(d => d.Name)
                );

            ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", SelectedDepartment);
            int departmentID = SelectedDepartment.GetValueOrDefault();

            var courses = unitOfWork.CourseRepository.Get(
                filter: c => !SelectedDepartment.HasValue || c.DepartmentID == SelectedDepartment,
                orderBy: x => x.OrderBy(c => c.CourseID),
                includeProperties: "Department"
                );

            string sql = courses.ToString(); // cheap way to inspect EF-generated SQL query without using EF Interceptor
            return View(courses.ToList());
        }

        // GET: Course/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Course course = unitOfWork.CourseRepository.GetByID(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            return View(course);
        }

        public ActionResult Create()
        {
            PopulateDepartmentDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,Title,Credits,DepartmentID")] Course course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    unitOfWork.CourseRepository.Insert(course);
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException dex)
            {
                System.Console.WriteLine("Couldn't save course: {0}", dex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            PopulateDepartmentDropDownList(course.DepartmentID);
            return View(course);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Course course = unitOfWork.CourseRepository.GetByID(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            PopulateDepartmentDropDownList(course.DepartmentID);
            return View(course);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Course courseToUpdate = unitOfWork.CourseRepository.GetByID(id);
            if(TryUpdateModel(courseToUpdate, new string[] { "Title", "Credits", "DepartmentID" }))
            {
                try
                {
                     unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException dex)
                {
                    System.Console.WriteLine("Couldn't save course: {0}", dex.Message);
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            PopulateDepartmentDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        private void PopulateDepartmentDropDownList(object selectedDepartment = null)
        {
            var deptsQuery = unitOfWork.DepartmentRepository.Get(orderBy: x => x.OrderBy(d => d.Name));
            ViewBag.DepartmentID = new SelectList(deptsQuery, "DepartmentID", "Name", selectedDepartment);
        }

        // GET: Course/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = unitOfWork.CourseRepository.GetByID(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            unitOfWork.CourseRepository.Delete(id);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult UpdateCourseCredits()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateCourseCredits(int multiplier)
        {
            if(multiplier != null)
            {
                ViewBag.RowsAffected = unitOfWork.ExecuteSqlCommand("UPDATE Course SET Credits = Credits * {0}", multiplier);
            }
            return View();
        }

    }
}
