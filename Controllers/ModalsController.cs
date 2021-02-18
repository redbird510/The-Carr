using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Portal.Models;
using System.Data.SqlClient;
using System.Data;

namespace PI_Portal.Controllers
{
    public class ModalsController : Controller
    {
         private Entities db = new Entities();

        [HttpPost]
        [Authorize]
        public ActionResult GetContractVersions(int id)
        {
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == id).FirstOrDefault();
            var MyCarrDetails = db.CARRDetails.Where(x => x.idCARR == MyCarrDetail.idCARR).ToList();
            ViewBag.MyCarrDetails = MyCarrDetails;

            return PartialView();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateNewCarr()
        {
            ViewBag.Title = "New CARR Creation";

            return PartialView();
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateNewCarr([Bind(Include = "CustomerLegalName")] CARR carr)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "New CARR Creation";
            if (ModelState.IsValid)
            {
               
                if (string.IsNullOrEmpty(carr.CustomerLegalName)) {
                    ModelState.AddModelError("CustomerLegalName", "Name Required");
                    json.Data = new { success = false, error = "Name Required", html = RenderPartialViewToString("CreateNewCarr", carr) };
                } 
                else
                {
                    string username = Session["accountname"].ToString();

                    carr.ActiveFlag = true;
                    carr.CreatedBy = username;
                    carr.CreatedOn = DateTime.Now;
                    

                    db.CARRs.Add(carr);

                    //Create Placeholder Corporate Contact
                    Contact contact = new Contact();
                    contact.CreatedBy = username;
                    contact.CreatedOn = DateTime.Now;
                    contact.ActiveFlag = true;
                    db.Contacts.Add(contact);

                    //Create Placeholder Dangerous Goods Contact
                    DGContact dgcontact = new DGContact();
                    dgcontact.CreatedBy = username;
                    dgcontact.CreatedOn = DateTime.Now;
                    dgcontact.ActiveFlag = true;
                    db.DGContacts.Add(dgcontact);

                    //Create Detail Row
                    CARRDetail carrdetail = new CARRDetail();
                    carrdetail.idCARR = carr.idCarr;
                    carrdetail.idCorporateContact = contact.idContact;
                    carrdetail.idDGContact = dgcontact.idDGContact;
                    //always start with version 1
                    carrdetail.VersionNumber = 1;
                    //carrdetail.VersionComment = "Original Version";
                    carrdetail.VersionComment = "";
                    carrdetail.CompletedFlag = false;
                    carrdetail.ActiveFlag = true;
                    carrdetail.CreatedBy = username;
                    carrdetail.CreatedOn = DateTime.Now;
                    carrdetail.returnsFlag = true;
                    carrdetail.SalesProfessional = username;


                    db.CARRDetails.Add(carrdetail);
                    db.SaveChanges();

                    //Create RoutedTo Row
                    Routing routedto = new Routing();
                    routedto.RoutedTo = username;
                    routedto.CreatedBy = username;
                    routedto.CreatedOn = DateTime.Now;
                    routedto.DateRoutedTo = DateTime.Now;
                    routedto.idCARRDetail = carrdetail.idCARRDetail;
                    db.Routings.Add(routedto);
                    carrdetail.idRoutedTo = routedto.idRouting;
                    db.SaveChanges();

                    //Accessorials now begin exported to US Pricing Export
                    //Generate Accessorials for this CARR
                    //SqlConnection cnn;
                    //String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
                    //cnn = new SqlConnection(connectionString);
                    //cnn.Open();
                    //SqlCommand cmd = new SqlCommand();
                    //try
                    //{
                    //    cmd = new SqlCommand("sp_GenerateCARRAccessorials", cnn);
                    //    cmd.CommandType = CommandType.StoredProcedure;
                    //    cmd.Parameters.Add(new SqlParameter("@idCARRDetail", carrdetail.idCARRDetail));
                    //    cmd.Parameters.Add(new SqlParameter("@username", username));
                    //    cmd.ExecuteNonQuery();

                    //}
                    //catch (System.Exception ex)
                    //{
                    //    string errMsg = ex.Message.ToString();
                    //}
                    //finally
                    //{
                    //    cnn.Close();
                    //    cnn.Dispose();
                    //}

                    var defaultDimFactor = db.KeyValuePairs.Where(x => x.Key == "ReturnsDimFactor").FirstOrDefault();
                    float dimfactor = Convert.ToSingle(defaultDimFactor.Value);

                    //Create Returns LHRF Form
                    CARRLHRF returnsform = new CARRLHRF();
                    returnsform.CreatedBy = username;
                    returnsform.CreatedOn = DateTime.Now;
                    returnsform.ActiveFlag = true;
                    returnsform.idCARRDetail = carrdetail.idCARRDetail;
                    returnsform.LHRFType = "Returns";
                    returnsform.ReturnsDimFactor = dimfactor;
                    db.CARRLHRFs.Add(returnsform);
                    db.SaveChanges();

                   

                    try
                    {
                        db.SaveChanges();
                        json.Data = new { success = true, idcarrdetail = carrdetail.idCARRDetail };
                    }
                    catch (System.Exception ex)
                    {
                        json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("CreateNewCarr", carr) };
                    }
                }
            }
            else
            {
                PartialViewResult view = PartialView(carr);
                json.Data = new { success = false, error = "Invalid Input", html = RenderPartialViewToString("CreateNewCarr", carr) };
            }                      
            return json;
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateNewLocation(int id)
        {
            int idCARRDetail = id;
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            CARRLocation carrlocation = new CARRLocation();
            carrlocation.idCARRDetail = idCARRDetail;

            ViewBag.Title = "Add New Location";
            return PartialView(carrlocation);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewLocation([Bind(Include = "LocationName,idCARRDetail")] CARRLocation carrlocation)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Add New Location";
            if (ModelState.IsValid)
            {
                if (db.CARRLocations.Any(x => x.LocationName.Equals(carrlocation.LocationName) && x.idCARRDetail == carrlocation.idCARRDetail && carrlocation.ActiveFlag == true))
                {
                    //MK TODO - getting error when trying to return the error message to the modal pop up
                    ModelState.AddModelError("LocationName", "Name already Exists");
                    json.Data = new { success = false, error = "Already Exists", html = RenderPartialViewToString("CreateNewLocation", carrlocation) };
                }
                else if (db.CARRLocations.Any(x => x.LocationName.Equals(carrlocation.LocationName) && x.idCARRDetail == carrlocation.idCARRDetail && x.ActiveFlag == true))
                {
                    ModelState.AddModelError("LocationName", "Please enter a unique Location Name");
                    json.Data = new { success = false, error = "Already Exists", html = RenderPartialViewToString("CreateNewLocation", carrlocation) };

                }
                else if (string.IsNullOrEmpty(carrlocation.LocationName))
                {
                    ModelState.AddModelError("LocationName", "Name Required");
                    json.Data = new { success = false, error = "Name Required", html = RenderPartialViewToString("CreateNewLocation", carrlocation) };
                }
                else
                {
                    string username = Session["accountname"].ToString();
                    var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == carrlocation.idCARRDetail).FirstOrDefault();
                    var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

                    
                    //Create Placeholder Location Contact
                    Contact contact = new Contact();
                    contact.CreatedBy = username;
                    contact.CreatedOn = DateTime.Now;
                    contact.ActiveFlag = true;
                    db.Contacts.Add(contact);

                    carrlocation.ActiveFlag = true;
                    carrlocation.CreatedBy = username;
                    carrlocation.CreatedOn = DateTime.Now;                    
                    carrlocation.idBranchContact = contact.idContact;
                    //default company name to the corporate name
                    carrlocation.CompanyName = MyCarr.CustomerLegalName;

                    db.CARRLocations.Add(carrlocation);

                    db.SaveChanges();


                    var idCARR = MyCarrDetail.idCARR;
                    var version = MyCarrDetail.VersionNumber;

                    //Details used in Partial Info Bar
                    ViewBag.MyCarr = MyCarr;
                    ViewBag.MyCarrDetail = MyCarrDetail;
                    ViewBag.MyLocation = carrlocation;

                    try
                    {
                        db.SaveChanges();
                        json.Data = new { success = true, idlocation = carrlocation.idLocation, idcarrdetail = MyCarrDetail.idCARRDetail };
                    }
                    catch (System.Exception ex)
                    {
                        json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("CreateNewLocation", carrlocation) };
                    }
                    // return RedirectToAction("Location", "Contract", new { @idloc = carrlocation.idLocation });
                }
            }
            else
            {
                PartialViewResult view = PartialView(carrlocation);
                json.Data = new { success = false, error = "Invalid Input", html = RenderPartialViewToString("CreateNewLocation", carrlocation) };
            }
            return json;            
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateNewService(int id)
        {

            var MyLocation = db.CARRLocations.Where(x => x.idLocation == id).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyLocation = MyLocation;


            Service service = new Service();
            service.idLocation = id;

           List<ShippingService>  svcdd = new List<ShippingService>();
            if (MyCarrDetail.idCarrType == 5)
            {
                  svcdd = (List<ShippingService>)db.ShippingServices.Where(x => x.ActiveFlag == true).OrderBy(x => x.Description).ToList();
            }
            else
            {
                  svcdd = (List<ShippingService>)db.ShippingServices.Where(x => x.ActiveFlag == true).Where(x => x.AccountCreationOnly != true).OrderBy(x => x.Description).ToList();
            }
           
            

            ViewBag.servicelist = svcdd.AsEnumerable()
                .Select(x => new SelectListItem
                {
                    Value = x.idShippingService.ToString(),
                    Text = x.Description.ToString()
                });

            return PartialView(service);

        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateNewService(FormCollection objfrm, [Bind(Include = "idLocation")] Service service)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Add New Service";
            if (ModelState.IsValid)
            {
                int selectedservice = Convert.ToInt32(objfrm["servicelist"]);
                service.idShippingService = selectedservice;

                string username = Session["accountname"].ToString();

                service.ActiveFlag = true;
                service.CreatedBy = username;
                service.CreatedOn = DateTime.Now;

                db.Services.Add(service);

                //mk
                //Create Placeholder Billing Contact For Service                
                Contact contact = new Contact();
                contact.CreatedBy = username;
                contact.CreatedOn = DateTime.Now;
                contact.ActiveFlag = true;
              

                db.Contacts.Add(contact);
                db.SaveChanges();
                service.idBillingContact = contact.idContact;
   

                var MyLocation = db.CARRLocations.Where(x => x.idLocation == service.idLocation).FirstOrDefault();
                var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail).FirstOrDefault();
                var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
                var idLocation = service.idLocation;


                //Details used in Partial Info Bar
                ViewBag.MyCarr = MyCarr;
                ViewBag.MyCarrDetail = MyCarrDetail;
                ViewBag.MyLocation = MyLocation;
                ViewBag.MyService = service;

                try
                {
                    db.SaveChanges();
                    json.Data = new { success = true, idservice = service.idService, idcarrdetail = MyCarrDetail.idCARRDetail };
                }
                catch (System.Exception ex)
                {
                    json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("CreateNewService", service) };
                }

