using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Portal.Models;


namespace PI_Portal.Controllers
{
    public class HomeController : Controller
    {

        // GET: Test
        public ActionResult Index()
        {
            if (Session["accountname"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            ViewBag.Title = "Login to get started";
            return View();
        }

    }
}