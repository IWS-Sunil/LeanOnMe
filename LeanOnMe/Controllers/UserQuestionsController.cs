using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;


namespace LeanOnMe.Controllers
{    
    public class UserQuestionsController : Controller
    {
        // GET: UserQuestions
        LeanOnMeEntities db = new LeanOnMeEntities();

        public ActionResult Index()
        {
            return View();
        }

        // GET: UserQuestions/Details/5
        public ActionResult ShowMenu()
        {
            var adm = db.AdminQuestions.ToList();
            return View(adm);
        }

        // GET: UserQuestions/Create
        public ActionResult Create()
        {
            ViewBag.QuestionID = new SelectList(db.AdminQuestions, "QuestionID", "QuestionText");
            ViewBag.HelperID = new SelectList(db.Registrations.Where(m => m.Role == "Helper"), "UserID", "UserName");
            //ViewBag.UserID = new SelectList(db.UserRegistrations.Where(m => m.HelperID == id), "UserID", "UserName");
            return View();
        }

        // POST: UserQuestions/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserQuestions/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserQuestions/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserQuestions/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserQuestions/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
