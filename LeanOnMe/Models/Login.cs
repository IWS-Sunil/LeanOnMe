using LeanOnMe.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanOnMe.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Email is required!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);
        RegisterUserController reg = new RegisterUserController();

        public DataTable GetUser(Login log)
        {
            DataTable dt = new DataTable();
            try
            {
                string pass = reg.Encryptdata(log.Password);
                SqlDataAdapter adp = new SqlDataAdapter("spLogin", con);
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                adp.SelectCommand.Parameters.AddWithValue("@username", log.UserName.ToLower());
                adp.SelectCommand.Parameters.AddWithValue("@password", pass.ToLower());
                adp.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                return dt;
            }

        }

    }
}
