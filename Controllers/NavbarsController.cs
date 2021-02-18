using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PI_Portal.Models;
using PI_Portal.Classes;

namespace PI_Portal.Controllers
{    
    public class NavbarsController : Controller
    {
        private Entities db = new Entities();

        [HttpPost]
        [Authorize]
        public ActionResult SideBarLeftCARR()
        {
            int idCARRDetail = int.Parse(Request.Form["idCARRDetail"]);

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            var routing = db.Routings.Where(x => x.idRouting == MyCarrDetail.idRoutedTo).FirstOrDefault();            
            
            //Check for Session Timeout and redirect to Login Page
            if (Session["accountname"].ToString() == null)
            {
                return Redirect("/User/Login");
            }

            var MySections = db.Sections.Where(x => x.IsActive == true).OrderBy(x => x.SortOrder).ToList();

            var myrole = Session["userrole"].ToString().ToLower();
            //CARRType 3=Renewal   CARRType 5=Account Creation  --both have shortened contract menus
            if (MyCarrDetail.idCarrType == 3 || MyCarrDetail.idCarrType == 5)
            {
                //For Renewals, use a shortened Contract Menu, plus Contracts, Export, Account Creation
                if (MyCarrDetail.idCarrType == 3)
                {
                    switch (myrole)
                    {
                        case "contracts":
                            MySections = db.Sections.Where(x => x.IsActive == true && (x.idSection == 1 && (x.idSubSection == 0 || x.idSubSection == 1 || x.idSubSection == 3 || x.idSubSection == 6 || x.idSubSection == 12 || x.idSubSection == 14)) || x.idSection == 3 || (x.idSection == 10)).OrderBy(x => x.SortOrder).ToList();
                            break;
                        default:
                            MySections = db.Sections.Where(x => x.IsActive == true && (x.idSection == 1 && (x.idSubSection == 0 || x.idSubSection == 1 || x.idSubSection == 3 || x.idSubSection == 6 || x.idSubSection == 12 || x.idSubSection == 14)) || (x.idSection == 10)).OrderBy(x => x.SortOrder).ToList();
                            break;
                    }
                }
                //For New Account, use a shortened Contract Menu, plus File Upload, Export, Account Creation
                if (MyCarrDetail.idCarrType == 5)
                {
                    switch (myrole)
                    {
                        case "contracts":
                            MySections = db.Sections.Where(x => x.IsActive == true && (x.idSection == 1 && (x.idSubSection == 0 || x.idSubSection == 1 || x.idSubSection == 2 || x.idSubSection == 3 || x.idSubSection == 4 || x.idSubSection == 6 || x.idSubSection == 12 || x.idSubSection == 14)) || x.idSection == 3 || (x.idSection == 10) || (x.idSection == 6)).OrderBy(x => x.SortOrder).ToList();
                            break;
                        case "districtadmin":
                            MySections = db.Sections.Where(x => x.IsActive == true && (x.idSection == 1 && (x.idSubSection == 0 || x.idSubSection == 1 || x.idSubSection == 2 || x.idSubSection == 4 || x.idSubSection == 6 || x.idSubSection == 12)) ||  (x.idSection == 10) || x.idSection == 5  || (x.idSection == 6)).OrderBy(x => x.SortOrder).ToList();
                            break;
                        default:
                            MySections = db.Sections.Where(x => x.IsActive == true && (x.idSection == 1 && (x.idSubSection == 0 || x.idSubSection == 1 || x.idSubSection == 2 || x.idSubSection == 4 || x.idSubSection == 6 || x.idSubSection == 12 || x.idSubSection == 14)) || (x.idSection == 10) || (x.idSection == 6)).OrderBy(x => x.SortOrder).ToList();
                            break;
                    }
                    
                }
            }
            else
            {

                //1=Contract, 2=Pricing, 3=US Contracts, 4=Import, 5=Clone, 6=Account Creation, 10=Export
                //Account Creation should show only when PricingReqCompleteFlag = 1
                switch (myrole)
                {
                    case "sales":
                        //Contract, Export, Clone and Account Creation if Pricing Complete Flag is  true
                        MySections = db.Sections.Where(x => x.IsActive == true && ((x.idSection == 1 &&  (x.idSubSection != 3 && x.idSubSection != 5)) || x.idSection == 10 || x.idSection == 5 || (x.idSection == 6 && MyCarrDetail.PricingReqCompleteFlag == true))).OrderBy(x => x.SortOrder).ToList();
                        break;
                    case "salesdsm":
                    case "districtmanager":
                        //Contract, Export, Clone and Account Creation if Pricing Complete Flag is  true
                        MySections = db.Sections.Where(x => x.IsActive == true && ((x.idSection == 1 && x.idSubSection != 3) || x.idSection == 10 || x.idSection == 5 || (x.idSection == 6 && MyCarrDetail.PricingReqCompleteFlag == true))).OrderBy(x => x.SortOrder).ToList();
                        break;
                    case "pricing":
                        //Contract, Pricing, Export, Clone and Account Creation if Pricing is Complete
                        MySections = db.Sections.Where(x => x.IsActive == true && ((x.idSection == 1 && x.idSubSection != 3) || x.idSection == 2 || x.idSection == 10 || x.idSection == 5 || (x.idSection == 6 && MyCarrDetail.PricingReqCompleteFlag == true))).OrderBy(x => x.SortOrder).ToList();
                        break;               
                    case "contracts":
                        //Contract, US Contracts, Export and Account Creation if Pricing is Complete
                        MySections = db.Sections.Where(x => x.IsActive == true && ((x.idSection == 1 && x.idSubSection != 3) || x.idSection == 3 || x.idSection == 4 || x.idSection == 10 || (x.idSection == 6 && MyCarrDetail.PricingReqCompleteFlag == true && x.idSubSection != 2 && x.idSubSection != 3))).OrderBy(x => x.SortOrder).ToList();
                        break;
                    case "decisionsupport":
                        //Contract, Export, Admin and Account Creation if Pricing is Complete
                        MySections = db.Sections.Where(x => x.IsActive == true && ((x.idSection == 1 && x.idSubSection != 3) || x.idSection == 4 || x.idSection == 10 || (x.idSection == 6 && MyCarrDetail.PricingReqCompleteFlag == true))).OrderBy(x => x.SortOrder).ToList();
                        break;
                    case "operations":
                        //Contract,  Export
                        MySections = db.Sections.Where(x => x.IsActive == true && ((x.idSection == 1 && x.idSubSection != 3) ||  x.idSection == 10)).OrderBy(x => x.SortOrder).ToList();
                        break;
                    case "financedirector":
                        //all
                        MySections = db.Sections.Where(x => x.IsActive == true && ((x.idSection == 1 && x.idSubSection != 3) || x.idSection == 2 || x.idSection == 3 || x.idSection == 10 || x.idSection == 5 || (x.idSection == 6 && MyCarrDetail.PricingReqCompleteFlag == true))).OrderBy(x => x.SortOrder).ToList();
                        break;
                    case "districtadmin":
                        MySections = db.Sections.Where(x => x.IsActive == true && ((x.idSection == 1 && x.idSubSection != 3) || x.idSection == 4 || x.idSection == 10 || (x.idSection == 6 && MyCarrDetail.PricingReqCompleteFlag == true))).OrderBy(x => x.SortOrder).ToList();
                        break;
                    default:
                        //default is Contract, Export and Account Creation if Pricing is Complete
                        MySections = db.Sections.Where(x => x.IsActive == true && ((x.idSection == 1 && x.idSubSection != 3) || x.idSection == 10 || (x.idSection == 6 && MyCarrDetail.PricingReqCompleteFlag == true))).OrderBy(x => x.SortOrder).ToList();
                        break;

                }
            }

            //Check Requirements
            var contractcontroller = new ContractController();
            ViewBag.CorporateInfoIcon = "";
            ViewBag.CustDetailIcon = "";
            ViewBag.ShipProfIcon = "";            
            ViewBag.ReturnsIcon = "";
            ViewBag.ShowReturnsIcon = false;
            ViewBag.DGIcon = "";
            ViewBag.LHRFIcon = "";
            ViewBag.BrokerageIcon = "";
            ViewBag.RenewalIcon = "";

            // Check if PPST           
            //if (MyCarrDetail.ppstAccessorialFlag == null)
            //    MyCarrDetail.ppstAccessorialFlag = false;
            //ViewBag.showPPST = MyCarrDetail.ppstAccessorialFlag;
            bool PuroPostFlag = Utility.getPPSTFlag(idCARRDetail);
            ViewBag.showPPST = PuroPostFlag;

            //Details
            List<string> reqCorporateInfoFields = contractcontroller.CheckCorporateInfoRequiredFields(idCARRDetail);
            var corporateInfoIconDisplay = "none";
            if (reqCorporateInfoFields.Count > 0)
            {
                corporateInfoIconDisplay = "inline-block";
            }
            ViewBag.CorporateInfoIcon = "<span id='corporateInfowrench' style='display:" + corporateInfoIconDisplay + "' class='fas fa-wrench'>&nbsp;</span>";

            List<string> reqRenewalFields = contractcontroller.CheckRenewalRequiredFields(idCARRDetail);
            var renewalIconDisplay = "none";
            if (reqRenewalFields.Count > 0)
            {
                renewalIconDisplay = "inline-block";
            }
            ViewBag.RenewalIcon = "<span id='renewalwrench' style='display:" + renewalIconDisplay + "' class='fas fa-wrench'>&nbsp;</span>";

            List<string> reqCustFields = contractcontroller.CheckCustomerRequiredFields(idCARRDetail);
            var custIconDisplay = "none";
            if (reqCustFields.Count > 0)
            {
                custIconDisplay = "inline-block";
            }           
            ViewBag.CustDetailIcon = "<span id='custwrench' style='display:" + custIconDisplay + "' class='fas fa-wrench'>&nbsp;</span>";

            //Shipping Profile
            List<string> reqLocFields = contractcontroller.CheckRequiredLocations(idCARRDetail);
            var locIconDisplay = "none";
            if (reqLocFields.Count > 0)
            {
                locIconDisplay = "inline-block";
            }
            ViewBag.ShipProfIcon = "<span id='locwrench' style='display:" + locIconDisplay + "' class='fas fa-wrench'>&nbsp;</span>";

            //Estimate Revenue
            List<string> reqRevFields = contractcontroller.CheckRequiredRevenueEstimates(Session["accountname"].ToString(), idCARRDetail);
            var estRevIconDisplay = "none";
            if (reqRevFields.Count > 0)
            {
                estRevIconDisplay = "inline-block";
            }
            ViewBag.EstRevIcon = "<span id='estrevwrench' style='display: " + estRevIconDisplay + "' class='fas fa-wrench'>&nbsp;</span>";

            //Returns
            List<string> reqReturnFields = contractcontroller.CheckRequiredReturns(idCARRDetail);
            var returnIconDisplay = "none";
            if (reqReturnFields.Count > 0)
            {
                returnIconDisplay = "inline-block";
                ViewBag.ReturnsIcon = "<span class='fas fa-wrench'>&nbsp;</span>";
                ViewBag.ShowReturnsIcon = true;
            }
            ViewBag.ReturnsIcon = "<span id='returnswrench' style='display:" + returnIconDisplay + "' class='fas fa-wrench'>&nbsp;</span>";

            //Dangerous Goods
            List<string> reqDGFields = contractcontroller.CheckRequiredDG(idCARRDetail);
            var dgIconDisplay = "none";
            if (reqDGFields.Count > 0)
            {
                dgIconDisplay = "inline-block";
            }
            ViewBag.DGICon = "<span id='dgwrench' style='display:" + dgIconDisplay + "' class='fas fa-wrench'>&nbsp;</span>";

            //LHRF
            List<string> reqLHRFFields = contractcontroller.CheckRequiredLHRFForm(idCARRDetail);
            var lhrfIconDisplay = "none";
            if (reqLHRFFields.Count > 0)
            {
                lhrfIconDisplay = "inline-block";
            }
            ViewBag.LHRFIcon = "<span id='lhrfwrench' class='fas fa-wrench' style='display:" + lhrfIconDisplay + "'>&nbsp;</span>";

            //Brokerage
            bool reqBrokerage = contractcontroller.CheckForBrokerage(idCARRDetail);
            var brokerageIconDisplay = "none";
            if (reqBrokerage == true)
            {
                brokerageIconDisplay = "inline-block";
            }
            ViewBag.BrokerageIcon = "<span id='brokeragewrench' class='fas fa-wrench' style='display:" + brokerageIconDisplay + "'>&nbsp;</span>";

            //PuroPost
            List<string> reqPpstFields = contractcontroller.CheckRequiredPPSTFields(idCARRDetail);
            var ppstIconDisplay = "none";
            if (reqPpstFields.Count > 0)
            {
                ppstIconDisplay = "inline-block";
            }
            ViewBag.ppstIcon = "<span id='ppstwrench' style='display: " + ppstIconDisplay + "' class='fas fa-wrench'>&nbsp;</span>";

            // Account Details
            List<string> reqAcctDetailFields = contractcontroller.CheckAllRequiredAccountFields(idCARRDetail);
            var accountdetailIconDisplay = "none";
            if (reqAcctDetailFields.Count > 0)
            {
                accountdetailIconDisplay = "inline-block";
            }
            ViewBag.accountdetailIcon = "<span id='accountwrench' style='display: " + accountdetailIconDisplay + "' class='fas fa-wrench'>&nbsp;</span>";


            //locked by pricing, locked by another user, or completed 
            string iconname = "fas fa-lock-open";
            string infoText = "CARR is unlocked";
            string userName = Session["accountname"].ToString().Trim();
            
            
            string routedToName = routing.RoutedTo.Trim();
            if (!userName.ToLower().Equals(routedToName.ToLower()))
            {
                iconname = "fas fa-lock";
                infoText = "Locked By " + routedToName;
            }
            if (MyCarrDetail.PricingReqCompleteFlag == true)
            {
                iconname = "fas fa-key";
                infoText = "Locked By Pricing";
            }
           
            if (MyCarrDetail.CompletedFlag == true)
            {
                iconname = "fas fa-check-circle";
                infoText = "CARR has been Completed";
            }          
            //ViewBag.lockedIcon = "<span id='lockedicon' class='" + iconname + "' style='display:" + lockedIconDisplay + "'>&nbsp;</span>";
            //ViewBag.lockedIcon = "<span id='lockedicon' class='" + iconname + "' style='display:inline-block' data-toggle='tooltip' data-placement='top' title='" + infoText + "'>&nbsp;</span>";
            ViewBag.lockedIcon = "<i class='" + iconname + "' style='display:inline-block' data-toggle='tooltip' data-placement='top' title='" + infoText + "'>&nbsp;</i>";

            //< i class="fas fa-info-circle" data-toggle="tooltip" data-placement="top" title="When Freight Forwarding Single is selected, the LH Rating Tool is enabled"></i>

            ViewBag.idCARRDetail = idCARRDetail;
            ViewBag.MySections = MySections;
            ViewBag.Title = "CARR Container";            
            return PartialView();
        }
    }
}