using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Peps
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            String _path = String.Concat(System.Environment.GetEnvironmentVariable("PATH"), ";", System.AppDomain.CurrentDomain.RelativeSearchPath);
            System.Environment.SetEnvironmentVariable("PATH", _path, EnvironmentVariableTarget.Process);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        void Application_Error(object sender, EventArgs e)
        {
            //// Code that runs when an unhandled error occurs

            //// Get the exception object.
            //Exception exc = Server.GetLastError();

           
            //// For other kinds of errors give the user some information
            //// but stay on the default page
            //Response.Write("<h2>Global Error</h2>\n");
            //Response.Write(
            //    "<p>" + exc.Message + "</p>\n");
            //Response.Write("Return to the <a href='Index.aspx'>" +
            //    "Main Page</a>\n");
            //// Clear the error from the server
            //Server.ClearError();
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}