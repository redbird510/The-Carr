using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI_Portal.Controllers
{
    public class AccessorialsController : Controller
    {

        [HttpPost]
        [Authorize]
        public ActionResult Index()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);

            ViewBag.Title = "Accessorials";
            return View();
        }

        
    }
}