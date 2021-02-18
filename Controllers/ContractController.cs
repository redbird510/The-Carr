using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Portal.Models;
using PI_Portal.Classes;
using System.IO;
using System.Web.WebPages;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Configuration;

namespace PI_Portal.Controllers
{
    public class ContractController : Controller
    {
        private Entities db = new Entities();
        //private prepumaSQLEntities pdb = new prepumaSQLEntities();

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            //MY CARRS
            //Routed to User, Salesperson is the User, or Created by the User 
            string userName = Session["accountname"].ToString();
            //string sql = @"select d.idCARRDetail,Concat(d.VersionNumber,' - ',d.VersionComment) 'Version',
            //                d.PricingReqCompleteDate, dd.Name 'Type',
            //                c.CustomerLegalName,d.SalesProfessional,d.SalesManager,
            //                con.Address1,con.City,con.State,
            //                r.RoutedTo,d.DecisionMakerBranch,reg.District,d.CreatedOn,d.CreatedBy,r.CreatedOn 'lastUpdated',
            //                d.DFLApproved
            //                from CAR.CARRDetail d
            //                join CAR.CARR c on c.idCARR=d.idCARR
            //                join CAR.Routing r on d.idRoutedTo = r.idRouting
            //                left join prepumaSQL.dbo.tblRegions reg on reg.Airport=d.DecisionMakerBranch
            //                left join CAR.Contacts con on con.idContact = d.idCorporateContact
            //                left join CAR.DropdownsOptions dd on (dd.value = d.idCARRType and dd.id=1)
            //                where d.ActiveFlag=1  and d.CompletedFlag != 1
            //                and 
            //                (r.RoutedTo =  '" + userName + "' or d.SalesProfessional = '" + userName + "' or c.CreatedBy = '" + userName +
            //                "') " +
            //                "order by C.customerlegalname,d.VersionNumber";

            //String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;

            //DataTable dt = new DataTable();
            //SqlConnection connection = new SqlConnection(connectionString);

            //connection.Open();
            //SqlDataAdapter da = new SqlDataAdapter(sql, connection);
            //da.Fill(dt);
            //connection.Close();
            //connection.Dispose();
            //ViewBag.result = dt;
            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                bool activeOnlyFlag = false;
                cmd = new SqlCommand("sp_GetMyCARRs", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@username", userName));
                da.SelectCommand = cmd;
                da.Fill(dt);
                ViewBag.projrevbyloc = dt;

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            ViewBag.result = dt;

          

            // Create a Delete flag column
            dt.Columns.Add("isDeletable", typeof(int));
            dt.Columns.Add("isDeactivated", typeof(int));
            foreach (DataRow row in dt.Rows)
            {
                int idCARRDetail = int.Parse(row["idCARRDetail"].ToString());
                CARRDetail MyCARR = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
                bool routedToCreator = row["RoutedTo"].ToString().Trim() == MyCARR.CreatedBy;
                bool notRoutedYet = false;

                //Check if CARR has not been routed anywhere yet to allow delete
                List<Routing> versions = db.Routings.Where(x => x.idCARRDetail == idCARRDetail).ToList();
                if (versions.Count == 1 && versions[0].RoutedTo == versions[0].CreatedBy)
                {
                    notRoutedYet = true;
                }


                //row["isDeletable"] = routedToCreator ? 1 : 0;
                row["isDeletable"] = false;
                if (notRoutedYet == true && routedToCreator == true)
                {
                    row["isDeletable"] = true;
                }

                if (row["Deactivate"] == System.DBNull.Value)
                    row["isDeactivated"] = 0;
                else if ((bool)(row["Deactivate"]) == false)
                    row["isDeactivated"] = 0;
                else
                    row["isDeactivated"] = 1;

            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetMyCARR()
        {
            string userName = Session["accountname"].ToString();
            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                cmd = new SqlCommand("sp_GetMyCARRs", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@username", userName));
                da.SelectCommand = cmd;
                da.Fill(dt);

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }
            if (dt != null)
            {
                StringBuilder sb = new StringBuilder();
                string[] columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();

                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dt.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                    sb.AppendLine(string.Join(",", fields));
                }
                string exportpath = ConfigurationManager.AppSettings["USPricingExport"].ToString();
                string exportFilename = "MyCarrs_" + userName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                string exportFilepath = exportpath + exportFilename;

                System.IO.File.WriteAllText(exportFilepath, sb.ToString());
                byte[] filedata = System.IO.File.ReadAllBytes(exportFilepath);
                string contentType = MimeMapping.GetMimeMapping(exportFilepath);
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = exportFilename,
                    Inline = true,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                System.IO.File.Delete(exportFilepath);
                return File(filedata, contentType);
            }
            return null;
        }


        [HttpGet]
        [Authorize]
        public ActionResult AllCARRs()
        {
            //MY TEAM CARRS
            //Descision Maker Branch within allowed Districts, or Where any location's origin branch is within allowd districts
            //Or Decision Maker Branch within allowed Regions
            //As well as Created by User or Saleperson is User
            string userName = Session["accountname"].ToString();
            
            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                bool activeOnlyFlag = false;
                cmd = new SqlCommand("sp_GetTeamCARRs", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@username", userName));
                cmd.Parameters.Add(new SqlParameter("@activeOnlyFlag", activeOnlyFlag));
                da.SelectCommand = cmd;
                da.Fill(dt);
                ViewBag.projrevbyloc = dt;

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            ViewBag.result = dt;

            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAllCARRs()
        {
            string userName = Session["accountname"].ToString();

            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                bool activeOnlyFlag = false;
                cmd = new SqlCommand("sp_GetTeamCARRs", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@username", userName));
                cmd.Parameters.Add(new SqlParameter("@activeOnlyFlag", activeOnlyFlag));
                da.SelectCommand = cmd;
                da.Fill(dt);

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }
            if (dt != null)
            {
                StringBuilder sb = new StringBuilder();
                string[] columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();

                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dt.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                    sb.AppendLine(string.Join(",", fields));
                }
                string exportpath = ConfigurationManager.AppSettings["USPricingExport"].ToString();
                string exportFilename = "AllCarrs_" + userName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                string exportFilepath = exportpath + exportFilename;

                System.IO.File.WriteAllText(exportFilepath, sb.ToString());
                byte[] filedata = System.IO.File.ReadAllBytes(exportFilepath);
                string contentType = MimeMapping.GetMimeMapping(exportFilepath);
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = exportFilename,
                    Inline = true,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                System.IO.File.Delete(exportFilepath);
                return File(filedata, contentType);
            }

            return null;
        }

        [HttpGet]
        [Authorize]
        public ActionResult ActiveCARRs()
        {
            string userName = Session["accountname"].ToString();
            
            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                bool activeOnlyFlag = true;
                cmd = new SqlCommand("sp_GetTeamCARRs", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@username", userName));
                cmd.Parameters.Add(new SqlParameter("@activeOnlyFlag", activeOnlyFlag));
                da.SelectCommand = cmd;
                da.Fill(dt);
                ViewBag.projrevbyloc = dt;

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            ViewBag.result = dt;

            return View();

        }

        [HttpGet]
        [Authorize]
        public ActionResult GetActiveCARRs()
        {
            string userName = Session["accountname"].ToString();

            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                bool activeOnlyFlag = true;
                cmd = new SqlCommand("sp_GetTeamCARRs", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@username", userName));
                cmd.Parameters.Add(new SqlParameter("@activeOnlyFlag", activeOnlyFlag));
                da.SelectCommand = cmd;
                da.Fill(dt);

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            if (dt != null)
            {
                StringBuilder sb = new StringBuilder();
                string[] columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();

                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dt.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                    sb.AppendLine(string.Join(",", fields));
                }
                string exportpath = ConfigurationManager.AppSettings["USPricingExport"].ToString();
                string exportFilename = "AllActiveCarrs_" + userName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                string exportFilepath = exportpath + exportFilename;

                System.IO.File.WriteAllText(exportFilepath, sb.ToString());
                byte[] filedata = System.IO.File.ReadAllBytes(exportFilepath);
                string contentType = MimeMapping.GetMimeMapping(exportFilepath);
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = exportFilename,
                    Inline = true,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                System.IO.File.Delete(exportFilepath);
                return File(filedata, contentType);
            }

            return null;

        }
        // TODO - Figure out why Session["accountname"] is null here
        public List<string> CheckRequiredRevenueEstimates(string username, int idCARRDetail)
        {
            // Check for missing row(s)
            int procCount = Utility.getEstRevData(idCARRDetail).Rows.Count;

            //int tableCount = db.CARREstRevSalesPricings.Count();
            int tableCount = db.CARREstRevSalesPricings.Where(x => x.idCARRDetail == idCARRDetail).Count();            

            bool missingRows = Convert.ToBoolean(Math.Abs(procCount - tableCount));

            if (missingRows)
            {
                for (int i = 0; i < Math.Abs(procCount - tableCount); i++)
                {
                    CARREstRevSalesPricing temp = new CARREstRevSalesPricing();
                    temp.idCARRDetail = idCARRDetail;
                    temp.idRevType = tableCount + (i + 1);
                    temp.CreatedBy = username;
                    temp.CreatedOn = DateTime.Now;
                    temp.ActiveFlag = true;

                    db.CARREstRevSalesPricings.Add(temp);
                }

                db.SaveChanges();
            }

            List<string> estRevList = new List<string>();


            bool valuesMissing = false;
            foreach (CARREstRevSalesPricing price in db.CARREstRevSalesPricings.Where(x => x.idCARRDetail == idCARRDetail))
            {
                if (price.EstWklyRevSales is null || price.EstWklyRevSales < 0 )
                {
                    valuesMissing = true;
                    //estRevList.Add("Value required for Sales Estimated Weekly Revenue " );
                }
            }

            if (valuesMissing == true)
            {
                estRevList.Add("Value required for Sales Estimated Weekly Revenue - Enter  zero for services that do not apply");
            }

            return estRevList;
        }

        public List<string> CheckRequiredPPSTFields(int idCARRDetail)
        {
            List<string> ppstList = new List<string>();
            List<string> returnList = new List<string>();

            var MyPuroPost = db.CARRPuroPosts.Where(x => x.idCARRDetail == idCARRDetail).ToList();

            foreach (CARRPuroPost ppst in MyPuroPost)
            {
                if (ppst.NormalYrValue is null || ppst.NormalYrValue < 0)
                {
                    ppstList.Add("Normal Year Qualifier $ missing.");
                }
                if (ppst.NormalYrShpmts is null || ppst.NormalYrShpmts < 0)
                {
                    ppstList.Add("Normal Year #Shipments missing.");
                }
                if (ppst.minWtRange is null || ppst.minWtRange < 0)
                {
                    ppstList.Add("Min W tRange missing.");
                }
                if (ppst.maxWtRange is null || ppst.maxWtRange < 0)
                {
                    ppstList.Add("Max Wt Range missing.");
                }
                if (db.CARRPuroPostFiles.Where(x => x.idCARRDetail == ppst.idCARRDetail && x.ActiveFlag == true).Count().Equals(0))
                {
                    ppstList.Add("Shipping Profile is missing.");
                }
              
            }

            //Returning just a message to point them to the PuroPost Screen
            // return ppstList;
            if (ppstList.Count >= 1)
            {
                returnList.Add("Missing Required Fields for PuroPost");
            }

            return returnList;
        }

        [HttpGet]
        [Authorize]
        public ActionResult CompletedCARRs()
        {
            //Compeleted CARRs - show only the selected Version
            string userName = Session["accountname"].ToString();
            //string sql = @" select d.idCARRDetail,Concat(d.VersionNumber,' - ',d.VersionComment) 'Version',
            //                d.PricingReqCompleteDate,d.CompletedDate, dd.Name 'Type',
            //                c.CustomerLegalName,d.SalesProfessional,d.SalesManager,
            //                con.Address1,con.City,con.State,
            //                r.RoutedTo,d.DecisionMakerBranch,reg.District,d.CreatedOn,d.CreatedBy,r.CreatedOn 'lastUpdated',
            //                d.DFLApproved
            //                from CAR.CARRDetail d
            //                join CAR.CARR c on c.idCARR=d.idCARR
            //                join CAR.Routing r on d.idRoutedTo = r.idRouting
            //                left join prepumaSQL.dbo.tblRegions reg on reg.Airport=d.DecisionMakerBranch
            //                left join CAR.Contacts con on con.idContact = d.idCorporateContact
            //                left join CAR.DropdownsOptions dd on (dd.value = d.idCARRType and dd.id=1)
            //                where d.ActiveFlag=1 and
            //                (District in (select District from COM.ApplicationDistrictsAllowed where ActiveDirectory = '" + userName +
            //                "') or DecisionMakerBranch in (select Region from COM.ApplicationRegionsAllowed where ActiveDirectory = '" + userName +
            //                "') or d.SalesProfessional = '" + userName +
            //                "' or c.CreatedBy = '" + userName +
            //                "') and d.CompletedFlag = 1 order by C.customerlegalname,d.VersionNumber";

            //String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;

            //DataTable dt = new DataTable();
            //SqlConnection connection = new SqlConnection(connectionString);

            //connection.Open();
            //SqlDataAdapter da = new SqlDataAdapter(sql, connection);
            //da.Fill(dt);
            //connection.Close();
            //connection.Dispose();
            //ViewBag.result = dt;
            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                bool activeOnlyFlag = false;
                cmd = new SqlCommand("sp_GetCompletedCARRs", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@username", userName));               
                da.SelectCommand = cmd;
                da.Fill(dt);
                ViewBag.projrevbyloc = dt;

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            ViewBag.result = dt;


            return View();

        }

        [HttpGet]
        [Authorize]
        public ActionResult GetCompletedCARRs()
        {
            string userName = Session["accountname"].ToString();

            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                cmd = new SqlCommand("sp_GetCompletedCARRs", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@username", userName));
                da.SelectCommand = cmd;
                da.Fill(dt);

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }
            if (dt != null)
            {
                StringBuilder sb = new StringBuilder();
                string[] columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();

                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dt.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                    sb.AppendLine(string.Join(",", fields));
                }
                string exportpath = ConfigurationManager.AppSettings["USPricingExport"].ToString();
                string exportFilename = "CompletedCarrs_" + userName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                string exportFilepath = exportpath + exportFilename;

                System.IO.File.WriteAllText(exportFilepath, sb.ToString());
                byte[] filedata = System.IO.File.ReadAllBytes(exportFilepath);
                string contentType = MimeMapping.GetMimeMapping(exportFilepath);
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = exportFilename,
                    Inline = true,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                System.IO.File.Delete(exportFilepath);
                return File(filedata, contentType);
            }
            return null;

        }

        [HttpPost]
        [Authorize]
        public ActionResult CorporateInfo()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);

           

            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }
            List<SelectListItem> statelist = HelperDropdowns.getStateList();

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            ViewBag.stateList = statelist;

            //ViewBag.DisableChanges = disableChanges(idCARRDetail);
            //Corp info is different than standard DisableChanges where it's locked down after Pricing Rates are provided
            string userName = Session["accountname"].ToString();
            var routing = db.Routings.Where(x => x.idRouting == MyCarrDetail.idRoutedTo).FirstOrDefault();
            string routedToName = routing.RoutedTo.Trim();
            string userrole = Session["userrole"].ToString().ToLower();
            ViewBag.DisableChanges = true;

            //For Corporate Ino Page, if CARR is routed to the sales user, allow changes
            if ((userrole == "sales"  || userrole == "salesdsm" || userrole  == "districtmanager" || userrole == "contracts") && routedToName.ToLower() == userName.ToLower())
            {
                ViewBag.DisableChanges = false;
            }
            //end DisableChanges Check

            // enable change of corp info for certain user roles
            if ((userrole == "contracts" || userrole == "districtadmin" || userrole == "decisionsupport" || userrole == "salesdsm")  && routedToName.ToLower() == userName.ToLower())
            {
                ViewBag.DisableChanges = false;
            }
            //if (userrole == "districtadmin" && routedToName == userName)
            //{
            //    ViewBag.DisableChanges = false;
            //}
            if (MyCarrDetail.CompletedFlag == true)
            {
                ViewBag.DisableChanges = true;
            }

            ViewBag.userrole = Session["userrole"].ToString();
            ViewBag.Title = "Contract: " + MyCarr.CustomerLegalName.ToString() + "   Version: " + MyCarrDetail.VersionNumber.ToString();
            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult Details()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);

            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }

