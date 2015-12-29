using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models;
using System.Data.Entity.SqlServer;
using PagedList;
using System.Data;
using System.Data.Entity;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LeanOnMe.Controllers
{
    public class LeanController : Controller
    {
        LeanOnMeEntities db = new LeanOnMeEntities();
        // GET: Lean
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.QuestionIdSort = String.IsNullOrEmpty(sortOrder) ? "UserID_Desc" : "";
            ViewBag.QuestionSort = sortOrder == "UserNameAsc" ? "UserNameDesc" : "UserNameAsc";
            ViewBag.LocationSort = sortOrder == "DescAsc" ? "DescDesc" : "DescAsc";
            ViewBag.InfoSort = sortOrder == "HelperAsc" ? "HelperDesc" : "HelperAsc";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            string role = "";
            int helper = 0;

            DataTable dt = new DataTable();
            dt = (DataTable)Session["User"];
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    role = Convert.ToString(dt.Rows[0]["Role"]);
                    if (role != "Admin")
                    {
                        helper = Convert.ToInt32(dt.Rows[0]["UserID"]);
                    }

                }
            }

            var employees = from e in db.UserRegistrations
                            select e;
            if (helper > 0)
            {
                employees = employees.Where(m => m.HelperID == helper);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(s => s.UserName.Contains(searchString) || s.Description.Contains(searchString)
                                      || SqlFunctions.StringConvert((double)s.UserID).Contains(searchString) || s.Registration.UserName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "UserID_Desc":
                    employees = employees.OrderByDescending(s => s.UserID);
                    break;
                case "USerNameAsc":
                    employees = employees.OrderBy(s => s.UserName);
                    break;
                case "UserNameDesc":
                    employees = employees.OrderByDescending(s => s.UserName);
                    break;

                case "DescAsc":
                    employees = employees.OrderBy(s => s.Description);
                    break;
                case "DescDesc":
                    employees = employees.OrderByDescending(s => s.Description);
                    break;
                case "HelperAsc":
                    employees = employees.OrderBy(s => s.Registration.UserName);
                    break;
                case "HelperDesc":
                    employees = employees.OrderByDescending(s => s.Registration.UserName);
                    break;
                default:
                    employees = employees.OrderBy(s => s.UserID);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(employees.ToPagedList(pageNumber, pageSize));
        }

        // GET: Lean/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Lean/Create
        public ActionResult Create()
        {
            ViewBag.User = new SelectList(db.Registrations.Where(x => x.Role == "Helper"), "UserID", "UserName");
            return View();
        }

        // POST: Lean/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection, UserRegistration user)
        {
            try
            {
                long count = 0;
                if (db.UserRegistrations.Any())
                {
                    count = db.UserRegistrations.Max(m => m.UserID);
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
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/LeanProfiles"), name);
                    file.SaveAs(path);
                }
                user.ProfileImage = name;
                user.OTP = GenerateOTP();
                user.RegDate = DateTime.Now.ToShortDateString();
                if (user.HelperID == 0)
                {
                    user.HelperID = Convert.ToInt32(Session["userid"]);
                }
                if (ModelState.IsValid)
                {
                    db.UserRegistrations.Add(user);
                    db.SaveChanges();
                    string email = (from categorytypes in db.Registrations
                                    where categorytypes.UserID.Equals(user.HelperID)
                                    select categorytypes.EmailID).SingleOrDefault();
                    var body = "<p>Email From: {0}</p><p>User ID:</p><p>{1}</p><p>User Name:</p><p>{2}</p><p>OTP:</p><p>{3}</p><p><strong>Note :</strong></p><p>{4}</p>";
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(email)); //replace with valid value
                    message.CC.Add(new MailAddress("tim.leanonmeapp@gmail.com"));
                    message.Bcc.Add(new MailAddress("kulvinder18may@gmail.com"));
                    message.Subject = "One Time Password";
                    message.Body = string.Format(body, "LeanOnMe", user.UserID, user.UserName, user.OTP, "The OTP is valid for seven(7) days only.");
                    message.IsBodyHtml = true;
                    using (var smtp = new SmtpClient())
                    {
                        await smtp.SendMailAsync(message);
                    }
                    return RedirectToAction("Index");
                }
                return View(user);
            }
            catch
            {
                return View();
            }
        }

        // GET: Lean/Edit/5
        public ActionResult Edit(int id)
        {
            UserRegistration reg = db.UserRegistrations.Find(id);
            ViewBag.User = new SelectList(db.Registrations.Where(x => x.Role == "Helper"), "UserID", "UserName");
            return View(reg);
        }

        // POST: Lean/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(FormCollection frm, UserRegistration reg)
        {
            try
            {
                var file = Request.Files["file"];
                string name = "";
                if (file.ContentLength > 0)
                {
                    string extension = System.IO.Path.GetExtension(file.FileName);
                    name = Convert.ToString(reg.UserID) + extension;
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/LeanProfiles"), name);
                    file.SaveAs(path);
                    reg.ProfileImage = name;
                }
                if (reg.HelperID == 0)
                {
                    reg.HelperID = Convert.ToInt32(Session["userid"]);
                }
                if(reg.RegDate=="" || reg.RegDate==null)
                {
                    reg.RegDate = DateTime.Now.ToShortDateString();
                    reg.OTP = GenerateOTP();
                }
                if (ModelState.IsValid)
                {
                    
                    db.Entry(reg).State = EntityState.Modified;
                    db.SaveChanges();
                    //var email = db.Registrations.Where(m => m.UserID == reg.HelperID).Select(m=>m.EmailID);
                    string email = (from categorytypes in db.Registrations
                                           where categorytypes.UserID.Equals(reg.HelperID)
                                           select categorytypes.EmailID).SingleOrDefault();
                    var body = "<p>Email From: {0}</p><p>User ID:</p><p>{1}</p><p>User Name:</p><p>{2}</p><p>OTP:</p><p>{3}</p><p><strong>Note :</strong></p><p>{4}</p>";
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(email)); //replace with valid value
                    message.CC.Add(new MailAddress("tim.leanonmeapp@gmail.com"));
                    message.Bcc.Add(new MailAddress("kulvinder18may@gmail.com"));
                    message.Subject = "One Time Password";
                    message.Body = string.Format(body, "LeanOnMe",reg.UserID,reg.UserName,reg.OTP,"The OTP is valid for seven(7) days only.");
                    message.IsBodyHtml = true;
                    using (var smtp = new SmtpClient())
                    {
                        await smtp.SendMailAsync(message);                       
                    }
                    return RedirectToAction("Index");
                }
                return View(reg);
            }
            catch (Exception ex)
            {
                return View(reg);
            }
        }

        // GET: Lean/Delete/5
        public ActionResult Delete(int id)
        {
            UserRegistration adm = db.UserRegistrations.Find(id);
            try
            {
                db.UserRegistrations.Remove(adm);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Relationship Exists with other table!";
                return View(adm);
            }
        }

        // POST: Lean/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            UserRegistration adm = db.UserRegistrations.Find(id);
            try
            {
                db.UserRegistrations.Remove(adm);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Relationship Exists with other table!";
                return View(adm);
            }
        }
        public string GenerateOTP()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
