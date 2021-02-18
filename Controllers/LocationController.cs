using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Portal.Models;

namespace PI_Portal.Controllers
{
    public class LocationController : Controller
    {

        private Entities db = new Entities();

        // GET: Location
        public ActionResult Index()
        {
            return View();
        }

        

    }
}