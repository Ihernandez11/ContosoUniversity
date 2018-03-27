using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using PagedList;

namespace ContosoUniversity.Controllers
{
    public class StudentController : Controller
    {
        //Instantiate a database context object
        private SchoolContext db = new SchoolContext();

        // GET: Student
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            /*ternary statement: if condition is true, return first expression. 
             * If false, return second expression*/
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var students = from s in db.Students
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                || s.FirstName.Contains(searchString));
            }

            /*The first time the Index page is requested, there's no query string. 
             * The students are displayed in ascending order by LastName, which 
             * is the default as established by the fall-through case in the 
             * switch statement. When the user clicks a column heading hyperlink, 
             * the appropriate sortOrder value is provided in the query string.
             
             The Viewbag variables are used so we can configure the column 
             heading hyperlinks with the corresponding query string values*/
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 6;
            /*Null-coalescing operater defines a default value for a nullable type. This
             means return the value of page if it has a value, or return 1 if it is null. 
             Like nvl*/
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        // GET: Student/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }


        // GET: Movies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        /*The Bind attribute is used to maintain the data submitted by a form. 
         It gets the values posted and passes them to the action method in parameters. 
         So in this case, It's getting the lastname, firstname, and enrollmentDate
         as parameters of a student entity and submitting them to Add method. 
         
         The Id parameter is not needed because SQL Server automatically 
         generates the primary key value. 
         
         The Include parameter sets a whitelist of values that can be submitted*/
        public ActionResult Create([Bind(Include = "LastName, FirstName, EnrollmentDate")]Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                /*Log the error (uncomment dex variable name and add a line to
                 write a log*/
                ModelState.AddModelError("", "Unable to save changes.");
            }
            return View(student);
        }

        

        // GET: Student/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]

        /*So we don't want to use the Bind attribute here because it clears
         out any pre-existing data in fields not listed in the Include param
         
         The new code reads the existing entity and calls TryUpdateModel to 
         update fields from user input in the posted form data*/
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var studentToUpdate = db.Students.Find(id);
            /*The TryUpdateModel updates fields if the Modified flag on the 
             entity returns true.
             
             The whitelisted fields are also included as parameters in the 
             TryUpdateModel method
             
             The following Entity states are:
             -Added: The entity does not yet exist in the database. The 
             SaveChanges method must issue an INSERT statement.

            -Unchanged: Nothing needs to be done with this entity by the 
            SaveChanges method. When you read an entity from the database, 
            the entity starts out with this status.

            -Modified. Some or all of the entity's property values have been 
             modified. The SaveChanges method must issue an UPDATE statement.

            -Deleted. The entity has been marked for deletion. The SaveChanges 
            method must issue a DELETE statement.

            -Detached: The entity isn't being tracked by the database context.
             */
            if (TryUpdateModel(studentToUpdate, "", 
                new string[] { "LastName", "FirstName", "EnrollmentDate"}))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }

                catch (RetryLimitExceededException /* dex */)
                {
                    //Log error using dex variable
                    ModelState.AddModelError("", "Unable to save changes.");
                }
            }

            return View(studentToUpdate);
        }





        // GET: Student/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError=false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            /*Use GetValueOrDefault to check if the value was provided as a 
            method parameter*/
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again.";
            }

            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            /*We will try to update the db, but if there is an error, we will
             redirect to the Delete page and send the id, and saveChangesError 
             value of true to the HttpGet method*/
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException /*dex*/)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
