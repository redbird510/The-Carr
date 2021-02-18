using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Portal.Models;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.Entity.Core.Objects;
using PI_Portal.Classes;

namespace PI_Portal.Controllers
{
    
    public class ExportController : Controller
    {
        private Entities db = new Entities();

        [HttpPost]
        [Authorize]
        
        // GET: Export
        public ActionResult Index()
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

           

            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.idCARRDetail = idCARRDetail;


            //Get DGFlag info for this CARRDetail
            bool DGFlag = false;
            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            DataTable dtDGFlag = new DataTable();
            string sqlDGFlag = @"select count(l.dgFlag) dgFlags,c.returnsFlag,c.courierAccessorialFlag,
                                        c.cpcAccessorialFlag,c.freightAccessorialFlag,ppstAccessorialFlag
                                        from CAR.CARRDetail c
                                        join CAR.CARRLocation l on l.idCARRDetail = c.idCARRDetail
                                        join CAR.Service s on s.idLocation = l.idLocation
                                        join CAR.ShippingServices ss on ss.idShippingService = s.idShippingService
                                        where s.ActiveFlag=1 and l.ActiveFlag=1 and l.DGFlag=1 and c.idCARRDetail = ";
            sqlDGFlag = sqlDGFlag + idCARRDetail + " group by c.returnsFlag,c.courierAccessorialFlag,c.cpcAccessorialFlag,c.freightAccessorialFlag,ppstAccessorialFlag";

            SqlDataAdapter daDGFlag = new SqlDataAdapter(sqlDGFlag, cnn);
            daDGFlag.Fill(dtDGFlag);
            foreach (DataRow row in dtDGFlag.Rows)
            {
                if ((int)row["dgFlags"] > 0)
                {
                    DGFlag = true;
                }      
            }
            ViewBag.DGFlag = DGFlag;


            return View();
        }


        public ActionResult Report(string id,int idCARRDetail)      
        {

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);

            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            DataTable dt4 = new DataTable();
            DataTable dt5 = new DataTable();
            DataTable dt6 = new DataTable();
            DataTable dt7 = new DataTable();
            DataTable dt8 = new DataTable();
            DataTable dt9 = new DataTable();
            DataTable dt10 = new DataTable();
            DataTable dt11 = new DataTable();
            DataTable dt12 = new DataTable();
            DataTable dt13 = new DataTable();
            DataTable dt14 = new DataTable();
            DataTable dt15 = new DataTable();
            DataTable dt16 = new DataTable();
            DataTable dt17 = new DataTable();
            DataTable dt18 = new DataTable();
            DataTable dt19 = new DataTable();
            DataTable dt20 = new DataTable();
            DataTable dt21 = new DataTable();


            SqlConnection cnn;
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            cnn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da1 = new SqlDataAdapter();
            SqlDataAdapter da2 = new SqlDataAdapter();
            SqlDataAdapter da3 = new SqlDataAdapter();
            SqlDataAdapter da4 = new SqlDataAdapter();
            SqlDataAdapter da5 = new SqlDataAdapter();
            SqlDataAdapter da6 = new SqlDataAdapter();
            SqlDataAdapter da7 = new SqlDataAdapter();
            SqlDataAdapter da8 = new SqlDataAdapter();
            SqlDataAdapter da9 = new SqlDataAdapter();
            SqlDataAdapter da10 = new SqlDataAdapter();
            SqlDataAdapter da11 = new SqlDataAdapter();
            SqlDataAdapter da12 = new SqlDataAdapter();
            SqlDataAdapter da13 = new SqlDataAdapter();
            SqlDataAdapter da14 = new SqlDataAdapter();
            SqlDataAdapter da15 = new SqlDataAdapter();
            SqlDataAdapter da16 = new SqlDataAdapter();
            SqlDataAdapter da17 = new SqlDataAdapter();
            SqlDataAdapter da18 = new SqlDataAdapter();
            SqlDataAdapter da19 = new SqlDataAdapter();
            SqlDataAdapter da20 = new SqlDataAdapter();
            SqlDataAdapter da21 = new SqlDataAdapter();