            if (String.IsNullOrEmpty(MyCarrDetail.BtoBBtoC))
                MyCarrDetail.BtoBBtoC = "";
            MyCarrDetail.BtoBBtoC = MyCarrDetail.BtoBBtoC.Trim();


            List<SelectListItem> salesList = HelperDropdowns.getSalesList();
            
            List<SelectListItem> salesDMList = HelperDropdowns.getSalesDMList();
            List<SelectListItem> salesDSMList = HelperDropdowns.getSalesDSMList();
            List<SelectListItem> regionlist = HelperDropdowns.getRegionList();
            List<SelectListItem> statelist = HelperDropdowns.getStateList();
            List<SelectListItem> ISPList = HelperDropdowns.getISPSRIDList();

            // Retrieve flags from DB
            CARRDetail CARRDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            ViewBag.courierFlag = CARRDetail.courierAccessorialFlag ?? false;
            ViewBag.freightFlag = CARRDetail.freightAccessorialFlag ?? false;
            ViewBag.cpcFlag = CARRDetail.cpcAccessorialFlag ?? false;
            ViewBag.ppstFlag = CARRDetail.ppstAccessorialFlag ?? false;
            ViewBag.ltlyyzFlag = CARRDetail.ltlyyzFlag ?? false;
            ViewBag.ltlyvrFlag = CARRDetail.ltlyvrFlag ?? false;
            ViewBag.ltlywgFlag = CARRDetail.ltlywgFlag ?? false;
            ViewBag.ltlyulFlag = CARRDetail.ltlyulFlag ?? false;
            

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            ViewBag.salesList = salesList;
            ViewBag.salesDMList = salesDMList;
            ViewBag.salesDSMList = salesDSMList;
            ViewBag.regionList = regionlist;
            ViewBag.stateList = statelist;
            ViewBag.ispList = ISPList;

            ViewBag.SalesForce = false;
            if (MyCarrDetail.idCarrType == 1 || MyCarrDetail.idCarrType == 2 || MyCarrDetail.idCarrType == 4)
            {
                ViewBag.SalesForce = true;
            }

            ViewBag.userRole = Session["userrole"].ToString();
            

            ViewBag.DisableChanges = disableChanges(idCARRDetail);
            //MK Handle all logic in DisableChanges
            //If this is a pricing user, disable Details editing
            //if (Session["userrole"].ToString() == "Pricing")
            //{
            //    ViewBag.DisableChanges = true;
            //}

            //For Details Page, if this user is a DistrictAdmin, and the CARR is routed to the user, allow changes
            string userName = Session["accountname"].ToString().ToLower();
            var routing = db.Routings.Where(x => x.idRouting == MyCarrDetail.idRoutedTo).FirstOrDefault();
            string routedToName = routing.RoutedTo.Trim().ToLower();
            if (Session["userrole"].ToString() == "DistrictAdmin" && routedToName == userName)
            {
                ViewBag.DisableChanges = false;
            }

