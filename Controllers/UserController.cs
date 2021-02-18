using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.DirectoryServices;
using PI_Portal.Models;

namespace PI_Portal.Controllers
{
    public class UserController : Controller
    {
        private Entities db = new Entities();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
           
            return View();
        }

       

        [HttpPost]
        public JsonResult CheckSession()
        {
            JsonResult json = new JsonResult();
            if (Session["AccountName"] != null) {
                json.Data = new { success = true };
            } else {
                json.Data = new { success = false };
            }
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public JsonResult Authenticate()
        {
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            string path = System.Configuration.ConfigurationManager.AppSettings["LDAP"];
            JsonResult json = new JsonResult();

            if (String.IsNullOrEmpty(username) )
            {
                json.Data = new { success = false, error = "Username Required" };
                return json;
            }

            if (String.IsNullOrEmpty(password))
            {
                json.Data = new { success = false, error = "Password Required" };
                return json;
            }

            if (username.Contains("@purolator.com"))
            {
                json.Data = new { success = false, error = "Remove @purolator.com from your User Name" };
                return json;
            }

            try
            {
                DirectoryEntry DE = new DirectoryEntry(@"LDAP://" + path, username, password);
                //DirectoryEntry DE = new DirectoryEntry(@"LDAP://cpggpc.ca:636", username, password);
                DirectorySearcher DS = new DirectorySearcher(DE);
                

                DS.Filter = "sAMAccountName=" + username;
                SearchResult SR = DS.FindOne();
                DirectoryEntry USER = SR.GetDirectoryEntry();

                if ( USER != null)
                {
                    string displayname = USER.Properties["displayName"].Value.ToString();
                    string firstname = USER.Properties["givenName"].Value.ToString();
                    string lastname = USER.Properties["sn"].Value.ToString();
                    string email = USER.Properties["userPrincipalName"].Value.ToString();
                    string objectname = USER.Properties["objectCategory"].Value.ToString();
                    string accountname = USER.Properties["sAMAccountName"].Value.ToString();
                    string name = USER.Properties["name"].Value.ToString();
                    string distinguishedName = USER.Properties["distinguishedName"].Value.ToString();

                    //foreach (string key in USER.Properties.PropertyNames)
                    //{
                    //    string sPropertyValues = String.Empty;
                    //    foreach (object pc in USER.Properties[key])
                    //    {
                    //        sPropertyValues += Convert.ToString(pc) + ";";
                    //    }
                    //    sPropertyValues = sPropertyValues.Substring(0, sPropertyValues.Length - 1);
                    //    System.Diagnostics.Debug.WriteLine(key + "=" + sPropertyValues);
                    //}



                    Session["accountname"] = accountname;
                    Session["name"] = displayname;
                    Session["email"] = email;
                    //Session["role"] = "Administrator";
                    //Get Role from database
                    var roleObject = (from aur in db.applicationUserRoles
                                      join ar in db.applicationRoles on aur.idRole equals ar.id
                                      where aur.ActiveDirectory == accountname && aur.ActiveFlag == true && aur.idApplication == 1
                                      select ar)
                                     .SingleOrDefault();
                    if (roleObject != null)
                    {
                        string userRole = roleObject.Role.Trim();
                        Session["userrole"] = userRole;
                        //See if user has an image
                        string imgLoc = "/Images/users/" + Session["accountname"] + ".jpg";
                        if (System.IO.File.Exists(Server.MapPath(imgLoc)))
                        {
                            Session["thumbnail"] = imgLoc;
                        }
                        Startup.IdentitySignin(accountname, displayname);
                        json.Data = new { success = true };
                    }
                    else
                    {
                        json.Data = new { success = false, error = "You Authenicated, but your account is not assigned to a role" };
                    }                    
                } 
                else
                {
                    json.Data = new { success = false, error = "User Not Found" };
                }
            }
            catch (System.Exception e)
            {
                json.Data = new { success = false, error = e.Message.ToString() };
            }
            return json;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewBag.Title = "Login";
            ViewBag.Error = "";

            // Check for Chromium-based browsers (e.g., Chrome, the newest build of Edge)
            if (Request.Browser.Browser.ToLower() != "chrome")
            {
                ViewBag.Error = "Please Use Your Chrome Browser";
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Startup.IdentitySignout();
            return RedirectToAction("Login");      
            //return Redirect(Url.Action("Login", "User"));
        }
    }

}