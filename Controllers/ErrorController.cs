using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PI_Portal.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index(string error)
        {
            ViewBag.Title = "Error";
            ViewBag.Message = error;
            return View();
        }

        [HttpGet]
        public ActionResult Http403(string error)
        {
            ViewBag.Title = "Error Access Denied";
            ViewBag.Message = error;
            Response.StatusCode = 404;
            return View();
        }

        [HttpGet]
        public ActionResult Http404(string error)
        {
            ViewBag.Title = "Error Page not found";
            ViewBag.Message = error;
            Response.StatusCode = 404;
            return View();
        }

        [HttpGet]
        public ActionResult Http500(string error)
        {
            ViewBag.Title = "Server Error";
            ViewBag.Message = error;
            Response.StatusCode = 500;
            return View();
        }

        public ActionResult Test()
        {
            throw new Exception("Test Exception");
        }
    }


}