            ViewBag.Title = "Contract: " + MyCarr.CustomerLegalName.ToString() + "   Version: " + MyCarrDetail.VersionNumber.ToString();
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Accessorials()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);

            ViewBag.Title = "Accessorials";

            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            DataTable dtSvc = new DataTable();
            DataTable dtDGFlag = new DataTable();
            DataTable dtffwFlag = new DataTable();
            try
            {
                string sqlDG = @"select idDG,
                                        Class,
			                            Description,
                                        acceptableFlag
	                            FROM [PURO_APPS].[CAR].[DangerousGoods]";
                DataTable dtDG = new DataTable();
                SqlDataAdapter daDG = new SqlDataAdapter(sqlDG, connection);
                daDG.Fill(dtDG);
                ViewBag.resultDG = dtDG;

                string sqlSvc = @"select  distinct ss.AccessorialType
                                        from CAR.CARRDetail c
                                        join CAR.CARRLocation l on l.idCARRDetail = c.idCARRDetail
                                        join CAR.Service s on s.idLocation = l.idLocation
                                        join CAR.ShippingServices ss on ss.idShippingService = s.idShippingService
                                        where s.ActiveFlag=1 and l.ActiveFlag=1 and c.idCARRDetail = ";
                sqlSvc = sqlSvc + idCARRDetail;

                SqlDataAdapter daSvc = new SqlDataAdapter(sqlSvc, connection);
                daSvc.Fill(dtSvc);



                string sqlffwFlag = @"select count(*) ffwFlags
                                        from CAR.CARRLocation                                        
                                        where ActiveFlag=1 and (FFWDType='Single' or FFWDType='Team') and idCARRDetail = ";
                sqlffwFlag = sqlffwFlag + idCARRDetail;

                SqlDataAdapter daffwFlag = new SqlDataAdapter(sqlffwFlag, connection);
                daffwFlag.Fill(dtffwFlag);


                ViewBag.DisableChanges = disableChanges(idCARRDetail);

                ViewBag.resultAccessorial = GetAccessorials(idCARRDetail);
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }

            //Update Flags used later for printing
            ViewBag.courierAccessorialFlag = 0;
            MyCarrDetail.courierAccessorialFlag = false;
            ViewBag.cpcAccessorialFlag = 0;
            MyCarrDetail.cpcAccessorialFlag = false;
            ViewBag.freightAccessorialFlag = 0;
            MyCarrDetail.freightAccessorialFlag = false;
            ViewBag.ppstAccessorialFlag = 0;
            MyCarrDetail.ppstAccessorialFlag = false;

            foreach (DataRow row in dtSvc.Rows)
            {

                switch (row["AccessorialType"].ToString().ToLower())
                {
                    case "courier":
                        ViewBag.courierAccessorialFlag = 1;
                        MyCarrDetail.courierAccessorialFlag = true;
                        break;
                    case "cpc":
                        ViewBag.cpcAccessorialFlag = 1;
                        MyCarrDetail.cpcAccessorialFlag = true;
                        break;
                    case "freight":
                        ViewBag.freightAccessorialFlag = 1;
                        MyCarrDetail.freightAccessorialFlag = true;
                        break;
                    case "ppst":
                        ViewBag.ppstAccessorialFlag = 1;
                        MyCarrDetail.ppstAccessorialFlag = true;
                        break;
                }

            }
            db.SaveChanges();

            ViewBag.returnsFlag = 1;
            if (MyCarrDetail.returnsFlag == null || MyCarrDetail.returnsFlag == false)
            {
                ViewBag.returnsFlag = 0;
            }




            //Freight Accessorials apply if there is any linehaul
            foreach (DataRow ffwrow in dtffwFlag.Rows)
            {
                if ((int)ffwrow["ffwFlags"] > 0)
                {
                    ViewBag.freightAccessorialFlag = 1;
                }

            }


            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult DG()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);

            ViewBag.Title = "Dangerous Goods";

            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            DataTable dtSvc = new DataTable();
            DataTable dtDGFlag = new DataTable();
            DataTable dtffwFlag = new DataTable();
            try
            {

                string sqlDG = @"select idDG,
                                        Class,
			                            Description,
                                        acceptableFlag
	                            FROM [PURO_APPS].[CAR].[DangerousGoods]";
                DataTable dtDG = new DataTable();
                SqlDataAdapter daDG = new SqlDataAdapter(sqlDG, connection);
                daDG.Fill(dtDG);
                ViewBag.resultDG = dtDG;

                string sqlDGFlag = @"select count(l.dgFlag) dgFlags
                                        from CAR.CARRDetail c
                                        join CAR.CARRLocation l on l.idCARRDetail = c.idCARRDetail
                                        join CAR.Service s on s.idLocation = l.idLocation
                                        join CAR.ShippingServices ss on ss.idShippingService = s.idShippingService
                                        where s.ActiveFlag=1 and l.ActiveFlag=1 and l.DGFlag=1 and c.idCARRDetail = ";
                sqlDGFlag = sqlDGFlag + idCARRDetail;

                SqlDataAdapter daDGFlag = new SqlDataAdapter(sqlDGFlag, connection);
                daDGFlag.Fill(dtDGFlag);

                ViewBag.DisableChanges = disableChanges(idCARRDetail);

            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }
            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            ViewBag.dgFlag = 0;
            foreach (DataRow dgrow in dtDGFlag.Rows)
            {
                if ((int)dgrow["dgFlags"] > 0)
                {
                    ViewBag.dgFlag = 1;
                }

            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Brokerage()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            ViewBag.idCARRDetail = idCARRDetail;

            ViewBag.Title = "Brokerage";

            string sql = @" select *
             from CAR.CARRBrokerageDetails
            where idCARRDetail = ";
            sql = sql + idCARRDetail;
            sql = sql + "and ActiveFlag = 1";
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, connection);
            da.Fill(dt);
            connection.Close();
            connection.Dispose();
            ViewBag.result = dt;

            var MyBrokerage = db.CARRBrokerages.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }


            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyBrokerage = MyBrokerage;
            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);

            ViewBag.BrokerageFlagselected = "";
            if (MyCarrDetail.brokerageFlag == true)
                ViewBag.BrokerageFlagselected = "1";
            if (MyCarrDetail.brokerageFlag == false)
                ViewBag.BrokerageFlagselected = "0";

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Upload()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            string desc = Request.Form["Description"];
            string username = Session["accountname"].ToString();
            string userrole = Session["userrole"].ToString();
            ViewBag.userrole = userrole;

            ViewBag.Title = "Contracts - Upload";
            string sql = @" select idFileUpload, FilePath, Description, CreatedBy, CreatedOn 
                        from CAR.CARRFileUploads 
                        where ActiveFlag = 1 and PricingOnlyFlag = 0 and idCARRDetail = ";
            sql = sql + idCARRDetail;
            sql = sql + "order by CreatedOn desc";
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

            ViewBag.DisableChanges = disableChanges(idCARRDetail);

            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);

            // Certain roles are allowed to upload , all other users must either have the CARR routed to them or be viewing an non-pricing-locked CARR
            bool routedToUser = db.Routings.Where(x => x.idRouting == MyCarrDetail.idRoutedTo).FirstOrDefault().RoutedTo == Session["accountname"].ToString();
            bool pricingLocked = MyCarrDetail.PricingReqCompleteDate != null;
            string userRole = Session["userrole"].ToString();
            if (userRole == "Contracts" || userRole == "DecisionSupport" || userRole == "DistrictAdmin")
            {
                ViewBag.DisableFileUpload = false;
            }
            else if (routedToUser && !pricingLocked)
            {
                ViewBag.DisableFileUpload = false;
            }
            else
            {
                ViewBag.DisableFileUpload = true;
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Exceptions()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            ViewBag.idCARRDetail = idCARRDetail;

            ViewBag.Title = "Exceptions";

            string sql = @" select e.*,
		                das.Name as 'ApprovalStatus'
                        from CAR.Exceptions e
                        join CAR.DropdownsOptions das on e.ApprovalType = das.Value
                        where ActiveFlag = 1 and idCARRDetail = ";
            sql = sql + idCARRDetail;
            sql = sql + "order by idException";
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, connection);
            da.Fill(dt);
            connection.Close();
            connection.Dispose();
            ViewBag.result = dt;

            var MyExceptions = db.Exceptions.FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }


            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyExceptions = MyExceptions;
            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);
            string userrole = Session["userrole"].ToString();
            ViewBag.DisableExButton = ViewBag.DisableChanges;
            if (userrole == "SalesDSM" || userrole == "DistrictManager" || userrole == "Pricing" || userrole == "Sales")
            {
                ViewBag.DisableExButton = false;
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Notes()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);

            ViewBag.Title = "Notes";

            string sql = @" select *
             from CAR.Notes
            where idCARRDetail = ";
            sql = sql + idCARRDetail;
            sql = sql + "and ActiveFlag = 1";
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, connection);
            da.Fill(dt);
            connection.Close();
            connection.Dispose();
            ViewBag.result = dt;

            var MyNotes = db.Notes.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }


            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);
            // Certain roles are allowed to add notes, all other users must either have the CARR routed to them or be viewing an non-pricing-locked CARR
            bool routedToUser = db.Routings.Where(x => x.idRouting == MyCarrDetail.idRoutedTo).FirstOrDefault().RoutedTo == Session["accountname"].ToString();
            bool pricingLocked = MyCarrDetail.PricingReqCompleteDate != null;
            string userRole = Session["userrole"].ToString();
            if (userRole == "Contracts" || userRole == "DecisionSupport")
            {
                ViewBag.DisableAddNote = false;
                ViewBag.DisableChanges = false;
            }
            else if (routedToUser && !pricingLocked)
            {
                ViewBag.DisableAddNote = false;
            }
            else
            {
                ViewBag.DisableAddNote = true;
            }

            

            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyNotes = MyNotes;

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult RequestForms()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);

            ViewBag.Title = "Linehaul Request Forms";

            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();

                string sql = @" select *
                    FROM CAR.CARRLHRates r
                    join CAR.CARRLocation l on l.idLocation = r.idLocation
                    where l.idCARRDetail = ";
                sql = sql + idCARRDetail;
                sql = sql + " and r.ActiveFlag = 1 and l.ActiveFlag = 1";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, connection);
                da.Fill(dt);
                ViewBag.resultLHRating = dt;

                string sqlLHRF = @" select *
                    FROM CAR.CARRLHRF r
                   join CAR.CARRLocation l on l.idLocation = r.idLocation
                    where r.LHRFType != 'Returns' and r.ActiveFlag = 1 and l.ActiveFlag = 1 and l.idCARRDetail = ";
                sqlLHRF = sqlLHRF + idCARRDetail + " order by LHRFType";
                DataTable dtLHRF = new DataTable();
                SqlDataAdapter daLHRF = new SqlDataAdapter(sqlLHRF, connection);
                daLHRF.Fill(dtLHRF);
                ViewBag.resultLHRF = dtLHRF;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }

            List<string> reqlist = CheckRequiredLHRFForm(idCARRDetail);
            string ReqHdr = CheckRequiredHdr(reqlist);
            ViewBag.ReqHdr = ReqHdr;
            ViewBag.MyReqlist = reqlist;

            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Returns()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);

            var MyLHRF = db.CARRLHRFs.Where(x => x.idCARRDetail == idCARRDetail && x.LHRFType == "Returns" && x.ActiveFlag == true).FirstOrDefault();

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR && x.ActiveFlag == true).FirstOrDefault();
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }


            List<LHRFAddresses> puAddressList = getLHRFDestination();
            List<LHRFAddresses> destAddressList = getLHRFOrigin(idCARRDetail);
            destAddressList[0].DropdownOption = "Select Destination";
            puAddressList[0].DropdownOption = "Select Pickup Origin";

            ViewBag.puOriginList = puAddressList;
            ViewBag.destList = destAddressList;

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.idCARRDetail = idCARRDetail;
            ViewBag.MyLHRF = MyLHRF;

            ViewBag.ReturnsFlagselected = "";
            if (MyCarrDetail.returnsFlag == true)
                ViewBag.ReturnsFlagselected = "1";
            if (MyCarrDetail.returnsFlag == false)
                ViewBag.ReturnsFlagselected = "0";

            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);

            ViewBag.Title = "Contracts - Returns";
            return View();
        }




        [HttpPost]
        [Authorize]
        public ActionResult EditLHRF()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            int idLHRF = int.Parse(Request.Form["idlhrf"]);
        
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
           

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            

            var lhrf = db.CARRLHRFs.Where(x => x.idLHRF == idLHRF).FirstOrDefault();
            var MyLocation = db.CARRLocations.Where(x => x.idLocation== lhrf.idLocation).FirstOrDefault();
            ViewBag.MyLocation = MyLocation;

            List<LHRFAddresses> puAddressList = getLHRFOrigin(idCARRDetail);
            List<LHRFAddresses> destAddressList = getLHRFDestination();

            ViewBag.puOriginList = puAddressList;
            ViewBag.destList = destAddressList;
            ViewBag.Title = "Edit LHRF";
            ViewBag.MyLHRF = lhrf;

            List<string> puDays = new List<string>();
            puDays.Add(lhrf.MondayPickup.ToString());
            puDays.Add(lhrf.TuesdayPickup.ToString());
            puDays.Add(lhrf.WednesdayPickup.ToString());
            puDays.Add(lhrf.ThursdayPickup.ToString());
            puDays.Add(lhrf.FridayPickup.ToString());
            puDays.Add(lhrf.SaturdayPickup.ToString());
            puDays.Add(lhrf.SundayPickup.ToString());
            ViewBag.puDays = puDays;

            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);
            return View(lhrf);
        }

        

        [HttpPost]
        [Authorize]
        public ActionResult Submit()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();

            bool DisableChanges = disableChanges(idCARRDetail);
            ViewBag.DisableChanges = DisableChanges;

            ViewBag.Title = "Submission and Approvals";

            string userName = Session["accountname"].ToString();


            //Determine what is still required
            ViewBag.SubmitDisable = false;
            List<string> reqlist = CheckAllRequiredFields(idCARRDetail);
            //Requirements needed
            bool requirementsFlag = reqlist.Count > 0 ? true : false;
            if (reqlist.Count > 0 || DisableChanges == true)
            {
                ViewBag.SubmitDisable = true;              
                
            }
            string ReqHdr = CheckRequiredHdr(reqlist);
            ViewBag.ReqHdr = ReqHdr;

            ViewBag.MyReqlist = reqlist;
           
            ViewBag.idCARRDetail = idCARRDetail;


            //Get Approved By List
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            string sql = @"select a.idApproval,a.ApprovedBy,ar.Role,a.DateApproved,a.idCARRDetail,s.ImageURL,Concat('..\',s.ImageURL) ImageURL2
             from CAR.Approvals a
			 left outer join CAR.Signatures s on a.ApprovedBy=s.ActiveDirectory
			 left outer join COM.applicationUserRoles ur on ur.ActiveDirectory = a.ApprovedBy and idApplication = 1
			 left outer join COM.applicationRoles ar on ar.id = ur.idRole and ar.idApplication = 1
            where idCARRDetail = ";
            sql = sql + idCARRDetail;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, connection);
            da.Fill(dt);
            ViewBag.result = dt;

            //Get Routed List
            string routesql = @"select RoutedTo,DateRoutedTo,CreatedBy,RoutingMsg from CAR.Routing where idCARRDetail = ";
            routesql = routesql + idCARRDetail;            
            DataTable routedt = new DataTable();
            //SqlConnection connection = new SqlConnection(connectionString);
            //connection.Open();
            SqlDataAdapter routeda = new SqlDataAdapter(routesql, connection);
            routeda.Fill(routedt);
            ViewBag.Routing = routedt;

            //Get the Approvals Needed List     
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da2 = new SqlDataAdapter();
            DataTable dt2 = new DataTable();
            try
            {
                cmd = new SqlCommand("sp_GetApprovalList", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCARRDetail", MyCarrDetail.idCARRDetail));
                da.SelectCommand = cmd;
                da.Fill(dt2);

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            ViewBag.ApprovalsNeeded = dt2;

            string userrole = Session["userrole"].ToString();
            ViewBag.userrole = userrole;
            if (userrole == "Sales")
            {
                if (MyCarrDetail.idCarrType == 3)
                {
                    //For Renewals just get the first person in the approval list
                    ViewBag.SubmitTo = dt2.Rows[0][1].ToString().Trim();
                }
                else
                {
                    ViewBag.SubmitTo = MyCarrDetail.SalesManager;
                }
                
            }
            else
            {
                //Look for current user in the approvals needed list, if it's not the last row, select the next Approver              
                for (int i = 0; i < dt2.Rows.Count - 1; i++)
                {
                    if (dt2.Rows[i][1].ToString().ToLower() == userName.ToLower())
                    {
                        if (i + 1 < dt2.Rows.Count)
                            ViewBag.SubmitTo = dt2.Rows[i + 1][1].ToString().Trim();
                    }
                }

            }
           


            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }

            //Three Different Possibilities for Disabling Submit Button                        
            var routing = db.Routings.Where(x => x.idRouting == MyCarrDetail.idRoutedTo).FirstOrDefault();
            string routedToName = routing.RoutedTo.Trim();
            ViewBag.routedToName = routedToName;

            //CARR is routed to a different user
            bool routedFlag = !userName.ToLower().Equals(routedToName.ToLower()) ? true : false;
            //pricing is complete
            bool beforepricingFlag = MyCarrDetail.PricingReqCompleteFlag.Equals(true) ? false : true;
            //CARR is complete
            bool completedFlag = MyCarrDetail.CompletedFlag.Equals(true) ? true : false;


            //GET ROUNTING LIST OF USERS THEY CAN ROUTE TO
           // ViewBag.userList = HelperDropdowns.getRoutingUserList(idCARRDetail, userrole, pricingFlag, ViewBag.SubmitTo);
            ViewBag.userList = HelperDropdowns.getRoutingUserList(idCARRDetail, userrole, beforepricingFlag);

            //Contracts and Operations - if not routed to this user, Routing User List should be anyone in their own user group
            if (routedFlag == true)
            {
                if (userrole == "Contracts")
                {
                    ViewBag.userList = HelperDropdowns.getContractsUserList();
                }
                if (userrole == "Operations")
                {
                    ViewBag.userList = HelperDropdowns.getUserList();
                }
            }


            ViewBag.disableSubmit = (ViewBag.userList.Count == 0);

            ViewBag.RoutedFlag = routedFlag;
            ViewBag.PricingFlag = beforepricingFlag;
            ViewBag.CompletedFlag = completedFlag;
            ViewBag.RequirementsFlag = requirementsFlag;

            IList<tempCarrLoc> MyLocations = db.CARRLocations.Where(x => x.CARRDetail.idCARRDetail == idCARRDetail && x.ActiveFlag == true).Select(p => new tempCarrLoc() { LocalSRID = p.LocalSRID.ToLower(), LocationName = p.LocationName, StrategicSRID = p.StrategicSRID.ToLower() }).ToList();
            IList<String> ReqLocations = new List<String>();
            foreach (tempCarrLoc cl in MyLocations)
            {
                if (cl._LocalSRID == null && cl._StrategicSRID == null)
                {
                    ReqLocations.Add("Missing SRID for " + cl.LocationName);
                }
            }
            ViewBag.ReqLocations = ReqLocations;

            return View();
        }

        public class tempCarrLoc
        {
            public string LocalSRID { set; get; }
            public string _LocalSRID
            {
                get
                {
                    if (String.IsNullOrEmpty(LocalSRID))
                        LocalSRID = null;
                    else if (LocalSRID.Contains("null"))
                        LocalSRID = null;
                    return LocalSRID;
                }
                set { LocalSRID = value; }
            }
            public string StrategicSRID { set; get; }
            public string _StrategicSRID
            {
                get
                {
                    if (String.IsNullOrEmpty(StrategicSRID))
                        StrategicSRID = null;
                    else if (StrategicSRID.Contains("null"))
                        StrategicSRID = null;
                    return StrategicSRID;
                }
                set { StrategicSRID = value; }
            }
            public string LocationName { set; get; }
        }


        [HttpPost]
        [Authorize]
        public ActionResult Locations()
        {
            ViewBag.Title = "Locations";
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            var MyLocations = db.CARRLocations.Where(x => x.CARRDetail.idCARRDetail == idCARRDetail && x.ActiveFlag == true);
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);
            var AllServices = db.ShippingServices;
            //Details used in Partial Info Bar
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyLocations = MyLocations;
            ViewBag.AllServices = AllServices;
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }

            //Call Stored Procedure for Estimated Revenue by Service for this Location         
            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                //cmd = new SqlCommand("sp_CalcRevBySvc", cnn);
                cmd = new SqlCommand("sp_CalcRevBySvc", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCARRDetail", MyCarrDetail.idCARRDetail));
                cmd.Parameters.Add(new SqlParameter("@idLocation", "all"));
                cmd.Parameters.Add(new SqlParameter("@detailFlag", 1));
                cmd.Parameters.Add(new SqlParameter("@totalBy", "location"));
                da.SelectCommand = cmd;
                da.Fill(dt);
                ViewBag.projrevbyloc = dt;

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd2 = new SqlCommand();
            SqlDataAdapter da2 = new SqlDataAdapter();
            DataTable dt2 = new DataTable();
            try
            {
                cmd2 = new SqlCommand("sp_CalcRevBySvc", cnn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add(new SqlParameter("@idCARRDetail", MyCarrDetail.idCARRDetail));
                cmd2.Parameters.Add(new SqlParameter("@idLocation", "all"));
                cmd2.Parameters.Add(new SqlParameter("@detailFlag", "0"));
                cmd2.Parameters.Add(new SqlParameter("@totalBy", "location"));
                da2.SelectCommand = cmd2;
                da2.Fill(dt2);
                ViewBag.projrevtotal = dt2;
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd3 = new SqlCommand();
            SqlDataAdapter da3 = new SqlDataAdapter();
            DataTable dt3 = new DataTable();
            try
            {
                cmd3 = new SqlCommand("sp_CalcRevBySvc", cnn);
                cmd3.CommandType = CommandType.StoredProcedure;
                cmd3.Parameters.Add(new SqlParameter("@idCARRDetail", MyCarrDetail.idCARRDetail));
                cmd3.Parameters.Add(new SqlParameter("@idLocation", "all"));
                cmd3.Parameters.Add(new SqlParameter("@detailFlag", "1"));
                cmd3.Parameters.Add(new SqlParameter("@totalBy", "service"));
                da3.SelectCommand = cmd3;
                da3.Fill(dt3);
                ViewBag.projrevbysvc = dt3;
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }
            //Service Totals 
            string sql = @" select ss.Description,
             ISNULL(sum(Case When PickUpFrequency = 'Month' Then Round(NumberShipments*12/52,1)
            When PickUpFrequency = 'Year' Then Round(NumberShipments/52,1)
            When PickUpFrequency = 'Week' Then NumberShipments
             Else 0
            End),0) as ShipmentsPerWeek
             from CAR.ServiceVolume sv
             join CAR.Service s on s.idService = sv.idService
             join CAR.CARRLocation cl on s.idLocation = cl.idLocation
            join CAR.CARRDetail cd on cd.idCARRDetail = cl.idCARRDetail
             join CAR.ShippingServices ss on ss.idShippingService = s.idShippingService
            where sv.ActiveFlag = 1 and s.ActiveFlag=1 and cl.ActiveFlag=1 and cd.idCARRDetail = ";
            sql = sql + MyCarrDetail.idCARRDetail;
            sql = sql + "group by ss.Description";
            //String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            DataTable dt4 = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter da4 = new SqlDataAdapter(sql, connection);
            da4.Fill(dt4);
            connection.Close();
            connection.Dispose();
            ViewBag.svctotals = dt4;
            //Determine what is still required
            List<string> reqlist = CheckRequiredLocations(idCARRDetail);

            //For New Account we don't need all the service volumes entered, skip the unfinished location check
            List<string> unfinishedLocs = new List<string>();
            if (MyCarrDetail.idCarrType != 5)
            {
                unfinishedLocs = GetUnfinishedLocList(idCARRDetail);
            }            
            ViewBag.MyUnfinishedLocList = unfinishedLocs;

            ViewBag.DisableChanges = disableChanges(idCARRDetail);
            

            ViewBag.MyReqlist = reqlist;
            string ReqHdr = CheckRequiredHdr(reqlist);
            ViewBag.ReqHdr = ReqHdr;

            //AccountCreationFlag
            bool AcctCreation = false;
            if (MyCarrDetail.idCarrType == 5)
            {
                AcctCreation = true;
            }
            ViewBag.AcctCreation = AcctCreation;
            //Renewals
            bool Renewal = false;
            if (MyCarrDetail.idCarrType == 3)
            {
                Renewal = true;
            }
            ViewBag.Renewal = Renewal;

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult LocationsProfile()
        {
            ViewBag.Title = "Locations";
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            var MyLocations = db.CARRLocations.Where(x => x.CARRDetail.idCARRDetail == idCARRDetail && x.ActiveFlag == true);
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);
            var AllServices = db.ShippingServices;
            //Details used in Partial Info Bar
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyLocations = MyLocations;
            ViewBag.AllServices = AllServices;
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }

            //Call Stored Procedure for Estimated Revenue by Service for this Location         
            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                //cmd = new SqlCommand("sp_CalcRevBySvc", cnn);
                cmd = new SqlCommand("sp_CalcRevBySvc", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCARRDetail", MyCarrDetail.idCARRDetail));
                cmd.Parameters.Add(new SqlParameter("@idLocation", "all"));
                cmd.Parameters.Add(new SqlParameter("@detailFlag", 1));
                cmd.Parameters.Add(new SqlParameter("@totalBy", "location"));
                da.SelectCommand = cmd;
                da.Fill(dt);
                ViewBag.projrevbyloc = dt;

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd2 = new SqlCommand();
            SqlDataAdapter da2 = new SqlDataAdapter();
            DataTable dt2 = new DataTable();
            try
            {
                cmd2 = new SqlCommand("sp_CalcRevBySvc", cnn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add(new SqlParameter("@idCARRDetail", MyCarrDetail.idCARRDetail));
                cmd2.Parameters.Add(new SqlParameter("@idLocation", "all"));
                cmd2.Parameters.Add(new SqlParameter("@detailFlag", "0"));
                cmd2.Parameters.Add(new SqlParameter("@totalBy", "location"));
                da2.SelectCommand = cmd2;
                da2.Fill(dt2);
                ViewBag.projrevtotal = dt2;
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd3 = new SqlCommand();
            SqlDataAdapter da3 = new SqlDataAdapter();
            DataTable dt3 = new DataTable();
            try
            {
                cmd3 = new SqlCommand("sp_CalcRevBySvc", cnn);
                cmd3.CommandType = CommandType.StoredProcedure;
                cmd3.Parameters.Add(new SqlParameter("@idCARRDetail", MyCarrDetail.idCARRDetail));
                cmd3.Parameters.Add(new SqlParameter("@idLocation", "all"));
                cmd3.Parameters.Add(new SqlParameter("@detailFlag", "1"));
                cmd3.Parameters.Add(new SqlParameter("@totalBy", "service"));
                da3.SelectCommand = cmd3;
                da3.Fill(dt3);
                ViewBag.projrevbysvc = dt3;
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }
            //Service Totals 
            string sql = @" select ss.Description,
             sum(Case When PickUpFrequency = 'Month' Then Round(NumberShipments*12/52,1)
            When PickUpFrequency = 'Year' Then Round(NumberShipments/52,1)
            When PickUpFrequency = 'Week' Then NumberShipments
             Else 0
            End) as ShipmentsPerWeek
             from CAR.ServiceVolume sv
             join CAR.Service s on s.idService = sv.idService
             join CAR.CARRLocation cl on s.idLocation = cl.idLocation
            join CAR.CARRDetail cd on cd.idCARRDetail = cl.idCARRDetail
             join CAR.ShippingServices ss on ss.idShippingService = s.idShippingService
            where s.ActiveFlag=1 and cl.ActiveFlag=1 and cd.idCARRDetail = ";
            sql = sql + MyCarrDetail.idCARRDetail;
            sql = sql + "group by ss.Description";
            //String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            DataTable dt4 = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter da4 = new SqlDataAdapter(sql, connection);
            da4.Fill(dt4);
            connection.Close();
            connection.Dispose();
            ViewBag.svctotals = dt4;
            //Determine what is still required
            List<string> reqlist = CheckRequiredLocations(idCARRDetail);

            //For New Account we don't need all the service volumes entered, skip the unfinished location check
            List<string> unfinishedLocs = new List<string>();
            if (MyCarrDetail.idCarrType != 5)
            {
                unfinishedLocs = GetUnfinishedLocList(idCARRDetail);
            }
            ViewBag.MyUnfinishedLocList = unfinishedLocs;

            ViewBag.DisableChanges = disableChanges(idCARRDetail);
            //MK keep all disble logic in disableChanges
            //If this is a pricing user, disable Locations editing
            //if (Session["userrole"].ToString() == "Pricing")
            //{
            //    ViewBag.DisableChanges = true;
            //}

            ViewBag.MyReqlist = reqlist;
            string ReqHdr = CheckRequiredHdr(reqlist);
            ViewBag.ReqHdr = ReqHdr;

            //AccountCreationFlag
            bool AcctCreation = false;
            if (MyCarrDetail.idCarrType == 5)
            {
                AcctCreation = true;
            }
            ViewBag.AcctCreation = AcctCreation;
            //Renewals
            bool Renewal = false;
            if (MyCarrDetail.idCarrType == 3)
            {
                Renewal = true;
            }
            ViewBag.Renewal = Renewal;

            return View();
        }



        [HttpPost]
        [Authorize]
        public ActionResult Location(int? idloc)
        {
            ViewBag.Title = "Location";
            int idlocation = int.Parse(Request.Form["idlocation"]);
            var MyLocation = db.CARRLocations.Where(x => x.idLocation == idlocation && x.ActiveFlag == true).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR && x.ActiveFlag == true).FirstOrDefault();


            var MyServices = db.Services.Where(x => x.idLocation == idlocation && x.ActiveFlag == true);
            ViewBag.MyServices = MyServices;
           

            //DropDown Lists           
            List<SelectListItem> salesList = HelperDropdowns.getSalesList();
            List<SelectListItem> localSRIDList = HelperDropdowns.getLocalSRIDList();
            List<SelectListItem> strategicSRIDList = HelperDropdowns.getStrategicSRIDList();
            List<SelectListItem> ISPSRIDList = HelperDropdowns.getISPSRIDList();
            List<SelectListItem> salesDMList = HelperDropdowns.getSalesDMList();
            List<SelectListItem> regionList = HelperDropdowns.getRegionList();
            List<SelectListItem> originList = HelperDropdowns.getOriginList();
            List<SelectListItem> gatewayList = HelperDropdowns.getGatewayList();
            List<SelectListItem> statelist = HelperDropdowns.getStateList();
            ViewBag.salesList = salesList;
            ViewBag.salesDMList = salesDMList;
            ViewBag.regionList = regionList;
            ViewBag.originList = originList;
            ViewBag.gatewayList = gatewayList;
            ViewBag.localsridList = localSRIDList;
            ViewBag.strategicsridList = strategicSRIDList;
            ViewBag.ispsridList = ISPSRIDList;
            ViewBag.stateList = statelist;

            //Need to check for null values
            if (MyLocation.LocalSRID == null)
                ViewBag.localsridselected = "";
            else
                ViewBag.localsridselected = MyLocation.LocalSRID;
            if (MyLocation.StrategicSRID == null)
                ViewBag.strategicsridselected = "";
            else
                ViewBag.strategicsridselected = MyLocation.StrategicSRID;
            if (MyLocation.ISPSRID == null)
                ViewBag.ispsridselected = "";
            else
                ViewBag.ispsridselected = MyLocation.ISPSRID;
            if (MyLocation.locationDM == null)
                ViewBag.locationdmselected = "";
            else
                ViewBag.locationdmselected = MyLocation.locationDM;
            if (MyLocation.ControlBranch == null)
                ViewBag.controlbranchelected = "";
            else
                ViewBag.controlbranchselected = MyLocation.ControlBranch;
            if (MyLocation.Branch == null)
                ViewBag.branchselected = "";
            else
                ViewBag.branchselected = MyLocation.Branch;

            if (MyLocation.Gateway == null)
                ViewBag.gatewayselected = "";
            else
                ViewBag.gatewayselected = MyLocation.Gateway;

            if (MyLocation.idServiceType == null)
                ViewBag.servicetypeselected = "";
            else
                ViewBag.servicetypeselected = MyLocation.idServiceType;

            ViewBag.DGDFlagselected = "";
            if (MyLocation.DGFlag == true)
                ViewBag.DGFlagselected = "1";
            if (MyLocation.DGFlag == false)
                ViewBag.DGFlagselected = "0";



            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;            
            ViewBag.MyLocation = MyLocation;
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
                ViewBag.idCARRType = MyCarrDetail.idCarrType;
            }
            else
            {
                ViewBag.idCARRType = "";
            }

            try
            {
                ViewBag.serviceType = db.DropdownsOptions.Where(x => x.Value == MyLocation.idServiceType.ToString()).FirstOrDefault().Name.Trim();
            }
            catch (System.Exception ex)
            {
                ViewBag.serviceType = "";
            }
          

            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            SqlCommand cmd2 = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlDataAdapter da2 = new SqlDataAdapter();
            SqlDataAdapter daInd = new SqlDataAdapter();
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dtInd = new DataTable();

            DataController dataController = new DataController();
            ViewBag.resultInd = dataController.GetIndCodesByLoc(idlocation);
            try
            {
                bool detailFlag = true;
                cmd = new SqlCommand("sp_CalcRevBySvc", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCARRDetail", MyCarrDetail.idCARRDetail));
                cmd.Parameters.Add(new SqlParameter("@idLocation", idlocation));
                cmd.Parameters.Add(new SqlParameter("@detailFlag", detailFlag));
                cmd.Parameters.Add(new SqlParameter("@totalBy", "service"));
                da.SelectCommand = cmd;
                da.Fill(dt);
                ViewBag.projrevbyloc = dt;

                //For totals
                detailFlag = false;
                cmd2 = new SqlCommand("sp_CalcRevBySvc", cnn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add(new SqlParameter("@idCARRDetail", MyCarrDetail.idCARRDetail));
                cmd2.Parameters.Add(new SqlParameter("@idLocation", idlocation));
                cmd2.Parameters.Add(new SqlParameter("@detailFlag", detailFlag));
                cmd2.Parameters.Add(new SqlParameter("@totalBy", "location"));
                da2.SelectCommand = cmd2;
                da2.Fill(dt2);
                ViewBag.projtotalbyloc = dt2;


                string sqlLHRF = @" select *
                    FROM CAR.CARRLHRF
                    where LHRFType != 'Returns' and ActiveFlag = 1 and idCARRDetail = ";
                sqlLHRF = sqlLHRF + MyCarrDetail.idCARRDetail;
                sqlLHRF = sqlLHRF + " and idLocation = " + idlocation + " order by LHRFType";
                DataTable dtLHRF = new DataTable();
                SqlDataAdapter daLHRF = new SqlDataAdapter(sqlLHRF, cnn);
                daLHRF.Fill(dtLHRF);
                ViewBag.resultLHRF = dtLHRF;

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);
         
            string userName = Session["accountname"].ToString().ToLower();
            var routing = db.Routings.Where(x => x.idRouting == MyCarrDetail.idRoutedTo).FirstOrDefault();
            string routedToName = routing.RoutedTo.Trim().ToLower();
            if (Session["userrole"].ToString() == "DistrictAdmin" && routedToName == userName)
            {
                ViewBag.DisableChanges = false;
            }

            ViewBag.userRole = Session["userrole"].ToString();

            //Determine what is still required
          
            List<string> reqlist = CheckRequiredLocations(MyCarrDetail.idCARRDetail, idlocation);
            ViewBag.MyReqlist = reqlist;
            string ReqHdr = CheckRequiredHdr(reqlist);
            ViewBag.ReqHdr = ReqHdr;

            ViewBag.LHRFCount = GetLHRFCountForLocation(MyCarrDetail.idCARRDetail, idlocation);

            //AccountCreationFlag
            bool AcctCreation = false;
            if (MyCarrDetail.idCarrType == 5)
            {
                AcctCreation = true;
            }
            ViewBag.AcctCreation = AcctCreation;
            //Renewals
            bool Renewal = false;
            if (MyCarrDetail.idCarrType == 3)
            {
                Renewal = true;
            }
            ViewBag.Renewal = Renewal;

            //Get Volumes            
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                string sql = @"Select ss.Description, ind.code,ind.Description 'Induction',sv.idVolume,sv.idService,sv.idInduction,ISNULL(sv.NumberShipments,0) NumberShipments,
                    sv.PickUpFrequency,sv.AvgWt,sv.AvgPcsOrSkids,sv.PkgLength,sv.PkgWidth,sv.PkgHeight,
                    sv.ProductDesc,sv.NumberPickups,sv.idTransitType,dd1.Name 'TransitType', sv.DimWeight
                    from CAR.ServiceVolume sv
                    left join CAR.DropDownsOptions dd1 on dd1.value = sv.idTransitType and dd1.id=7
                    left join CAR.InductionPoints ind on ind.idInduction = sv.idInduction
					left join CAR.Service s on s.idService = sv.idService
					left join CAR.ShippingServices ss on ss.idShippingService = s.idShippingService
                    where sv.ActiveFlag = 1 and s.idLocation =  ";
                sql = sql + MyLocation.idLocation;
                sql = sql + " order by ss.Description";
                DataTable svcdt = new DataTable();
                ViewBag.volumes = svcdt; 
                connection.Open();
                SqlDataAdapter svcda = new SqlDataAdapter(sql, connection);
                svcda.Fill(svcdt);
                ViewBag.volumes = svcdt;
             
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return View();

        }

        public ActionResult LocationProfile(int? idloc)
        {
            ViewBag.Title = "Location";
            int idlocation = int.Parse(Request.Form["idlocation"]);
            var MyLocation = db.CARRLocations.Where(x => x.idLocation == idlocation && x.ActiveFlag == true).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR && x.ActiveFlag == true).FirstOrDefault();


            var MyServices = db.Services.Where(x => x.idLocation == idlocation && x.ActiveFlag == true);
            ViewBag.MyServices = MyServices;

           

            //DropDown Lists           
            List<SelectListItem> salesList = HelperDropdowns.getSalesList();
            List<SelectListItem> localSRIDList = HelperDropdowns.getLocalSRIDList();
            List<SelectListItem> strategicSRIDList = HelperDropdowns.getStrategicSRIDList();
            List<SelectListItem> ISPSRIDList = HelperDropdowns.getISPSRIDList();
            List<SelectListItem> salesDMList = HelperDropdowns.getSalesDMList();
            List<SelectListItem> regionList = HelperDropdowns.getRegionList();
            List<SelectListItem> originList = HelperDropdowns.getOriginList();
            List<SelectListItem> gatewayList = HelperDropdowns.getGatewayList();
            List<SelectListItem> statelist = HelperDropdowns.getStateList();
            ViewBag.salesList = salesList;
            ViewBag.salesDMList = salesDMList;
            ViewBag.regionList = regionList;
            ViewBag.originList = originList;
            ViewBag.gatewayList = gatewayList;
            ViewBag.localsridList = localSRIDList;
            ViewBag.strategicsridList = strategicSRIDList;
            ViewBag.ispsridList = ISPSRIDList;
            ViewBag.stateList = statelist;

            //Need to check for null values
            if (MyLocation.LocalSRID == null)
                ViewBag.localsridselected = "";
            else
                ViewBag.localsridselected = MyLocation.LocalSRID;
            if (MyLocation.StrategicSRID == null)
                ViewBag.strategicsridselected = "";
            else
                ViewBag.strategicsridselected = MyLocation.StrategicSRID;
            if (MyLocation.ISPSRID == null)
                ViewBag.ispsridselected = "";
            else
                ViewBag.ispsridselected = MyLocation.ISPSRID;
            if (MyLocation.locationDM == null)
                ViewBag.locationdmselected = "";
            else
                ViewBag.locationdmselected = MyLocation.locationDM;
            if (MyLocation.ControlBranch == null)
                ViewBag.controlbranchelected = "";
            else
                ViewBag.controlbranchselected = MyLocation.ControlBranch;
            if (MyLocation.Branch == null)
                ViewBag.branchselected = "";
            else
                ViewBag.branchselected = MyLocation.Branch;

            if (MyLocation.Gateway == null)
                ViewBag.gatewayselected = "";
            else
                ViewBag.gatewayselected = MyLocation.Gateway;

            if (MyLocation.idServiceType == null)
                ViewBag.servicetypeselected = "";
            else
                ViewBag.servicetypeselected = MyLocation.idServiceType;

            ViewBag.DGDFlagselected = "";
            if (MyLocation.DGFlag == true)
                ViewBag.DGFlagselected = "1";
            if (MyLocation.DGFlag == false)
                ViewBag.DGFlagselected = "0";



            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyLocation = MyLocation;
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }



            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            SqlCommand cmd2 = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlDataAdapter da2 = new SqlDataAdapter();
            SqlDataAdapter daInd = new SqlDataAdapter();
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dtInd = new DataTable();

            DataController dataController = new DataController();
            ViewBag.resultInd = dataController.GetIndCodesByLoc(idlocation);
            try
            {
                bool detailFlag = true;
                cmd = new SqlCommand("sp_CalcRevBySvc", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCARRDetail", MyCarrDetail.idCARRDetail));
                cmd.Parameters.Add(new SqlParameter("@idLocation", idlocation));
                cmd.Parameters.Add(new SqlParameter("@detailFlag", detailFlag));
                cmd.Parameters.Add(new SqlParameter("@totalBy", "service"));
                da.SelectCommand = cmd;
                da.Fill(dt);
                ViewBag.projrevbyloc = dt;

                //For totals
                detailFlag = false;
                cmd2 = new SqlCommand("sp_CalcRevBySvc", cnn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add(new SqlParameter("@idCARRDetail", MyCarrDetail.idCARRDetail));
                cmd2.Parameters.Add(new SqlParameter("@idLocation", idlocation));
                cmd2.Parameters.Add(new SqlParameter("@detailFlag", detailFlag));
                cmd2.Parameters.Add(new SqlParameter("@totalBy", "location"));
                da2.SelectCommand = cmd2;
                da2.Fill(dt2);
                ViewBag.projtotalbyloc = dt2;


                string sqlLHRF = @" select *
                    FROM CAR.CARRLHRF
                    where LHRFType != 'Returns' and ActiveFlag = 1 and idCARRDetail = ";
                sqlLHRF = sqlLHRF + MyCarrDetail.idCARRDetail;
                sqlLHRF = sqlLHRF + " and idLocation = " + idlocation + " order by LHRFType";
                DataTable dtLHRF = new DataTable();
                SqlDataAdapter daLHRF = new SqlDataAdapter(sqlLHRF, cnn);
                daLHRF.Fill(dtLHRF);
                ViewBag.resultLHRF = dtLHRF;

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);
          

            string userName = Session["accountname"].ToString().ToLower();
            var routing = db.Routings.Where(x => x.idRouting == MyCarrDetail.idRoutedTo).FirstOrDefault();
            string routedToName = routing.RoutedTo.Trim().ToLower();
            if (Session["userrole"].ToString() == "DistrictAdmin" && routedToName == userName)
            {
                ViewBag.DisableChanges = false;
            }

            ViewBag.userRole = Session["userrole"].ToString();

            //Determine what is still required

            List<string> reqlist = CheckRequiredLocations(MyCarrDetail.idCARRDetail, idlocation);
            ViewBag.MyReqlist = reqlist;
            string ReqHdr = CheckRequiredHdr(reqlist);
            ViewBag.ReqHdr = ReqHdr;

            ViewBag.LHRFCount = GetLHRFCountForLocation(MyCarrDetail.idCARRDetail, idlocation);

            //AccountCreationFlag
            bool AcctCreation = false;
            if (MyCarrDetail.idCarrType == 5)
            {
                AcctCreation = true;
            }
            ViewBag.AcctCreation = AcctCreation;
            //Renewals
            bool Renewal = false;
            if (MyCarrDetail.idCarrType == 3)
            {
                Renewal = true;
            }
            ViewBag.Renewal = Renewal;

            //Get Volumes            
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                string sql = @"Select ss.Description, ind.code,ind.Description 'Induction',sv.idVolume,sv.idService,sv.idInduction,sv.NumberShipments,
                    sv.PickUpFrequency,sv.AvgWt,sv.AvgPcsOrSkids,sv.PkgLength,sv.PkgWidth,sv.PkgHeight,
                    sv.ProductDesc,sv.NumberPickups,sv.idTransitType,dd1.Name 'TransitType', sv.DimWeight
                    from CAR.ServiceVolume sv
                    left join CAR.DropDownsOptions dd1 on dd1.value = sv.idTransitType and dd1.id=7
                    left join CAR.InductionPoints ind on ind.idInduction = sv.idInduction
					left join CAR.Service s on s.idService = sv.idService
					left join CAR.ShippingServices ss on ss.idShippingService = s.idShippingService
                    where sv.ActiveFlag = 1 and s.idLocation =  ";
                sql = sql + MyLocation.idLocation;
                sql = sql + " order by ss.Description";
                DataTable svcdt = new DataTable();
                ViewBag.volumes = svcdt;
                connection.Open();
                SqlDataAdapter svcda = new SqlDataAdapter(sql, connection);
                svcda.Fill(svcdt);
                ViewBag.volumes = svcdt;

            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return View();

        }

        [HttpGet]
        [Authorize]
        public ActionResult LocationInductionTable(int idLocation)
        {
            var MyLocation = db.CARRLocations.Where(x => x.idLocation == idLocation && x.ActiveFlag == true).FirstOrDefault();

            DataController dataController = new DataController();
            ViewBag.resultInd = dataController.GetIndCodesByLoc(idLocation);
            ViewBag.MyLocation = MyLocation;
            return PartialView("Partial_LocationInductionTable");
        }

        [HttpGet]
        [Authorize]
        public ActionResult AccessorialTable(string shippingService, int idCARRDetail)
        {
            ViewBag.ShippingService = shippingService;
            ViewBag.resultAccessorial = GetAccessorials(idCARRDetail);
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            ViewBag.MyCarrDetail = MyCarrDetail;
            return PartialView("Partial_AccessorialTable");
        }

        [HttpPost]
        [Authorize]
        public ActionResult Services()
        {
            ViewBag.Title = "Services";
            int idlocation = int.Parse(Request.Form["idlocation"]);

            var MyServices = db.Services.Where(x => x.idLocation == idlocation && x.ActiveFlag == true);
            ViewBag.MyServices = MyServices;

            var MyLocation = db.CARRLocations.Where(x => x.idLocation == idlocation && x.ActiveFlag == true).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR && x.ActiveFlag == true).FirstOrDefault();
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyLocation = MyLocation;
            ViewBag.idLocation = idlocation;
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }



            //Call Stored Procedure for Estimated Revenue by Service for this Location         
            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                cmd = new SqlCommand("sp_CalcRevBySvc", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCARRDetail", MyCarrDetail.idCARRDetail));
                cmd.Parameters.Add(new SqlParameter("@idLocation", idlocation));
                cmd.Parameters.Add(new SqlParameter("@detailFlag", 1));
                cmd.Parameters.Add(new SqlParameter("@totalBy", "service"));
                da.SelectCommand = cmd;
                da.Fill(dt);
                ViewBag.result = dt;

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                cnn.Close();
            }

            //Determine what is still required
            List<string> reqlist = CheckRequiredServices(idlocation);
            ViewBag.MyReqlist = reqlist;
            string ReqHdr = CheckRequiredHdr(reqlist);
            ViewBag.ReqHdr = ReqHdr;
            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Service()
        {
            ViewBag.Title = "Service";
            int idservice = int.Parse(Request.Form["idservice"]);
            

            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                string sql = @" select ind.code,ind.Description 'Induction',sv.idVolume,sv.idService,sv.idInduction,sv.NumberShipments,
                    sv.PickUpFrequency,sv.AvgWt,sv.AvgPcsOrSkids,sv.PkgLength,sv.PkgWidth,sv.PkgHeight,
                    sv.ProductDesc,sv.NumberPickups,sv.idTransitType,dd1.Name 'TransitType', sv.DimWeight
                    from CAR.ServiceVolume sv
                    left join CAR.DropDownsOptions dd1 on dd1.value = sv.idTransitType and dd1.id=7
                    left join CAR.InductionPoints ind on ind.idInduction = sv.idInduction
                    where sv.ActiveFlag = 1 and idService = ";
                sql = sql + idservice;
                sql = sql + " order by sv.idVolume";
                DataTable dt = new DataTable();
                connection.Open();
                SqlDataAdapter da = new SqlDataAdapter(sql, connection);
                da.Fill(dt);
                ViewBag.result = dt;

                string sqlDG = @"select idDG,
                                        Class,
			                            Description,
                                        acceptableFlag
	                            FROM [PURO_APPS].[CAR].[DangerousGoods]";
                DataTable dtDG = new DataTable();
                SqlDataAdapter daDG = new SqlDataAdapter(sqlDG, connection);
                daDG.Fill(dtDG);
                ViewBag.resultDG = dtDG;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }


            var MyService = db.Services.Where(x => x.idService == idservice).FirstOrDefault();
            var MyLocation = db.CARRLocations.Where(x => x.idLocation == MyService.idLocation).FirstOrDefault();
            var MyContact = db.Contacts.Where(x => x.idContact == MyLocation.idBranchContact).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();


            ViewBag.idShippingService = MyService.idShippingService;


            List<SelectListItem> inductionlist = HelperDropdowns.getInductionList();
            ViewBag.inductionList = inductionlist;

            List<SelectListItem> lhBranchList = HelperDropdowns.getLHRatingBranchList();
            ViewBag.lhBranchList = lhBranchList;

            List<SelectListItem> lhInductionList = HelperDropdowns.getLHRatingInductionList();
            ViewBag.lhInductionList = lhInductionList;

            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyLocation = MyLocation;
            ViewBag.MyService = MyService;
            ViewBag.MyContact = MyContact;
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }


            //Determine what is still required
            List<string> reqlist = CheckRequiredSvc(idservice);
            ViewBag.MyReqlist = reqlist;
            string ReqHdr = CheckRequiredHdr(reqlist);
            ViewBag.ReqHdr = ReqHdr;

            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);

            //AccountCreationFlag
            bool AcctCreation = false;
            if (MyCarrDetail.idCarrType == 5)
            {
                AcctCreation = true;
            }
            ViewBag.AcctCreation = AcctCreation;
            //Renewals
            bool Renewal = false;
            if (MyCarrDetail.idCarrType == 3)
            {
                Renewal = true;
            }
            ViewBag.Renewal = Renewal;

            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult Renewals()
        {
            ViewBag.Title = "Renewals";
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);

            //Details used in Partial Info Bar
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);

            string userName = Session["accountname"].ToString().ToLower();
            var routing = db.Routings.Where(x => x.idRouting == MyCarrDetail.idRoutedTo).FirstOrDefault();
            string routedToName = routing.RoutedTo.Trim().ToLower();
            if (Session["userrole"].ToString() == "DistrictAdmin" && routedToName == userName)
            {
                ViewBag.DisableChanges = false;
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        public bool CheckAllRequiredAccountFieldsBool(int idCARRDetail)
        {
            List<string> locationReqList = CheckAllRequiredAccountFields(idCARRDetail);
            if (locationReqList.Count() > 0)
                return true;
            else
                return false;
        }

        public List<string> CheckAllRequiredAccountFields(int idCARRDetail)
        {
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();


            List<string> reqlist = new List<string>();

            if (string.IsNullOrEmpty(MyCarrDetail.ContractNumber) || string.IsNullOrEmpty(MyCarrDetail.ContractFromDate.ToString()) || string.IsNullOrEmpty(MyCarrDetail.ContractToDate.ToString()))
            {
                reqlist.Add("Required fields are missing for the Contract Number and Dates");
            }

            //Payment Terms
            if (string.IsNullOrEmpty(MyCarrDetail.PaymentTerms))
            {
                reqlist.Add("Required field is missing for Payment Terms");
            }
            //Invoice Type
            //if (string.IsNullOrEmpty(MyCarrDetail.idBillingOption.ToString()))
            //{
            //    reqlist.Add("Required field is missing for Invoice Type");
            //}


            var MyLocation = db.CARRLocations.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).ToList();
            foreach (var l in MyLocation)
            {
                var MyService = db.Services.Where(x => x.idLocation == l.idLocation && x.ActiveFlag == true).ToList();
                foreach (var s in MyService)
                {
                    if (
                        string.IsNullOrEmpty(s.Contact.FirstName) ||
                        string.IsNullOrEmpty(s.Contact.LastName) ||
                        string.IsNullOrEmpty(s.Contact.Address1) ||
                        string.IsNullOrEmpty(s.Contact.PostalCode) ||
                        string.IsNullOrEmpty(s.Contact.City) ||
                        string.IsNullOrEmpty(s.Contact.State) ||
                        string.IsNullOrEmpty(s.Contact.Country))
                        
                    {
                        reqlist.Add("Required fields are missing for Account Creation: " + l.LocationName + " " + s.ShippingService.Description);
                    }
                    //AP Email Address
                    if (string.IsNullOrEmpty(s.APEmail))
                    {
                        reqlist.Add("Required field is missing for AP Email Address: " + l.LocationName + " " + s.ShippingService.Description);
                    }
                    //Invoice Type
                    if (string.IsNullOrEmpty(s.idBillingOption.ToString()) || s.idBillingOption.ToString() == "0")
                    {
                        reqlist.Add("Required field is missing for Invoice Type");
                    }
                }
            }
            return reqlist;
        }

        public List<string> CheckAllRequiredAccountNumbers(int idCARRDetail)
        {
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();


            List<string> reqlist = new List<string>();
            if (string.IsNullOrEmpty(MyCarrDetail.ContractNumber) || string.IsNullOrEmpty(MyCarrDetail.ContractFromDate.ToString()) || string.IsNullOrEmpty(MyCarrDetail.ContractToDate.ToString()))
            {
                reqlist.Add("Required fields are missing for the Contract Number and Dates");
            }

            //Renewals don't need account creation details
            if (MyCarrDetail.idCarrType != 3)
            {
                var MyLocation = db.CARRLocations.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).ToList();
                foreach (var l in MyLocation)
                {
                    var MyService = db.Services.Where(x => x.idLocation == l.idLocation && x.ActiveFlag == true).ToList();
                    foreach (var s in MyService)
                    {
                        if (string.IsNullOrEmpty(s.Contact.FirstName) ||
                            string.IsNullOrEmpty(s.Contact.LastName) ||
                            string.IsNullOrEmpty(s.Contact.Address1) ||
                            string.IsNullOrEmpty(s.Contact.PostalCode) ||
                            string.IsNullOrEmpty(s.Contact.City) ||
                            string.IsNullOrEmpty(s.Contact.State) ||
                            string.IsNullOrEmpty(s.Contact.Country) ||
                            string.IsNullOrEmpty(s.AccountNumber))
                        {
                            reqlist.Add("Required fields are missing for Account Creation: " + l.LocationName + " " + s.ShippingService.Description);
                        }
                    }
                }
            }
           
            return reqlist;
        }

        public List<string> CheckAllRequiredFields(int idCARRDetail)
        {
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();

            //Customer Details
            List<string> reqlist = new List<string>();

            if (MyCarrDetail.idCarrType == 3)
            {
                List<string> reqRenewalFields = CheckRenewalRequiredFields(idCARRDetail);
                reqlist = reqlist.Concat(reqRenewalFields).ToList();
            }
            else
            {
                List<string> reqCustFields = CheckCustomerRequiredFields(idCARRDetail);
                List<string> reqLocFields = CheckRequiredLocations(idCARRDetail);
                reqlist = reqlist.Concat(reqCustFields).ToList();
                reqlist = reqlist.Concat(reqLocFields).ToList();
                if (MyCarrDetail.idCarrType != 5)
                {
                    List<string> reqBrokerageFields = CheckRequiredBrokerage(idCARRDetail);                                      
                    List<string> reqDGFields = CheckRequiredDG(idCARRDetail);
                    List<string> reqReturnFields = CheckRequiredReturns(idCARRDetail);
                    List<string> reqLHRFFields = CheckRequiredLHRFForm(idCARRDetail);
                    List<string> reqEstRevFields = CheckRequiredRevenueEstimates(Session["accountname"].ToString(), idCARRDetail);
                    List<string> reqPpstRevFields = CheckRequiredPPSTFields(idCARRDetail);
                    reqlist = reqlist.Concat(reqBrokerageFields).ToList();                                     
                    reqlist = reqlist.Concat(reqDGFields).ToList();
                    reqlist = reqlist.Concat(reqReturnFields).ToList();
                    reqlist = reqlist.Concat(reqLHRFFields).ToList();
                    reqlist = reqlist.Concat(reqEstRevFields).ToList();
                    reqlist = reqlist.Concat(reqPpstRevFields).ToList();
                }
                

            }
            
            List<string> reqCorporateInfoFields = CheckCorporateInfoRequiredFields(idCARRDetail);
            reqlist = reqlist.Concat(reqCorporateInfoFields).ToList();


            return reqlist;
        }

        public List<string> CheckRequiredLocations(int idCARRDetail, int? idLocation = null)
        {
            List<CARRLocation> MyLocations = null;

            if (idLocation == null)
            {
                MyLocations = db.CARRLocations.Where(x => x.CARRDetail.idCARRDetail == idCARRDetail && x.ActiveFlag == true).ToList();
            }
            else
            {
                MyLocations = db.CARRLocations.Where(x => x.idLocation == idLocation && x.ActiveFlag == true).ToList();
            }
            List<string> reqlist = new List<string>();
            List<string> reqlisttemp = new List<string>();
            List<string> reqlisttemp2 = new List<string>();
            int numLocations = 0;
            foreach (var loc in MyLocations)
            {
                if (loc.ActiveFlag == true)
                {
                    numLocations = numLocations + 1;
                    reqlisttemp = CheckRequiredLocation(loc.idLocation);
                    reqlisttemp2 = CheckRequiredServices(loc.idLocation);
                    reqlist = reqlist.Concat(reqlisttemp).ToList();
                    reqlist = reqlist.Concat(reqlisttemp2).ToList();
                }

            }
            //make sure there is at least one location entered
            if (numLocations == 0)
            {
                reqlist.Add("No Locations have been entered");
            }

            return reqlist;
        }

        [HttpPost]
        [Authorize]
        public bool CheckRequiredLocationsBool(int idCARRDetail)
        {
            List<string> locationReqList = CheckRequiredLocations(idCARRDetail);
            if (locationReqList.Count() > 0)
                return true;
            else
                return false;
        }

        public List<string> GetUnfinishedLocList(int idCARRDetail)
        {
            var MyLocations = db.CARRLocations.Where(x => x.CARRDetail.idCARRDetail == idCARRDetail && x.ActiveFlag == true);
            List<string> reqlist = new List<string>();

            foreach (var loc in MyLocations)
            {
                bool addit = false;
                if (loc.ActiveFlag == true)
                {
                    //Call routine to check for req fields in location
                    bool unfinishedFlag = CheckLocationRequirements(loc);
                    bool unfinishedFlag2 = CheckLocationLHRFRequirements(loc);
                    if (unfinishedFlag == true || unfinishedFlag2 == true)
                    {
                        addit = true;
                    }
                    else
                    {
                        var MyServices = db.Services.Where(x => x.idLocation == loc.idLocation && x.ActiveFlag == true);
                        int numServices = 0;
                        foreach (var svc in MyServices)
                        {
                            if (svc.ActiveFlag == true)
                            {
                                numServices = numServices + 1;
                                unfinishedFlag = CheckRequiredService(svc);
                                if (unfinishedFlag == true)
                                {
                                    addit = true;
                                }
                            }

                        }
                        //make sure there is at least one Volume entered
                        if (numServices == 0)
                        {
                            addit = true;
                        }
                    }

                }
                if (addit == true)
                {
                    reqlist.Add(loc.LocationName);
                }

            }


            return reqlist;
        }

        public List<string> CheckRenewalRequiredFields(int idCARRDetail)
        {
            bool somethingmissing = CheckRenewalRequiredFieldsBool(idCARRDetail);
            List<string> reqlist = new List<string>();
            if (somethingmissing == true)
            {
                reqlist.Add("Required fields are missing for Renewal Info");
            }
            return reqlist;
        }
        [HttpPost]
        [Authorize]
        public bool CheckRenewalRequiredFieldsBool(int idCARRDetail)
        {
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            bool somethingmissing = false;

            if (string.IsNullOrEmpty(MyCarrDetail.Currency))
                somethingmissing = true;
            if (string.IsNullOrEmpty(MyCarrDetail.ContractNumber))
                somethingmissing = true;
            if (MyCarrDetail.ContractFromDate is null)
                somethingmissing = true;
            if (MyCarrDetail.ContractToDate is null)
                somethingmissing = true;

            return somethingmissing;
        }

        public List<string> CheckCustomerRequiredFields(int idCARRDetail)
        {
            bool somethingmissing = CheckCustomerRequiredFieldsBool(idCARRDetail);
            List<string> reqlist = new List<string>();
            if (somethingmissing == true)
            {
                reqlist.Add("Required fields are missing for Customer Details");
            }
            return reqlist;
        }

        [HttpPost]
        [Authorize]
        public bool CheckCustomerRequiredFieldsBool(int idCARRDetail)
        {
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            bool somethingmissing = false;

            if (string.IsNullOrEmpty(MyCarrDetail.Currency))
            {
                somethingmissing = true;
            }
            if (MyCarrDetail.idProgramType is null || MyCarrDetail.idProgramType == 0)
            {
                somethingmissing = true;
            }
            if (string.IsNullOrEmpty(MyCarrDetail.DecisionMakerBranch) || MyCarrDetail.DecisionMakerBranch.Contains("Select"))
            {
                somethingmissing = true;
            }
            if (string.IsNullOrEmpty(MyCarrDetail.SalesProfessional) || MyCarrDetail.SalesProfessional.Contains("Select"))
            {
                somethingmissing = true;
            }
            if (string.IsNullOrEmpty(MyCarrDetail.SalesManager) || MyCarrDetail.SalesManager.Contains("Select"))
            {
                somethingmissing = true;
            }
            if (string.IsNullOrEmpty(MyCarrDetail.DistrictManager) || MyCarrDetail.DistrictManager.Contains("Select"))
            {
                somethingmissing = true;
            }
            if (MyCarrDetail.idIndustry is null || MyCarrDetail.idIndustry == 0)
            {
                somethingmissing = true;
            }
            if (string.IsNullOrEmpty(MyCarrDetail.BtoBBtoC) || MyCarrDetail.BtoBBtoC.Contains("Select"))
            {
                somethingmissing = true;
            }
            if (string.IsNullOrEmpty(MyCarrDetail.VersionComment))
            {
                somethingmissing = true;
            }
            if(MyCarrDetail.CurrentProvider == null)
            {
                somethingmissing = true;
            }
            if (MyCarrDetail.ContractFromDate is null)
                somethingmissing = true;
            if (MyCarrDetail.ContractToDate is null)
                somethingmissing = true;

            if (MyCarrDetail.idCarrType == 1 || MyCarrDetail.idCarrType == 2 || MyCarrDetail.idCarrType == 4)
            {
                if (string.IsNullOrEmpty(MyCarrDetail.SalesForceCaseLink))
                {
                    somethingmissing = true;
                }
            }

            return somethingmissing;
        }

        public List<string> CheckCorporateInfoRequiredFields(int idCARRDetail)
        {
            bool somethingmissing = CheckCorporateInfoRequiredFieldsBool(idCARRDetail);
            List<string> reqlist = new List<string>();
            if (somethingmissing == true)
            {
                reqlist.Add("Required fields are missing for Corporate Info");
            }
            return reqlist;
        }

        [HttpPost]
        [Authorize]
        public bool CheckCorporateInfoRequiredFieldsBool(int idCARRDetail)
        {
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            bool somethingmissing = false;

            if (string.IsNullOrEmpty(MyCarr.CustomerLegalName) ||
                MyCarrDetail.idCarrType is null || MyCarrDetail.idCarrType == 0 ||
                string.IsNullOrEmpty(MyCarrDetail.Contact.FirstName) || string.IsNullOrEmpty(MyCarrDetail.Contact.LastName) ||
                string.IsNullOrEmpty(MyCarrDetail.Contact.Address1) || string.IsNullOrEmpty(MyCarrDetail.Contact.City) ||
                string.IsNullOrEmpty(MyCarrDetail.Contact.State) || string.IsNullOrEmpty(MyCarrDetail.Contact.PostalCode) ||
                string.IsNullOrEmpty(MyCarrDetail.Contact.Country))
            {
                somethingmissing = true;
            }

            return somethingmissing;
        }

        public List<string> CheckRequiredReturns(int idCARRDetail)
        {
            List<string> reqlist = new List<string>();
            bool somethingmissing = CheckRequiredReturnsLHRF(idCARRDetail);            

            if (somethingmissing == true)
            {
                    reqlist.Add("Required fields are missing for Returns Transportation");
            }                

            
            return reqlist;
        }

        public List<string> CheckRequiredBrokerage(int idCARRDetail)
        {
            List<string> reqlist = new List<string>();

            //Make sure a Brokerage row has been added
            bool somethingmissing = CheckForBrokerage(idCARRDetail);
            
            if (somethingmissing == true)
            {
                reqlist.Add("Brokerage Selection is Required");
            }


            return reqlist;
        }

        public List<string> CheckRequiredLHRFForm(int idCARRDetail, int? idLocation = null)
        {
            List<string> reqlist = new List<string>();
            List<string> incompleteLHRFTypes = CheckRequiredLHRF(idCARRDetail, idLocation);

            foreach (string type in incompleteLHRFTypes)
            {
                reqlist.Add("Required fields are missing for LHRF: " + type);
            }


            return reqlist;
        }

        [HttpPost]
        [Authorize]
        public bool CheckRequiredReturnsLHRF(int idCARRDetail)
        {
            bool somethingmissing = false;
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            if (MyCarrDetail.returnsFlag == true)
            {
                var MyReturn = db.CARRLHRFs.Where(x => x.idCARRDetail == idCARRDetail && x.LHRFType == "Returns" && x.ActiveFlag == true).FirstOrDefault();               

                //if (MyReturn.LHDateNeeded is null)
                //{
                //    somethingmissing = true;
                //}
                if (string.IsNullOrEmpty(MyReturn.PUAddress))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyReturn.PUCity))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyReturn.PUState))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyReturn.PUPostalCode))
                {
                    somethingmissing = true;
                }

                if (string.IsNullOrEmpty(MyReturn.DestAddress))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyReturn.DestCity))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyReturn.DestState))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyReturn.DestPostalCode))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyReturn.FreqOfPickup))
                {
                    somethingmissing = true;
                }

                if (string.IsNullOrEmpty(MyReturn.FreqOfPickupPer))
                {
                    somethingmissing = true;
                }

                if (string.IsNullOrEmpty(MyReturn.TransitDays))
                {
                    somethingmissing = true;
                }
                //if (string.IsNullOrEmpty(MyReturn.LHMode))
                //{
                //    somethingmissing = true;
                //}
                if (string.IsNullOrEmpty(MyReturn.AvgSkidsPerShpmt))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyReturn.AvgWtPerShpmt))
                {
                    somethingmissing = true;
                }

                if (string.IsNullOrEmpty(MyReturn.ReadyTime))
                {
                    somethingmissing = true;
                }

                if (string.IsNullOrEmpty(MyReturn.CloseTime))
                {
                    somethingmissing = true;
                }

            }

            return somethingmissing;
        }

        [HttpPost]
        [Authorize]
        public bool CheckForBrokerage(int idCARRDetail)
        {
            bool somethingmissing = false;
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            if (MyCarrDetail.brokerageFlag != false)
            {
                var MyCarrBrokerage = db.CARRBrokerages.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
                if (MyCarrBrokerage is null)
                {
                    somethingmissing = true;
                }
            }
           
            return somethingmissing;
        }
            public List<string> CheckRequiredLHRF(int idCARRDetail, int? idLocation)
        {
            List<string> incompleteLHRFTypes = new List<string>();

            List<CARRLHRF> lhrfList;
            
            if(idLocation != null)
                lhrfList = db.CARRLHRFs.Where(x => x.idCARRDetail == idCARRDetail && x.LHRFType != "Returns" && x.idLocation == idLocation && x.ActiveFlag == true && x.CARRLocation.ActiveFlag == true).ToList();
            else
                lhrfList = db.CARRLHRFs.Where(x => x.idCARRDetail == idCARRDetail && x.LHRFType != "Returns" && x.ActiveFlag == true && x.CARRLocation.ActiveFlag == true).ToList();

            foreach (var lhrf in lhrfList)
            {
                if (string.IsNullOrEmpty(lhrf.TeamOrSingle) ||
                    string.IsNullOrEmpty(lhrf.PUAddress) || string.IsNullOrEmpty(lhrf.PUCity) ||
                    string.IsNullOrEmpty(lhrf.PUState) || string.IsNullOrEmpty(lhrf.PUPostalCode) || string.IsNullOrEmpty(lhrf.DestAddress) ||
                    string.IsNullOrEmpty(lhrf.DestCity) || string.IsNullOrEmpty(lhrf.DestState) || string.IsNullOrEmpty(lhrf.DestPostalCode) ||
                    string.IsNullOrEmpty(lhrf.FreqOfPickup) || string.IsNullOrEmpty(lhrf.FreqOfPickupPer) || string.IsNullOrEmpty(lhrf.TransitDays) ||
                    string.IsNullOrEmpty(lhrf.AvgSkidsPerShpmt) || string.IsNullOrEmpty(lhrf.AvgWtPerShpmt) ||
                    string.IsNullOrEmpty(lhrf.ReadyTime) || string.IsNullOrEmpty(lhrf.CloseTime) || 
                    string.IsNullOrEmpty(lhrf.Commodity) || string.IsNullOrEmpty(lhrf.DGFlag) ||
                    (lhrf.MondayPickup == false && lhrf.TuesdayPickup == false && lhrf.WednesdayPickup == false && lhrf.ThursdayPickup == false && lhrf.FridayPickup == false && lhrf.SaturdayPickup == false && lhrf.SundayPickup == false))
                {
                    incompleteLHRFTypes.Add(lhrf.LHRFType);
                }
            }
            
            return incompleteLHRFTypes;
        }

        [HttpPost]
        [Authorize]
        public bool CheckRequiredLHRFBool(int idCARRDetail, int idLocation)
        {
            List<string> incompleteLHRFTypes = CheckRequiredLHRF(idCARRDetail, idLocation);
            if (incompleteLHRFTypes.Count() > 0)
                return true;
            else
                return false;
        }

        public List<string> CheckRequiredDG(int idCARRDetail)
        {
            List<string> reqlist = new List<string>();
            var somethingmissing = CheckRequiredDGBool(idCARRDetail);
            if (somethingmissing == true)
            {
                reqlist.Add("Required fields are missing for Accessorials Dangerous Goods Contact");
            }
            
            return reqlist;
        }


        [HttpPost]
        [Authorize]
        public bool CheckRequiredDGBool(int idCARRDetail)
        {
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            DataTable dtDGFlag = new DataTable();
            string sqlDGFlag = @"select count(l.dgFlag) dgFlags
                                            from CAR.CARRDetail c
                                            join CAR.CARRLocation l on l.idCARRDetail = c.idCARRDetail
                                            join CAR.Service s on s.idLocation = l.idLocation
                                            join CAR.ShippingServices ss on ss.idShippingService = s.idShippingService
                                            where s.ActiveFlag=1 and l.ActiveFlag=1 and l.DGFlag=1 and c.idCARRDetail = ";
            sqlDGFlag = sqlDGFlag + idCARRDetail;

            SqlDataAdapter daDGFlag = new SqlDataAdapter(sqlDGFlag, connection);
            daDGFlag.Fill(dtDGFlag);


            bool dgFlag = false;
            foreach (DataRow dgrow in dtDGFlag.Rows)
            {
                if ((int)dgrow["dgFlags"] > 0)
                {
                    dgFlag = true;
                }

            }

            bool somethingmissing = false;
            if (dgFlag == true)
            {
                var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
                var MyDGContact = db.DGContacts.Where(x => x.idDGContact == MyCarrDetail.idDGContact).FirstOrDefault();


                if (string.IsNullOrEmpty(MyDGContact.FirstName))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyDGContact.LastName))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyDGContact.PhoneNumber))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyDGContact.Address1))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyDGContact.PostalCode))
                {
                    somethingmissing = true;
                }

                if (string.IsNullOrEmpty(MyDGContact.City))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyDGContact.State))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyDGContact.Country))
                {
                    somethingmissing = true;
                }
                if (string.IsNullOrEmpty(MyDGContact.AltPhoneNumber))
                {
                    somethingmissing = true;
                }
            }
            return somethingmissing;
        }

        public List<string> CheckRequiredLocation(int idlocation)
        {
            var loc = db.CARRLocations.Where(x => x.idLocation == idlocation && x.ActiveFlag == true).FirstOrDefault();
            List<string> reqlist = new List<string>();
            bool somethingmissing = CheckLocationRequirements(loc);
          

            if (somethingmissing == true)
            {
                reqlist.Add("Required fields are missing for " + loc.LocationName + " details");
            }

            bool somethingmissing2 = CheckLocationLHRFRequirements(loc);


            if (somethingmissing2 == true)
            {
                reqlist.Add("Required fields are missing for a LHRF for " + loc.LocationName);
            }

            return reqlist;
        }

        public bool CheckLocationRequirements(CARRLocation loc)
        {
            bool somethingmissing = false;

            
            if (string.IsNullOrEmpty(loc.CompanyName))
                somethingmissing = true;
            if (string.IsNullOrEmpty(loc.locationDM) || loc.locationDM.Contains("Select"))
                somethingmissing = true;
            if (loc.ControlBranch is  null || loc.ControlBranch=="0")
                somethingmissing = true;
            if (loc.Branch is null || loc.Branch=="0")
                somethingmissing = true;
            if (loc.Gateway is null || loc.Gateway=="0")
                somethingmissing = true;
            if (loc.idServiceType is null || loc.idServiceType==0)
                somethingmissing = true;
            //if (loc.FFWDType is null || loc.FFWDType == "")
            //    somethingmissing = true;
            if (loc.DGFlag is null)
                somethingmissing = true;

            if (string.IsNullOrEmpty(loc.Contact.FirstName))
                somethingmissing = true;
            if (string.IsNullOrEmpty(loc.Contact.LastName))
                somethingmissing = true;
            if (string.IsNullOrEmpty(loc.Contact.Address1))
                somethingmissing = true;
            if (string.IsNullOrEmpty(loc.Contact.City))
                somethingmissing = true;
            if (string.IsNullOrEmpty(loc.Contact.State))
                somethingmissing = true;
            if (string.IsNullOrEmpty(loc.Contact.PostalCode))
                somethingmissing = true;
            if (string.IsNullOrEmpty(loc.Contact.Country))
                somethingmissing = true;
            //if (loc.DGFlag == null)
            //    somethingmissing = true;
            //if(CheckRequiredLHRFBool(loc.idCARRDetail.GetValueOrDefault(), loc.idLocation))
            //        somethingmissing = true;
            
            return somethingmissing;
        }

        public bool CheckLocationLHRFRequirements(CARRLocation loc)
        {
            bool somethingmissing = false;            
            
            if (CheckRequiredLHRFBool(loc.idCARRDetail.GetValueOrDefault(), loc.idLocation))
                somethingmissing = true;

            return somethingmissing;
        }

        public List<string> CheckRequiredServices(int idlocation)
        {
            var MyServices = db.Services.Where(x => x.idLocation == idlocation && x.ActiveFlag == true);
            var MyLocation = db.CARRLocations.Where(x => x.idLocation == idlocation).FirstOrDefault();
           
            int numServices = 0;
            List<string> reqlist = new List<string>();
            List<string> reqlisttemp = new List<string>();
            foreach (var svc in MyServices)
            {
                if (svc.ActiveFlag == true)
                {
                    numServices = numServices + 1;
                    reqlisttemp = CheckRequiredSvc(svc.idService);
                    reqlist = reqlist.Concat(reqlisttemp).ToList();
                }
               
            }
            //make sure there is at least one Service entered
            if (numServices == 0)
            {               
                     reqlist.Add("No Services have been entered for " + MyLocation.LocationName);
            }
            return reqlist;
        }

        public List<string> CheckRequiredSvc(int idservice)
        {
            var MyService = db.Services.Where(x => x.idService == idservice).FirstOrDefault();
            var MyLocation = db.CARRLocations.Where(x => x.idLocation == MyService.idLocation).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail).FirstOrDefault();
            List<string> reqlist = new List<string>();
            int numVolumes = 0;
            bool somethingmissing = false;
            bool thisvolmissing;
            foreach (ServiceVolume vol in MyService.ServiceVolumes)
            {
                if (vol.ActiveFlag == true)
                {
                    thisvolmissing = false;
                    numVolumes = numVolumes + 1;
                    if (vol.NumberShipments is null || vol.NumberShipments < 0)
                    {
                        somethingmissing = true;
                        thisvolmissing = true;
                    }
                    if (string.IsNullOrEmpty(vol.PickUpFrequency))
                    {
                        somethingmissing = true;
                        thisvolmissing = true;
                    }
                    if (vol.AvgWt is null || vol.AvgWt <= 0)
                    {
                        somethingmissing = true;
                        thisvolmissing = true;
                    }
                    if (vol.AvgPcsOrSkids is null || vol.AvgPcsOrSkids <= 0)
                    {
                        somethingmissing = true;
                        thisvolmissing = true;
                    }
                    if (vol.PkgLength is null || vol.PkgLength <= 0)
                    {
                        somethingmissing = true;
                        thisvolmissing = true;
                    }
                    if (vol.PkgWidth is null || vol.PkgWidth <= 0)
                    {
                        somethingmissing = true;
                        thisvolmissing = true;
                    }
                    if (vol.PkgHeight is null || vol.PkgHeight <= 0)
                    {
                        somethingmissing = true;
                        thisvolmissing = true;
                    }
                    if (string.IsNullOrEmpty(vol.ProductDesc))
                    {
                        somethingmissing = true;
                        thisvolmissing = true;
                    }
                    //if (vol.NumberPickups is null || vol.NumberPickups <= 0)
                    //{
                    //    somethingmissing = true;
                    //    thisvolmissing = true;
                    //}
                    if (vol.idTransitType is null)
                    {
                        somethingmissing = true;
                        thisvolmissing = true;
                    }
                    if (vol.DimWeight is null)
                    {
                        somethingmissing = true;
                        thisvolmissing = true;
                    }
                    if (thisvolmissing == true)
                    {
                        if (MyCarrDetail.idCarrType != 5)
                        {
                            string reqtext = "Required fields are missing for " + MyService.CARRLocation.LocationName + " " + MyService.ShippingService.Description + " Volume ";
                            if (numVolumes > 1)
                            {
                                reqtext = reqtext + " " + numVolumes.ToString();
                            }
                            reqlist.Add(reqtext);
                        }
                     
                    }
                }

            }
            
            if (somethingmissing == true)
            {
                //do nothing
            }
            else
            {
                //make sure there is at least one Volume entered
                if (numVolumes == 0)
                {
                    if (MyCarrDetail.idCarrType != 5)
                        reqlist.Add("No Volumes have been entered for " + MyService.CARRLocation.LocationName + " "  + MyService.ShippingService.Description);
                }            

            }

            return reqlist;
        }

        public bool CheckRequiredService(Service svc)
        {
            var MyService = db.Services.Where(x => x.idService == svc.idService).FirstOrDefault();
            var MyLocation = db.CARRLocations.Where(x => x.idLocation == MyService.idLocation).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail).FirstOrDefault();
            bool somethingmissing = false;
            int numVolumes = 0;           

            foreach (ServiceVolume vol in svc.ServiceVolumes)
            {              
                    if (vol.ActiveFlag == true)
                   {
                    numVolumes = numVolumes + 1;
                    if (vol.NumberShipments is null)
                    {
                        somethingmissing = true;
                    }
                    if (string.IsNullOrEmpty(vol.PickUpFrequency))
                    {
                        somethingmissing = true;
                    }
                    if (vol.AvgWt is null)
                    {
                        somethingmissing = true;
                    }
                    if (vol.AvgPcsOrSkids is null)
                    {
                        somethingmissing = true;
                    }
                    if (vol.PkgLength is null)
                    {
                        somethingmissing = true;
                     }
                    if (vol.PkgWidth is null)
                    {
                        somethingmissing = true;
                    }
                    if (vol.PkgHeight is null)
                    {
                        somethingmissing = true;
                    }
                    if (string.IsNullOrEmpty(vol.ProductDesc))
                    {
                        somethingmissing = true;
                    }
                    //No longer required
                    //if (vol.NumberPickups is null)
                    //{
                    //    somethingmissing = true;
                    //}
                    if (vol.idTransitType is null)
                    {
                        somethingmissing = true;                        
                    }

                    if (vol.DimWeight is null)
                    {
                        somethingmissing = true;
                    }
                   
                }

            }            
            //make sure there is at least one Volume entered
            if (numVolumes == 0)
            {
                if (MyCarrDetail.idCarrType != 5)
                    somethingmissing = true;
            }
            
            return somethingmissing;
        }

        public string CheckRequiredHdr(List<string> reqlist)
        {            
            string ReqHdr = "";           
            if (reqlist.Count > 0)
            {
                ReqHdr = "<h4> &nbsp; &nbsp;<i class='fas fa-wrench'></i> Requirements Needed</h4>";
            }
            else
            {                
                //ReqHdr = " <div class='bg-success'><h3> &nbsp; &nbsp; " + @ViewBag.Title  + " Requirements Complete &nbsp;<i class='fas fa-check'></i>&nbsp; &nbsp; </h3></div>";
                ReqHdr = "<h4> &nbsp; &nbsp; " + @ViewBag.Title + " Requirements Complete &nbsp;<i class='fas fa-check'></i>&nbsp; &nbsp; </h4>";

                reqlist.Add("  Well done - requirements completed!");
            }
            return ReqHdr;
        }

        public string CheckAccountRequiredHdr(List<string> reqlist)
        {
            string ReqHdr = "";
            if (reqlist.Count > 0)
            {
                ReqHdr = "<h4> &nbsp; &nbsp;<i class='fas fa-wrench'></i> Requirements Needed</h4>";
            }
            else
            {
                ReqHdr = "<h4> &nbsp; &nbsp; Account Requirements Complete &nbsp;<i class='fas fa-check'></i>&nbsp; &nbsp; </h4>";

                reqlist.Add("  Well done - requirements completed!");
            }
            return ReqHdr;
        }


        [HttpPost]
        [Authorize]
        public ActionResult Volume()
        {
            int idvolume = int.Parse(Request.Form["idvolume"]);

            //ViewBag.result = dt;
            var MyVolume = db.ServiceVolumes.Where(x => x.idVolume == idvolume).FirstOrDefault();
            var MyService = db.Services.Where(x => x.idService == MyVolume.idService).FirstOrDefault();
            var MyLocation = db.CARRLocations.Where(x => x.idLocation == MyService.idLocation).FirstOrDefault();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == MyLocation.idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            
            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.MyLocation = MyLocation;
            ViewBag.MyService = MyService;
            ViewBag.MyVolume = MyVolume;

            var inductiondd = db.InductionPoints;
            ViewBag.inductionlist = inductiondd.AsEnumerable()
                .Select(x => new SelectListItem
                {
                    Value = x.idInduction.ToString(),
                    Text = x.code + " - " + x.Description.ToString()
                });

            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);

            string userName = Session["accountname"].ToString();
            var routing = db.Routings.Where(x => x.idRouting == MyCarrDetail.idRoutedTo).FirstOrDefault();
            string routedToName = routing.RoutedTo.Trim();
            //Pricing needs access to make changes to volumes 
            if (Session["userrole"].ToString() == "Pricing")
            {
                ViewBag.DisableChanges = false;
            }

            

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ProjectedRevenue()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);

            string sql = @" select ss.Description,
             sum(Case When PickUpFrequency = 'Month' Then Round(NumberShipments*12/52,1)
	        When PickUpFrequency = 'Year' Then Round(NumberShipments/52,1)
	        Else NumberShipments
            End) as ShipmentsPerWeek
             from CAR.ServiceVolume sv
             join CAR.Service s on s.idService = sv.idService
             join CAR.CARRLocation cl on s.idLocation = cl.idLocation
            join CAR.CARRDetail cd on cd.idCARRDetail = cl.idCARRDetail
             join CAR.ShippingServices ss on ss.idShippingService = s.idShippingService
            where sv.ActiveFlag = 1 and s.ActiveFlag = 1 and and cl.ActiveFlag = 1 and cd.idCARRDetail = ";
            sql = sql + idCARRDetail;
            sql = sql + "group by ss.Description";
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, connection);
            da.Fill(dt);
            connection.Close();
            connection.Dispose();
            ViewBag.result = dt;

            return View();
        }

        [HttpPost]
        [Authorize]
        public DataTable GetAccessorials(int idCARRDetail)
        {
            String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection connection = new SqlConnection(cs);

            try
            {
                //string sqlAccesorial = @" select *
                //        from CAR.CARRAccessorials
                //        where ActiveFlag = 1 and idCARRDetail = ";
                string sqlAccesorial = @" select *
                        from CAR.AccessorialDefaults order by Accessorial";
                //sqlAccesorial = sqlAccesorial + idCARRDetail;
               // sqlAccesorial = sqlAccesorial + " order by Accessorial";
                DataTable dtAccessorial = new DataTable();
                connection.Open();
                SqlDataAdapter daAccessorial = new SqlDataAdapter(sqlAccesorial, connection);
                daAccessorial.Fill(dtAccessorial);
                connection.Close();
                connection.Dispose();

                return dtAccessorial;
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return null;
        }

        public List<LHRFAddresses> getLHRFOrigin(int idCARRDetail)
        {
            List<LHRFAddresses> puAddressList = new List<LHRFAddresses>();
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                string sql = @" select loc.LocationName as 'Name', 
                                   con.Address1 as 'Address',
                                   con.City as 'City',
                                   con.State as 'State',
                                   con.PostalCode as 'Code'
                        from CAR.CARRDetail det
                        left join CAR.CARRLocation loc on loc.idCARRDetail=det.idCARRDetail
                        left join CAR.Contacts con on con.idContact=loc.idBranchContact
                        where loc.ActiveFlag=1
                        and det.idCARRDetail=";
                sql = sql + idCARRDetail;
                using (SqlCommand com = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        puAddressList.Add(new LHRFAddresses(0, "Select Pickup Origin", null, null, null, null, null));

                        int puCount = 1;
                        while (sdr.Read())
                        {
                            string puLocationName = sdr["Name"].ToString();
                            string puAddress = sdr["Address"].ToString();
                            string puCity = sdr["City"].ToString();
                            string puState = sdr["State"].ToString();
                            string puPostalCode = sdr["Code"].ToString();
                            String puOption = String.Format("{0} {1} {2} {3} {4}",
                                             puLocationName, puAddress, puCity, puState, puPostalCode);

                            LHRFAddresses puLoc = new LHRFAddresses(puCount, puOption, puLocationName, puAddress, puCity, puState, puPostalCode);
                            puAddressList.Add(puLoc);
                            puCount++;
                        }
                        puAddressList.Add(new LHRFAddresses(puCount++, "Other", null, null, null, null, null));
                    }
                }

            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return puAddressList;
        }

        public List<LHRFAddresses> getLHRFDestination()
        {
            List<LHRFAddresses> destAddressList = new List<LHRFAddresses>();
            var destinations = db.InductionPoints.Where(a => a.ActiveFlag == true).ToList();

            destAddressList.Add(new LHRFAddresses(0, "Select Destination", null, null, null, null, null));

            int destCount = 1;
            for (int i = 0; i < destinations.Count; i++)
            {
                string destLocationName = destinations[i].Description;
                string destAddress = destinations[i].address;
                string destCity = destinations[i].city;
                string destState = destinations[i].state;
                string destPostalCode = destinations[i].postalcode;
                String destOption = String.Format("{0} {1} {2} {3} {4}",
                                 destLocationName, destAddress, destCity, destState, destPostalCode);

                destAddressList.Add(new LHRFAddresses(destCount, destOption, destLocationName, destAddress, destCity, destState, destPostalCode));
                destCount++;
            }

            return destAddressList;
        }

        public bool disableChanges(int idCARRDetail)
        {
            if (Session == null)
            {
                RedirectToAction("Login", "User");
            }
            string userName = Session["accountname"].ToString().ToLower();
            var carrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var routing = db.Routings.Where(x => x.idRouting == carrDetail.idRoutedTo).FirstOrDefault();

            string routedToName = routing.RoutedTo.Trim().ToLower();
            string createdByName = carrDetail.CreatedBy.ToLower();
            string userrole = Session["userrole"].ToString().ToLower();


            //if CARR is routed to a different user, you cannot edit
            bool disableflag =  !userName.Equals(routedToName) ? true : false;

            // If this user created the carr and it's routed to this user, they can edit (DM on behalf of a sales person, DistrictAdmin)
            if (createdByName == userName && routedToName == userName)
            {
                disableflag = false;
            }

            //MK - even when routed to a user, if they are not sales or dsm, do not allow changes  (pricing, DistrictAdmin)        
            if (userrole != "sales" && userrole != "salesdsm"  && userrole != "districtmanager")          
            {
                disableflag = true;
            }
                     

            //if pricing is complete, this CARR is considered locked
            if (carrDetail.PricingReqCompleteFlag == true)
            {
                disableflag = true;
            }

            //if CARR is complete, no one can edit
            var MyCarr = db.CARRs.Where(x => x.idCarr == carrDetail.idCARR).FirstOrDefault();
            if (carrDetail.CompletedFlag == true)
            {
                disableflag = true;
            }
            

            return disableflag;
        }

        public bool disableChangesAcctCreation(int idCARRDetail)
        {
            if (Session == null)
            {
                RedirectToAction("Login", "User");
            }
            string userName = Session["accountname"].ToString().ToLower();
            var carrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var routing = db.Routings.Where(x => x.idRouting == carrDetail.idRoutedTo).FirstOrDefault();

            string routedToName = routing.RoutedTo.Trim().ToLower();
            string createdByName = carrDetail.CreatedBy.ToLower();
            string userrole = Session["userrole"].ToString().ToLower();


            //if CARR is routed to a different user, you cannot edit
            bool disableflag = !userName.Equals(routedToName) ? true : false;

            // If this user created the carr and it's routed to this user, they can edit (DM on behalf of a sales person, DistrictAdmin)
            if (createdByName == userName && routedToName == userName)
            {
                disableflag = false;
            }

            //MK - even when routed to a user, if they are not sales or dsm, do not allow changes  (pricing)        
            if (userrole != "sales" && userrole != "salesdsm" && userrole != "districtmanager" && userrole != "districtadmin" && userrole != "contracts")
            {
                disableflag = true;
            }          

            //if CARR is complete, no one can edit
            var MyCarr = db.CARRs.Where(x => x.idCarr == carrDetail.idCARR).FirstOrDefault();
            if (carrDetail.CompletedFlag == true)
            {
                disableflag = true;
            }


            return disableflag;
        }

        public int GetLHRFCountForLocation(int idCARRDetail, int idLocation)
        {
            int lhrfCount = db.CARRLHRFs.Where(x => x.idCARRDetail == idCARRDetail && x.LHRFType != "Returns" && x.idLocation == idLocation && x.ActiveFlag == true).Count();
            return lhrfCount;
        }

        [HttpPost]
        [Authorize]
        public ActionResult RateSelection()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            ViewBag.idCARRDetail = idCARRDetail;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            
            //Details used in Partial Info Bar   
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            var MyDropdowns = db.DropdownsOptions;
            if (MyCarrDetail.idCarrType != null)
            {
                var MyCarrType = MyDropdowns.Where(t => t.Value == MyCarrDetail.idCarrType.ToString()).First();
                ViewBag.MyCarrType = MyCarrType.Name;
            }
            ViewBag.MyCarr = MyCarr;

            ViewBag.Title = "Rate Selections";

            var MyDefaultExchangeRate = db.KeyValuePairs.Where(x => x.Key == "DefaultExchangeRate").FirstOrDefault();
            ViewBag.DefaultExchangeRate = MyDefaultExchangeRate.Value;
            var CPCExpeditedMinValue = db.KeyValuePairs.Where(x => x.Key == "CPCExpeditedMinValue").FirstOrDefault();
            ViewBag.CPCExpeditedMinValue = CPCExpeditedMinValue.Value;
            var CPCExpeditedMaxValue = db.KeyValuePairs.Where(x => x.Key == "CPCExpeditedMaxValue").FirstOrDefault();
            ViewBag.CPCExpeditedMaxValue = CPCExpeditedMaxValue.Value;
            var CPCXpressPostMinValue = db.KeyValuePairs.Where(x => x.Key == "CPCXpressPostMinValue").FirstOrDefault();
            ViewBag.CPCXpressPostMinValue = CPCXpressPostMinValue.Value;
            var CPCXpressPostMaxValue = db.KeyValuePairs.Where(x => x.Key == "CPCXpressPostMaxValue").FirstOrDefault();
            ViewBag.CPCXpressPostMaxValue = CPCXpressPostMaxValue.Value;
            var LTLDiscountMinValue = db.KeyValuePairs.Where(x => x.Key == "LTLDiscountMinValue").FirstOrDefault();
            ViewBag.LTLDiscountMinValue = LTLDiscountMinValue.Value;
            var LTLDiscountMaxValue = db.KeyValuePairs.Where(x => x.Key == "LTLDiscountMaxValue").FirstOrDefault();
            ViewBag.LTLDiscountMaxValue = LTLDiscountMaxValue.Value;

            if (MyCarrDetail.CPCExpeditedMarkup is null)
            {
                MyCarrDetail.CPCExpeditedMarkup = 0;
            }
            if (MyCarrDetail.CPCXpressPostMarkup is null)
            {
                MyCarrDetail.CPCXpressPostMarkup = 0;
            }
            if (MyCarrDetail.ltlyyzDiscount is null)
            {
                MyCarrDetail.ltlyyzDiscount = 0;
            }
            if (MyCarrDetail.ltlyvrDiscount is null)
            {
                MyCarrDetail.ltlyvrDiscount = 0;
            }
            if (MyCarrDetail.ltlywgDiscount is null)
            {
                MyCarrDetail.ltlywgDiscount = 0;
            }
            if (MyCarrDetail.ltlyulDiscount is null)
            {
                MyCarrDetail.ltlyulDiscount = 0;
            }

            if (MyCarrDetail.ExchangeRate is null)
            {
                //If Exhcnage Rate is null, put in default Exchange Rate and save
                MyCarrDetail.ExchangeRate = Convert.ToDouble(MyDefaultExchangeRate.Value);
                db.SaveChanges();

            }

            ViewBag.DisableExchange = true;
            if (Session["userrole"].ToString().ToLower() == "pricing" || Session["userrole"].ToString().ToLower() == "financedirector")
            {
                ViewBag.DisableExchange = false;
            }
            ViewBag.DisableChanges = disableChanges(idCARRDetail);           
                      
            
            ViewBag.ExchangeRateForSTP = MyCarrDetail.ExchangeRate;
            if(MyCarrDetail.Currency == "CAD")
            {
                ViewBag.ExchangeRateForSTP = 1;
            }

            //Expedited Effective Date
            List<SelectListItem> dateList = HelperDropdowns.getCPCExpEffDates();
            ViewBag.CPCEXpEffDates = dateList;
            ViewBag.CPCExpeditedEffDate = MyCarrDetail.CPCExpeditedEffDate.ToString();
            if (MyCarrDetail.CPCExpeditedEffDate is null)
                ViewBag.CPCExpeditedEffDate = "";

            //XpressPost Effective Date
            List<SelectListItem> dateList2 = HelperDropdowns.getCPCXpressEffDates();
            ViewBag.CPCXpressEffDates = dateList2;
            ViewBag.CPCXpressPostEffDate = MyCarrDetail.CPCXpressPostEffDate.ToString();
            if (MyCarrDetail.CPCXpressPostEffDate is null)
                ViewBag.CPCXpressPostEffDate = "";


            ViewBag.MyCarrDetail = MyCarrDetail;
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ShippingProfile()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            int idCARR = MyCarr.idCarr;

            ViewBag.idCARRDetail = idCARRDetail;
            ViewBag.shippingProfile = Utility.getShippingProfileData("sp_ShippingProfile", idCARRDetail);
            ViewBag.routingHistory = Utility.getShippingTableData("sp_CARRRoutingHistory ", idCARR);
            ViewBag.notes = Utility.getShippingTableData("sp_CARRNoteHistory ", idCARR);
            ViewBag.fileAttachments = Utility.getShippingTableData("sp_CARRFileHistory ", idCARR);

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            return View();
        }

        public ActionResult getShippingProfileData(int idCARRDetail)
        {
            DataTable dt = Utility.getShippingProfileData("sp_ShippingProfile", idCARRDetail);

            if (dt != null)
            {
                StringBuilder sb = new StringBuilder();
                string[] columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();

                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dt.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                    sb.AppendLine(string.Join(",", fields));
                }
                string exportpath = ConfigurationManager.AppSettings["USPricingExport"].ToString();
                string userName = Session["accountname"].ToString();
                string exportFilename = "ShippingProfile_" + userName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                string exportFilepath = exportpath + exportFilename;

                System.IO.File.WriteAllText(exportFilepath, sb.ToString());
                byte[] filedata = System.IO.File.ReadAllBytes(exportFilepath);
                string contentType = MimeMapping.GetMimeMapping(exportFilepath);
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = exportFilename,
                    Inline = true,
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());
                System.IO.File.Delete(exportFilepath);
                return File(filedata, contentType);
            }
            return null;
        }

        [HttpPost]
        [Authorize]
        public ActionResult EstRev()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.routedTo = db.Routings.Where(x => x.idRouting == MyCarrDetail.idRoutedTo).FirstOrDefault().RoutedTo.Trim();

            ViewBag.services = Utility.getEstRevData(idCARRDetail);
            string userName = Session["accountname"].ToString();
            ViewBag.DisableChanges = true;
            if (ViewBag.routedTo.ToLower() == userName.ToLower())
            {
                ViewBag.DisableChanges = false;
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult PuroPost()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;

            ViewBag.DisableChanges = disableChanges(MyCarrDetail.idCARRDetail);
            //if (Session["userrole"].ToString() == "Pricing")
            //{
            //    ViewBag.DisableChanges = false;
            //}

            // Check for no PuroPost rows matching current idCARRDetail
            CARRPuroPost ppst = db.CARRPuroPosts.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            if (ppst == null)
            {
                CARRPuroPost newPPST = new CARRPuroPost();
                newPPST.idCARRDetail = idCARRDetail;
                newPPST.ActiveFlag = true;
                newPPST.CreatedBy = MyCarrDetail.CreatedBy;
                newPPST.CreatedOn = DateTime.Now;

                db.CARRPuroPosts.Add(newPPST);

                db.SaveChanges();

                ViewBag.MyPPST = newPPST;
            }
            else
            {
                ViewBag.MyPPST = ppst;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;

            // Retrieve Average Weight value
            DataTable dt = new DataTable();
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    string sql = $"select l.Branch,sv.AvgWt from CAR.ServiceVolume sv join CAR.Service s on s.idService = sv.idService join CAR.CARRLocation l on l.idLocation = s.idLocation join CAR.InductionPoints ip on ip.idInduction = sv.idInduction where l.idCARRDetail = {idCARRDetail} and s.ActiveFlag=1 and sv.ActiveFlag=1 and l.ActiveFlag=1 and s.idShippingService in (6, 7, 9, 12);";
                    SqlCommand cmd = new SqlCommand(sql, cnn);
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
                catch (System.Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message.ToString());
                }
            }

            // Format Average Weight value
            string AvgWt = "";
             foreach (DataRow row in dt.Rows)
            {
                if (AvgWt != "")
                    AvgWt = AvgWt + ", ";

                string weight = string.IsNullOrEmpty(row["AvgWt"].ToString()) ? "0" : row["AvgWt"].ToString();

                if (dt.Rows.Count > 1)
                {
                    AvgWt += $"{row["Branch"]} - {weight} ";
                }
                else
                {
                    AvgWt = weight;
                }
                     
            }
            ViewBag.AvgWt = AvgWt;
           
            // Retrieve files
            dt = new DataTable();
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    string sql = $"select idPPSTFile as fileID, FilePath, Description, CreatedBy, CreatedOn from CAR.CARRPuroPostFiles where ActiveFlag = 1 and idCARRDetail = {idCARRDetail} order by CreatedOn desc";
                    SqlCommand cmd = new SqlCommand(sql, cnn);
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
                catch (System.Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message.ToString());
                }
            }

            ViewBag.files = dt;
            ViewBag.fileUploaded = dt.Rows.Count > 0;

            return View();
        }

        public ActionResult ReportExceptions(int idCARRDetail)
        {

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);

            DataTable dt4 = new DataTable();
            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
          
            SqlDataAdapter da4 = new SqlDataAdapter();
            //Exceptions
            cmd = new SqlCommand("sp_Exceptions", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da4.SelectCommand = cmd;
            da4.Fill(dt4);

            LocalReport lr = new LocalReport();
            lr.EnableHyperlinks = true;
            string path = "";
            string filename = "CARR_" + MyCarr.CustomerLegalName + "_Exceptions";
            string DisplayName = "";
            string ExportType = "PDF";
            path = System.IO.Path.Combine(Server.MapPath("~/Report"), "ReportExceptions.rdlc");
            DisplayName = filename + ".pdf";          

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Index");
            }
            //Set Parameters
            string ContractNumber = MyCarrDetail.ContractNumber;
            if (ContractNumber == null)
                ContractNumber = "";

            string ContractFromDate = MyCarrDetail.ContractFromDate.ToString();
            DateTime convdate;
            if (ContractFromDate == null)
                ContractFromDate = "";
            else
            {
                if (ContractFromDate != "")
                {
                    convdate = Convert.ToDateTime(MyCarrDetail.ContractFromDate);
                    ContractFromDate = convdate.ToString("MM/dd/yyyy");
                }
            }
            string ContractToDate = MyCarrDetail.ContractToDate.ToString();
            if (ContractToDate == null)
                ContractToDate = "";
            else
            {
                if (ContractToDate != "")
                {
                    convdate = Convert.ToDateTime(MyCarrDetail.ContractToDate);
                    ContractToDate = convdate.ToString("MM/dd/yyyy");
                }
            }
            ReportParameter[] Params = new ReportParameter[4];           
            Params[0] = new ReportParameter("CustomerName", MyCarr.CustomerLegalName);
            Params[1] = new ReportParameter("ContractNumber", ContractNumber.ToString());
            Params[2] = new ReportParameter("ContractEffectiveDate", ContractFromDate);
            Params[3] = new ReportParameter("ContractExpiryDate", ContractToDate);           

            lr.SetParameters(Params);
            
            ReportDataSource rptData4 = new ReportDataSource("DataSet4", dt4);
            lr.DataSources.Add(rptData4);
            string reportType = ExportType;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.5in</MarginLeft>" +
                "  <MarginRight>0.5in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);


            return File(renderedBytes, mimeType, DisplayName);

        }

        public ActionResult ReportLHRF(int idLHRF)
        {

            var MyLHRF =   db.CARRLHRFs.Where(x => x.idLHRF == idLHRF).FirstOrDefault();
            

            DataTable dt1 = new DataTable();
            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();

            SqlDataAdapter da4 = new SqlDataAdapter();
            //Exceptions
            cmd = new SqlCommand("sp_getLHRFDetails", cnn);
            cmd.Parameters.Add(new SqlParameter("@idLHRF", idLHRF));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da4.SelectCommand = cmd;
            da4.Fill(dt1);

            LocalReport lr = new LocalReport();
            lr.EnableHyperlinks = true;
            string path = "";
            string filename = "LHRFExport_" + idLHRF;
            string DisplayName = "";
            //string ExportType = "PDF";
            string ExportType = "Excel";
            path = System.IO.Path.Combine(Server.MapPath("~/Report"), "ReportLHRF.rdlc");
            //DisplayName = filename + ".pdf";
            DisplayName = filename + ".xls";

            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Index");
            }
            //Set Parameters
            //string ContractNumber = MyCarrDetail.ContractNumber;
            //if (ContractNumber == null)
            //    ContractNumber = "";

            //string ContractFromDate = MyCarrDetail.ContractFromDate.ToString();
            //DateTime convdate;
            //if (ContractFromDate == null)
            //    ContractFromDate = "";
            //else
            //{
            //    if (ContractFromDate != "")
            //    {
            //        convdate = Convert.ToDateTime(MyCarrDetail.ContractFromDate);
            //        ContractFromDate = convdate.ToString("MM/dd/yyyy");
            //    }
            //}
            //string ContractToDate = MyCarrDetail.ContractToDate.ToString();
            //if (ContractToDate == null)
            //    ContractToDate = "";
            //else
            //{
            //    if (ContractToDate != "")
            //    {
            //        convdate = Convert.ToDateTime(MyCarrDetail.ContractToDate);
            //        ContractToDate = convdate.ToString("MM/dd/yyyy");
            //    }
            //}
            //ReportParameter[] Params = new ReportParameter[4];
            //Params[0] = new ReportParameter("CustomerName", MyCarr.CustomerLegalName);
            //Params[1] = new ReportParameter("ContractNumber", ContractNumber.ToString());
            //Params[2] = new ReportParameter("ContractEffectiveDate", ContractFromDate);
            //Params[3] = new ReportParameter("ContractExpiryDate", ContractToDate);

           //lr.SetParameters(Params);

            ReportDataSource rptData1 = new ReportDataSource("DataSet1", dt1);
            lr.DataSources.Add(rptData1);
            string reportType = ExportType;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.5in</PageWidth>" +
                "  <PageHeight>11in</PageHeight>" +
                "  <MarginTop>0.5in</MarginTop>" +
                "  <MarginLeft>0.5in</MarginLeft>" +
                "  <MarginRight>0.5in</MarginRight>" +
                "  <MarginBottom>0.5in</MarginBottom>" +
                "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);


            return File(renderedBytes, mimeType, DisplayName);

        }

    }
}