                //return RedirectToAction("Locations", "Contract", new { @id = MyCarrDetail.idCARR, @ver = MyCarrDetail.VersionNumber });
            }
            else
            {
                PartialViewResult view = PartialView(service);
                json.Data = new { success = false, error = "Invalid Input", html = RenderPartialViewToString("CreateNewService", service) };
            }
            return json;

            //return PartialView(service);
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateNewAccountService(int id)
        {

            var MyLocation = db.CARRLocations.Where(x => x.idLocation == id).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyLocation = MyLocation;


            Service service = new Service();
            service.idLocation = id;

            List<ShippingService> svcdd = new List<ShippingService>();
            if (MyCarrDetail.idCarrType == 5)
            {
                svcdd = (List<ShippingService>)db.ShippingServices.Where(x => x.ActiveFlag == true).OrderBy(x => x.Description).ToList();
            }
            else
            {
                svcdd = (List<ShippingService>)db.ShippingServices.Where(x => x.ActiveFlag == true).Where(x => x.AccountCreationOnly != true).OrderBy(x => x.Description).ToList();
            }



            ViewBag.servicelist = svcdd.AsEnumerable()
                .Select(x => new SelectListItem
                {
                    Value = x.idShippingService.ToString(),
                    Text = x.Description.ToString()
                });

            return PartialView(service);

        }

        public ActionResult CreateNewAccountService(FormCollection objfrm, [Bind(Include = "idLocation")] Service service)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Add  Service for Account";
            if (ModelState.IsValid)
            {
                int selectedservice = Convert.ToInt32(objfrm["servicelist"]);
                service.idShippingService = selectedservice;

                string username = Session["accountname"].ToString();

                service.ActiveFlag = true;
                service.CreatedBy = username;
                service.CreatedOn = DateTime.Now;

                db.Services.Add(service);

                //mk
                //Create Placeholder Billing Contact For Service                
                Contact contact = new Contact();
                contact.CreatedBy = username;
                contact.CreatedOn = DateTime.Now;
                contact.ActiveFlag = true;


                db.Contacts.Add(contact);
                db.SaveChanges();
                service.idBillingContact = contact.idContact;


                var MyLocation = db.CARRLocations.Where(x => x.idLocation == service.idLocation).FirstOrDefault();
                var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail).FirstOrDefault();
                var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
                var idLocation = service.idLocation;


                //Details used in Partial Info Bar
                ViewBag.MyCarr = MyCarr;
                ViewBag.MyCarrDetail = MyCarrDetail;
                ViewBag.MyLocation = MyLocation;
                ViewBag.MyService = service;

                try
                {
                    db.SaveChanges();
                    json.Data = new { success = true, idservice = service.idService, idcarrdetail = MyCarrDetail.idCARRDetail };
                }
                catch (System.Exception ex)
                {
                    json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("CreateNewService", service) };
                }

                //return RedirectToAction("Locations", "Contract", new { @id = MyCarrDetail.idCARR, @ver = MyCarrDetail.VersionNumber });
            }
            else
            {
                PartialViewResult view = PartialView(service);
                json.Data = new { success = false, error = "Invalid Input", html = RenderPartialViewToString("CreateNewService", service) };
            }
            return json;

            //return PartialView(service);
        }


        [HttpGet]
        [Authorize]
        public ActionResult CreateNewVolume(int id)
        {
           
                var MySerivce = db.Services.Where(x => x.idService == id).FirstOrDefault();
                var MyLocation = db.CARRLocations.Where(x => x.idLocation == MySerivce.idLocation).FirstOrDefault();
                var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail).FirstOrDefault();
                var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
                ViewBag.MyCarr = MyCarr;
                ViewBag.MyCarrDetail = MyCarrDetail;
                ViewBag.MyLocation = MyLocation;
                ViewBag.MyService = MySerivce;


                ServiceVolume volume = new ServiceVolume();
                volume.idService = id;


            //var inductiondd = db.DropdownsOptions.Where(x => x.id == 8);
            var inductiondd = db.InductionPoints.OrderByDescending(x => x.code);

                ViewBag.inductionlist = inductiondd.AsEnumerable()
                    .Select(x => new SelectListItem
                    {
                        Value = x.idInduction.ToString(),
                        Text = x.code + " - " + x.Description.ToString()
                    });
            
          
               return PartialView(volume);

        }


        [HttpPost]
        [Authorize]
        public ActionResult CreateNewVolume(FormCollection objfrm, [Bind(Include = "idService")] ServiceVolume volume)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Add New Volume";
            if (ModelState.IsValid)
            {
                string selectedinduction = objfrm["inductionlist"];
                volume.idInduction = Convert.ToInt32(selectedinduction);

                string username = Session["accountname"].ToString();

                volume.ActiveFlag = true;
                volume.CreatedBy = username;
                volume.CreatedOn = DateTime.Now;
                

                

                var MyService = db.Services.Where(x => x.idService == volume.idService).First();
                var MyLocation = db.CARRLocations.Where(x => x.idLocation == MyService.idLocation).FirstOrDefault();
                var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail).FirstOrDefault();
                var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
                var idVolume = volume.idVolume;
                volume.DimFactor = MyService.ShippingService.DimFactor;

                //Details used in Partial Info Bar
                ViewBag.MyCarr = MyCarr;
                ViewBag.MyCarrDetail = MyCarrDetail;
                ViewBag.MyLocation = MyLocation;
                ViewBag.MyService = MyService;

                db.ServiceVolumes.Add(volume);

                // db.SaveChanges();
                try
                {
                    db.SaveChanges();
                    json.Data = new { success = true, idvolume = volume.idVolume, idcarrdetail = MyCarrDetail.idCARRDetail };
                }
                catch (System.Exception ex)
                {
                    json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("CreateNewVolume", volume) };
                }                        

            }
            else
            {
                PartialViewResult view = PartialView(volume);
                json.Data = new { success = false, error = "Invalid Input", html = RenderPartialViewToString("CreateNewVolume", volume) };
            }
            return json;
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateNewException(int id)
        {
            int idCARRDetail = id;
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            Models.Exception newException = new Models.Exception();
            newException.idCARRDetail = idCARRDetail;

            ViewBag.Title = "Add New Exception";
            return PartialView(newException);
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateNewException(FormCollection objfrm, [Bind(Include = "idCARRDetail")] Models.Exception exception)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Add New Exception";

            String selectedApprovalType = objfrm["approvaltypes"];
            String enteredDesc = objfrm["Description"];
            String enteredComment = objfrm["ApprovalComment"];

            if (string.IsNullOrEmpty(enteredDesc))
            {
                ModelState.AddModelError("Description", "Issue Required");
                json.Data = new { success = false, error = "Issue Required", html = RenderPartialViewToString("CreateNewException", exception) };
            }
            else if (ModelState.IsValid)
            {
                string username = Session["accountname"].ToString();
                var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == exception.idCARRDetail).FirstOrDefault();
                var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
                var idException = exception.idException;

                exception.Description = enteredDesc;
                exception.ApprovalType = "NotYetAddressed";
                exception.ActiveFlag = true;
                exception.CreatedBy = username;
                exception.CreatedOn = DateTime.Now;
                exception.VersionNumber = MyCarrDetail.VersionNumber;

                db.Exceptions.Add(exception);

                //Details used in Partial Info Bar
                ViewBag.MyCarr = MyCarr;
                ViewBag.MyCarrDetail = MyCarrDetail;

                try
                {
                    db.SaveChanges();
                    json.Data = new { success = true, idexception = exception.idException, idcarrdetail = MyCarrDetail.idCARRDetail };
                }
                catch (System.Exception ex)
                {
                    json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("CreateNewException", exception) };
                }

            }
            else
            {
                PartialViewResult view = PartialView(exception);
                json.Data = new { success = false, error = "Invalid Input", html = RenderPartialViewToString("CreateNewException", exception) };
            }
            return json;

        }

        [HttpGet]
        [Authorize]
        public ActionResult EditException(int id, int idexception)
        {
            int idCARRDetail = id;
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            var exception = db.Exceptions.Where(x => x.idException == idexception).FirstOrDefault();

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            Models.Exception newException = new Models.Exception();
            newException.idCARRDetail = idCARRDetail;

            var approvalTypes = db.DropdownsOptions.Where(x => x.id == 14);

            ViewBag.approvaltypes = approvalTypes.AsEnumerable()
                .Select(x => new SelectListItem
                {
                    Value = x.Value.ToString(),
                    Text = x.Name.ToString(),
                    Selected = x.Value.Equals(exception.ApprovalType) ? true : false
                });

            ViewBag.Exception = exception;
            ViewBag.Title = "Edit Exception";
            return PartialView(exception);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditException(FormCollection objfrm, [Bind(Include = "idCARRDetail,idException,CreatedBy")] Models.Exception exception)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Edit Exception";

            String selectedApprovalType = objfrm["approvaltypes"];
            String enteredDesc = objfrm["Description"];
            String enteredComment = objfrm["ApprovalComment"];
            String approvalDate = objfrm["approvalDate"];

            string username = Session["accountname"].ToString();
            string userrole = Session["userrole"].ToString();
            var editException = db.Exceptions.Find(exception.idException);

            if (username.Equals(editException.CreatedBy) && string.IsNullOrEmpty(enteredDesc))
            {
                var approvalTypes = db.DropdownsOptions.Where(x => x.id == 14);

                ViewBag.approvaltypes = approvalTypes.AsEnumerable()
                    .Select(x => new SelectListItem
                    {
                        Value = x.Value.ToString(),
                        Text = x.Name.ToString(),
                        Selected = x.Value.Equals(selectedApprovalType) ? true : false
                    });

                ModelState.AddModelError("Description", "Issue Required");
                json.Data = new { success = false, error = "Issue Required", html = RenderPartialViewToString("EditException", exception) };
            }
            else if (ModelState.IsValid)
            {
                var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == exception.idCARRDetail).FirstOrDefault();
                var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

                editException.Description = enteredDesc != null ? enteredDesc : editException.Description;
                editException.ApprovalType = selectedApprovalType == null ? "NotYetAddressed" : selectedApprovalType;
                editException.ApprovalComment = enteredComment;
                editException.ActiveFlag = true;
                editException.UpdatedBy = username;
                editException.UpdatedOn = DateTime.Now;

                editException.ApprovedBy = userrole == "Pricing" && !selectedApprovalType.Equals("NotYetAddressed") ? username : null;
                
                if (!String.IsNullOrEmpty(approvalDate) && !selectedApprovalType.Equals("NotYetAddressed"))
                    editException.ApprovalDate = Convert.ToDateTime(approvalDate);
                else if (!String.IsNullOrEmpty(selectedApprovalType) && !selectedApprovalType.Equals("NotYetAddressed"))
                    editException.ApprovalDate = DateTime.Now;

                //Details used in Partial Info Bar
                ViewBag.MyCarr = MyCarr;
                ViewBag.MyCarrDetail = MyCarrDetail;

                try
                {
                    db.SaveChanges();
                    json.Data = new { success = true, idexception = exception.idException, idcarrdetail = MyCarrDetail.idCARRDetail };
                }
                catch (System.Exception ex)
                {
                    json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("EditException", exception) };
                }

            }
            else
            {
                PartialViewResult view = PartialView(exception);
                json.Data = new { success = false, error = "Invalid Input", html = RenderPartialViewToString("EditException", exception) };
            }
            return json;

        }


        [HttpGet]
        [Authorize]
        public ActionResult CreateNewBrokerage(int id)
        {
            int idCARRDetail = id;
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            CARRBrokerage newBrokerage = new CARRBrokerage();
            newBrokerage.idCARRDetail = idCARRDetail;

            var existingBrokerages = db.CARRBrokerages.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).Select(x => x.Description).ToList();
            var brokerageTypes = db.BrokerageFees.Select(x => x.Description).Except(existingBrokerages);

            ViewBag.BrokerageTypes = brokerageTypes.AsEnumerable()
                .Select(x => new SelectListItem
                {
                    Value = x.ToString(),
                    Text = x.ToString()
                });

            ViewBag.Title = "Add New Brokerage";
            return PartialView(newBrokerage);
        }


        [HttpPost]
        [Authorize]
        public ActionResult CreateNewBrokerage(FormCollection objfrm, [Bind(Include = "idCARRDetail")] CARRBrokerage brokerage)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Add New Brokerage";

            if (ModelState.IsValid)
            {
                string username = Session["accountname"].ToString();
                string brokerageType = objfrm["BrokerageTypes"];

                var brokerageFees = db.BrokerageFees.Where(x => x.Description.Equals(brokerageType)).FirstOrDefault();

                brokerage.Description = brokerageFees.Description;
                brokerage.Fee = brokerageFees.Fee;
                brokerage.TierAddlFee = brokerageFees.TierAddlFee;
                brokerage.MaxCharge = brokerageFees.MaxCharge;
                brokerage.Surcharge = brokerageFees.Surcharge;
                brokerage.MinCharge = brokerageFees.MinCharge;
                brokerage.ActiveFlag = true;
                brokerage.CreatedBy = username;
                brokerage.CreatedOn = DateTime.Now;

                db.CARRBrokerages.Add(brokerage);

                var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == brokerage.idCARRDetail).FirstOrDefault();
                var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
                var idBrokerage = brokerage.idBrokerage;

                //Details used in Partial Info Bar
                ViewBag.MyCarr = MyCarr;
                ViewBag.MyCarrDetail = MyCarrDetail;

                try
                {
                    db.SaveChanges();
                    json.Data = new { success = true, idbrokerage = brokerage.idBrokerage, idcarrdetail = MyCarrDetail.idCARRDetail };
                }
                catch (System.Exception ex)
                {
                    json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("CreateNewBrokerage", brokerage) };
                }

            }
            else
            {
                PartialViewResult view = PartialView(brokerage);
                json.Data = new { success = false, error = "Invalid Input", html = RenderPartialViewToString("CreateNewBrokerage", brokerage) };
            }
            return json;

        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateNewNote(int id)
        {
            int idCARRDetail = id;
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            Note newNote = new Note();
            newNote.idCARRDetail = idCARRDetail;

            ViewBag.Title = "Add New Note";
            return PartialView(newNote);
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateNewNote(FormCollection objfrm, [Bind(Include = "idCARRDetail")] Note note)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Add New Note";

            String enteredNote = objfrm["Note1"];
            if (string.IsNullOrEmpty(enteredNote))
            {
                ModelState.AddModelError("Note1", "Note Required");
                json.Data = new { success = false, error = "Name Required", html = RenderPartialViewToString("CreateNewNote", note) };
            }
            else if (ModelState.IsValid)
            {
                string username = Session["accountname"].ToString();

                note.Note1 = enteredNote;
                note.ActiveFlag = true;
                note.CreatedBy = username;
                note.CreatedOn = DateTime.Now;

                db.Notes.Add(note);

                var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == note.idCARRDetail).FirstOrDefault();
                var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
                var idNote = note.idNote;

                //Details used in Partial Info Bar
                ViewBag.MyCarr = MyCarr;
                ViewBag.MyCarrDetail = MyCarrDetail;

                try
                {
                    db.SaveChanges();
                    json.Data = new { success = true, idservice = note.idNote, idcarrdetail = MyCarrDetail.idCARRDetail };
                }
                catch (System.Exception ex)
                {
                    json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("CreateNewNote", note) };
                }

            }
            else
            {
                PartialViewResult view = PartialView(note);
                json.Data = new { success = false, error = "Invalid Input", html = RenderPartialViewToString("CreateNewNote", note) };
            }
            return json;

        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateNewLHRF(int id, int idLocation)
        {
            int idCARRDetail = id;
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            var MyLocation = db.CARRLocations.Where(x => x.idLocation == idLocation).FirstOrDefault();

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyLocation = MyLocation;

            CARRLHRF lhrf = new CARRLHRF();
            lhrf.idCARRDetail = idCARRDetail;

            List<SelectListItem> LHRFTypes = Classes.HelperDropdowns.getLHRFTypes();
            ViewBag.LHRFTypes = LHRFTypes;

            ViewBag.Title = "Add LHRF";
            return PartialView(lhrf);
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateNewLHRF(FormCollection objfrm, [Bind(Include = "idCARRDetail")] CARRLHRF lhrf)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Add New LHRF";

            String idLocation = objfrm["idLocation"];

            String enteredType = objfrm["LHRFTypes"];
            if (string.IsNullOrEmpty(enteredType) || enteredType.Equals("0"))
            {
                List<SelectListItem> LHRFTypes = Classes.HelperDropdowns.getLHRFTypes();
                ViewBag.LHRFTypes = LHRFTypes;
                ModelState.AddModelError("LHRFType", "Type Required");
                json.Data = new { success = false, error = "Type Required", html = RenderPartialViewToString("CreateNewLHRF", lhrf) };
            }
            else if (ModelState.IsValid)
            {
                string username = Session["accountname"].ToString();

                lhrf.LHMode = enteredType;
                lhrf.LHRFType = enteredType;
                lhrf.ActiveFlag = true;
                lhrf.CreatedBy = username;
                lhrf.CreatedOn = DateTime.Now;
                lhrf.idLocation = Int32.Parse(idLocation);

                db.CARRLHRFs.Add(lhrf);

                var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == lhrf.idCARRDetail).FirstOrDefault();
                var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

                //Details used in Partial Info Bar
                ViewBag.MyCarr = MyCarr;
                ViewBag.MyCarrDetail = MyCarrDetail;

                try
                {
                    db.SaveChanges();
                    json.Data = new { success = true, idlhrf = lhrf.idLHRF, idcarrdetail = MyCarrDetail.idCARRDetail };
                }
                catch (System.Exception ex)
                {
                    json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("CreateNewLHRF", lhrf) };
                }

            }
            else
            {
                PartialViewResult view = PartialView(lhrf);
                json.Data = new { success = false, error = "Invalid Input", html = RenderPartialViewToString("CreateNewLHRF", lhrf) };
            }
            return json;

        }

        [HttpGet]
        [Authorize]
        public ActionResult EditNote(int id, int idnote)
        {
            int idCARRDetail = id;
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            var note = db.Notes.Where(x => x.idNote == idnote).FirstOrDefault();

            ViewBag.Title = "Edit Note";
            return PartialView(note);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditNote(FormCollection objfrm, [Bind(Include = "idCARRDetail,idNote")] Note note)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Edit Note";

            String enteredNote = objfrm["Note1"];
            if (string.IsNullOrEmpty(enteredNote))
            {
                ModelState.AddModelError("Note1", "Note Required");
                json.Data = new { success = false, error = "Name Required", html = RenderPartialViewToString("EditNote", note) };
            }
            else if (ModelState.IsValid)
            {
                string username = Session["accountname"].ToString();

                var editNote = db.Notes.Find(note.idNote);
                editNote.Note1 = enteredNote;
                editNote.ActiveFlag = true;
                editNote.UpdatedBy = username;
                editNote.UpdatedOn = DateTime.Now;

                var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == note.idCARRDetail).FirstOrDefault();
                var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
                var idNote = note.idNote;

                //Details used in Partial Info Bar
                ViewBag.MyCarr = MyCarr;
                ViewBag.MyCarrDetail = MyCarrDetail;

                try
                {
                    db.SaveChanges();
                    json.Data = new { success = true, idservice = note.idNote, idcarrdetail = MyCarrDetail.idCARRDetail };
                }
                catch (System.Exception ex)
                {
                    json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("EditNote", note) };
                }

            }
            else
            {
                PartialViewResult view = PartialView(note);
                json.Data = new { success = false, error = "Invalid Input", html = RenderPartialViewToString("EditNote", note) };
            }
            return json;

        }

        [HttpGet]
        [Authorize]
        public ActionResult EditBrokerage(int id, int idbrokerage)
        {
            int idCARRDetail = id;
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            var brokerage = db.CARRBrokerages.Where(x => x.idBrokerage == idbrokerage).FirstOrDefault();

            ViewBag.Title = "Edit Brokerage";
            return PartialView(brokerage);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditBrokerage(FormCollection objfrm, [Bind(Include = "idCARRDetail,idBrokerage")] CARRBrokerage brokerage)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Edit Brokerage";


            bool isPricingUser = Session["userrole"].ToString().Equals("Pricing") ? true : false;
            var editBrokerage = db.CARRBrokerages.Find(brokerage.idBrokerage);
            
            decimal fee = -1;
            decimal surcharge = -1;
            decimal mincharge = -1;
            decimal tierAddlFee = -1;
            decimal maxcharge = -1;
            bool errorFound = false;

            if (isPricingUser)
            {
                if (!Decimal.TryParse(objfrm["Fee"], out fee))
                {
                    errorFound = true;
                    ModelState.AddModelError("Fee", "Fee Required and must be in a valid currency format");
                    json.Data = new { success = false, error = "Fee required", html = RenderPartialViewToString("EditBrokerage", editBrokerage) };
                }
                else if (!Decimal.TryParse(objfrm["Surcharge"], out surcharge) && editBrokerage.Description == "E-Link")
                {
                    errorFound = true;
                    ModelState.AddModelError("Surcharge", "Surcharge required and must be in a valid currency format");
                    json.Data = new { success = false, error = "Surcharge Required", html = RenderPartialViewToString("EditBrokerage", editBrokerage) };
                }
                else if (!Decimal.TryParse(objfrm["MinCharge"], out mincharge) && editBrokerage.Description == "PASS")
                {
                    errorFound = true;
                    ModelState.AddModelError("MinCharge", "MinCharge required and must be in a valid currency format");
                    json.Data = new { success = false, error = "MinCharge Required", html = RenderPartialViewToString("EditBrokerage", editBrokerage) };
                }
                else if (!Decimal.TryParse(objfrm["TierAddlFee"], out tierAddlFee) && editBrokerage.Description == "Tier")
                {
                    errorFound = true;
                    ModelState.AddModelError("TierAddlFee", "TierAddlFee required and must be in a valid currency format");
                    json.Data = new { success = false, error = "TierAddlFee Required", html = RenderPartialViewToString("EditBrokerage", editBrokerage) };
                }
                else if (!Decimal.TryParse(objfrm["MaxCharge"], out maxcharge) && editBrokerage.Description == "Tier")
                {
                    errorFound = true;
                    ModelState.AddModelError("MaxCharge", "MaxCharge required and must be in a valid currency format");
                    json.Data = new { success = false, error = "MaxCharge Required", html = RenderPartialViewToString("EditBrokerage", editBrokerage) };
                }

                if (errorFound)
                {
                    return json;
                }
            
            }
            
            if (ModelState.IsValid && !errorFound)
            {
                string username = Session["accountname"].ToString();
                String enteredComment = objfrm["Comment"];

                editBrokerage.Comment = enteredComment != null ? enteredComment : editBrokerage.Comment;
                editBrokerage.Fee = fee > -1 ? Math.Round(fee, 2) : editBrokerage.Fee;
                editBrokerage.Surcharge = surcharge > -1 ? Math.Round(surcharge, 2) : editBrokerage.Surcharge;
                editBrokerage.MinCharge = mincharge > -1 ? Math.Round(mincharge, 2) : editBrokerage.MinCharge;
                editBrokerage.TierAddlFee = tierAddlFee > -1 ? Math.Round(tierAddlFee, 2) : editBrokerage.TierAddlFee;
                editBrokerage.MaxCharge = maxcharge > -1 ? Math.Round(maxcharge, 2) : editBrokerage.MaxCharge;
                editBrokerage.ActiveFlag = true;
                editBrokerage.UpdatedBy = username;
                editBrokerage.UpdatedOn = DateTime.Now;

                var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == brokerage.idCARRDetail).FirstOrDefault();
                var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
                var idBrokerage = brokerage.idBrokerage;

                //Details used in Partial Info Bar
                ViewBag.MyCarr = MyCarr;
                ViewBag.MyCarrDetail = MyCarrDetail;

                try
                {
                    db.SaveChanges();
                    json.Data = new { success = true, idbrokerage = brokerage.idBrokerage, idcarrdetail = MyCarrDetail.idCARRDetail };
                }
                catch (System.Exception ex)
                {
                    json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("EditBrokerage", brokerage) };
                }

            }
            else
            {
                PartialViewResult view = PartialView(brokerage);
                json.Data = new { success = false, error = "Invalid Input", html = RenderPartialViewToString("EditBrokerage", brokerage) };
            }
            return json;

        }

        [HttpGet]
        [Authorize]
        public ActionResult EditAccessorial(int id, int idAccessorial)
        {
            int idCARRDetail = id;
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            var accessorial = db.CARRAccessorials.Where(x => x.idAccessorial == idAccessorial).FirstOrDefault();

            ViewBag.Title = "Edit Accessorial";
            return PartialView(accessorial);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditAccessorial(FormCollection objfrm, [Bind(Include = "idCARRDetail,idAccessorial")] CARRAccessorial accessorial)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Edit Accessorial";

            String enteredRate = objfrm["CARRrate"];
            if (string.IsNullOrEmpty(enteredRate))
            {
                ModelState.AddModelError("CARRrate", "CARRrate Required");
                json.Data = new { success = false, error = "CARRrate Required", html = RenderPartialViewToString("EditAccessorial", accessorial) };
            }
            else if (ModelState.IsValid)
            {
                string username = Session["accountname"].ToString();

                var editAccessorial = db.CARRAccessorials.Find(accessorial.idAccessorial);
                editAccessorial.CARRrate = enteredRate;
                editAccessorial.ActiveFlag = true;
                editAccessorial.UpdatedBy = username;
                editAccessorial.UpdatedOn = DateTime.Now;

                var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == accessorial.idCARRDetail).FirstOrDefault();
                var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
                var idAccessorial = accessorial.idAccessorial;

                //Details used in Partial Info Bar
                ViewBag.MyCarr = MyCarr;
                ViewBag.MyCarrDetail = MyCarrDetail;

                try
                {
                    db.SaveChanges();
                    json.Data = new { success = true, idaccessorial = editAccessorial.idAccessorial, idcarrdetail = MyCarrDetail.idCARRDetail, shippingservice = editAccessorial.ShippingService };
                }
                catch (System.Exception ex)
                {
                    json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("EditAccessorial", accessorial) };
                }

            }
            else
            {
                PartialViewResult view = PartialView(accessorial);
                json.Data = new { success = false, error = "Invalid Input", html = RenderPartialViewToString("EditAccessorial", accessorial) };
            }
            return json;

        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteVolume(int id)
        {
            var volume = db.ServiceVolumes.Where(x => x.idVolume == id).FirstOrDefault();
            return PartialView(volume);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteVolume(FormCollection objfrm, [Bind(Include = "idVolume")] ServiceVolume volume)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete Volume";
            var volumeToDelete = db.ServiceVolumes.Where(x => x.idVolume == volume.idVolume).FirstOrDefault();

            string username = Session["accountname"].ToString();

            volumeToDelete.ActiveFlag = false;
            volumeToDelete.UpdatedBy = username;
            //db.SaveChanges();
            
            var MyService = db.Services.Where(x => x.idService == volumeToDelete.idService).First();
            var MyLocation = db.CARRLocations.Where(x => x.idLocation == MyService.idLocation).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idservice = volumeToDelete.idService, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteVolume", volume) };
            }

            return json;
        }

        [HttpGet]
        public ActionResult DeleteLocation(int id)
        {
            var location = db.CARRLocations.Where(x => x.idLocation == id).FirstOrDefault();
            return PartialView(location);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteLocation(FormCollection objfrm, [Bind(Include = "idLocation")] Service location)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete Location";
            var locationToDelete = db.CARRLocations.Where(x => x.idLocation == location.idLocation).FirstOrDefault();

            string username = Session["accountname"].ToString();

            locationToDelete.ActiveFlag = false;
            locationToDelete.UpdatedBy = username;
            //db.SaveChanges();

            //var MyLocation = db.CARRLocations.Where(x => x.idLocation ==locationToDelete.idLocation).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == locationToDelete.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idcarrdetail = locationToDelete.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteLocation", location) };
            }

            return json;
        }

        public ActionResult DeleteCARR(int id)
        {
            var CARR = db.CARRDetails.Where(x => x.idCARRDetail == id).FirstOrDefault();
            return PartialView(CARR);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteCARR(FormCollection objfrm, [Bind(Include = "idCARRDetail")] CARRDetail CARR)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete CARR";
            var CARRToDelete = db.CARRDetails.Where(x => x.idCARRDetail == CARR.idCARRDetail).FirstOrDefault();

            string username = Session["accountname"].ToString();

            CARRToDelete.ActiveFlag = false;
            CARRToDelete.UpdatedBy = username;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == CARR.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteCARR", CARR) };
            }

            return json;
        }


        public ActionResult DeactivateCARR(int id)
        {
            var CARR = db.CARRDetails.Where(x => x.idCARRDetail == id).FirstOrDefault();
            return PartialView(CARR);
        }
        [HttpPost]
        [Authorize]
        public ActionResult DeactivateCARR(FormCollection objfrm, [Bind(Include = "idCARRDetail")] CARRDetail CARR)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Deactivate CARR";
            var CARRToDelete = db.CARRDetails.Where(x => x.idCARRDetail == CARR.idCARRDetail).FirstOrDefault();

            string username = Session["accountname"].ToString();

            CARRToDelete.Deactivate = true;
            CARRToDelete.UpdatedBy = username;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == CARR.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeactivateCARR", CARR) };
            }

            return json;
        }
        public ActionResult ActivateCARR(int id)
        {
            var CARR = db.CARRDetails.Where(x => x.idCARRDetail == id).FirstOrDefault();
            return PartialView(CARR);
        }
        [HttpPost]
        [Authorize]
        public ActionResult ActivateCARR(FormCollection objfrm, [Bind(Include = "idCARRDetail")] CARRDetail CARR)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Activate CARR";
            var CARRToDelete = db.CARRDetails.Where(x => x.idCARRDetail == CARR.idCARRDetail).FirstOrDefault();

            string username = Session["accountname"].ToString();

            CARRToDelete.Deactivate = false;
            CARRToDelete.UpdatedBy = username;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == CARR.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("ActivateCARR", CARR) };
            }

            return json;
        }

        public ActionResult DeleteService(int id)
        {
            var service = db.Services.Where(x => x.idService == id).FirstOrDefault();
            return PartialView(service);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteService(FormCollection objfrm, [Bind(Include = "idService")] Service service)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete Serivce";
            var serviceToDelete = db.Services.Where(x => x.idService == service.idService).FirstOrDefault();

            string username = Session["accountname"].ToString();

            serviceToDelete.ActiveFlag = false;
            serviceToDelete.UpdatedBy = username;
            //db.SaveChanges();

            var MyLocation = db.CARRLocations.Where(x => x.idLocation == serviceToDelete.idLocation).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idlocation = serviceToDelete.idLocation, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteService", service) };
            }

            return json;
        }


        [HttpGet]
        [Authorize]
        public ActionResult DeleteException(int id)
        {
            var note = db.Exceptions.Where(x => x.idException == id).FirstOrDefault();
            return PartialView(note);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteException(FormCollection objfrm, [Bind(Include = "idException")] Models.Exception exception)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete Exception";
            var exceptionToDelete = db.Exceptions.Where(x => x.idException == exception.idException).FirstOrDefault();

            string username = Session["accountname"].ToString();

            exceptionToDelete.ActiveFlag = false;
            exceptionToDelete.UpdatedBy = username;
            exceptionToDelete.UpdatedOn = DateTime.Now;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == exceptionToDelete.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idservice = exceptionToDelete.idException, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteException", exception) };
            }

            return json;
        }


        [HttpGet]
        [Authorize]
        public ActionResult DeleteBrokerage(int id)
        {
            var brokerage = db.CARRBrokerages.Where(x => x.idBrokerage == id).FirstOrDefault();
            return PartialView(brokerage);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteBrokerage(FormCollection objfrm, [Bind(Include = "idBrokerage")] CARRBrokerage brokerage)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete Brokerage";
            var brokerageToDelete = db.CARRBrokerages.Where(x => x.idBrokerage == brokerage.idBrokerage).FirstOrDefault();

            string username = Session["accountname"].ToString();

            brokerageToDelete.ActiveFlag = false;
            brokerageToDelete.UpdatedBy = username;
            brokerageToDelete.UpdatedOn = DateTime.Now;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == brokerageToDelete.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idbrokerage = brokerageToDelete.idBrokerage, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteBrokerage", brokerage) };
            }

            return json;
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteNote(int id)
        {
            var note = db.Notes.Where(x => x.idNote == id).FirstOrDefault();
            return PartialView(note);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteNote(FormCollection objfrm, [Bind(Include = "idNote")] Note note)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete Note";
            var noteToDelete = db.Notes.Where(x => x.idNote == note.idNote).FirstOrDefault();

            string username = Session["accountname"].ToString();

            noteToDelete.ActiveFlag = false;
            noteToDelete.UpdatedBy = username;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == noteToDelete.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idservice = noteToDelete.idNote, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteNote", note) };
            }

            return json;
        }

        [HttpGet]
        public ActionResult DeleteLHRate(int id)
        {
            var lhRate = db.CARRLHRates.Where(x => x.idCARRLHRate == id).FirstOrDefault();
            return PartialView(lhRate);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteLHRate(FormCollection objfrm, [Bind(Include = "idCARRLHRate")] CARRLHRate rate)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete LH Rate";
            var rateToDelete = db.CARRLHRates.Where(x => x.idCARRLHRate == rate.idCARRLHRate).FirstOrDefault();

            string username = Session["accountname"].ToString();

            rateToDelete.ActiveFlag = false;
            rateToDelete.UpdatedBy = username;
            rateToDelete.UpdatedOn = DateTime.Now;

            var location = db.CARRLocations.Where(x => x.idLocation == rateToDelete.idLocation).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == location.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idlocation = rateToDelete.idLocation, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteLHRate", rate) };
            }

            return json;
        }
        [HttpGet]
        [Authorize]
        public ActionResult DeleteLHRF(int id)
        {
            var lhrf = db.CARRLHRFs.Where(x => x.idLHRF == id).FirstOrDefault();
            return PartialView(lhrf);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteLHRF(FormCollection objfrm, [Bind(Include = "idLHRF")] CARRLHRF lhrf)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete LHRF";
            var lhrfToDelete = db.CARRLHRFs.Where(x => x.idLHRF == lhrf.idLHRF).FirstOrDefault();

            string username = Session["accountname"].ToString();

            lhrfToDelete.ActiveFlag = false;
            lhrfToDelete.UpdatedBy = username;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == lhrfToDelete.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idlhrf = lhrfToDelete.idLHRF, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteLHRF", lhrf) };
            }

            return json;
        }


        [HttpGet]
        [Authorize]
        public ActionResult DeleteRateUpload(int id)
        {
            var rate = db.CARRRateFiles.Where(x => x.idRateFile == id).FirstOrDefault();
            return PartialView(rate);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteRateUpload(FormCollection objfrm, [Bind(Include = "idRateFile")] CARRRateFile rate)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete Rate File";
            var rateToDelete = db.CARRRateFiles.Where(x => x.idRateFile == rate.idRateFile).FirstOrDefault();

            string username = Session["accountname"].ToString();

            rateToDelete.ActiveFlag = false;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == rateToDelete.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idservice = rateToDelete.idRateFile, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteRateUpload", rate) };
            }

            return json;
        }


        [HttpGet]
        [Authorize]
        public ActionResult DeleteComparisonUpload(int id)
        {
            var rate = db.UPSComparisonFiles.Where(x => x.idUPSFile == id).FirstOrDefault();
            return PartialView(rate);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteComparisonUpload(FormCollection objfrm, [Bind(Include = "idUPSFile")] UPSComparisonFile file)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete UPS Comparison File";
            var fileToDelete = db.UPSComparisonFiles.Where(x => x.idUPSFile == file.idUPSFile).FirstOrDefault();

            string username = Session["accountname"].ToString();

            fileToDelete.ActiveFlag = false;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == fileToDelete.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idservice = fileToDelete.idUPSFile, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteComparisonUpload", file) };
            }

            return json;
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteSalesUpload(int id)
        {
            var rate = db.CARRFileUploads.Where(x => x.idFileUpload == id).FirstOrDefault();
            return PartialView(rate);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteSalesUpload(FormCollection objfrm, [Bind(Include = "idFileUpload")] CARRFileUpload file)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete Sales File";
            var fileToDelete = db.CARRFileUploads.Where(x => x.idFileUpload == file.idFileUpload).FirstOrDefault();

            string username = Session["accountname"].ToString();

            fileToDelete.ActiveFlag = false;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == fileToDelete.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idFileUpload = fileToDelete.idFileUpload, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteSalesUpload", file) };
            }

            return json;
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteContractsUpload(int id)
        {
            return PartialView(db.CARRVDAFiles.Where(x => x.idVDAFile == id).FirstOrDefault());
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteContractsUpload(FormCollection objfrm, [Bind(Include = "idVDAFile")] CARRVDAFile file)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete Contracts File";
            var fileToDelete = db.CARRVDAFiles.Where(x => x.idVDAFile == file.idVDAFile).FirstOrDefault();

            try
            {
                fileToDelete.ActiveFlag = false;

                db.SaveChanges();

                var MyCarrDetail = db.CARRVDAFiles.Where(x => x.idCARRDetail == fileToDelete.idCARRDetail).FirstOrDefault();

                json.Data = new { success = true, idFileUpload = fileToDelete.idVDAFile, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception e)
            {
                json.Data = new { success = false, error = e.Message.ToString(), html = RenderPartialViewToString("DeleteContractsUpload", file) };
            }

            return json;
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteShippingProfileUpload(int id)
        {
            return PartialView(db.CARRPuroPostFiles.Where(x => x.idPPSTFile == id).FirstOrDefault());
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteShippingProfileUpload(FormCollection objfrm, [Bind(Include = "idPPSTFile")] CARRPuroPostFile file)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete Shipping Profile";
            var fileToDelete = db.CARRPuroPostFiles.Where(x => x.idPPSTFile == file.idPPSTFile).FirstOrDefault();

            try
            {
                fileToDelete.ActiveFlag = false;

                db.SaveChanges();

                var MyCarrDetail = db.CARRPuroPostFiles.Where(x => x.idCARRDetail == fileToDelete.idCARRDetail).FirstOrDefault();

                json.Data = new { success = true, idFileUpload = fileToDelete.idPPSTFile, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception e)
            {
                json.Data = new { success = false, error = e.Message.ToString(), html = RenderPartialViewToString("DeleteShippingProfileUpload", file) };
            }

            return json;
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteUSContractUpload(int id)
        {
            var rate = db.USContractsUploads.Where(x => x.idUSContractsFile == id).FirstOrDefault();
            return PartialView(rate);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteUSContractUpload(FormCollection objfrm, [Bind(Include = "idUSContractsFile")] USContractsUpload file)
        {
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete US Contracts File Attachment";
            var fileToDelete = db.USContractsUploads.Where(x => x.idUSContractsFile == file.idUSContractsFile).FirstOrDefault();

            string username = Session["accountname"].ToString();

            fileToDelete.ActiveFlag = false;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == fileToDelete.idCARRDetail).FirstOrDefault();

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, idFileUpload = fileToDelete.idUSContractsFile, idcarrdetail = MyCarrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteUSContractUpload", file) };
            }

            return json;
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteUser(int id)
        {
            var user = db.applicationUserRoles.Where(x => x.id.Equals(id)).FirstOrDefault();
            return PartialView(user);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteUser(FormCollection objfrm, [Bind(Include = "id")] applicationUserRole user)
        {
            string username = Session["accountname"].ToString();
            JsonResult json = new JsonResult();
            ViewBag.Title = "Delete User";

            var userToDelete = db.applicationUserRoles.Where(x => x.id == user.id).FirstOrDefault();

            var districtsToDelete = db.ApplicationDistrictsAlloweds.Where(x => x.ActiveDirectory.Equals(userToDelete.ActiveDirectory)).ToList();
            foreach (var d in districtsToDelete)
            {
                db.ApplicationDistrictsAlloweds.Remove(d);
            }

            var branchesToDelete = db.ApplicationRegionsAlloweds.Where(x => x.ActiveDirectory == userToDelete.ActiveDirectory).ToList();
            foreach (var b in branchesToDelete)
            {
                db.ApplicationRegionsAlloweds.Remove(b);
            }

            var directReportsToDelete = db.ApplicationUsersAlloweds.Where(x => x.ActiveDirectory.Equals(userToDelete.ActiveDirectory)).ToList();
            foreach (var u in directReportsToDelete)
            {
                db.ApplicationUsersAlloweds.Remove(u);
            }

            userToDelete.ActiveFlag = false;

            try
            {
                db.SaveChanges();
                json.Data = new { success = true, id = userToDelete.idApplication };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("DeleteUser", user) };
            }

            return json;
        }

        [HttpGet]
        [Authorize]
        public ActionResult CompleteTheCARRConfirm(int id)
        {
            var carrDetail = db.CARRDetails.Where(x => x.idCARRDetail == id).FirstOrDefault();
            return PartialView(carrDetail);
        }

        [HttpPost]
        [Authorize]
        public ActionResult CompleteTheCARRConfirm(FormCollection objfrm, [Bind(Include = "idCARRDetail")] CARRDetail carrDetail)
        {

            JsonResult json = new JsonResult();
            try
            {
                string username = Session["accountname"].ToString();
                DataController dataController = new DataController();
                dataController.processComplete(carrDetail.idCARRDetail, username);
                json.Data = new { success = true, idcarrdetail = carrDetail.idCARRDetail };
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message.ToString(), html = RenderPartialViewToString("CompleteTheCARRConfirm", carrDetail) };
            }

            return Json(new
            {
                redirectUrl = Url.Action("Index", "Home"),
                isRedirect = true
            });
        }

            protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult DisplayApprovals(int id)
        {
            int idCARRDetail = id;
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            //var MyApprovals = db.Approvals.Where(x => x.idCARRDetail == idCARRDetail);
            //ViewBag.MyApprovals = MyApprovals;

            //var MyDistrict = db.Regions.Where(x => x.Airport == MyCarrDetail.DecisionMakerBranch).FirstOrDefault();
            //ViewBag.MyDistrict = MyDistrict;
            string MyDistrict = "";
            string sql = "Select District from tblRegions where Airport = '" + MyCarrDetail.DecisionMakerBranch + "'";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PrePumaDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {

                            MyDistrict = sdr["District"].ToString().Trim();
                        }
                    }
                    con.Close();
                }
            }
            ViewBag.MyDistrict = MyDistrict;


            sql = @"select a.idApproval,a.ApprovedBy,a.DateApproved,a.idCARRDetail,Concat('..\',s.ImageURL) ImageURL
             from CAR.Approvals a
			 left outer join CAR.Signatures s on a.ApprovedBy=s.ActiveDirectory
            where idCARRDetail = ";
            sql = sql + idCARRDetail;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, connection);
            da.Fill(dt);
            connection.Close();
            connection.Dispose();
            ViewBag.result = dt;

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            
            

            ViewBag.Title = "Approvals";
            return PartialView();
        }

    }
}