            //Customer Details
            cmd = new SqlCommand("sp_CustomerDetails", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da1.SelectCommand = cmd;
            da1.Fill(dt1);
            //Location Details
            cmd = new SqlCommand("sp_LocationDetails", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da2.SelectCommand = cmd;
            da2.Fill(dt2);
            //Service Volumes
            cmd = new SqlCommand("sp_ServiceVolumes", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da3.SelectCommand = cmd;
            da3.Fill(dt3);
            //Exceptions
            cmd = new SqlCommand("sp_Exceptions", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da4.SelectCommand = cmd;
            da4.Fill(dt4);
            //Brokerage
            cmd = new SqlCommand("sp_Brokerage", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da5.SelectCommand = cmd;
            da5.Fill(dt5);
            //Notes
            cmd = new SqlCommand("sp_Notes", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da6.SelectCommand = cmd;
            da6.Fill(dt6);
            //Accessorials
            cmd = new SqlCommand("sp_Accessorials", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da7.SelectCommand = cmd;
            da7.Fill(dt7);
            //Billing Accounts
            cmd = new SqlCommand("sp_BillingAccounts", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da8.SelectCommand = cmd;
            da8.Fill(dt8);
            //DangerousGoods - acceptable
            cmd = new SqlCommand("sp_DGAcceptable", cnn);            
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da9.SelectCommand = cmd;
            da9.Fill(dt9);
            //DangerousGoods - unacceptable
            cmd = new SqlCommand("sp_DGUnAcceptable", cnn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da10.SelectCommand = cmd;
            da10.Fill(dt10);
            //DG Contact
            cmd = new SqlCommand("sp_DGContact", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da11.SelectCommand = cmd;
            da11.Fill(dt11);
            //Returns Info
            cmd = new SqlCommand("sp_ReturnsInfo", cnn);            
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da12.SelectCommand = cmd;
            da12.Fill(dt12);
            //Returns Accessorials
            cmd = new SqlCommand("sp_returnsAccessorials", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da13.SelectCommand = cmd;
            da13.Fill(dt13);
            //Revenue Estimates
            cmd = new SqlCommand("sp_getAllRevEst", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da14.SelectCommand = cmd;
            da14.Fill(dt14);
            //LHRF Requests
            cmd = new SqlCommand("sp_LHRFRequests", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da15.SelectCommand = cmd;
            da15.Fill(dt15);
            //Exceptions
            cmd = new SqlCommand("sp_ExceptionsAccepted", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da16.SelectCommand = cmd;
            da16.Fill(dt16);
            //PPST Volumes
            cmd = new SqlCommand("sp_PPSTVolumes", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da17.SelectCommand = cmd;
            da17.Fill(dt17);
            //Accessorials - Courier
            string svctype = "Courier";
            cmd = new SqlCommand("sp_AccessorialsBySvc", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.Parameters.Add(new SqlParameter("@serviceType", svctype));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da18.SelectCommand = cmd;
            da18.Fill(dt18);
            //Accessorials - Freight
            svctype = "Freight";
            cmd = new SqlCommand("sp_AccessorialsBySvc", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.Parameters.Add(new SqlParameter("@serviceType", svctype));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da19.SelectCommand = cmd;
            da19.Fill(dt19);
            //Accessorials - CPC
            svctype = "CPC";
            cmd = new SqlCommand("sp_AccessorialsBySvc", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.Parameters.Add(new SqlParameter("@serviceType", svctype));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da20.SelectCommand = cmd;
            da20.Fill(dt20);
            //Accessorials - PPST
            svctype = "PPST";
            cmd = new SqlCommand("sp_AccessorialsBySvc", cnn);
            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
            cmd.Parameters.Add(new SqlParameter("@serviceType", svctype));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 10800;
            da21.SelectCommand = cmd;
            da21.Fill(dt21);


            LocalReport lr = new LocalReport();
            lr.EnableHyperlinks = true;
            string path = "";
            string filename = "CARR_" + MyCarr.CustomerLegalName + "_V" + MyCarrDetail.VersionNumber.ToString();
            string DisplayName = "";
            string ExportType = "";
            switch (id)
            {
                case "Excel":
                    path = System.IO.Path.Combine(Server.MapPath("~/Report"), "Report1.rdlc");
                    DisplayName = filename + ".xls";
                    ExportType = "Excel";
                    break;
                case "Word":
                    path = System.IO.Path.Combine(Server.MapPath("~/Report"), "Report2.rdlc");
                    DisplayName = filename + "_Loc.doc";
                    ExportType = "Word";
                    break;
                case "WordAccessorials":
                    path = System.IO.Path.Combine(Server.MapPath("~/Report"), "ReportAccessorials.rdlc");
                    DisplayName = filename + "_Acc.doc";
                    ExportType = "Word";
                    break;
                case "WordBrokerage":
                    path = System.IO.Path.Combine(Server.MapPath("~/Report"), "ReportBrokerage.rdlc");
                    DisplayName = filename + "_Brok.doc";
                    ExportType = "Word";
                    break;
                case "WordAccounts":
                    path = System.IO.Path.Combine(Server.MapPath("~/Report"), "ReportAccounts.rdlc");
                    DisplayName = filename + "_Accts.doc";
                    ExportType = "Word";
                    break;
                case "WordDG":
                    path = System.IO.Path.Combine(Server.MapPath("~/Report"), "ReportDG.rdlc");
                    DisplayName = filename + "_DG.doc";
                    ExportType = "Word";
                    break;
                case "ShippingDet":
                    path = System.IO.Path.Combine(Server.MapPath("~/Report"), "ReportShippingPDF.rdlc");
                    DisplayName = filename + "_Loc.pdf";
                    ExportType = "PDF";
                    break;
                case "Accessorials":
                    path = System.IO.Path.Combine(Server.MapPath("~/Report"), "ReportAccessorialsPDF.rdlc");
                    DisplayName = filename + "_Acc.pdf";
                    ExportType = "PDF";
                    break;
                case "Brokerage":
                    path = System.IO.Path.Combine(Server.MapPath("~/Report"), "ReportBrokeragePDF.rdlc");
                    DisplayName = filename + "_Brok.pdf";
                    ExportType = "PDF";
                    break;
                case "Accounts":
                    path = System.IO.Path.Combine(Server.MapPath("~/Report"), "ReportAccountsPDF.rdlc");
                    DisplayName = filename + "_Accts.pdf";
                    ExportType = "PDF";
                    break;
                default:
                path = System.IO.Path.Combine(Server.MapPath("~/Report"), "Report1.rdlc");
                break;
            }
                
                       
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;                
            }
            else
            {
                return View("Index");
            }

            

            //Get Accessorial Flags info for this CARRDetail
            DataTable dtAccFlag = new DataTable();
            string sqlAccFlag = @"select isnull(c.returnsFlag,0) returnsFlag,isnull(c.courierAccessorialFlag,0) courierAccessorialFlag,
                                        isnull(c.cpcAccessorialFlag,0) cpcAccessorialFlag,isnull(c.freightAccessorialFlag,0) freightAccessorialFlag,
                                        isnull(c.ppstAccessorialFlag,0) ppstAccessorialFlag
                                        from CAR.CARRDetail c
                                        where c.idCARRDetail = ";
            sqlAccFlag = sqlAccFlag + idCARRDetail;
            SqlDataAdapter daAccFlag = new SqlDataAdapter(sqlAccFlag, cnn);
            daAccFlag.Fill(dtAccFlag);


            //Get DGFlag info for this CARRDetail
            DataTable dtDGFlag = new DataTable();
            string sqlDGFlag = @"select count(l.dgFlag) dgFlags
                                        from CAR.CARRDetail c
                                        join CAR.CARRLocation l on l.idCARRDetail = c.idCARRDetail
                                        join CAR.Service s on s.idLocation = l.idLocation
                                        join CAR.ShippingServices ss on ss.idShippingService = s.idShippingService
                                        where s.ActiveFlag=1 and l.ActiveFlag=1 and l.DGFlag=1 and c.idCARRDetail = ";
            sqlDGFlag = sqlDGFlag + idCARRDetail + " group by c.returnsFlag,c.courierAccessorialFlag,c.cpcAccessorialFlag,c.freightAccessorialFlag,ppstAccessorialFlag";

            SqlDataAdapter daDGFlag = new SqlDataAdapter(sqlDGFlag, cnn);
            daDGFlag.Fill(dtDGFlag);

           
            bool PuroPostFlag = Utility.getPPSTFlag(idCARRDetail);

            bool DGFlag = false;
            bool returnsFlag = false;
            bool courierFlag = false;
            bool cpcFlag = false;
            bool freightFlag = false;
            bool ppstFlag = false;

            bool expshippingprofile = true;            
            bool expbrokerage = true;
            bool expaccessorials = true;
            bool expaccounts = true;
            bool explhrf = true;   
            
            //Accessorial Flags
            foreach (DataRow row in dtAccFlag.Rows)
            {
                row["returnsFlag"] = (bool)row["returnsFlag"];                
                courierFlag = (bool)row["courierAccessorialFlag"];
                cpcFlag = (bool)row["cpcAccessorialFlag"];
                freightFlag = (bool)row["freightAccessorialFlag"];
                //ppstFlag = (bool)row["ppstAccessorialFlag"];

                //if (row["cpcAccessorialFlag"] != null)
                //{
                //    cpcFlag = (bool)row["cpcAccessorialFlag"];
                //}
                
                //if (row["freightAccessorialFlag"] != null)
                //{
                //    freightFlag = (bool)row["freightAccessorialFlag"];
                //}
                
                //if (row["ppstAccessorialFlag"] != null)
                //{
                //    ppstFlag = (bool)row["ppstAccessorialFlag"];
                //}
               
            }

            //DG Flag
            foreach (DataRow row in dtDGFlag.Rows)
            {
                if ((int)row["dgFlags"] > 0)
                {
                    DGFlag = true;
                }
            }

            //For renewals, set export flags
            if (MyCarrDetail.idCarrType == 3)
            {
                expshippingprofile = false;
                expbrokerage = false;
                expaccessorials = false;
                expaccounts = false;
                returnsFlag = false;
                DGFlag = false;
                explhrf = false;
            }

            double returnsDimFactor;
            var MyReturnsLHRF = db.CARRLHRFs.Where(x => x.idCARRDetail == idCARRDetail && x.LHRFType == "Returns" && x.ActiveFlag == true).FirstOrDefault();
            returnsDimFactor = (double)MyReturnsLHRF.ReturnsDimFactor;

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
            ReportParameter[] Params = new ReportParameter[16];
            Params[0] = new ReportParameter("DGFlag", DGFlag.ToString());
            Params[1] = new ReportParameter("returnsFlag", returnsFlag.ToString());
            Params[2] = new ReportParameter("ReturnsDimFactor", returnsDimFactor.ToString());
            Params[3] = new ReportParameter("expShippingProfile", expshippingprofile.ToString());
            Params[4] = new ReportParameter("expBrokerage", expbrokerage.ToString());
            Params[5] = new ReportParameter("expAccessorials", expaccessorials.ToString());
            Params[6] = new ReportParameter("expAccounts", expaccounts.ToString());
            Params[7] = new ReportParameter("expLHRF", explhrf.ToString());
            Params[8] = new ReportParameter("CustomerName", MyCarr.CustomerLegalName);
            Params[9] = new ReportParameter("ContractNumber",ContractNumber.ToString());
            Params[10] = new ReportParameter("ContractEffectiveDate", ContractFromDate);
            Params[11] = new ReportParameter("ContractExpiryDate", ContractToDate);
            Params[12] = new ReportParameter("ppstFlag", PuroPostFlag.ToString());
            Params[13] = new ReportParameter("courierFlag", courierFlag.ToString());
            Params[14] = new ReportParameter("freightFlag", freightFlag.ToString());
            Params[15] = new ReportParameter("cpcFlag", cpcFlag.ToString());
           
            lr.SetParameters(Params);
           


            ReportDataSource rptData1 = new ReportDataSource("DataSet1", dt1);
            lr.DataSources.Add(rptData1);
            ReportDataSource rptData2 = new ReportDataSource("DataSet2", dt2);
            lr.DataSources.Add(rptData2);
            ReportDataSource rptData3 = new ReportDataSource("DataSet3", dt3);
            lr.DataSources.Add(rptData3);
            ReportDataSource rptData4 = new ReportDataSource("DataSet4", dt4);
            lr.DataSources.Add(rptData4);
            ReportDataSource rptData5 = new ReportDataSource("DataSet5", dt5);
            lr.DataSources.Add(rptData5);
            ReportDataSource rptData6 = new ReportDataSource("DataSet6", dt6);
            lr.DataSources.Add(rptData6);
            ReportDataSource rptData7 = new ReportDataSource("DataSet7", dt7);
            lr.DataSources.Add(rptData7);
            ReportDataSource rptData8 = new ReportDataSource("DataSet8", dt8);
            lr.DataSources.Add(rptData8);
            ReportDataSource rptData9 = new ReportDataSource("DataSet9", dt9);
            lr.DataSources.Add(rptData9);
            ReportDataSource rptData10 = new ReportDataSource("DataSet10", dt10);
            lr.DataSources.Add(rptData10);
            ReportDataSource rptData11 = new ReportDataSource("DataSet11", dt11);
            lr.DataSources.Add(rptData11);
            ReportDataSource rptData12 = new ReportDataSource("DataSet12", dt12);
            lr.DataSources.Add(rptData12);
            ReportDataSource rptData13 = new ReportDataSource("DataSet13", dt13);
            lr.DataSources.Add(rptData13);
            ReportDataSource rptData14 = new ReportDataSource("DataSet14", dt14);
            lr.DataSources.Add(rptData14);
            ReportDataSource rptData15 = new ReportDataSource("DataSet15", dt15);
            lr.DataSources.Add(rptData15);
            ReportDataSource rptData16 = new ReportDataSource("DataSet16", dt16);
            lr.DataSources.Add(rptData16);
            ReportDataSource rptData17 = new ReportDataSource("DataSet17", dt17);
            lr.DataSources.Add(rptData17);
            ReportDataSource rptData18 = new ReportDataSource("DataSet18", dt18);
            lr.DataSources.Add(rptData18);
            ReportDataSource rptData19 = new ReportDataSource("DataSet19", dt19);
            lr.DataSources.Add(rptData19);
            ReportDataSource rptData20 = new ReportDataSource("DataSet20", dt20);
            lr.DataSources.Add(rptData20);
            ReportDataSource rptData21 = new ReportDataSource("DataSet21", dt21);
            lr.DataSources.Add(rptData21);


            string reportType = ExportType;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>" + id + "</OutputFormat>" +
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

        [HttpPost]
        [Authorize]
        public ActionResult FinalRates()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            string desc = Request.Form["Description"];
            string username = Session["accountname"].ToString();
            string userrole = Session["userrole"].ToString();
            ViewBag.userrole = userrole;

            ViewBag.Title = "Export Final Rates";
            ViewBag.VDAFiles = Utility.getVDAFileTableData(idCARRDetail);
            ViewBag.rateFiles = Utility.getRateFileTableData(idCARRDetail);
            

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
            ViewBag.idCARRDetail = idCARRDetail;
            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult USPricingExport()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            ViewBag.idCARRDetail = idCARRDetail;

            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();
            
           

            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.ppstFlag = MyCarrDetail.ppstAccessorialFlag;
            ViewBag.courierFlag = MyCarrDetail.courierAccessorialFlag ?? false;
            ViewBag.freightFlag = MyCarrDetail.freightAccessorialFlag ?? false;
            ViewBag.cpcFlag = MyCarrDetail.cpcAccessorialFlag ?? false;
            ViewBag.ppstFlag = MyCarrDetail.ppstAccessorialFlag ?? false;

            //Need a PPST only flag
            bool otherFlag = false;
           otherFlag = Utility.getOtherFlag(idCARRDetail); ; 
            ViewBag.USPricingExport = otherFlag;

            bool PuroPostFlag = Utility.getPPSTFlag(idCARRDetail);
            ViewBag.USPuroPostExport = PuroPostFlag;

            ViewBag.Title = "US Pricing Excel Export";

            return View();
        }
        //public ActionResult DoUSPricingExport(int idCARRDetail, bool? courierAccessorialFlag, bool? freightAccessorialFlag, bool? cpcAccessorialFlag)
    //    public ActionResult DoUSPricingExport(int idCARRDetail)
    //    {
    //        //{            

    //        //
    //        //GET DATA FIRST
    //        //
    //        //DATA ELEMENTS NEEDED
    //        var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
    //        var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);
    //        var MyDefaultExchange = db.KeyValuePairs.Where(x => x.Key == "DefaultExchangeRate").FirstOrDefault();

    //        if (MyCarrDetail.courierAccessorialFlag == null)
    //            MyCarrDetail.courierAccessorialFlag = false;
    //        if (MyCarrDetail.freightAccessorialFlag == null)
    //            MyCarrDetail.freightAccessorialFlag = false;
    //        if (MyCarrDetail.cpcAccessorialFlag == null)
    //            MyCarrDetail.cpcAccessorialFlag = false;
    //        if (MyCarrDetail.returnsFlag == null)
    //            MyCarrDetail.returnsFlag = false;

    //        //Set Service Flags            
    //        bool courierFlag = false;
    //        bool courierAccessorialFlag = (bool)MyCarrDetail.courierAccessorialFlag;
    //        bool freightAccessorialFlag = (bool)MyCarrDetail.freightAccessorialFlag;
    //        bool cpcAccessorialFlag = (bool)MyCarrDetail.cpcAccessorialFlag;
    //        bool cpcexpeditedFlag = false;
    //        bool cpcxpresspostFlag = false;
    //        bool ltlyyzFlag = false;
    //        bool ltlyvrFlag = false;
    //        bool ltlyulFlag = false;
    //        bool ltlywgFlag = false;
    //        bool returnsFlag = (bool)MyCarrDetail.returnsFlag;                
    //        bool courieryyzFlag = false;
    //        bool courieryvrFlag = false;
    //        bool courieryulFlag = false;
    //        bool courierywgFlag = false;
    //        //bool ppstFlag = false;
    //        string errormessage = "";

    //        DataTable dtLHRFSingle = new DataTable();
    //        DataTable dtLHRFDual = new DataTable();
    //        DataTable dtLHRFForms = new DataTable();
    //        DataTable dtReturn = new DataTable();
    //        DataTable dtSvcs = new DataTable();
    //        DataTable dtVol = new DataTable();

    //        //LHRFs - Non GLHG
    //        String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;            
    //        SqlConnection connection = new SqlConnection(connectionString);
    //        try
    //        {
    //            connection.Open();
    //            string sqlLHRF = @" select Concat(lh.PUAddress,' ',lh.PUCity,' ',lh.PUState,' ',lh.PUPostalCode) Origin,
    //                            Concat(lh.DestAddress,' ',lh.DestCity,' ',lh.DestState,' ',lh.DestPostalCode) Destination,
    //                            PUCity,DestCity,LHRFType
    //                FROM CAR.CARRLHRF lh
    //                where LHRFType != 'Returns' and LHRFType != 'GLHG' and ActiveFlag = 1 and idCARRDetail = ";
    //            sqlLHRF = sqlLHRF + idCARRDetail + " order by LHRFType";
                
    //            SqlDataAdapter daLHRF = new SqlDataAdapter(sqlLHRF, connection);
    //            daLHRF.Fill(dtLHRFForms);                
    //        }
    //        finally
    //        {
    //            //connection.Close();
    //            //connection.Dispose();
    //        }

    //        //LHRF rates generated by Tool, and LHRF forms for GLHG - Single
    //        try
    //        {
    //            SqlCommand cmd = new SqlCommand();
    //            SqlDataAdapter da = new SqlDataAdapter();
    //            bool dualFlag = false;
    //            cmd = new SqlCommand("sp_LHRFRatesWithDual", connection);
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
    //            cmd.Parameters.Add(new SqlParameter("@dualFlag", dualFlag));
    //            da.SelectCommand = cmd;
    //            da.Fill(dtLHRFSingle);
    //        }
    //        finally
    //        {
    //            // connection.Close();
    //            //connection.Dispose();
    //        }

    //        //LHRF rates generated by Tool, and LHRF forms for GLHG - Dual
    //        try
    //        {
    //            SqlCommand cmd = new SqlCommand();
    //            SqlDataAdapter da = new SqlDataAdapter();
    //            bool dualFlag = true;
    //            cmd = new SqlCommand("sp_LHRFRatesWithDual", connection);
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
    //            cmd.Parameters.Add(new SqlParameter("@dualFlag", dualFlag));
    //            da.SelectCommand = cmd;
    //            da.Fill(dtLHRFDual);
    //        }
    //        finally
    //        {
    //            // connection.Close();
    //            //connection.Dispose();
    //        }


    //        //Returns           
    //        try
    //        {
    //            //connection.Open();
    //            string sqlLHRF = @" select *
    //                FROM CAR.CARRLHRF
    //                where LHRFType = 'Returns' and ActiveFlag = 1 and idCARRDetail = ";
    //            sqlLHRF = sqlLHRF + idCARRDetail;

    //            SqlDataAdapter daLHRF = new SqlDataAdapter(sqlLHRF, connection);
    //            daLHRF.Fill(dtReturn);
    //        }
    //        finally
    //        {
    //            //connection.Close();
    //            //connection.Dispose();
    //        }

    //        //Check Services For Courier, CPC and PPST Flags
    //        try
    //        {
    //            //connection.Open();
    //            string sqlSVCs = @" select ss.idShippingService,ss.Description,s.idService
    //                FROM CAR.Service s
				//	join CAR.CARRLocation l on l.idLocation = s.idLocation
				//	join CAR.ShippingServices ss on ss.idShippingService = s.idShippingService
    //                where s.ActiveFlag = 1 and l.ActiveFlag = 1 and l.idCARRDetail = ";
    //            sqlSVCs = sqlSVCs + idCARRDetail;

    //            SqlDataAdapter daSVC = new SqlDataAdapter(sqlSVCs, connection);
    //            daSVC.Fill(dtSvcs);
    //        }
    //        finally
    //        {
    //            //connection.Close();
    //            //connection.Dispose();
    //        }

    //        foreach (DataRow svcrow in dtSvcs.Rows)
    //        {
    //            switch (svcrow["idShippingService"])
    //            {
    //                case 1:
    //                case 2:                    
    //                    courierFlag = true;
    //                    //get CAVY flags
    //                    try
    //                    {
    //                        string sqlVol = @" select distinct ip.code FROM CAR.ServiceVolume sv
				//	        join CAR.InductionPoints ip on ip.idInduction = sv.idInduction where sv.idService=";
    //                        sqlVol = sqlVol + svcrow["idService"];
    //                        SqlDataAdapter daVol = new SqlDataAdapter(sqlVol, connection);
    //                        daVol.Fill(dtVol);
    //                        foreach (DataRow volrow in dtVol.Rows)
    //                        {
    //                            switch (volrow["code"].ToString().ToLower())
    //                            {
    //                                case "yyz":
    //                                    courieryyzFlag = true;
    //                                    break;
    //                                case "yul":
    //                                    courieryulFlag = true;
    //                                    break;
    //                                case "yvr":
    //                                    courieryvrFlag = true;
    //                                    break;
    //                                case "ywg":
    //                                    courierywgFlag = true;
    //                                    break;
    //                            }
    //                        }
    //                    }
    //                    catch (System.Exception ex)
    //                    {

    //                    }
    //                    break;
    //                case 3:
    //                    cpcexpeditedFlag = true;
    //                    break;
    //                case 8:
    //                    cpcxpresspostFlag = true;
    //                    break;
    //                //case 6:
    //                //    ppstFlag = true;
    //                //    break;
    //                //case 7:
    //                //    ppstFlag = true;
    //                //    break;
    //                //case 9:
    //                //    ppstFlag = true;
    //                //    break;
    //                case 5:
    //                    //select Volume to get induction
    //                    try
    //                    {
    //                        string sqlVol = @" select distinct ip.code FROM CAR.ServiceVolume sv
				//	        join CAR.InductionPoints ip on ip.idInduction = sv.idInduction where  sv.idService=";
    //                        sqlVol = sqlVol + svcrow["idService"];
    //                        SqlDataAdapter daVol = new SqlDataAdapter(sqlVol, connection);
    //                        daVol.Fill(dtVol);
    //                        foreach (DataRow volrow in dtVol.Rows)
    //                        {
    //                            switch (volrow["code"].ToString().ToLower())
    //                            {
    //                                case "yyz":
    //                                    ltlyyzFlag = true;
    //                                    break;
    //                                case "yul":
    //                                    ltlyulFlag = true;
    //                                    break;
    //                                case "yvr":
    //                                    ltlyvrFlag = true;
    //                                    break;
    //                                case "ywg":
    //                                    ltlywgFlag = true;
    //                                    break;
    //                            }
    //                        }
    //                    } 
    //                    catch (System.Exception ex)
    //                    {

    //                    }
    //                    break;
    //            }
                    
    //        }

    //        try
    //        {
    //            connection.Close();
    //            connection.Dispose();
    //        }
    //        catch (System.Exception ex)
    //        {
    //            errormessage = ex.Message;
    //        }


    //        //Details used in Partial Info Bar            
    //        ViewBag.MyCarr = MyCarr;
    //        ViewBag.MyCarrDetail = MyCarrDetail;
    //        ViewBag.Title = "US Pricing Excel Export";

    //        //Data Needed in MASTERDATA sheet
    //        string customername = MyCarr.CustomerLegalName;
    //        string contractnumber = MyCarrDetail.ContractNumber;
    //        string startdate = MyCarrDetail.ContractFromDate.ToString();
    //        string enddate = MyCarrDetail.ContractToDate.ToString();
    //        string exchangerate = MyDefaultExchange.Value;
    //        string currencycode = MyCarrDetail.Currency;


    //        string template = ConfigurationManager.AppSettings["USPricingTemplate"].ToString();
    //        string exportpath = ConfigurationManager.AppSettings["USPricingExport"].ToString();
    //        string uspricingpwd = ConfigurationManager.AppSettings["USPricingPWD"].ToString();
    //        string exportFilename = "USPricing_" + MyCarr.CustomerLegalName + "_v" + MyCarrDetail.VersionNumber.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            
    //        //replace special characters
    //        exportFilename = exportFilename.Replace("/", "");
    //        exportFilename = exportFilename.Replace("&", "");
    //        exportFilename = exportFilename.Replace("<", "");
    //        exportFilename = exportFilename.Replace(">", "");


    //        string exportFilepath = exportpath + exportFilename;


    //        //
    //        //SECOND, CREATE NEW EXCEL FILE
    //        //
    //        //OPEN EXCEL TEMPLATE
    //        Excel.Application xlApp = new Excel.Application();
    //        xlApp.DisplayAlerts = false;
    //        Excel.Workbook templateWorkbook = xlApp.Workbooks.Open(template);

    //        //SAVE A NEW COPY TO WORK WITH
    //        templateWorkbook.SaveCopyAs(exportFilepath);
    //        //close and open new copy
    //        templateWorkbook.Close();
           
            
    //        Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(exportFilepath);


    //        //READ EXCEL TEMPLATE
    //        try
    //        {
    //            //Read the Master Data Sheet
    //            Excel._Worksheet masterDataWorksheet = xlWorkbook.Sheets["MasterData"];             

    //            //Fill In Master Data Values
    //            var customerNamecell = masterDataWorksheet.Evaluate("CustomerName");
    //            customerNamecell.Value = customername;
    //            var startDatecell = masterDataWorksheet.Evaluate("StartDate");
    //            startDatecell.Value = startdate;
    //            var endDatecell = masterDataWorksheet.Evaluate("EndDate");
    //            endDatecell.Value = enddate;
    //            var exchangeRatecell = masterDataWorksheet.Evaluate("ExchangeRate");
    //            exchangeRatecell.Value = exchangerate;
    //            var currencyCodecell = masterDataWorksheet.Evaluate("CurrencyCode");
    //            currencyCodecell.Value = currencycode;

    //            //CAVY - Hide unused ranges
               
    //            try
    //            {
    //                if (courierFlag == true)
    //                {
    //                    //Read the CAVY Sheet
    //                    Excel._Worksheet cavyWorksheet = xlWorkbook.Sheets["Courier SAP CAVY"];
    //                    if (courieryyzFlag == false)
    //                    {
    //                        //hide yyz range
    //                        var courieryyzrange = cavyWorksheet.Evaluate("CAVYYYZ");
    //                        courieryyzrange.EntireRow.Hidden = true;
    //                    }
    //                    if (courieryvrFlag == false)
    //                    {
    //                        //hide yvr range
    //                        var courieryvrrange = cavyWorksheet.Evaluate("CAVYYVR");
    //                        courieryvrrange.EntireRow.Hidden = true;
    //                    }
    //                    if (courieryulFlag == false)
    //                    {
    //                        //hide yul range
    //                        var courieryulrange = cavyWorksheet.Evaluate("CAVYYUL");
    //                        courieryulrange.EntireRow.Hidden = true;
    //                    }
    //                    if (courierywgFlag == false)
    //                    {
    //                        //hide ywg range
    //                        var courierywgrange = cavyWorksheet.Evaluate("CAVYYWG");
    //                        courierywgrange.EntireRow.Hidden = true;
    //                    }
    //                }
    //            }
    //            catch (System.Exception ex)
    //            {
    //                errormessage = ex.Message;
    //            }
             

    //            //Add LH Unique Number sequence
    //            int lhcount = 0;

    //            //1. LH RATES FROM TOOL & LHRF forms for GLHG Single
    //            //Open Single LH Template
    //            Excel._Worksheet LHSingleTemplate = xlWorkbook.Sheets["LH Single Template"];                
    //            var xlSheets = xlWorkbook.Sheets as Excel.Sheets;               

    //            try
    //            {
    //                foreach (DataRow lhrfRate in dtLHRFSingle.Rows)
    //                {
    //                    lhcount = lhcount + 1;                        
    //                    string newsheetname = "LH Single_" + lhcount.ToString();

    //                    int sheetnum = xlWorkbook.Sheets.Count;
    //                    LHSingleTemplate.Copy(xlWorkbook.Sheets[sheetnum]);
    //                    xlWorkbook.Sheets[sheetnum].Name = newsheetname;
                        
    //                    xlWorkbook.Save();

    //                    Excel._Worksheet LHSingleWorksheet = xlWorkbook.Sheets[newsheetname];

    //                    //Fill In CARR Data
    //                    var LHheadingcell = LHSingleWorksheet.Evaluate("LHSingleHeader");
    //                    LHheadingcell.Value = "Northbound Linehaul Pricing, Single Induction " + lhrfRate["LocationName"].ToString();
    //                    var Origincell = LHSingleWorksheet.Evaluate("LHSingleOrigin");
    //                    Origincell.Value = lhrfRate["Origin"].ToString();
    //                    var Destinationcell = LHSingleWorksheet.Evaluate("LHSingleDestination");
    //                    Destinationcell.Value = lhrfRate["Destination"].ToString();
    //                    var TransitDayscell = LHSingleWorksheet.Evaluate("LHSingleTransitDays");
    //                    TransitDayscell.Value = lhrfRate["TransitDays"].ToString();


    //                    //Rates
    //                    var LHmincell = LHSingleWorksheet.Evaluate("LHmin");
    //                    LHmincell.Value = lhrfRate["LHMin"].ToString();
    //                    var LHl5ccell = LHSingleWorksheet.Evaluate("LHl5c");
    //                    LHl5ccell.Value = lhrfRate["LH100"].ToString();
    //                    var LH500lbscell = LHSingleWorksheet.Evaluate("LH500lbs");
    //                    LH500lbscell.Value = lhrfRate["LH500"].ToString();
    //                    var LH1000lbscell = LHSingleWorksheet.Evaluate("LH1000lbs");
    //                    LH1000lbscell.Value = lhrfRate["LH1000"].ToString();
    //                    var LH2000lbscell = LHSingleWorksheet.Evaluate("LH2000lbs");
    //                    LH2000lbscell.Value = lhrfRate["LH2000"].ToString();
    //                    var LH5000lbscell = LHSingleWorksheet.Evaluate("LH5000lbs");
    //                    LH5000lbscell.Value = lhrfRate["LH5000"].ToString();
    //                    var LH10000lbscell = LHSingleWorksheet.Evaluate("LH10000lbs");
    //                    LH10000lbscell.Value = lhrfRate["LH10000"].ToString();                        

    //                    xlWorkbook.Save();

    //                }
    //            }
    //            catch (System.Exception ex)
    //            {
    //                errormessage = ex.Message;
    //            }

    //            //2. LH RATES FROM TOOL & LHRF forms for GLHG DUAL
    //            //Open Single LH Template
    //            Excel._Worksheet LHDualTemplate = xlWorkbook.Sheets["LH Dual Template"];
    //            //var xlSheets = xlWorkbook.Sheets as Excel.Sheets;

    //            try
    //            {
    //                foreach (DataRow lhrfRate in dtLHRFDual.Rows)
    //                {
    //                    lhcount = lhcount + 1;
    //                    string newsheetname = "LH Dual_" + lhcount.ToString();

    //                    int sheetnum = xlWorkbook.Sheets.Count;
    //                    LHDualTemplate.Copy(xlWorkbook.Sheets[sheetnum]);
    //                    xlWorkbook.Sheets[sheetnum].Name = newsheetname;

    //                    xlWorkbook.Save();

    //                    Excel._Worksheet LHDualWorksheet = xlWorkbook.Sheets[newsheetname];

    //                    //Fill In CARR Data - Location 1
    //                    var LHheadingcell = LHDualWorksheet.Evaluate("LHSingleHeader");
    //                    LHheadingcell.Value = "Northbound Linehaul Pricing, Single Induction, " + lhrfRate["LocationName"].ToString();

    //                    //ORIGIN DEST - Location 1
    //                    var DualOrigincell1 = LHDualWorksheet.Evaluate("LHDualOrigin1");
    //                    DualOrigincell1.Value = lhrfRate["Origin"].ToString();
    //                    var DualDestinationcell1 = LHDualWorksheet.Evaluate("LHDualDestination1");
    //                    DualDestinationcell1.Value = lhrfRate["Destination"].ToString();
    //                    var DualTransitDayscell1 = LHDualWorksheet.Evaluate("LHDualTransitDays1");
    //                    DualTransitDayscell1.Value = lhrfRate["TransitDays"].ToString();

    //                    //Rates - Location 1
    //                    var LHmincell1 = LHDualWorksheet.Evaluate("LHMin1");
    //                    LHmincell1.Value = lhrfRate["LHMin"].ToString();
    //                    var LHl5ccell1 = LHDualWorksheet.Evaluate("LHl5c1");
    //                    LHl5ccell1.Value = lhrfRate["LH100"].ToString();
    //                    var LH500lbscell1 = LHDualWorksheet.Evaluate("LH500lbs1");
    //                    LH500lbscell1.Value = lhrfRate["LH500"].ToString();
    //                    var LH1000lbscell1 = LHDualWorksheet.Evaluate("LH1000lbs1");
    //                    LH1000lbscell1.Value = lhrfRate["LH1000"].ToString();
    //                    var LH2000lbscell1 = LHDualWorksheet.Evaluate("LH2000lbs1");
    //                    LH2000lbscell1.Value = lhrfRate["LH2000"].ToString();
    //                    var LH5000lbscell1 = LHDualWorksheet.Evaluate("LH5000lbs1");
    //                    LH5000lbscell1.Value = lhrfRate["LH5000"].ToString();
    //                    var LH10000lbscell1 = LHDualWorksheet.Evaluate("LH10000lbs1");
    //                    LH10000lbscell1.Value = lhrfRate["LH10000"].ToString();

    //                    //ORIGIN DEST - Location 1                       
    //                    var DualOrigincell2 = LHDualWorksheet.Evaluate("LHDualOrigin2");
    //                    DualOrigincell2.Value = lhrfRate["DOrigin"].ToString();
    //                    var Destinationcell2 = LHDualWorksheet.Evaluate("LHDualDestination2");
    //                    Destinationcell2.Value = lhrfRate["DDestination"].ToString();
    //                    var TransitDayscell2 = LHDualWorksheet.Evaluate("LHDualTransitDays2");
    //                    TransitDayscell2.Value = lhrfRate["DTransitDays"].ToString();

    //                    //Rates - Location 2
    //                    var LHmincell2 = LHDualWorksheet.Evaluate("LHMin2");
    //                    LHmincell2.Value = lhrfRate["DMin"].ToString();
    //                    var LHl5ccell2 = LHDualWorksheet.Evaluate("LHl5c2");
    //                    LHl5ccell2.Value = lhrfRate["D100"].ToString();
    //                    var LH500lbscell2 = LHDualWorksheet.Evaluate("LH500lbs2");
    //                    LH500lbscell2.Value = lhrfRate["D500"].ToString();
    //                    var LH1000lbscell2 = LHDualWorksheet.Evaluate("LH1000lbs2");
    //                    LH1000lbscell2.Value = lhrfRate["D1000"].ToString();
    //                    var LH2000lbscell2 = LHDualWorksheet.Evaluate("LH2000lbs2");
    //                    LH2000lbscell2.Value = lhrfRate["D2000"].ToString();
    //                    var LH5000lbscell2 = LHDualWorksheet.Evaluate("LH5000lbs2");
    //                    LH5000lbscell2.Value = lhrfRate["D5000"].ToString();
    //                    var LH10000lbscell2 = LHDualWorksheet.Evaluate("LH10000lbs2");
    //                    LH10000lbscell2.Value = lhrfRate["D10000"].ToString();

    //                    xlWorkbook.Save();

    //                }
    //            }
    //            catch (System.Exception ex)
    //            {
    //                errormessage = ex.Message;
    //            }

    //            // 3. For each LHRF Form filled out
    //            // Do CASE on LHRFType: AIRCO, AIRD, LTLD, TL and select the right template
    //            foreach (DataRow lhrow in dtLHRFForms.Rows)
    //            {
    //                try
    //                {
    //                    string lhtype = lhrow["LHRFType"].ToString().Trim();
    //                    string templatename = "";
    //                    string sheetname = "";
    //                    string lhdesc = "";
    //                    string lhoriginname = "";
    //                    string lhdestinationname = "";
    //                    string lhheader = "";

    //                    lhcount = lhcount + 1;

    //                    switch (lhtype)
    //                    {
    //                        case "AIRCO":
    //                            templatename = "LH AIRCO Template";
    //                            sheetname = "LH AIRCO";
    //                            lhdesc = "Air Charter Pricing ";
    //                            lhheader = "LHAIRCOHeader";
    //                            lhoriginname = "LHAIRCOOrigin";
    //                            lhdestinationname = "LHAIRCODestination";
    //                            break;
    //                        case "AIRD":
    //                            templatename = "LH AIRD Template";
    //                            sheetname = "LH AIRD";
    //                            lhdesc = "Air Charter Pricing ";
    //                            lhheader = "LHAIRDHeader";
    //                            lhoriginname = "LHAIRDOrigin";
    //                            lhdestinationname = "LHAIRDDestination";
    //                            break;
    //                        case "GLHG":
    //                            //leave code here, but there should not be any GLHG
    //                            templatename = "LH Single Template";
    //                            sheetname = "LH Single";
    //                            lhdesc = "Northbound Linehaul Pricing, Single Induction ";
    //                            lhheader = "LHSingleHeader";
    //                            lhoriginname = "LHSingleOrigin";
    //                            lhdestinationname = "LHSingleDestination";
    //                            break;
    //                        case "LOC":
    //                            templatename = "LH Single Template";
    //                            sheetname = "LH Single";
    //                            lhdesc = "Customer Pickup";
    //                            lhheader = "LHSingleHeader";
    //                            lhoriginname = "LHSingleOrigin";
    //                            lhdestinationname = "LHSingleDestination";
    //                            break;
    //                        case "LTLD":
    //                            templatename = "LH Single Template";
    //                            sheetname = "LH Single";
    //                            lhdesc = "LTLD";
    //                            lhheader = "LHSingleHeader";
    //                            lhoriginname = "LHSingleOrigin";
    //                            lhdestinationname = "LHSingleDestination";
    //                            break;
    //                        case "TL":
    //                            templatename = "LH FTL Template";
    //                            sheetname = "LH FTL";
    //                            lhdesc = "Full Truck Load Pricing ";
    //                            lhheader = "LHFTLHeader";
    //                            lhoriginname = "LHFTLOrigin";
    //                            lhdestinationname = "LHFTLDestination";
    //                            break;
                           
    //                    }
    //                    string newsheetname = sheetname + "_" + lhcount.ToString();
    //                    int sheetnum = xlWorkbook.Sheets.Count;
    //                    Excel._Worksheet LHTemplate = xlWorkbook.Sheets[templatename];

    //                    LHTemplate.Copy(xlWorkbook.Sheets[sheetnum]);
    //                    xlWorkbook.Sheets[sheetnum].Name = newsheetname;

    //                    xlWorkbook.Save();


    //                    Excel._Worksheet LHWorksheet = xlWorkbook.Sheets[newsheetname];

    //                    var lhheadercell = LHWorksheet.Evaluate(lhheader);
    //                    lhheadercell.Value = lhdesc + lhrow["PUCity"].ToString() + " to " + lhrow["DestCity"].ToString();
    //                    var lhorigincell = LHWorksheet.Evaluate(lhoriginname);
    //                    lhorigincell.Value = lhrow["Origin"].ToString();
    //                    var lhdestinationcell = LHWorksheet.Evaluate(lhdestinationname);
    //                    lhdestinationcell.Value = lhrow["Destination"].ToString();


    //                    xlWorkbook.Save();
    //                }
    //                catch (System.Exception ex)
    //                {
    //                    errormessage = ex.Message;
    //                }
                    
                   

    //            }

    //            // 4. For Returns -dtReturn
    //            if (returnsFlag == true)
    //            {
    //                try
    //                {

    //                    // 1. LH FORM
    //                    DataRow returnLHRF = dtReturn.Rows[0];
    //                    string origin = returnLHRF["PUAddress"].ToString();
    //                    string destination = returnLHRF["DestAddress"].ToString();

    //                    Excel._Worksheet LHReturnsTemplate = xlWorkbook.Sheets["LH Returns Template"];
    //                    string newsheetname = "LH_RETURNS";

    //                    int sheetnum = xlWorkbook.Sheets.Count;
    //                    LHReturnsTemplate.Copy(xlWorkbook.Sheets[sheetnum]);
    //                    xlWorkbook.Sheets[sheetnum].Name = newsheetname;

    //                    Excel._Worksheet ReturnsLHWorksheet = xlWorkbook.Sheets[newsheetname];
    //                    var returnsorigincell = ReturnsLHWorksheet.Evaluate("LHReturnsOrigin");
    //                    returnsorigincell.Value = origin;
    //                    var returnsdestinationcell = ReturnsLHWorksheet.Evaluate("LHReturnsDestination");
    //                    returnsdestinationcell.Value = destination;

    //                    // 2.CANADIAN RETURNS FORM
    //                    Excel._Worksheet CanadianReturnsTemplate = xlWorkbook.Sheets["Canadian Returns Template"];
    //                    string newsheetname2 = "Canadian Returns";

    //                    int sheetnum2 = xlWorkbook.Sheets.Count;
    //                    CanadianReturnsTemplate.Copy(xlWorkbook.Sheets[sheetnum2]);
    //                    xlWorkbook.Save();
    //                    xlWorkbook.Sheets[sheetnum2].Name = newsheetname2;                        

    //                    xlWorkbook.Save();
    //                }
    //                catch (System.Exception ex)
    //                {
    //                    errormessage = ex.Message;
    //                }
                    

    //            }



    //            // WORKSHEETS
    //            // MasterData
    //            // Estimated Daily Revenue
    //            // Linehaul Single Origin Template                
    //            // Linehaul Dual Origin Template                
    //            // FTL Template                
    //            // Air Charter Template                
    //            // Commercial Air Template                
    //            // Pricing 
    //            // Courier SAP CAVY            
    //            // CPC Expedited 
    //            // CPC Xpresspost
    //            // LTL Charges-YYZ Data
    //            // LTL Charges-YVR Data
    //            // LTL Charges-YUL Data
    //            // LTL Charges-YWG Data
    //            // CDN LTL-YYZ
    //            // CDN LTL-YVR 
    //            // CDN LTL-YUL 
    //            // CDN LTL-YWG 
    //            // Courier Worksheet
    //            // Courier Accessorials
    //            // Freight Accessorials
    //            // CPC Accessorials

    //            //More sheets are added as the templates are copied


    //            //Hide Unneeded Worksheets

    //            //COURIER
    //            try
    //            {
    //                if (courierFlag == false)
    //                {
    //                    xlWorkbook.Sheets["Courier Worksheet"].Visible = Excel.XlSheetVisibility.xlSheetHidden;                       
    //                }                   
    //                if (courierAccessorialFlag == false)
    //                {                       
    //                    xlWorkbook.Sheets["Courier Accessorials"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                }     
    //                if (freightAccessorialFlag == false)
    //                {                       
    //                      xlWorkbook.Sheets["Freight Accessorials"].Visible = Excel.XlSheetVisibility.xlSheetHidden;                       
    //                }                    
    //                if (cpcAccessorialFlag == false)
    //                {
    //                    xlWorkbook.Sheets["CPC Accessorials"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                }                  
    //                //LTL YVR
    //                if (ltlyvrFlag == false)
    //                {
    //                    xlWorkbook.Sheets["CDN LTL-YVR"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                }                    
    //                //LTL YYZ
    //                if (ltlyyzFlag == false)
    //                {
    //                    xlWorkbook.Sheets["CDN LTL-YYZ"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                }                  
    //                //LTL YVR
    //                if (ltlyulFlag == false)
    //                {
    //                    xlWorkbook.Sheets["CDN LTL-YUL"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                }
    //                //LTL YYZ
    //                if (ltlywgFlag == false)
    //                {
    //                    xlWorkbook.Sheets["CDN LTL-YWG"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                }
    //                //LTL Pricing Sheets
    //                xlWorkbook.Sheets["LTL Charges-YYZ Data"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                xlWorkbook.Sheets["LTL Charges-YVR Data"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                xlWorkbook.Sheets["LTL Charges-YUL Data"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                xlWorkbook.Sheets["LTL Charges-YWG Data"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                //CPC XpressPost
    //                if (cpcxpresspostFlag == false)
    //                {
    //                    xlWorkbook.Sheets["CPC Xpresspost"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                }
    //                //CPC Expedited
    //                if (cpcexpeditedFlag == false)
    //                {
    //                    xlWorkbook.Sheets["CPC Expedited"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                }
    //                //COURIER
    //                if (courierFlag == false)
    //                {                        
    //                    xlWorkbook.Sheets["Courier SAP CAVY"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                }
    //                //else
    //                //{
    //                //    xlWorkbook.Sheets["Courier SAP CAVY"].Protect(uspricingpwd, true);
    //                //}
                   
    //                 //Pricing Data
    //                 xlWorkbook.Sheets["Pricing"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                //Templates  
    //                xlWorkbook.Sheets["LH Single Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                xlWorkbook.Sheets["LH Dual Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                xlWorkbook.Sheets["LH FTL Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                xlWorkbook.Sheets["LH AIRD Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                xlWorkbook.Sheets["LH AIRCO Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                xlWorkbook.Sheets["LH Returns Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                xlWorkbook.Sheets["Canadian Returns Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;

    //                //For Intr-Canada, no need to show the CAVY Sheet
    //                if (courieryyzFlag == false && courieryvrFlag == false && courieryulFlag == false && courierywgFlag)
    //                {
    //                    xlWorkbook.Sheets["Courier SAP CAVY"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
    //                }

    //                //password protect all sheets
    //                foreach (Excel.Worksheet sheet in xlWorkbook.Worksheets)
    //                {
    //                    sheet.Protect(uspricingpwd, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false);
    //                }

    //                //Save and Close
    //                //activate first sheet, masterdata
    //                masterDataWorksheet.Activate();
    //                xlWorkbook.Save();
    //            }
    //            catch (System.Exception ex)
    //            {
    //                errormessage = ex.Message;
    //            }
                

    //            //Output - stream back to the browser
    //            try
    //            {                    
                    
    //                return File(exportFilepath, "application/vnd.ms-excel", exportFilename);

    //            }
    //            finally
    //            {
    //                xlWorkbook.Close();
    //                xlApp.Quit();
    //            }
                 


    //        }

    //        catch (System.Exception ex)
    //        {

    //            xlWorkbook.Close();
    //            xlApp.Quit();

    //        }           
           
    //    //return errormessage if it gets to here
    //    if (errormessage != "")
    //        {
    //            //json errormessage
    //        }
    //    return null;
            
    //}



    //    public ActionResult DoPuroPostExport(int idCARRDetail)
    //    {

    //        //
    //        //GET DATA FIRST
    //        //
    //        //DATA ELEMENTS NEEDED
    //        var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
    //        var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);
    //        var MyDefaultExchange = db.KeyValuePairs.Where(x => x.Key == "DefaultExchangeRate").FirstOrDefault();


    //        //Details used in Partial Info Bar            
    //        ViewBag.MyCarr = MyCarr;
    //        ViewBag.MyCarrDetail = MyCarrDetail;



    //        //Check for PuroPost Plus
    //        String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
    //        SqlConnection connection = new SqlConnection(connectionString);
    //        DataTable dtPPSTPlusCount = new DataTable();
    //        int PPSTPlusCount = 0;
    //        string sql = @"SELECT count(*) as ppstpluscount
    //                    FROM [PURO_APPS].[CAR].[CARRLocation] l
    //                    join CAR.Service s on l.idLocation=s.idLocation
    //                     join CAR.ShippingServices ss on s.idShippingService=ss.idShippingService
    //                     where AccessorialType='PPST'
    //                    and l.ActiveFlag = 1
    //                    and s.ActiveFlag = 1
    //                    and s.idShippingService in (7,12)
    //                    and l.idCARRDetail  = ";
    //        sql = sql + idCARRDetail;

    //        SqlDataAdapter daLoc = new SqlDataAdapter(sql, connection);
    //        daLoc.Fill(dtPPSTPlusCount);

    //        foreach (DataRow dgrow in dtPPSTPlusCount.Rows)
    //        {
    //            PPSTPlusCount = PPSTPlusCount + (int)dgrow["ppstpluscount"];;
    //        }
    //        string template = "";
    //        if (PPSTPlusCount > 0)
    //        {
    //             template = ConfigurationManager.AppSettings["USPuroPostPlusTemplate"].ToString();
    //        }
    //        else
    //        {
    //             template = ConfigurationManager.AppSettings["USPuroPostTemplate"].ToString();
    //        }


    //        //Data Needed in MASTERDATA sheet
    //        string customername = MyCarr.CustomerLegalName;
    //        string exchangerate = MyDefaultExchange.Value;
    //        string currencycode = MyCarrDetail.Currency;


    //       // string template = ConfigurationManager.AppSettings["USPuroPostTemplate"].ToString();
    //        string exportpath = ConfigurationManager.AppSettings["USPricingExport"].ToString();
    //        string exportFilename = "USPuroPost_" + MyCarr.CustomerLegalName + "_v" + MyCarrDetail.VersionNumber.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsb";
    //        string exportFilepath = exportpath + exportFilename;


    //        //
    //        //SECOND, CREATE NEW EXCEL FILE
    //        //
    //        //OPEN EXCEL TEMPLATE
    //        Excel.Application xlApp = new Excel.Application();
    //        xlApp.DisplayAlerts = false;
    //        Excel.Workbook templateWorkbook = xlApp.Workbooks.Open(template);

    //        //SAVE A NEW COPY TO WORK WITH
    //        templateWorkbook.SaveCopyAs(exportFilepath);
    //        //close and open new copy
    //        templateWorkbook.Close();
    //        Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(exportFilepath);


    //        //READ EXCEL TEMPLATE
    //        try
    //        {
    //            //Read the Master Data Sheet
    //            Excel._Worksheet masterDataWorksheet = xlWorkbook.Sheets["Input"];

    //            //Fill In Master Data Values
    //            var customerNamecell = masterDataWorksheet.Evaluate("CustomerName");
    //            customerNamecell.Value = customername;
    //            var exchangeRatecell = masterDataWorksheet.Evaluate("ExchangeRate");
    //            exchangeRatecell.Value = "=1/" + exchangerate;
    //            var currencyCodecell = masterDataWorksheet.Evaluate("CurrencyCode");
    //            currencyCodecell.Value = currencycode;


    //            //Save and Close
    //            //activate summary sheet
    //            Excel._Worksheet SummaryWorksheet = xlWorkbook.Sheets["Summary"];
    //            SummaryWorksheet.Activate();
    //            xlWorkbook.Save();


    //            //Output - stream back to the browser
    //            try
    //            {

    //                return File(exportFilepath, "application/vnd.ms-excel", exportFilename);

    //            }
    //            finally
    //            {
    //                xlWorkbook.Close();
    //                xlApp.Quit();
    //            }

    //        }

    //        catch (System.Exception ex)
    //        {

    //            xlWorkbook.Close();
    //            xlApp.Quit();

    //        }

    //        return null;
    //    }

       
               


        [HttpGet]
        public virtual ActionResult Download(string fileName)
        {
            string fullPath = ConfigurationManager.AppSettings["USPricingExport"].ToString();
            return File(fullPath, "application/vnd.ms-excel", fileName);
        }



    }


}