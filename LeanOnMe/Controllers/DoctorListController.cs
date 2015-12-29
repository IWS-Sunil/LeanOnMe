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
    // Project      : Lean On Me
    // Client       : Tim J
    // Developer    : Sunil Thakur
    // Organisation : Infrawebsoft Technologies Pvt. Ltd.
    // Title        : Doctor List Controller
    public class DoctorListController : BaseController
    {
        // GET: DoctorList - Fetch all doctors according to the user
        public ActionResult Index(string sortOrder, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DoctorSort = String.IsNullOrEmpty(sortOrder) ? "Doctor_Desc" : "";
            ViewBag.SpecialitySort = sortOrder == "SpecialitySortAsc" ? "SpecialitySortDesc" : "SpecialitySortAsc";
            ViewBag.OrganisationSort = sortOrder == "OrganisationSortAsc" ? "OrganisationSortDesc" : "OrganisationSortAsc";
            ViewBag.UserSort = sortOrder == "UserSortAsc" ? "UserSortDesc" : "UserSortAsc";
            ViewBag.HelperSort = sortOrder == "HelperSortAsc" ? "HelperSortDesc" : "HelperSortAsc";

            var employees = from e in db.DoctorLists
                            select e;

            switch (sortOrder)
            {
                case "Doctor_Desc":
                    employees = employees.OrderByDescending(s => s.DoctorName);
                    break;
                case "SpecialitySortAsc":
                    employees = employees.OrderBy(s => s.Speciality);
                    break;
                case "SpecialitySortDesc":
                    employees = employees.OrderByDescending(s => s.Speciality);
                    break;
                case "OrganisationSortAsc":
                    employees = employees.OrderBy(s => s.Organisation);
                    break;
                case "OrganisationSortDesc":
                    employees = employees.OrderByDescending(s => s.Organisation);
                    break;
                case "UserSortAsc":
                    employees = employees.OrderBy(s => s.UserRegistration.UserName);
                    break;
                case "UserSortDesc":
                    employees = employees.OrderByDescending(s => s.UserRegistration.UserName);
                    break;
                case "HelperSortAsc":
                    employees = employees.OrderBy(s => s.Registration.UserName);
                    break;
                case "HelperSortDesc":
                    employees = employees.OrderByDescending(s => s.Registration.UserName);
                    break;

                default:
                    employees = employees.OrderBy(s => s.DoctorName);
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

        // GET: DoctorList/Create - Generate the Add New Doctor Screen
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

        // POST: DoctorList/Create - Adds the new Doctor into the database
        [HttpPost]
        public ActionResult Create(DoctorList list)
        {
            try
            {
                long count = 0;
                if (db.DoctorLists.Any())
                {
                    count = db.DoctorLists.Max(m => m.DoctorID);
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
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/MyDoctors"), name);
                    file.SaveAs(path);
                    list.Images = name;
                }
                string role = Convert.ToString(Session["RoleName"]);
                if (role == "Admin")
                {
                    if (list.DoctorName == "" || list.DoctorName == null || ModelState.IsValid == false)
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
                    db.DoctorLists.Add(list);
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

        // GET: DoctorList/Edit - Generate the Update Doctor Details Screen for a particular doctor
        public ActionResult Edit(int id)
        {
            int helper = Convert.ToInt32(Session["UserID"]);
            var lst = db.DoctorLists.Find(id);
            Session["listuser"] = lst.HelperID;
            ViewBag.HelperID = new SelectList(db.Registrations.Where(m => m.Role != "Admin"), "UserID", "UserName", lst.HelperID);
            ViewBag.UserID = new SelectList(db.UserRegistrations.Where(m => m.HelperID == lst.HelperID), "UserID", "UserName", lst.UserID);
            return View(lst);
        }

        // POST: DoctorList/Edit - Update the selected Doctor record in database
        [HttpPost]
        public ActionResult Edit(DoctorList list)
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
                    name = Convert.ToString(list.DoctorID) + extension;
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/MyDoctors"), name);
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
                        ViewBag.HelperID = new SelectList(db.Registrations.Where(m=>m.Role!="Admin"), "UserID", "UserName", list.HelperID);
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

        // GET: DoctorList/Delete - Delete the particular record 
        public ActionResult Delete(int id)
        {
            try
            {
                DoctorList lst = db.DoctorLists.Find(id);
                db.DoctorLists.Remove(lst);
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
