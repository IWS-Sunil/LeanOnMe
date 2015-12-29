using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using PagedList;
using System.Data.Entity;

namespace LeanOnMe.Controllers
{
    public class MyMedicineController : Controller
    {
        // GET: MyMedicine
        LeanOnMeEntities db = new LeanOnMeEntities();
        public ActionResult Index(string sortOrder, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.MedicineSort = String.IsNullOrEmpty(sortOrder) ? "Question_Desc" : "";
            ViewBag.DosageSort = sortOrder == "QuestionSortAsc" ? "QuestionSortDesc" : "QuestionSortAsc";
            ViewBag.DosageTimesSort = sortOrder == "LocationSortAsc" ? "LocationSortDesc" : "LocationSortAsc";
            ViewBag.DoctorSort = sortOrder == "InfoSortAsc" ? "InfoSortDesc" : "InfoSortAsc";
            ViewBag.UserSort = sortOrder == "AddSortAsc" ? "AddSortDesc" : "AddSortAsc";

            var employees = from e in db.MyMedicines
                            select e;

            switch (sortOrder)
            {
                case "Question_Desc":
                    employees = employees.OrderByDescending(s => s.MedicineName);
                    break;
                case "QuestionSortAsc":
                    employees = employees.OrderBy(s => s.DosagePerPill);
                    break;
                case "QuestionSortDesc":
                    employees = employees.OrderByDescending(s => s.DosagePerPill);
                    break;

                case "LocationSortAsc":
                    employees = employees.OrderBy(s => s.TimesTaken);
                    break;
                case "LocationSortDesc":
                    employees = employees.OrderByDescending(s => s.TimesTaken);
                    break;
                case "InfoSortAsc":
                    employees = employees.OrderBy(s => s.PrescribedDoctor);
                    break;
                case "InfoSortDesc":
                    employees = employees.OrderByDescending(s => s.PrescribedDoctor);
                    break;
                case "AddSortAsc":
                    employees = employees.OrderBy(s => s.UserRegistration.UserName);
                    break;
                case "AddSortDesc":
                    employees = employees.OrderByDescending(s => s.UserRegistration.UserName);
                    break;

                default:
                    employees = employees.OrderBy(s => s.MedicineName);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            string role = Convert.ToString(Session["RoleName"]);
            int ids = Convert.ToInt16(Session["UserID"]);
            if (role != "Admin")
            {
                employees = employees.Where(m => m.UserRegistration.Registration.UserID == ids);
            }
            return View(employees.ToPagedList(pageNumber, pageSize));
        }
        
        // GET: MyMedicine/Create
        public ActionResult Create()
        {
            int helper = Convert.ToInt32(Session["UserID"]);
            ViewBag.HelperID = new SelectList(db.Registrations.Where(m => m.Role != "Admin"), "UserID", "UserName");
            string role = Convert.ToString(Session["RoleName"]);
            if (role != "Admin")
            {
                ViewBag.UserID = new SelectList(db.UserRegistrations.Where(m => m.HelperID == helper), "UserID", "UserName");
            }
            else
            {
                ViewBag.UserID = new SelectList(db.UserRegistrations, "UserID", "UserName");
            }
            return View();
        }

        // POST: MyMedicine/Create
        [HttpPost]
        public ActionResult Create(MyMedicine list)
        {
            try
            {
                long count = 0;
                if (db.MyMedicines.Any())
                {
                    count = db.MyMedicines.Max(m => m.MedicineID);
                    count++;
                }
                else
                {
                    count = 1000;
                }
                var file = Request.Files["file"];
                string name = "";
                if (file.ContentLength > 0)
                {
                    string extension = System.IO.Path.GetExtension(file.FileName);
                    name = Convert.ToString(count) + extension;
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/MyMedicines"), name);
                    file.SaveAs(path);
                    list.Images = name;
                }
                string role = Convert.ToString(Session["RoleName"]);
                if (role == "Admin")
                {
                    if (list.MedicineName == "" || list.MedicineName == null)
                    {
                        int helper = Convert.ToInt32(list.HelperID);
                        ViewBag.HelperID = new SelectList(db.Registrations.Where(m => m.Role != "Admin"), "UserID", "UserName", list.HelperID);
                        ViewBag.UserID = new SelectList(db.UserRegistrations.Where(m => m.HelperID == helper), "UserID", "UserName");
                        return View();
                    }
                }
                if (ModelState.IsValid)
                {
                    if (list.HelperID <= 0 || list.HelperID == null)
                    {
                        list.HelperID = Convert.ToInt32(Session["UserID"]);
                    }                
                    db.MyMedicines.Add(list);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(list);
            }
            catch
            {
                return View(list);
            }
        }

        // GET: MyMedicine/Edit/5
        public ActionResult Edit(int id)
        {
            int helper = Convert.ToInt32(Session["UserID"]);
            var lst = db.MyMedicines.Find(id);
            Session["listuser"] = lst.HelperID;
            ViewBag.HelperID = new SelectList(db.Registrations.Where(m => m.Role != "Admin"), "UserID", "UserName", lst.HelperID);
            ViewBag.UserID = new SelectList(db.UserRegistrations.Where(m => m.HelperID == lst.HelperID), "UserID", "UserName", lst.UserID);
            return View(lst);
        }

        // POST: MyMedicine/Edit/5
        [HttpPost]
        public ActionResult Edit(MyMedicine list)
        {
            try
            {
                int user = Convert.ToInt32(Session["listuser"]);
                string role = Convert.ToString(Session["RoleName"]);
                var file = Request.Files["file"];
                string name = "";
                if (file.ContentLength > 0)
                {
                    string extension = System.IO.Path.GetExtension(file.FileName);
                    name = Convert.ToString(list.MedicineID) + extension;
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/MyMedicines"), name);
                    file.SaveAs(path);
                    list.Images = name;
                }
                else
                {
                    list.Images = Convert.ToString(list.Images);
                }
                if (role == "Admin")
                {
                    if (user != list.HelperID)
                    {
                        int helper = Convert.ToInt32(list.HelperID);
                        ViewBag.HelperID = new SelectList(db.Registrations, "UserID", "UserName", list.HelperID);
                        ViewBag.UserID = new SelectList(db.UserRegistrations.Where(m => m.HelperID == helper), "UserID", "UserName");
                        Session["listuser"] = list.HelperID;
                        return View(list);
                    }
                }
                if (ModelState.IsValid)
                {
                    db.Entry(list).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(list);
            }
            catch
            {
                return View(list);
            }
        }

        // GET: MyMedicine/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                MyMedicine lst = db.MyMedicines.Find(id);
                db.MyMedicines.Remove(lst);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View();
            }
        }       
    }
}
