﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using PagedList;
using System.Data.Entity;

namespace LeanOnMe.Controllers
{
    public class MyHelperController : Controller
    {
        LeanOnMeEntities db = new LeanOnMeEntities();
        // GET: MyHepler
        public ActionResult Index(string sortOrder, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.MyHelperSort = String.IsNullOrEmpty(sortOrder) ? "Question_Desc" : "";
            ViewBag.RoleSort = sortOrder == "QuestionSortAsc" ? "QuestionSortDesc" : "QuestionSortAsc";
            ViewBag.OrganisationSort = sortOrder == "LocationSortAsc" ? "LocationSortDesc" : "LocationSortAsc";
            ViewBag.UserSort = sortOrder == "InfoSortAsc" ? "InfoSortDesc" : "InfoSortAsc";
            ViewBag.HelperSort = sortOrder == "AddSortAsc" ? "AddSortDesc" : "AddSortAsc";

            var employees = from e in db.MyHelpers
                            select e;

            switch (sortOrder)
            {
                case "Question_Desc":
                    employees = employees.OrderByDescending(s => s.Name);
                    break;
                case "QuestionSortAsc":
                    employees = employees.OrderBy(s => s.Role);
                    break;
                case "QuestionSortDesc":
                    employees = employees.OrderByDescending(s => s.Role);
                    break;

                case "LocationSortAsc":
                    employees = employees.OrderBy(s => s.Organistaion);
                    break;
                case "LocationSortDesc":
                    employees = employees.OrderByDescending(s => s.Organistaion);
                    break;
                case "InfoSortAsc":
                    employees = employees.OrderBy(s => s.UserRegistration.UserName);
                    break;
                case "InfoSortDesc":
                    employees = employees.OrderByDescending(s => s.UserRegistration.UserName);
                    break;
                case "AddSortAsc":
                    employees = employees.OrderBy(s => s.Registration.UserName);
                    break;
                case "AddSortDesc":
                    employees = employees.OrderByDescending(s => s.Registration.UserName);
                    break;

                default:
                    employees = employees.OrderBy(s => s.Name);
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

        // GET: MyHepler/Create
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

        // POST: MyHepler/Create
        [HttpPost]
        public ActionResult Create(MyHelper list)
        {
            try
            {
                string role = Convert.ToString(Session["RoleName"]);
                long count = 0;
                if (db.MyHelpers.Any())
                {
                    count = db.MyHelpers.Max(m => m.MyHelperID);
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
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/MyHelpers"), name);
                    file.SaveAs(path);
                    list.Images = name;
                }
                if (role == "Admin")
                {
                    if (list.Name == "" || list.Name == null || ModelState.IsValid == false)
                    {
                        int helper = Convert.ToInt32(list.HelperID);
                        ViewBag.HelperID = new SelectList(db.Registrations.Where(m => m.Role != "Admin"), "UserID", "UserName", list.HelperID);
                        ViewBag.UserID = new SelectList(db.UserRegistrations.Where(m => m.HelperID == helper), "UserID", "UserName");
                        return View(list);
                    }
                }
                if (ModelState.IsValid)
                {
                    if (list.HelperID <= 0 || list.HelperID == null)
                    {
                        list.HelperID = Convert.ToInt32(Session["UserID"]);
                    }
                    db.MyHelpers.Add(list);
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

        // GET: MyHepler/Edit/5
        public ActionResult Edit(int id)
        {
            int helper = Convert.ToInt32(Session["UserID"]);
            var lst = db.MyHelpers.Find(id);
            Session["listuser"] = lst.HelperID;
            ViewBag.HelperID = new SelectList(db.Registrations.Where(m => m.Role != "Admin"), "UserID", "UserName", lst.HelperID);
            ViewBag.UserID = new SelectList(db.UserRegistrations.Where(m => m.HelperID == lst.HelperID), "UserID", "UserName", lst.UserID);
            return View(lst);
        }

        // POST: MyHepler/Edit/5
        [HttpPost]
        public ActionResult Edit(MyHelper list)
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
                    name = Convert.ToString(list.MyHelperID) + extension;
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/MyHelpers"), name);
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

        // GET: MyHepler/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                MyHelper lst = db.MyHelpers.Find(id);
                db.MyHelpers.Remove(lst);
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
