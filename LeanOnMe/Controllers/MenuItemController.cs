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
    public class MenuItemController : BaseController
    {        
        // GET: MenuItem
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
                    ViewBag.LinkSort = sortOrder == "LinkAsc" ? "LinkDesc" : "LinkAsc";
                    ViewBag.PositionSort = sortOrder == "PositionAsc" ? "PositionDesc" : "PositionAsc";
                    var employees = from e in db.MenuItems
                                    select e;
                    
                    switch (sortOrder)
                    {
                        case "PageID_Desc":
                            employees = employees.OrderByDescending(s => s.MenuItemID);
                            break;
                        case "PageNameAsc":
                            employees = employees.OrderBy(s => s.MenuPage.PageName);
                            break;
                        case "PageNameDesc":
                            employees = employees.OrderByDescending(s => s.MenuPage.PageName);
                            break;
                        case "PageLevelAsc":
                            employees = employees.OrderBy(s => s.MenuItemText);
                            break;
                        case "PageLevelDesc":
                            employees = employees.OrderByDescending(s => s.MenuItemText);
                            break;
                        case "LinkAsc":
                            employees = employees.OrderBy(s => s.MenuItemLink);
                            break;
                        case "LinkDesc":
                            employees = employees.OrderByDescending(s => s.MenuItemLink);
                            break;
                        case "PositionAsc":
                            employees = employees.OrderBy(s => s.MenuItemPosition);
                            break;
                        case "PositionDesc":
                            employees = employees.OrderByDescending(s => s.MenuItemPosition);
                            break;
                        default:
                            employees = employees.OrderBy(s => s.MenuItemID);
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

        // GET: MenuItem/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        // GET: MenuItem/Create
        public ActionResult Create()
        {
            string role = Convert.ToString(Session["RoleName"]);
            if (role.Length > 0 || role != null)
            {
                if (role == "Admin")
                {
                    List<MenuPage> lst = db.MenuPages.ToList();
                    HttpContext.Session["table"] = lst;
                    return View();
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return HttpNotFound();
        }

        // POST: MenuItem/Create
        [HttpPost]
        public ActionResult Create(FormCollection frm, MenuItem item)
        {
            try
            {
                long count = 0;
                if (db.MenuItems.Any())
                {
                    count = db.MenuItems.Max(m => m.MenuItemID);
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
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/AdminLevel"), name);
                    file.SaveAs(path);
                    item.MenuItemIcon = name;
                }
                if (ModelState.IsValid)
                {
                    db.MenuItems.Add(item);
                    db.SaveChanges();
                    ViewBag.success = "Record Inserted Successfully";
                    item = null;
                    return View("Create");
                }
                return View(item);
            }
            catch (Exception ex)
            {
                return View(item);
            }
        }

        // GET: MenuItem/Edit/5
        public ActionResult Edit(int id)
        {
            string role = Convert.ToString(Session["RoleName"]);
            if (role.Length > 0 || role != null)
            {
                if (role == "Admin")
                {
                    List<MenuPage> lst = db.MenuPages.ToList();
                    HttpContext.Session["table"] = lst;
                    var item = db.MenuItems.Find(id);
                    return View(item);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return HttpNotFound();
        }

        // POST: MenuItem/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection collection, MenuItem item)
        {
            try
            {
                var file = Request.Files["file"];
                string name = "";
                if (file.ContentLength > 0)
                {
                    string extension = System.IO.Path.GetExtension(file.FileName);
                    name = Convert.ToString(item.MenuItemID) + extension;
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/AdminLevel"), name);
                    file.SaveAs(path);
                    item.MenuItemIcon = name;
                }
                else
                {
                    item.MenuItemIcon = Convert.ToString(item.MenuItemIcon);
                }
                if (ModelState.IsValid)
                {
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.success = "Record Modified Successfully";
                    //item = null;
                    return RedirectToAction("Index");
                }
                return View(item);
            }
            catch (Exception ex)
            {
                return View(item);
            }
        }

        // GET: MenuItem/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                string role = Convert.ToString(Session["RoleName"]);
                if (role.Length > 0 || role != null)
                {
                    if (role == "Admin")
                    {
                        MenuItem item = db.MenuItems.Find(id);
                        db.MenuItems.Remove(item);
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
            catch(Exception ex)
            {
                return View();
            }
        }     
       
    }
}
