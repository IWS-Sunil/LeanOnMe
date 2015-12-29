using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;

namespace LeanOnMe.Controllers
{
    public class BaseController : Controller
    {
        public LeanOnMeEntities db = new LeanOnMeEntities();
    }
}
