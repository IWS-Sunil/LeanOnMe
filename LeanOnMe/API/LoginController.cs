using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataAccess.Models;
using System.Collections;
using System.Data.Entity;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LeanOnMe.API
{
    public class LoginController : ApiController
    {
        // Project      : Lean On Me(Web Services)
        // Client       : Tim J
        // Developer    : Sunil Thakur
        // Organisation : Infrawebsoft Technologies Pvt. Ltd.
        // Title        : Login API

        LeanOnMeEntities db = new LeanOnMeEntities();
        public IEnumerable Login(int id)
        {
            int x = id;
            List<CommonData> lst = new List<CommonData>();
            if (x > 0)
            {
                var res = db.UserRegistrations.Where(m => m.UserID == x).ToList();
                string path = "/Uploads/LeanProfiles/";
                string OTP = GenerateOTP();
                foreach (var i in res)
                {
                    DateTime regdate = Convert.ToDateTime(i.RegDate);
                    DateTime datenow = DateTime.Now;
                    double diff = (datenow - regdate).TotalDays;
                    if (diff > 7)
                    {
                        OTP = "";
                    }
                    else
                    {
                        OTP = i.OTP;
                    }
                    lst.Add(new CommonData
                    {
                        Icon = path + i.ProfileImage,
                        ID = i.UserID,
                        HelperID = i.HelperID,
                        UserName = i.UserName,
                        HelperName = i.Registration.UserName,
                        Mobile = i.Registration.ContactNo,
                        OTP = OTP
                    });
                }

            }
            return lst;
        }
        public string GenerateOTP()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task<IEnumerable> Get(string userid)
        {
            List<CommonData> lst = new List<CommonData>();
            string otp = GenerateOTP();
            int id = Convert.ToInt32(userid);
            try
            {
                UserRegistration reg = db.UserRegistrations.Find(id);
                reg.OTP = otp;
                db.Entry(reg).State = EntityState.Modified;
                db.SaveChanges();
                string email = (from categorytypes in db.Registrations
                                where categorytypes.UserID.Equals(reg.HelperID)
                                select categorytypes.EmailID).SingleOrDefault();
                var body = "<p>Email From: {0}</p><p>User ID:</p><p>{1}</p><p>User Name:</p><p>{2}</p><p>OTP:</p><p>{3}</p><p><strong>Note :</strong></p><p>{4}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(email)); //User Id of Helper
                message.CC.Add(new MailAddress("tim.leanonmeapp@gmail.com"));
                message.CC.Add(new MailAddress("kulvinder18may@gmail.com"));
                message.Subject = "One Time Password";
                message.Body = string.Format(body, "LeanOnMe", reg.UserID, reg.UserName, reg.OTP, "The OTP is valid for single use only.");
                message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {

            }
            lst.Add(new CommonData
            {
                OTP = otp
            });
            return lst;
        }

    }
}