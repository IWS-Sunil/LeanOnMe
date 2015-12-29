using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataAccess.Models;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using PagedList;

namespace LeanOnMe.Controllers
{
    public class RegisterUserController : Controller
    {
        LeanOnMeEntities db = new LeanOnMeEntities();
        //
        // GET: /RegisterUser/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            string role = Convert.ToString(Session["RoleName"]);
            if(role.Length>0 || role!=null)
            {
                if(role=="Admin")
                {
                    ViewBag.CurrentSort = sortOrder;
                    ViewBag.QuestionIdSort = String.IsNullOrEmpty(sortOrder) ? "Question_Desc" : "";
                    ViewBag.QuestionSort = sortOrder == "QuestionSortAsc" ? "QuestionSortDesc" : "QuestionSortAsc";
                    ViewBag.LocationSort = sortOrder == "LocationSortAsc" ? "LocationSortDesc" : "LocationSortAsc";
                    ViewBag.InfoSort = sortOrder == "InfoSortAsc" ? "InfoSortDesc" : "InfoSortAsc";
                    ViewBag.AddressSort = sortOrder == "AddSortAsc" ? "AddSortDesc" : "AddSortAsc";
                    ViewBag.RoleSort = sortOrder == "RoleSortAsc" ? "RoleSortDesc" : "RoleSortAsc";
                    if (searchString != null)
                    {
                        page = 1;
                    }
                    else
                    {
                        searchString = currentFilter;
                    }
                    ViewBag.CurrentFilter = searchString;
                    var employees = from e in db.Registrations
                                    select e;
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        employees = employees.Where(s => s.UserName.Contains(searchString) || s.EmailID.Contains(searchString) || s.ContactNo.Contains(searchString)
                                              || SqlFunctions.StringConvert((double)s.UserID).Contains(searchString) || s.Address.Contains(searchString)
                                              || s.Role.Contains(searchString));
                    }
                    switch (sortOrder)
                    {
                        case "Question_Desc":
                            employees = employees.OrderByDescending(s => s.UserID);
                            break;
                        case "QuestionSortAsc":
                            employees = employees.OrderBy(s => s.UserName);
                            break;
                        case "QuestionSortDesc":
                            employees = employees.OrderByDescending(s => s.UserName);
                            break;

                        case "LocationSortAsc":
                            employees = employees.OrderBy(s => s.EmailID);
                            break;
                        case "LocationSortDesc":
                            employees = employees.OrderByDescending(s => s.EmailID);
                            break;
                        case "InfoSortAsc":
                            employees = employees.OrderBy(s => s.ContactNo);
                            break;
                        case "InfoSortDesc":
                            employees = employees.OrderByDescending(s => s.ContactNo);
                            break;
                        case "AddSortAsc":
                            employees = employees.OrderBy(s => s.Address);
                            break;
                        case "AddSortDesc":
                            employees = employees.OrderByDescending(s => s.Address);
                            break;
                        case "RoleSortAsc":
                            employees = employees.OrderBy(s => s.Role);
                            break;
                        case "RoleSortDesc":
                            employees = employees.OrderByDescending(s => s.Role);
                            break;

                        default:
                            employees = employees.OrderBy(s => s.UserID);
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

        //
        // GET: /RegisterUser/Details/5
        public ActionResult Details(int id)
        {
            
            return View();
        }

        //
        // GET: /RegisterUser/Create
        public ActionResult Register()
        {
            return View();               
        }

        //
        // POST: /RegisterUser/Create
        [HttpPost]
        public ActionResult Register(FormCollection collection, Registration register)
        {
            string UserName = "";
            try
            {
                Registration reg = new Registration();
                UserName = Convert.ToString(Request.Form["UserName"]);
                string Pass = Convert.ToString(Request.Form["Password"]);
                string Password = Encryptdata(Pass);
                string Contact = Convert.ToString(Request.Form["ContactNo"]);
                string EmailID = Convert.ToString(Request.Form["EmailID"]);
                string Address = Convert.ToString(Request.Form["Address"]);
                long count = 0;
                if (db.Registrations.Any())
                {
                    count = db.Registrations.Max(m => m.UserID);
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
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/Profiles"), name);
                    file.SaveAs(path);
                }
                if (ModelState.IsValid)
                {
                    reg.UserName = UserName;
                    reg.EmailID = EmailID;
                    reg.Password = Password;
                    reg.Role = "Helper";
                    reg.Image = name;
                    reg.ContactNo = Contact;
                    reg.Address = Address;
                    reg.ConfirmPassword = Password;
                    db.Registrations.Add(reg);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    return View(reg);
                }

            }
            catch (Exception ex)
            {
                if (UserName=="" || UserName==null)
                {
                    ViewBag.Error = "Error, Please try again!";
                }
                else
                {
                    ViewBag.Error = "This Email is already Registered !";
                }
                return View();
            }
        }

        //
        // GET: /RegisterUser/Edit/5
        public ActionResult Profiles(int id)
        {
            Registration reg = db.Registrations.Find(id);
            string pass = Decryptdata(reg.Password);
            reg.Password = pass;
            if (reg == null)
            {
                return HttpNotFound();
            }
            ViewBag.Role = new SelectList(db.UserRoles, "Role", "RoleName",reg.Role);
            return View(reg);
        }

        //
        // POST: /RegisterUser/Edit/5
        [HttpPost]
        public ActionResult Profiles(FormCollection collection, Registration reg)
        {
            try
            {
                string Pass = Convert.ToString(Request.Form["Password"]);
                string Password = Encryptdata(Pass);
                var file = Request.Files["file"];
                string name = "";
                if (reg.Role == null || reg.Role=="")
                {
                    reg.Role = "Helper";
                }
                if (file.ContentLength > 0)
                {
                    string extension = System.IO.Path.GetExtension(file.FileName);
                    name = Convert.ToString(reg.UserID) + extension;
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Uploads/Profiles"), name);
                    file.SaveAs(path);
                    reg.Image = name;
                    Session["Pic"] = name;
                }
                reg.Password = Password;
                db.Entry(reg).State = EntityState.Modified;
                db.SaveChanges();
                if (reg.Role == "Helper")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "RegisterUser");
                }
            }
            catch
            {
                ViewBag.Role = new SelectList(db.UserRoles, "Role", "RoleName", reg.Role);
                return View();
            }
        }

        //
        // GET: /RegisterUser/Delete/5
        public ActionResult Delete(int id)
        {
            string role = Convert.ToString(Session["RoleName"]);
            if (role.Length > 0 || role != null)
            {
                if (role == "Admin")
                {
                    Registration adm = db.Registrations.Find(id);
                    try
                    {
                        db.Registrations.Remove(adm);
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
            else
            {
                return HttpNotFound();
            }
        }
        
        public string Encryptdata(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decryptdata(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public void SetRoles()
        {
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "Helper", Value = "Helper" });
            li.Add(new SelectListItem { Text = "Admin", Value = "Admin" });            
            ViewData["Role"] = li;
        }
    }
}
