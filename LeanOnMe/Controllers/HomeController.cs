using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;

namespace LeanOnMe.Controllers
{
    public class HomeController : Controller
    {
        LeanOnMeEntities db = new LeanOnMeEntities();
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }       
	}
}