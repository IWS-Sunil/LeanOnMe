using Microsoft.Owin;
using Owin;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System;

[assembly: OwinStartupAttribute(typeof(LeanOnMe.Startup))]
namespace LeanOnMe
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
        
    }
}
