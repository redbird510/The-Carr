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
    public class CloneController : Controller
    {
        private Entities db = new Entities();

        [HttpPost]
        public ActionResult Index()
        {
            
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.idCARRDetail = idCARRDetail;
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }
            if (MyCarrDetail.Currency != null)
            {
                var MyCurrency = MyDropdowns.Where(c => c.Name == MyCarrDetail.Currency).First();
                ViewBag.MyCurrency = MyCurrency.Name;
            }

            //ViewBag.DisableChanges = new ContractController().disableChanges(idCARRDetail);
            var controller = DependencyResolver.Current.GetService<ContractController>();
            controller.ControllerContext = new ControllerContext(this.Request.RequestContext, controller);
            ViewBag.DisableChanges = controller.disableChanges(idCARRDetail);

            ViewBag.ErrorMsg = "";
            return View();
        }

        [HttpPost]
        public ActionResult CloneCARR(string VersionComment,int idCARRDetail)

        {
            JsonResult json = new JsonResult();

            int OrigidCARRDetail = idCARRDetail;
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            try
            {
                if (string.IsNullOrEmpty(VersionComment))
                {
                    //ModelState.AddModelError("VersionComment", "Comment Required");
                    ViewBag.ErrorMsg = "Comment Required";
                    json.Data = new { success = false, error = "Comment Required" };
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        //int OrigidCARRDetail = idCARRDetail;
                        //var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).FirstOrDefault();
                        var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);

                                                
                        db.SaveChanges();

                        ViewBag.MyCarr = MyCarr;
                        ViewBag.MyCarrDetail = MyCarrDetail;
                        ViewBag.idCARRDetail = OrigidCARRDetail;

                        //get Next Version Number
                        int? newVersionNumber;
                        var allCarrDetails = db.CARRDetails.Where(x => x.idCARR == MyCarrDetail.idCARR && x.ActiveFlag == true);
                        newVersionNumber = allCarrDetails.Max(x => x.VersionNumber);
                        newVersionNumber = newVersionNumber + 1;

                        //CLONE ALL TABLES

                        string username = Session["accountname"].ToString();

                        //Carr Detail
                        CARRDetail CloneCarrDetail = new CARRDetail();
                        CloneCarrDetail = MyCarrDetail;
                        CloneCarrDetail.VersionNumber = newVersionNumber;
                        CloneCarrDetail.VersionComment = VersionComment;
                        CloneCarrDetail.CreatedBy = username;
                        CloneCarrDetail.CreatedOn = DateTime.Now;
                        CloneCarrDetail.PricingCompletedBy = "";
                        CloneCarrDetail.PricingReqCompleteDate = null;
                        CloneCarrDetail.PricingReqCompleteFlag = false;
                        CloneCarrDetail.CompletedFlag = false;
                        CloneCarrDetail.CompletedDate = null;
                        db.CARRDetails.Add(CloneCarrDetail);
                        db.SaveChanges();

                        int CloneidCARRDetail = CloneCarrDetail.idCARRDetail;

                        //Locations
                        IList<CARRLocation> OrigLocations = db.CARRLocations.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).ToList();
                        foreach (CARRLocation OrigLocation in OrigLocations)
                        {
                            //Clone this Location
                            CARRLocation CloneLocation = new CARRLocation();
                            //Copy individual columns because of foreign keys
                            CloneLocation.CompanyName = OrigLocation.CompanyName;
                            CloneLocation.locationDM = OrigLocation.locationDM;
                            CloneLocation.ControlBranch = OrigLocation.ControlBranch;
                            CloneLocation.Gateway = OrigLocation.Gateway;
                            CloneLocation.Branch = OrigLocation.Branch;
                            CloneLocation.LocalSRID = OrigLocation.LocalSRID;
                            CloneLocation.StrategicSRID = OrigLocation.StrategicSRID;
                            CloneLocation.ISPSRID = OrigLocation.ISPSRID;
                            CloneLocation.idServiceType = OrigLocation.idServiceType;
                            CloneLocation.idBranchContact = OrigLocation.idBranchContact;
                            CloneLocation.LocationName = OrigLocation.LocationName;
                            CloneLocation.FFWDType = OrigLocation.FFWDType;
                            CloneLocation.DGFlag = OrigLocation.DGFlag;
                            CloneLocation.CreatedBy = OrigLocation.CreatedBy;
                            CloneLocation.CreatedOn = OrigLocation.CreatedOn;
                            CloneLocation.UpdatedBy = OrigLocation.UpdatedBy;
                            CloneLocation.UpdatedOn = OrigLocation.UpdatedOn;
                            CloneLocation.ActiveFlag = OrigLocation.ActiveFlag;
                            CloneLocation.idCARRDetail = CloneidCARRDetail;
                            db.CARRLocations.Add(CloneLocation);
                            db.SaveChanges();

                            //Services
                            IList<Service> OrigServices = db.Services.Where(x => x.idLocation == OrigLocation.idLocation && x.ActiveFlag == true).ToList();
                            foreach (Service OrigService in OrigServices)
                            {
                                //Clone this Service
                                Service CloneService = new Service();
                                
                                //Copy individual columns beacuse of foreign keys
                                CloneService.idLocation = CloneLocation.idLocation;
                                CloneService.idShippingService = OrigService.idShippingService;
                                CloneService.AccountNumber = OrigService.AccountNumber;
                                CloneService.PINPrefix = OrigService.PINPrefix;                                
                                CloneService.primaryBillingFlag = OrigService.primaryBillingFlag;
                                CloneService.idBillingContact = OrigService.idBillingContact;
                                CloneService.freightAuditorFlag = OrigService.freightAuditorFlag;
                                CloneService.combineBillingFlag = OrigService.combineBillingFlag;
                                CloneService.separateBillingFlag = OrigService.separateBillingFlag;
                                CloneService.RateBand = OrigService.RateBand;
                                CloneService.Zones = OrigService.Zones;
                                CloneService.ShippingSystem = OrigService.ShippingSystem;
                                CloneService.Comments = OrigService.Comments;
                                CloneService.MarkupPct = OrigService.MarkupPct;
                                CloneService.DiscountPct = OrigService.DiscountPct;
                                CloneService.PricingLevel = OrigService.PricingLevel;
                                CloneService.LHRateBand = OrigService.LHRateBand;
                                CloneService.ppstcurrentProvider = OrigService.ppstcurrentProvider;
                                CloneService.cpcExpeditedFlag = OrigService.cpcExpeditedFlag;
                                CloneService.cpcXpressPostFlag = OrigService.cpcXpressPostFlag;
                                CloneService.airCharterFlag = OrigService.airCharterFlag;
                                CloneService.airCommercialFlag = OrigService.airCommercialFlag;
                                CloneService.courierUSPricingFlag = OrigService.courierUSPricingFlag;
                                CloneService.courierSelfPricingFlag = OrigService.courierSelfPricingFlag;
                                CloneService.CreatedBy = OrigService.CreatedBy;
                                CloneService.CreatedOn = OrigService.CreatedOn;
                                CloneService.UpdatedBy = OrigService.UpdatedBy;
                                CloneService.UpdatedOn = OrigService.UpdatedOn;
                                CloneService.ActiveFlag = OrigService.ActiveFlag;
                                db.Services.Add(CloneService);
                                db.SaveChanges();

                                //Service Volumes
                                IList<ServiceVolume> OrigSvcVolumes = db.ServiceVolumes.Where(x => x.idService == OrigService.idService && x.ActiveFlag == true).ToList();
                                foreach (ServiceVolume OrigSvcVolume in OrigSvcVolumes)
                                {
                                    //Clone this volume
                                    ServiceVolume CloneSvcVolume = new ServiceVolume();
                                    CloneSvcVolume = OrigSvcVolume;
                                    CloneSvcVolume.idService = CloneService.idService;
                                    db.ServiceVolumes.Add(CloneSvcVolume);
                                    db.SaveChanges();
                                }

                            }

                            //LHRates
                            IList<CARRLHRate> OrigLHRates = db.CARRLHRates.Where(x => x.idLocation == OrigLocation.idLocation && x.ActiveFlag == true).ToList();
                            foreach (CARRLHRate OrigLHRate in OrigLHRates)
                            {
                                CARRLHRate CloneLHRate = new CARRLHRate();
                                CloneLHRate = OrigLHRate;
                                CloneLHRate.idLocation = CloneLocation.idLocation;
                                db.CARRLHRates.Add(CloneLHRate);
                                db.SaveChanges();
                            }


                        }

                        //Create First RoutedTo Row
                        Routing routedto = new Routing();
                        routedto.RoutedTo = username;
                        routedto.CreatedBy = username;
                        routedto.CreatedOn = DateTime.Now;
                        routedto.DateRoutedTo = DateTime.Now;
                        routedto.idCARRDetail = CloneCarrDetail.idCARRDetail;
                        db.Routings.Add(routedto);
                        CloneCarrDetail.idRoutedTo = routedto.idRouting;
                        db.SaveChanges();

                        //Accessorials  
                        //IList<CARRAccessorial> OrigAccessorials = db.CARRAccessorials.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).ToList();
                        //foreach (CARRAccessorial OrigAccessorial in OrigAccessorials)
                        //{
                        //    CARRAccessorial CloneAccessorial = new CARRAccessorial();
                        //    CloneAccessorial = OrigAccessorial;
                        //    CloneAccessorial.idCARRDetail = CloneidCARRDetail;
                        //    db.CARRAccessorials.Add(CloneAccessorial);
                        //    db.SaveChanges();
                        //}

                        //Brokerage
                        IList<CARRBrokerage> OrigBrokerages = db.CARRBrokerages.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).ToList();
                        foreach (CARRBrokerage OrigBrokerage in OrigBrokerages)
                        {
                            CARRBrokerage CloneBrokerage = new CARRBrokerage();
                            CloneBrokerage = OrigBrokerage;
                            CloneBrokerage.idCARRDetail = CloneidCARRDetail;
                            db.CARRBrokerages.Add(CloneBrokerage);
                            db.SaveChanges();
                        }

                        //CARRLHRF
                        IList<CARRLHRF> OrigLHRFs = db.CARRLHRFs.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).ToList();
                        foreach (CARRLHRF OrigLHRF in OrigLHRFs)
                        {
                            CARRLHRF CloneLHRF = new CARRLHRF();
                            CloneLHRF = OrigLHRF;
                            CloneLHRF.idCARRDetail = CloneidCARRDetail;
                            db.CARRLHRFs.Add(CloneLHRF);
                            db.SaveChanges();
                        }

                        //Exceptions
                        IList<PI_Portal.Models.Exception> OrigExceptions = db.Exceptions.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).ToList();
                        foreach (PI_Portal.Models.Exception OrigException in OrigExceptions)
                        {
                            PI_Portal.Models.Exception CloneException = new PI_Portal.Models.Exception();
                            CloneException = OrigException;
                            CloneException.idCARRDetail = CloneidCARRDetail;
                            db.Exceptions.Add(CloneException);
                            db.SaveChanges();
                        }

                        //File Uploads
                        IList<CARRFileUpload> OrigFileUploads = db.CARRFileUploads.Where(x => x.idCARRDetail == OrigidCARRDetail).ToList();
                        foreach (CARRFileUpload OrigFileUpload in OrigFileUploads)
                        {
                            CARRFileUpload CloneFileUpload = new CARRFileUpload();
                            CloneFileUpload = OrigFileUpload;
                            CloneFileUpload.idCARRDetail = CloneidCARRDetail;
                            db.CARRFileUploads.Add(CloneFileUpload);
                            db.SaveChanges();
                        }

                        //Notes
                        IList<Note> OrigNotes = db.Notes.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).ToList();
                        foreach (Note OrigNote in OrigNotes)
                        {
                            Note CloneNote = new Note();
                            CloneNote = OrigNote;
                            CloneNote.idCARRDetail = CloneidCARRDetail;
                            db.Notes.Add(CloneNote);
                            db.SaveChanges();
                        }

                        //Approvals
                        //Approvals should have to start over - commenting
                        //IList<Approval> OrigApprovals = db.Approvals.Where(x => x.idCARRDetail == OrigidCARRDetail).ToList();
                        //foreach (Approval OrigApproval in OrigApprovals)
                        //{
                        //    Approval CloneApproval = new Approval();
                        //    CloneApproval = OrigApproval;
                        //    CloneApproval.idCARRDetail = CloneidCARRDetail;
                        //    db.Approvals.Add(CloneApproval);
                        //    db.SaveChanges();
                        //}

                        //PuroPost
                        //CARRPuroPost
                        IList<CARRPuroPost> OrigPPSTs = db.CARRPuroPosts.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).ToList();
                        foreach (CARRPuroPost OrigPPST in OrigPPSTs)
                        {
                            CARRPuroPost ClonePPST = new CARRPuroPost();
                            ClonePPST = OrigPPST;
                            ClonePPST.idCARRDetail = CloneidCARRDetail;
                            db.CARRPuroPosts.Add(ClonePPST);
                            db.SaveChanges();
                        }
                        //CARRPuroPostFIles
                        IList<CARRPuroPostFile> OrigPPSTFiles = db.CARRPuroPostFiles.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).ToList();
                        foreach (CARRPuroPostFile OrigPPSTFile in OrigPPSTFiles)
                        {
                            CARRPuroPostFile ClonePPSTFile = new CARRPuroPostFile();
                            ClonePPSTFile = OrigPPSTFile;
                            ClonePPSTFile.idCARRDetail = CloneidCARRDetail;
                            db.CARRPuroPostFiles.Add(ClonePPSTFile);
                            db.SaveChanges();
                        }

                        //Estimated Revenue
                        //EstRevSalesPricing
                        IList<CARREstRevSalesPricing> OrigCARREstRevSalesPricings = db.CARREstRevSalesPricings.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).ToList();
                        foreach (CARREstRevSalesPricing OrigCARREstRevSalesPricing in OrigCARREstRevSalesPricings)
                        {
                            CARREstRevSalesPricing CloneCARREstRevSalesPricing = new CARREstRevSalesPricing();
                            CloneCARREstRevSalesPricing = OrigCARREstRevSalesPricing;
                            CloneCARREstRevSalesPricing.idCARRDetail = CloneidCARRDetail;
                            db.CARREstRevSalesPricings.Add(CloneCARREstRevSalesPricing);
                            db.SaveChanges();
                        }


                        //Contracts Upload
                        //USContractsUploads
                        IList<USContractsUpload> OrigUSContractsUploads = db.USContractsUploads.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).ToList();
                        foreach (USContractsUpload OrigUsContractUpload in OrigUSContractsUploads)
                        {
                            USContractsUpload CloneUSContractUpload = new USContractsUpload();
                            CloneUSContractUpload = OrigUsContractUpload;
                            CloneUSContractUpload.idCARRDetail = CloneidCARRDetail;
                            db.USContractsUploads.Add(CloneUSContractUpload);
                            db.SaveChanges();
                        }

                        //VDA
                        //CARRVDAFiles
                        IList<CARRVDAFile> OrigCARRVDAFiles = db.CARRVDAFiles.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).ToList();
                        foreach (CARRVDAFile OrigCARRVDAFile in OrigCARRVDAFiles)
                        {
                            CARRVDAFile CloneCARRVDAFile = new CARRVDAFile();
                            CloneCARRVDAFile = OrigCARRVDAFile;
                            CloneCARRVDAFile.idCARRDetail = CloneidCARRDetail;
                            db.CARRVDAFiles.Add(CloneCARRVDAFile);
                            db.SaveChanges();
                        }


                        //UPSComparison
                        //UPSComparisonFile
                        IList<UPSComparisonFile> OrigUPSComparisonFiles = db.UPSComparisonFiles.Where(x => x.idCARRDetail == OrigidCARRDetail && x.ActiveFlag == true).ToList();
                        foreach (UPSComparisonFile OrigUPSComparisonFile in OrigUPSComparisonFiles)
                        {
                            UPSComparisonFile CloneUPSComparisonFile = new UPSComparisonFile();
                            CloneUPSComparisonFile = OrigUPSComparisonFile;
                            CloneUPSComparisonFile.idCARRDetail = CloneidCARRDetail;
                            db.UPSComparisonFiles.Add(CloneUPSComparisonFile);
                            db.SaveChanges();
                        }


                        json.Data = new { success = true, idcarrdetail = CloneidCARRDetail };
                    }
                    else
                    {
                        //ModelState.AddModelError("VersionComment", "Comment Required");
                        json.Data = new { success = false, error = "Enter Comment", html = View("Index", MyCarrDetail) };
                    }
                }
                
      
            }
            catch (System.Exception ex)
            {
                //ModelState.AddModelError("VersionComment", ex);
                json.Data = new { success = false, error = ex, html = View("Index", MyCarrDetail) };
            }       
                     
            return json;
        }
    }
}