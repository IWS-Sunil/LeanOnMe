using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using PagedList;
using System.Data.Entity.SqlServer;
using System.Data.Entity;

namespace LeanOnMe.Controllers
{
    public class AdminQuestionController : Controller
    {
        LeanOnMeEntities db = new LeanOnMeEntities();
        // GET: AdminQuestion
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            string role = Convert.ToString(Session["RoleName"]);
            if (role.Length > 0 || role != null)
            {
                if (role == "Admin")
                {
                    ViewBag.CurrentSort = sortOrder;
                    ViewBag.QuestionIdSort = String.IsNullOrEmpty(sortOrder) ? "Question_Desc" : "";
                    ViewBag.QuestionSort = sortOrder == "QuestionSortAsc" ? "QuestionSortDesc" : "QuestionSortAsc";
                    ViewBag.LocationSort = sortOrder == "LocationSortAsc" ? "LocationSortDesc" : "LocationSortAsc";
                    ViewBag.InfoSort = sortOrder == "InfoSortAsc" ? "InfoSortDesc" : "InfoSortAsc";
                    if (searchString != null)
                    {
                        page = 1;
                    }
                    else
                    {
                        searchString = currentFilter;
                    }
                    ViewBag.CurrentFilter = searchString;
                    var employees = from e in db.AdminQuestions
                                    select e;
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        employees = employees.Where(s => s.QuestionText.Contains(searchString) || s.Location.Contains(searchString) || s.Description.Contains(searchString)
                                              || SqlFunctions.StringConvert((double)s.QuestionID).Contains(searchString));
                    }
                    switch (sortOrder)
                    {
                        case "Question_Desc":
                            employees = employees.OrderByDescending(s => s.QuestionID);
                            break;
                        case "QuestionSortAsc":
                            employees = employees.OrderBy(s => s.QuestionText);
                            break;
                        case "QuestionSortDesc":
                            employees = employees.OrderByDescending(s => s.QuestionText);
                            break;

                        case "LocationSortAsc":
                            employees = employees.OrderBy(s => s.Location);
                            break;
                        case "LocationSortDesc":
                            employees = employees.OrderByDescending(s => s.Location);
                            break;
                        case "InfoSortAsc":
                            employees = employees.OrderBy(s => s.Description);
                            break;
                        case "InfoSortDesc":
                            employees = employees.OrderByDescending(s => s.Description);
                            break;

                        default:
                            employees = employees.OrderBy(s => s.QuestionID);
                            break;
                    }
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(employees.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return HttpNotFound();
        }

        // GET: AdminQuestion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminQuestion/Create
        public ActionResult AddQuestion()
        {
            string role = Convert.ToString(Session["RoleName"]);
            if (role.Length > 0 || role != null)
            {
                if (role == "Admin")
                {
                    return View();
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return HttpNotFound();
        }

        // POST: AdminQuestion/Create
        [HttpPost]
        public ActionResult AddQuestion(FormCollection collection,AdminQuestion adm)
        {
            try
            {
                long count = 0;
                if (db.AdminQuestions.Any())
                {
                    count = db.AdminQuestions.Max(m => m.QuestionID);
                    count++;
                }
                else
                {
                    count = 1;
                }
                var file = Request.Files["file"];
                string name = "";
                if (file.ContentLength > 0)
                {
                    string extension = System.IO.Path.GetExtension(file.FileName);
                    name = Convert.ToString(count) + extension;
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/AdminLevel"), name);
                    file.SaveAs(path);
                }
                if (ModelState.IsValid)
                {
                    adm.Image = name;
                    db.AdminQuestions.Add(adm);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(adm);
            }
            catch (Exception ex)
            {
                return View(adm);
            }
        }

        // GET: AdminQuestion/Edit/5
        public ActionResult Edit(int id)
        {
            string role = Convert.ToString(Session["RoleName"]);
            if (role.Length > 0 || role != null)
            {
                if (role == "Admin")
                {
                    AdminQuestion questions = db.AdminQuestions.Find(id);
                    Session["Image"] = questions.Image;
                    if (questions == null)
                    {
                        return HttpNotFound();
                    }
                    return View(questions);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return HttpNotFound();
        }

        // POST: AdminQuestion/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection collection,AdminQuestion adm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var file = Request.Files["file"];
                    string name = "";
                    if (file.ContentLength > 0)
                    {
                        string extension = System.IO.Path.GetExtension(file.FileName);
                        name = Convert.ToString(adm.QuestionID) + extension;
                        string pic = System.IO.Path.GetFileName(file.FileName);
                        string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/AdminLevel"), name);
                        file.SaveAs(path);
                        adm.Image = name;
                    }
                    else
                    {
                        adm.Image = Convert.ToString(Session["Image"]);
                    }
                    db.Entry(adm).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(adm);
            }
            catch
            {
                return View(adm);
            }
        }

        // GET: AdminQuestion/Delete/5       
        public ActionResult Delete(int id)
        {
            string role = Convert.ToString(Session["RoleName"]);
            if (role.Length > 0 || role != null)
            {
                if (role == "Admin")
                {
                    AdminQuestion adm = db.AdminQuestions.Find(id);
                    try
                    {
                        db.AdminQuestions.Remove(adm);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        ViewBag.Message = "Relationship Exists with other table!";
                        return View(adm);
                    }
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return HttpNotFound();
        }
    }
}
