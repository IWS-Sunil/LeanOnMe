using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using LeanOnMe.Models;
using System.Data;

namespace LeanOnMe.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        public ActionResult Index()
        {
            Session["user"] = null;
            Session["RoleName"] = null;
            Session["Pic"] = null;
            Session["UserName"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection frm, Login logs)
        {
            Session["Ref"] = "1";
            Login log = new Login();
            if (Session["user"] != null)
            {
                RegisterUserController reg = new RegisterUserController();
                string pass = Convert.ToString(frm["Password"]);
                DataTable ft = (DataTable)Session["user"];
                string Role = Convert.ToString(ft.Rows[0]["Role"]);
                //if (Role == "Admin")
                //{
                if (pass != null)
                {
                    string da = Convert.ToString(ft.Rows[0]["Password"]);
                    string ca = reg.Decryptdata(da);
                    if (pass == ca)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return View("Lock");
                    }
                }
                int userid = Convert.ToInt32(ft.Rows[0]["UserID"]);
                Session["UserID"] = userid;
                string Image = Convert.ToString(ft.Rows[0]["Image"]);
                Session["UserName"] = Convert.ToString(ft.Rows[0]["UserName"]);
                Session["Pic"] = Image;
                Session["RoleName"] = Role;
                return RedirectToAction("Index", "Home");
                //}
                //else
                //{
                //    return View();
                //}
            }
            else
            {
                if (ModelState.IsValid)
                {

                    log.UserName = Convert.ToString(frm["UserName"]);
                    log.Password = Convert.ToString(frm["Password"]);
                    DataTable dtable = log.GetUser(log);
                    if (dtable.Rows.Count > 0)
                    {
                        string Role = Convert.ToString(dtable.Rows[0]["Role"]);
                        //if (Role == "Admin")
                        //{
                        Session["user"] = dtable;
                        Session["UserName"] = Convert.ToString(dtable.Rows[0]["UserName"]);
                        GetUser.UserID = Convert.ToInt32(dtable.Rows[0]["UserID"]);
                        GetUser.User = Convert.ToString(dtable.Rows[0]["UserName"]);
                        int userid = Convert.ToInt32(dtable.Rows[0]["UserID"]);
                        Session["UserID"] = userid;
                        Session["RoleName"] = Role;
                        string Image = Convert.ToString(dtable.Rows[0]["Image"]);
                        Session["Pic"] = Image;
                        return RedirectToAction("Index", "Home");
                        //}
                        //else
                        //{
                        //    return View();
                        //}
                    }
                    else
                    {
                        ViewBag.Msg = "Wrong Email or Password!";
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }
        }


        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Lock()
        {
            return View();
        }
    }
}