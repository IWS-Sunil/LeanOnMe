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
    public class MenuPageController : Controller
    {
        // GET: MenuPage
        LeanOnMeEntities db = new LeanOnMeEntities();
        public ActionResult Index(string sortOrder, string currentFilter, int? page)
        {
            string role = Convert.ToString(Session["RoleName"]);
            if (role.Length > 0 || role != null)
            {
                if (role == "Admin")
                {
                    ViewBag.CurrentSort = sortOrder;
                    ViewBag.PageIDSort = String.IsNullOrEmpty(sortOrder) ? "PageID_Desc" : "";
                    ViewBag.PageNameSort = sortOrder == "PageNameAsc" ? "PageNameDesc" : "PageNameAsc";
                    ViewBag.PageLevelSort = sortOrder == "PageLevelAsc" ? "PageLevelDesc" : "PageLevelAsc";
                    
                    var employees = from e in db.MenuPages
                                    select e;
                   
                    switch (sortOrder)
                    {
                        case "PageID_Desc":
                            employees = employees.OrderByDescending(s => s.PageID);
                            break;
                        case "PageNameAsc":
                            employees = employees.OrderBy(s => s.PageName);
                            break;
                        case "PageNameDesc":
                            employees = employees.OrderByDescending(s => s.PageName);
                            break;
                        case "PageLevelAsc":
                            employees = employees.OrderBy(s => s.PageLevel);
                            break;
                        case "PageLevelDesc":
                            employees = employees.OrderByDescending(s => s.PageLevel);
                            break;

                        default:
                            employees = employees.OrderBy(s => s.PageID);
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

        // GET: MenuPage/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MenuPage/Create
        public ActionResult Create()
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

        // POST: MenuPage/Create
        [HttpPost]
        public ActionResult Create(MenuPage page)
        {
            try
            {
                long count = 0;
                if (db.MenuPages.Any())
                {
                    count = db.MenuPages.Max(m => m.PageID);
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
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/MenuPage"), name);
                    file.SaveAs(path);
                    page.Images = name;
                }
                if (ModelState.IsValid)
                {
                    db.MenuPages.Add(page);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(page);
            }
            catch
            {
                return View(page);
            }
        }

        // GET: MenuPage/Edit/5
        public ActionResult Edit(int id)
        {
            string role = Convert.ToString(Session["RoleName"]);
            if (role.Length > 0 || role != null)
            {
                if (role == "Admin")
                {
                    var page = db.MenuPages.Find(id);
                    return View(page);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return HttpNotFound();
        }

        // POST: MenuPage/Edit/5
        [HttpPost]
        public ActionResult Edit(MenuPage page)
        {
            try
            {
                var file = Request.Files["file"];
                string name = "";
                if (file.ContentLength > 0)
                {
                    string extension = System.IO.Path.GetExtension(file.FileName);
                    name = Convert.ToString(page.PageID) + extension;
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/MenuPage"), name);
                    file.SaveAs(path);
                    page.Images = name;
                }
                else
                {
                    page.Images = Convert.ToString(page.Images);
                }
                if (ModelState.IsValid)
                {
                    db.Entry(page).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(page);
            }
            catch
            {
                return View();
            }
        }

        // GET: MenuPage/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                string role = Convert.ToString(Session["RoleName"]);
                if (role.Length > 0 || role != null)
                {
                    if (role == "Admin")
                    {
                        MenuPage page = db.MenuPages.Find(id);
                        db.MenuPages.Remove(page);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                return HttpNotFound();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

    }
}
