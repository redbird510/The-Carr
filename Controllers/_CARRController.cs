using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Portal.Models;


namespace PI_Portal.Controllers
{
    public class CARRController : Controller      
    {
        private Entities db = new Entities();

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            var carrlist = from c in db.CARRs
                               select c;
            carrlist = carrlist.OrderBy(c => c.CustomerLegalName);
            return View(carrlist);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerLegalName")] CARR carr)
        {
            if (ModelState.IsValid)            {
                string username = Session["accountname"].ToString();

                carr.ActiveFlag = true;
                carr.CreatedBy = username;
                //carr.CreatedOn = DateTime.Today;

                db.CARRs.Add(carr);

                //Create RoutedTo Row
                Routing routedto = new Routing();
                routedto.RoutedTo = username;
                routedto.CreatedBy = username;
                //routedto.CreatedOn = DateTime.Today;
                routedto.DateRoutedTo = DateTime.Now;
                db.Routings.Add(routedto);

                //Create Corporate Contact Row
                Contact contact = new Contact();
                contact.CreatedBy = username;
                //contact.CreatedOn = DateTime.Today;
                db.Contacts.Add(contact);


                //Create Detail Row
                CARRDetail carrdetail = new CARRDetail();
                carrdetail.idCARR = carr.idCarr;
                carrdetail.idRoutedTo = routedto.idRouting;
                carrdetail.idCorporateContact = contact.idContact;
                //always start with version 1
                carrdetail.VersionNumber = 1;
                //carrdetail.CurrentFlag = true;
                carrdetail.ActiveFlag = true;
                carrdetail.CreatedBy = username;
                //carrdetail.CreatedOn = DateTime.Today;
                carrdetail.SalesProfessional = username;
                db.CARRDetails.Add(carrdetail);               

                db.SaveChanges();

                //TODO: I would not redirect, I would have used ajax, and returned Json, and report if name already is use or other errors. 
                //      a good example is login form
                //      but i really dont get how binding and modal works so maybe this is better
                return RedirectToAction("Main","Contract");
                //return RedirectToAction("Index", "Contract", new {@id= carrdetail.idCARRDetail});
            }

            return View(carr);
        }
    }
}