using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Portal.Models;

namespace PI_Portal.Controllers
{
    public class LegalController : Controller
    {
        private Entities db = new Entities();

        [HttpPost]
        [Authorize]    
        public ActionResult Index(int id)   // id = iddocumnet
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);            
            var MyCarrDocument = db.CARRDocuments.Where(x => x.idCARRDetail == MyCarrDetail.idCARRDetail && x.idDocument == id).FirstOrDefault();
            var MyDocument = db.Documents.Where(x => x.idDocument == id).FirstOrDefault();

            if ( MyCarrDocument == null )
            {
                string username = Session["accountname"].ToString();

                CARRDocument CarrDocument = new CARRDocument();
                CarrDocument.idCARRDetail = MyCarrDetail.idCARRDetail;
                CarrDocument.idDocument = id;
                CarrDocument.CreatedBy = username;
                CarrDocument.CreatedOn = DateTime.Now;
                CarrDocument.DocContent = MyDocument.DefaultContent;
                db.CARRDocuments.Add(CarrDocument);
                try
                {
                    db.SaveChanges();
                    MyCarrDocument = db.CARRDocuments.Where(x => x.idCARRDetail == MyCarrDetail.idCARRDetail && x.idDocument == id).FirstOrDefault();
                }
                catch (System.Exception ex)
                {
                    //error handling
                }
            }

            ViewBag.MyDocument = MyDocument;
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyCarrDocument = MyCarrDocument;

            ViewBag.Title = "Legal: " + MyDocument.Name;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult Save()
        {
            JsonResult json = new JsonResult();
            string username = Session["accountname"].ToString();

            int idDocument = int.Parse(Request.Form["iddocument"]);
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            string docdata = Request.Unvalidated.Form["DocData"];

            CARRDetail MyCarrDetails = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).First();
            CARR MyCarr = db.CARRs.Find(MyCarrDetails.idCARR);
            CARRDocument MyCarrDocument = db.CARRDocuments.Where(x => x.idCARRDetail == idCARRDetail && x.idDocument == idDocument).FirstOrDefault();

            MyCarrDocument.idCARRDetail = MyCarrDetails.idCARRDetail;
            MyCarrDocument.idDocument = idDocument;
            MyCarrDocument.UpdatedBy = username;
            MyCarrDocument.UpdatedOn = DateTime.Now;
            MyCarrDocument.DocContent = docdata;

            try
            {                
                db.SaveChanges();
                json.Data = new { success = true };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString() };
            }
            return json;
        }

    }
}