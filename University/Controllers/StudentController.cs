using PagedList;
using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using University.DAL.Repositories;
using University.Models;

namespace University.Controllers
{
    public class StudentController : Controller
    {
        //private SchoolContext db = new SchoolContext();
        private IStudentRepository studentRepo;

        public StudentController(IStudentRepository studentRepo)
        {
            this.studentRepo = studentRepo;
        }

        // GET: Student
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParam = sortOrder == "date" ? "date_desc" : "date";

            if (searchString != null) {
                page = 1;
            } else {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            //var students = db.Students.Select(x => x);
            var students = studentRepo.GetAllStudents();


            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
                students = students.Where(s => s.LastName.ToUpper().Contains(searchString)
                                            || s.FirstMidName.ToUpper().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;
            int pageNumber = page ?? 1;
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        // GET: Student/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await studentRepo.GetStudentByID(id.Value);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    studentRepo.AddStudent(student);
                    studentRepo.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException ex)
            {
                System.Console.WriteLine("logged: {0}", ex.Message); //TODO use logger
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }

        // GET: Student/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = await studentRepo.GetStudentByID(id.Value);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Student studentToUpdate = await studentRepo.GetStudentByID(id.Value);
            if (studentToUpdate == null)
            {
                string error = "Invalid StudentID";
                System.Console.WriteLine("logged: {0}", error); //TODO use logger
                ModelState.AddModelError("", error);
                return View(studentToUpdate);
            }

            if (TryUpdateModel(studentToUpdate, "", new string[] {"LastName", "FirstMidName", "EnrollmentDate" }))
            {
                try
                {
                    studentRepo.Save();
                    return View("Index");
                }
                catch (RetryLimitExceededException ex)
                {
                    System.Console.WriteLine("logged: {0}", ex.Message); //TODO use logger
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            return View(studentToUpdate);
        }

        // GET: Student/Delete/5
        public async Task<ActionResult> Delete(int? id, bool? saveChangesError=false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete Failed!";
            }

            Student student = await studentRepo.GetStudentByID(id.Value);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Student student = await studentRepo.GetStudentByID(id);
                studentRepo.DeleteStudent(student);
                studentRepo.Save();
            }
            catch (RetryLimitExceededException ex)
            {
                System.Console.WriteLine("logged: {0}", ex.Message); //TODO use logger
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            studentRepo.Dispose();
            base.Dispose(disposing);
        }
    }
}
