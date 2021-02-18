using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Portal.Classes;
using PI_Portal.Models;

namespace PI_Portal.Controllers
{
    public class AdminController : Controller
    {
        private Entities db = new Entities();

        [HttpGet]
        [Authorize]     //[Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ViewBag.title = "Admin";
            return View();
        }

        [HttpGet]
        [Authorize]     //[Authorize(Roles = "Admin")]
        public ActionResult UserMaintenance()
        {

            ViewBag.Title = "User Maintenance";

            string sql = @" select aur.id,
                              aur.idApplication,
                              aur.ActiveDirectory,
                              ar.Role, 
                              aur.ActiveFlag
                          FROM COM.applicationUserRoles aur
                          join COM.applicationRoles ar on ar.id = aur.idRole 
                          WHERE aur.idApplication = 1 and aur.ActiveFlag = 1
                          order by aur.ActiveDirectory";
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, connection);
            da.Fill(dt);
            connection.Close();
            connection.Dispose();
            ViewBag.result = dt;

            List<SelectListItem> userRoleList = HelperDropdowns.getUserRoles();
            ViewBag.userRoleList = userRoleList;

            string userrole = Session["userrole"].ToString();
            ViewBag.userrole = userrole;

            return View();
        }

        [HttpPost]
        [Authorize]     //[Authorize(Roles = "Admin")]
        public ActionResult Legal(int id)
        {
            var MyDocument = db.Documents.Where(x => x.idDocument == id).FirstOrDefault();
            ViewBag.MyDocument = MyDocument;

            ViewBag.Title = "Legal Default: " + MyDocument.Name;
            return View();
        }
        [HttpPost]
        [Authorize]     //[Authorize(Roles = "Admin")]
        public ActionResult SaveDocuments()
        {
            JsonResult json = new JsonResult();
            string username = Session["accountname"].ToString();

            int iddocument = int.Parse(Request.Form["idDocument"]);
            string docdata = Request.Unvalidated.Form["DocData"];

            Document MyDocument = db.Documents.Where(x => x.idDocument == iddocument).FirstOrDefault();
            
            MyDocument.UpdatedBy = username;
            MyDocument.UpdatedOn = DateTime.Now;
            MyDocument.DefaultContent = docdata;

            try
            {
                db.SaveChanges();
                json.Data = new { success = true };
            }
            catch (System.Exception e)
            {
                json.Data = new { success = false, error = e.Message.ToString() };
            }

            return json;
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult EditAccess()
        {
            string username = Request.Form["username"].Trim();

            var aur = db.applicationUserRoles.Where(x => x.ActiveDirectory.Equals(username)).FirstOrDefault();
            List<SelectListItem> districts = HelperDropdowns.getDistrictList();
            List<SelectListItem> regions = HelperDropdowns.getRegionList();
            List<SelectListItem> users = HelperDropdowns.getUserList();

            var previouslySelectedDistricts = db.ApplicationDistrictsAlloweds.Where(x => x.ActiveDirectory.Equals(username)).ToList();
            var previouslySelectedBranches = db.ApplicationRegionsAlloweds.Where(x => x.ActiveDirectory.Equals(username)).ToList();
            var previouslySelectDR = db.ApplicationUsersAlloweds.Where(x => x.ActiveDirectory.Equals(username)).ToList();
            var signature = db.Signatures.Where(x => x.ActiveDirectory.Equals(username)).FirstOrDefault();

            List<SelectListItem> userRoleList = HelperDropdowns.getUserRoles();
            ViewBag.userRoleList = userRoleList;
           
            ViewBag.applicationUserRoles = aur;
            ViewBag.districtList = districts;
            ViewBag.regionList = regions;
            ViewBag.userList = users;
            ViewBag.previouslySelectedDistricts = previouslySelectedDistricts;
            ViewBag.previouslySelectedBranches = previouslySelectedBranches;
            ViewBag.previouslySelectDR = previouslySelectDR;
            ViewBag.signature = signature != null ? "../" + signature.ImageURL.Trim().Replace("\\", "/") : "";
            ViewBag.Title = "Edit Access";
            return View(aur);
        }
    }
}
