using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Portal.Models;
using PI_Portal.Classes;

namespace PI_Portal.Controllers
{
    public class AccountCreationController : Controller
    {

        private Entities db = new Entities();

        // GET: AccountCreation
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Accounts()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);

            ViewBag.Title = "Account Creation";

            string sql = @"	select cd.idCARRDetail,
                                    l.idLocation,
			                        l.LocationName as LocationName,
                                    l.CompanyName,
									Concat(ip.code,' - ',ip.Description) as Induction,
			                        ss.Description,
                                    s.idService,
			                        s.AccountNumber,
                                    s.AccountName,
									s.PINPrefix,									
									dd2.Name 'ShippingSystem',									
			                        c.FirstName,
			                        c.LastName
	                        FROM [PURO_APPS].[CAR].[CARRDetail] cd
	                        left outer join CAR.CARRLocation l on l.idCARRDetail = cd.idCARRDetail
	                        left outer join CAR.Service s on s.idLocation = l.idLocation
	                        left outer join CAR.ShippingServices ss on ss.idShippingService = s.idShippingService
	                        left outer join CAR.Contacts c on c.idContact = s.idBillingContact
							left outer join CAR.DropdownsOptions dd2 on Convert(varchar(10),dd2.value)= s.ShippingSystem
							left outer join CAR.ServiceVolume sv on sv.idService = s.idService
							left outer join CAR.InductionPoints ip on ip.idInduction = sv.idInduction
	                        where s.ActiveFlag = 1 and l.ActiveFlag = 1 and cd.idCARRDetail = ";
            sql = sql + idCARRDetail;
            sql = sql + " group by cd.idCARRDetail,l.idLocation,l.LocationName,l.CompanyName,ip.code,ip.Description,ss.Description,s.idService,s.AccountNumber,s.AccountName,s.PINPrefix,dd2.Name,c.FirstName,c.LastName";
            sql = sql + " order by l.LocationName";
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, connection);
            da.Fill(dt);
            connection.Close();
            connection.Dispose();
            ViewBag.result = dt;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }

            //GET REQ FIelds Missing for Info BAR
            var controller = DependencyResolver.Current.GetService<ContractController>();
            controller.ControllerContext = new ControllerContext(this.Request.RequestContext, controller);
            //Determine what is still required
            ViewBag.SubmitDisable = false;
            List<string> reqlist = controller.CheckAllRequiredAccountFields(idCARRDetail);
            //Requirements needed
            bool requirementsFlag = reqlist.Count > 0 ? true : false;
            if (reqlist.Count > 0)
            {
                ViewBag.SubmitDisable = true;
            }
            string ReqHdr = controller.CheckAccountRequiredHdr(reqlist);
            ViewBag.ReqHdr = ReqHdr;

            ViewBag.MyReqlist = reqlist;

            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            //AccountCreationFlag
            bool AcctCreation = false;
            if (MyCarrDetail.idCarrType == 5 )
            {
                AcctCreation = true;
            }
            ViewBag.AcctCreation = AcctCreation;


            bool DisableChanges = controller.disableChangesAcctCreation(idCARRDetail);
            ViewBag.DisableChanges = DisableChanges;

            return View();

        }

        [HttpPost]
        [Authorize]
        public ActionResult EditAccounts()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            int idService = int.Parse(Request.Form["idservice"]);
            int idLocation = int.Parse(Request.Form["idlocation"]);

            ViewBag.Title = "Contracts - Edit Account";

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            var MyLocation = db.CARRLocations.Where(x => x.idLocation == idLocation).FirstOrDefault();
            var MyService = db.Services.Where(x => x.idService == idService).FirstOrDefault();
            var MyLocationContact = db.Contacts.Where(x => x.idContact == MyLocation.idBranchContact).FirstOrDefault();

            //var MyBillingOption = db.DropdownsOptions.Where(x => x.Value == MyService.idBillingOption.ToString()).FirstOrDefault();
            //ViewBag.billingoptionselected = MyBillingOption.Name;

            var MyPrimary =
                from contact in db.Contacts
                join service in db.Services.Where(a => a.ActiveFlag == true && a.primaryBillingFlag == true) on contact.idContact equals service.idBillingContact
                join location in db.CARRLocations.Where(b => b.ActiveFlag == true) on service.idLocation equals location.idLocation
                join detail in db.CARRDetails.Where(c => c.idCARRDetail == MyCarrDetail.idCARRDetail && c.ActiveFlag == true) on location.idCARRDetail equals detail.idCARRDetail

                into primarycontact

                from pc in primarycontact.DefaultIfEmpty()

                select new
                {
                    contact.idContact,
                    contact.FirstName,
                    contact.LastName,
                    contact.Title,
                    contact.PhoneNumber,
                    contact.Address1,
                    contact.Address2,
                    contact.City,
                    contact.State,
                    contact.PostalCode,
                    contact.Country
                };

            Contact tempcontact = new Contact();
            tempcontact.idContact = 0;
            tempcontact.FirstName = "";
            tempcontact.LastName = "";
            tempcontact.Title = "";
            tempcontact.PhoneNumber = "";
            tempcontact.Address1 = "";
            tempcontact.Address2 = "";
            tempcontact.City = "";
            tempcontact.State = "";
            tempcontact.PostalCode = "";
            tempcontact.Country = "";

            if (MyPrimary != null)
            {
                foreach (var p in MyPrimary)
                {
                    if (p.FirstName != null)
                        tempcontact.FirstName = p.FirstName;
                    if (p.LastName != null)
                        tempcontact.LastName = p.LastName;
                    if (p.Title != null)
                        tempcontact.Title = p.Title;
                    if (p.PhoneNumber != null)
                        tempcontact.PhoneNumber = p.PhoneNumber;
                    if (p.Address1 != null)
                        tempcontact.Address1 = p.Address1;
                    if (p.Address2 != null)
                        tempcontact.Address2 = p.Address2;
                    if (p.City != null)
                        tempcontact.City = p.City;
                    if (p.State != null)
                        tempcontact.State = p.State;
                    if (p.PostalCode != null)
                        tempcontact.PostalCode = p.PostalCode;
                    if (p.Country != null)
                        tempcontact.Country = p.Country;

                }
            }
            ViewBag.MyPrimary = tempcontact;


           

            //YesNo Flags, Default to false 
            ViewBag.freightauditorselected = "0";
            if (MyService.freightAuditorFlag == true)
                ViewBag.freightauditorselected = "1";
            ViewBag.combinebillingselected = "0";
            if (MyService.combineBillingFlag == true)
                ViewBag.combinebillingselected = "1";
            ViewBag.separatebillingselected = "0";
            if (MyService.separateBillingFlag == true)
                ViewBag.separatebillingselected = "1";
            ViewBag.primarybillingselected = "0";
            if (MyService.primaryBillingFlag == true)
                ViewBag.primarybillingselected = "1";

            List<SelectListItem> statelist = HelperDropdowns.getStateList();
            ViewBag.stateList = statelist;

            List<SelectListItem> localSRIDList = HelperDropdowns.getLocalSRIDList();
            List<SelectListItem> strategicSRIDList = HelperDropdowns.getStrategicSRIDList();
            ViewBag.localsridList = localSRIDList;
            ViewBag.strategicsridList = strategicSRIDList;
            

            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyLocation = MyLocation;
            ViewBag.MyService = MyService;
            ViewBag.MyLocationContact = MyLocationContact;
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }
            var controller = DependencyResolver.Current.GetService<ContractController>();
            controller.ControllerContext = new ControllerContext(this.Request.RequestContext, controller);

            bool DisableChanges = controller.disableChangesAcctCreation(idCARRDetail);
            ViewBag.DisableChanges = DisableChanges;
            return View();


        }

        [HttpPost]
        [Authorize]
        public ActionResult SubmitContracts()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
                        
            ViewBag.Title = "Submit to Contracts";

            string userName = Session["accountname"].ToString();
                      

            var controller = DependencyResolver.Current.GetService<ContractController>();
            controller.ControllerContext = new ControllerContext(this.Request.RequestContext, controller);
            //Determine what is still required
            ViewBag.SubmitDisable = false;
            List<string> reqlist = controller.CheckAllRequiredAccountFields(idCARRDetail);
            //Requirements needed
            bool requirementsFlag = reqlist.Count > 0 ? true : false;
            if (reqlist.Count > 0)
            {
                ViewBag.SubmitDisable = true;
            }
            string ReqHdr = controller.CheckAccountRequiredHdr(reqlist);
            ViewBag.ReqHdr = ReqHdr;

            ViewBag.MyReqlist = reqlist;
            

            //Details used in Partial Info Bar                
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            ViewBag.idCARRDetail = idCARRDetail;
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
                        
            //Get Approval List - goes to DFL           
            ViewBag.userList = HelperDropdowns.getDFLUserList();
            


            return View();

        }

        [HttpPost]
        [Authorize]
        public ActionResult RequestVDA()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);

            ViewBag.Title = "Request VDA";

            string userName = Session["accountname"].ToString();

            var controller = DependencyResolver.Current.GetService<ContractController>();
            controller.ControllerContext = new ControllerContext(this.Request.RequestContext, controller);

            ViewBag.SubmitDisable = false;


            //Details used in Partial Info Bar                
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            ViewBag.idCARRDetail = idCARRDetail;
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            //Get Approval List - goes to DFL           
            ViewBag.userList = HelperDropdowns.getContractsUserList();
            

            return View();

        }


    }
}