using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Portal.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using PI_Portal.Classes;


namespace PI_Portal.Controllers
{
    public class PricingController : Controller
    {
        private Entities db = new Entities();

        [HttpPost]
        [Authorize]
        public ActionResult Courier()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            ViewBag.idCARRDetail = idCARRDetail;

            ViewBag.Title = "Pricing - Courier";
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult LTL()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            ViewBag.idCARRDetail = idCARRDetail;

            ViewBag.Title = "Pricing - LTL";
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult CPC()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            ViewBag.idCARRDetail = idCARRDetail;

            ViewBag.Title = "Pricing - CPC";
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult PuroPost()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            ViewBag.idCARRDetail = idCARRDetail;

            ViewBag.Title = "Pricing - PuroPost";
            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult LineHaul()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            ViewBag.idCARRDetail = idCARRDetail;

            ViewBag.Title = "Pricing - Line Haul";
            return View();
        }

        //[HttpPost]
        //[Authorize]
        //public ActionResult USPricingExport()
        //{
        //    int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
        //    ViewBag.idCARRDetail = idCARRDetail;

        //    var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
        //    var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

        //    //Details used in Partial Info Bar            
        //    ViewBag.MyCarr = MyCarr;
        //    ViewBag.MyCarrDetail = MyCarrDetail;


        //    ViewBag.Title = "US Pricing Excel Export";

        //    return View();
        //}


        //[HttpPost]
        //[Authorize]
        //public ActionResult DoUSPricingExport()
        //{
        //    int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
        //    ViewBag.idCARRDetail = idCARRDetail;

        //    var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
        //    var MyCarr = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

        //    var MyDefaultExchange = db.KeyValuePairs.Where(x => x.Key == "DefaultExchangeRate").FirstOrDefault();

        //    //LHRFs
        //    String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
        //    SqlConnection connection = new SqlConnection(connectionString);
        //    try
        //    {
        //        connection.Open();           
        //        string sqlLHRF = @" select *
        //            FROM CAR.CARRLHRF
        //            where LHRFType != 'Returns' and ActiveFlag = 1 and idCARRDetail = ";
        //        sqlLHRF = sqlLHRF + idCARRDetail + " order by LHRFType";
        //        DataTable dtLHRF = new DataTable();
        //        SqlDataAdapter daLHRF = new SqlDataAdapter(sqlLHRF, connection);
        //        daLHRF.Fill(dtLHRF);
        //        //ViewBag.resultLHRF = dtLHRF;
        //    }
        //    finally
        //    {
        //        connection.Close();
        //        connection.Dispose();
        //    }

        //    //Details used in Partial Info Bar            
        //    ViewBag.MyCarr = MyCarr;
        //    ViewBag.MyCarrDetail = MyCarrDetail;
           

        //    ViewBag.Title = "US Pricing Excel Export";


        //    //READ EXCEL TEMPLATE
        //    try
        //    {
        //        string path = ConfigurationSettings.AppSettings["USPricingTemplate"].ToString();
               
        //        string data = "";
        //        //Create COM Objects. Create a COM object for everything that is referenced
        //        Excel.Application xlApp = new Excel.Application();
        //        Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(path);
        //        Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
        //        Excel.Range xlRange = xlWorksheet.UsedRange;

        //        int rowCount = xlRange.Rows.Count;
        //        int colCount = xlRange.Columns.Count;

        //        //iterate over the rows and columns and print to the console as it appears in the file
        //        //excel is not zero based!!
        //        for (int i = 1; i <= rowCount; i++)
        //        {
        //            for (int j = 1; j <= colCount; j++)
        //            {
        //                //either collect data cell by cell or DO you job like insert to DB 
        //                if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
        //                    data += xlRange.Cells[i, j].Value2.ToString();
        //            }
        //        }


        //    }
        //    catch (System.Exception e)
        //    {

        //    }















        //    return View();
        //}

        

        [HttpGet]
        [Authorize]
        public ActionResult DefaultMaintenance()
        {
            var MyDefaultExchange = db.KeyValuePairs.Where(x => x.Key == "DefaultExchangeRate").FirstOrDefault();
            ViewBag.DefaultExchange = MyDefaultExchange;
            var CPCExpeditedMinValue = db.KeyValuePairs.Where(x => x.Key == "CPCExpeditedMinValue").FirstOrDefault();
            ViewBag.CPCExpeditedMinValue = CPCExpeditedMinValue;
            var CPCExpeditedMaxValue = db.KeyValuePairs.Where(x => x.Key == "CPCExpeditedMaxValue").FirstOrDefault();
            ViewBag.CPCExpeditedMaxValue = CPCExpeditedMaxValue;
            var CPCXpressPostMinValue = db.KeyValuePairs.Where(x => x.Key == "CPCXpressPostMinValue").FirstOrDefault();
            ViewBag.CPCXpressPostMinValue = CPCXpressPostMinValue;
            var CPCXpressPostMaxValue = db.KeyValuePairs.Where(x => x.Key == "CPCXpressPostMaxValue").FirstOrDefault();
            ViewBag.CPCXpressPostMaxValue = CPCXpressPostMaxValue;
            var LTLDiscountMinValue = db.KeyValuePairs.Where(x => x.Key == "LTLDiscountMinValue").FirstOrDefault();
            ViewBag.LTLDiscountMinValue = LTLDiscountMinValue;
            var LTLDiscountMaxValue = db.KeyValuePairs.Where(x => x.Key == "LTLDiscountMaxValue").FirstOrDefault();
            ViewBag.LTLDiscountMaxValue = LTLDiscountMaxValue;

            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult Upload()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            string desc = Request.Form["Description"];
            string username = Session["accountname"].ToString();

            ViewBag.Title = "Pricing - Upload";

            string sql = @" select idFileUpload, FilePath, Description, CreatedBy, CreatedOn 
                        from CAR.CARRFileUploads 
                        where PricingOnlyFlag = 1 and idCARRDetail = ";
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
            
            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult RateUpload()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            string desc = Request.Form["Description"];
            string username = Session["accountname"].ToString();

            ViewBag.Title = "Pricing - Rate Upload";

            string sql = @" select idRateFile, FilePath, Description, CreatedBy, CreatedOn 
                        from CAR.CARRRateFile 
                        where ActiveFlag = 1 and idCARRDetail = ";
            sql = sql + idCARRDetail;
            sql = sql + " order by CreatedOn desc";
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

            string routedTo = db.Routings.Where(x => x.idRouting == MyCarrDetail.idRoutedTo).FirstOrDefault().RoutedTo.Trim();
            ViewBag.DisableChanges = true;
            if (routedTo.ToLower() == username.ToLower())
            {
                ViewBag.DisableChanges = false;
            }

            ViewBag.EstimatedRevenueWarning = false;
            var MySalesPricing = db.CARREstRevSalesPricings.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true && x.EstWklyRevPricing != null).ToList();
            if (MySalesPricing.Count() > 0)
            {
                ViewBag.DisableChanges = false;
            }
            else
            {
                ViewBag.DisableChanges = true;
                ViewBag.EstimatedRevenueWarning = true;
            }
            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ComparisonUpload()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            string desc = Request.Form["Description"];
            string username = Session["accountname"].ToString();

            ViewBag.Title = "Pricing - Rate Upload";

            string sql = @" select idUPSFile, FilePath, Description, CreatedBy, CreatedOn 
                        from CAR.UPSComparisonFile 
                        where ActiveFlag = 1 and idCARRDetail = ";
            sql = sql + idCARRDetail;
            sql = sql + " order by CreatedOn desc";
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

            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
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

        public ActionResult DoUSPricingExport(int idCARRDetail)
        {
            //{            

            //
            //GET DATA FIRST
            //
            //DATA ELEMENTS NEEDED
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);
            var MyDefaultExchange = db.KeyValuePairs.Where(x => x.Key == "DefaultExchangeRate").FirstOrDefault();

            if (MyCarrDetail.courierAccessorialFlag == null)
                MyCarrDetail.courierAccessorialFlag = false;
            if (MyCarrDetail.freightAccessorialFlag == null)
                MyCarrDetail.freightAccessorialFlag = false;
            if (MyCarrDetail.cpcAccessorialFlag == null)
                MyCarrDetail.cpcAccessorialFlag = false;
            if (MyCarrDetail.returnsFlag == null)
                MyCarrDetail.returnsFlag = false;

            //Set Service Flags            
            bool courierFlag = false;
            bool courierAccessorialFlag = (bool)MyCarrDetail.courierAccessorialFlag;
            bool freightAccessorialFlag = (bool)MyCarrDetail.freightAccessorialFlag;
            bool cpcAccessorialFlag = (bool)MyCarrDetail.cpcAccessorialFlag;
            bool cpcexpeditedFlag = false;
            bool cpcxpresspostFlag = false;
            bool ltlyyzFlag = false;
            bool ltlyvrFlag = false;
            bool ltlyulFlag = false;
            bool ltlywgFlag = false;
            bool returnsFlag = (bool)MyCarrDetail.returnsFlag;
            bool courieryyzFlag = false;
            bool courieryvrFlag = false;
            bool courieryulFlag = false;
            bool courierywgFlag = false;
            //bool ppstFlag = false;
            string errormessage = "";

            DataTable dtLHRFSingle = new DataTable();
            DataTable dtLHRFDual = new DataTable();
            DataTable dtLHRFForms = new DataTable();
            DataTable dtReturn = new DataTable();
            DataTable dtSvcs = new DataTable();
            DataTable dtVol = new DataTable();

            //LHRFs - Non GLHG
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                string sqlLHRF = @" select Concat(lh.PUAddress,' ',lh.PUCity,' ',lh.PUState,' ',lh.PUPostalCode) Origin,
                                Concat(lh.DestAddress,' ',lh.DestCity,' ',lh.DestState,' ',lh.DestPostalCode) Destination,
                                PUCity,DestCity,LHRFType
                    FROM CAR.CARRLHRF lh
                    where LHRFType != 'Returns' and LHRFType != 'GLHG' and ActiveFlag = 1 and idCARRDetail = ";
                sqlLHRF = sqlLHRF + idCARRDetail + " order by LHRFType";

                SqlDataAdapter daLHRF = new SqlDataAdapter(sqlLHRF, connection);
                daLHRF.Fill(dtLHRFForms);
            }
            finally
            {
                //connection.Close();
                //connection.Dispose();
            }

            //LHRF rates generated by Tool, and LHRF forms for GLHG - Single
            try
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                bool dualFlag = false;
                cmd = new SqlCommand("sp_LHRFRatesWithDual", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
                cmd.Parameters.Add(new SqlParameter("@dualFlag", dualFlag));
                da.SelectCommand = cmd;
                da.Fill(dtLHRFSingle);
            }
            finally
            {
                // connection.Close();
                //connection.Dispose();
            }

            //LHRF rates generated by Tool, and LHRF forms for GLHG - Dual
            try
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                bool dualFlag = true;
                cmd = new SqlCommand("sp_LHRFRatesWithDual", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
                cmd.Parameters.Add(new SqlParameter("@dualFlag", dualFlag));
                da.SelectCommand = cmd;
                da.Fill(dtLHRFDual);
            }
            finally
            {
                // connection.Close();
                //connection.Dispose();
            }


            //Returns           
            try
            {
                //connection.Open();
                //string sqlLHRF = @" select *
                //    FROM CAR.CARRLHRF
                //    where LHRFType = 'Returns' and ActiveFlag = 1 and idCARRDetail = ";
                //sqlLHRF = sqlLHRF + idCARRDetail;
                string sqlLHRF = @" select Concat(lh.PUAddress,' ',lh.PUCity,' ',lh.PUState,' ',lh.PUPostalCode) Origin,
                                Concat(lh.DestAddress,' ',lh.DestCity,' ',lh.DestState,' ',lh.DestPostalCode) Destination,
                                PUCity,DestCity,LHRFType
                    FROM CAR.CARRLHRF lh
                    where LHRFType = 'Returns' and ActiveFlag = 1 and idCARRDetail = ";
                sqlLHRF = sqlLHRF + idCARRDetail + " order by LHRFType";

                SqlDataAdapter daLHRF = new SqlDataAdapter(sqlLHRF, connection);
                daLHRF.Fill(dtReturn);
            }
            finally
            {
                //connection.Close();
                //connection.Dispose();
            }

            //Check Services For Courier, CPC and PPST Flags
            try
            {
                //connection.Open();
                string sqlSVCs = @" select ss.idShippingService,ss.Description,s.idService
                    FROM CAR.Service s
					join CAR.CARRLocation l on l.idLocation = s.idLocation
					join CAR.ShippingServices ss on ss.idShippingService = s.idShippingService
                    where s.ActiveFlag = 1 and l.ActiveFlag = 1 and l.idCARRDetail = ";
                sqlSVCs = sqlSVCs + idCARRDetail;

                SqlDataAdapter daSVC = new SqlDataAdapter(sqlSVCs, connection);
                daSVC.Fill(dtSvcs);
            }
            finally
            {
                //connection.Close();
                //connection.Dispose();
            }

            foreach (DataRow svcrow in dtSvcs.Rows)
            {
                switch (svcrow["idShippingService"])
                {
                    case 1:
                    case 2:
                        courierFlag = true;
                        //get CAVY flags
                        try
                        {
                            string sqlVol = @" select distinct ip.code FROM CAR.ServiceVolume sv
					        join CAR.InductionPoints ip on ip.idInduction = sv.idInduction where sv.idService=";
                            sqlVol = sqlVol + svcrow["idService"];
                            SqlDataAdapter daVol = new SqlDataAdapter(sqlVol, connection);
                            daVol.Fill(dtVol);
                            foreach (DataRow volrow in dtVol.Rows)
                            {
                                switch (volrow["code"].ToString().ToLower())
                                {
                                    case "yyz":
                                        courieryyzFlag = true;
                                        break;
                                    case "yul":
                                        courieryulFlag = true;
                                        break;
                                    case "yvr":
                                        courieryvrFlag = true;
                                        break;
                                    case "ywg":
                                        courierywgFlag = true;
                                        break;
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {

                        }
                        break;
                    case 3:
                        cpcexpeditedFlag = true;
                        break;
                    case 8:
                        cpcxpresspostFlag = true;
                        break;
                    //case 6:
                    //    ppstFlag = true;
                    //    break;
                    //case 7:
                    //    ppstFlag = true;
                    //    break;
                    //case 9:
                    //    ppstFlag = true;
                    //    break;
                    case 5:
                        //select Volume to get induction
                        try
                        {
                            string sqlVol = @" select distinct ip.code FROM CAR.ServiceVolume sv
					        join CAR.InductionPoints ip on ip.idInduction = sv.idInduction where  sv.idService=";
                            sqlVol = sqlVol + svcrow["idService"];
                            SqlDataAdapter daVol = new SqlDataAdapter(sqlVol, connection);
                            daVol.Fill(dtVol);
                            foreach (DataRow volrow in dtVol.Rows)
                            {
                                switch (volrow["code"].ToString().ToLower())
                                {
                                    case "yyz":
                                        ltlyyzFlag = true;
                                        break;
                                    case "yul":
                                        ltlyulFlag = true;
                                        break;
                                    case "yvr":
                                        ltlyvrFlag = true;
                                        break;
                                    case "ywg":
                                        ltlywgFlag = true;
                                        break;
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {

                        }
                        break;
                }

            }

            try
            {
                connection.Close();
                connection.Dispose();
            }
            catch (System.Exception ex)
            {
                errormessage = ex.Message;
            }


            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;
            ViewBag.Title = "US Pricing Excel Export";

            //Data Needed in MASTERDATA sheet
            string customername = MyCarr.CustomerLegalName;
            string contractnumber = MyCarrDetail.ContractNumber;
            string startdate = MyCarrDetail.ContractFromDate.ToString();
            string enddate = MyCarrDetail.ContractToDate.ToString();
            string exchangerate = MyDefaultExchange.Value;
            string currencycode = MyCarrDetail.Currency;


            string template = ConfigurationManager.AppSettings["USPricingTemplate"].ToString();
            string exportpath = ConfigurationManager.AppSettings["USPricingExport"].ToString();
            string uspricingpwd = ConfigurationManager.AppSettings["USPricingPWD"].ToString();
            string exportFilename = "USPricing_" + MyCarr.CustomerLegalName + "_v" + MyCarrDetail.VersionNumber.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            //replace special characters
            exportFilename = exportFilename.Replace("/", "");
            exportFilename = exportFilename.Replace("&", "");
            exportFilename = exportFilename.Replace("<", "");
            exportFilename = exportFilename.Replace(">", "");


            string exportFilepath = exportpath + exportFilename;


            //
            //SECOND, CREATE NEW EXCEL FILE
            //
            //OPEN EXCEL TEMPLATE
            Excel.Application xlApp = new Excel.Application();
            xlApp.DisplayAlerts = false;
            Excel.Workbook templateWorkbook = xlApp.Workbooks.Open(template);

            //SAVE A NEW COPY TO WORK WITH
            templateWorkbook.SaveCopyAs(exportFilepath);
            //close and open new copy
            templateWorkbook.Close();


            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(exportFilepath);


            //READ EXCEL TEMPLATE
            try
            {
                //Read the Master Data Sheet
                Excel._Worksheet masterDataWorksheet = xlWorkbook.Sheets["MasterData"];

                //Fill In Master Data Values
                var customerNamecell = masterDataWorksheet.Evaluate("CustomerName");
                customerNamecell.Value = customername;
                var startDatecell = masterDataWorksheet.Evaluate("StartDate");
                startDatecell.Value = startdate;
                var endDatecell = masterDataWorksheet.Evaluate("EndDate");
                endDatecell.Value = enddate;
                var exchangeRatecell = masterDataWorksheet.Evaluate("ExchangeRate");
                exchangeRatecell.Value = exchangerate;
                var currencyCodecell = masterDataWorksheet.Evaluate("CurrencyCode");
                currencyCodecell.Value = currencycode;

                //CAVY - Hide unused ranges

                try
                {
                    if (courierFlag == true)
                    {
                        //Read the CAVY Sheet
                        Excel._Worksheet cavyWorksheet = xlWorkbook.Sheets["Courier SAP CAVY"];
                        if (courieryyzFlag == false)
                        {
                            //hide yyz range
                            var courieryyzrange = cavyWorksheet.Evaluate("CAVYYYZ");
                            courieryyzrange.EntireRow.Hidden = true;
                        }
                        if (courieryvrFlag == false)
                        {
                            //hide yvr range
                            var courieryvrrange = cavyWorksheet.Evaluate("CAVYYVR");
                            courieryvrrange.EntireRow.Hidden = true;
                        }
                        if (courieryulFlag == false)
                        {
                            //hide yul range
                            var courieryulrange = cavyWorksheet.Evaluate("CAVYYUL");
                            courieryulrange.EntireRow.Hidden = true;
                        }
                        if (courierywgFlag == false)
                        {
                            //hide ywg range
                            var courierywgrange = cavyWorksheet.Evaluate("CAVYYWG");
                            courierywgrange.EntireRow.Hidden = true;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    errormessage = ex.Message;
                }


                //Add LH Unique Number sequence
                int lhcount = 0;

                //1. LH RATES FROM TOOL & LHRF forms for GLHG Single
                //Open Single LH Template
                Excel._Worksheet LHSingleTemplate = xlWorkbook.Sheets["LH Single Template"];
                var xlSheets = xlWorkbook.Sheets as Excel.Sheets;

                try
                {
                    foreach (DataRow lhrfRate in dtLHRFSingle.Rows)
                    {
                        lhcount = lhcount + 1;
                        string newsheetname = "LH Single_" + lhcount.ToString();

                        int sheetnum = xlWorkbook.Sheets.Count;
                        LHSingleTemplate.Copy(xlWorkbook.Sheets[sheetnum]);
                        xlWorkbook.Sheets[sheetnum].Name = newsheetname;

                        xlWorkbook.Save();

                        Excel._Worksheet LHSingleWorksheet = xlWorkbook.Sheets[newsheetname];

                        //Fill In CARR Data
                        var LHheadingcell = LHSingleWorksheet.Evaluate("LHSingleHeader");
                        LHheadingcell.Value = "Northbound Linehaul Pricing, Single Induction " + lhrfRate["LocationName"].ToString();
                        var Origincell = LHSingleWorksheet.Evaluate("LHSingleOrigin");
                        Origincell.Value = lhrfRate["Origin"].ToString();
                        var Destinationcell = LHSingleWorksheet.Evaluate("LHSingleDestination");
                        Destinationcell.Value = lhrfRate["Destination"].ToString();
                        var TransitDayscell = LHSingleWorksheet.Evaluate("LHSingleTransitDays");
                        TransitDayscell.Value = lhrfRate["TransitDays"].ToString();


                        //Rates
                        var LHmincell = LHSingleWorksheet.Evaluate("LHmin");
                        LHmincell.Value = lhrfRate["LHMin"].ToString();
                        var LHl5ccell = LHSingleWorksheet.Evaluate("LHl5c");
                        LHl5ccell.Value = lhrfRate["LH100"].ToString();
                        var LH500lbscell = LHSingleWorksheet.Evaluate("LH500lbs");
                        LH500lbscell.Value = lhrfRate["LH500"].ToString();
                        var LH1000lbscell = LHSingleWorksheet.Evaluate("LH1000lbs");
                        LH1000lbscell.Value = lhrfRate["LH1000"].ToString();
                        var LH2000lbscell = LHSingleWorksheet.Evaluate("LH2000lbs");
                        LH2000lbscell.Value = lhrfRate["LH2000"].ToString();
                        var LH5000lbscell = LHSingleWorksheet.Evaluate("LH5000lbs");
                        LH5000lbscell.Value = lhrfRate["LH5000"].ToString();
                        var LH10000lbscell = LHSingleWorksheet.Evaluate("LH10000lbs");
                        LH10000lbscell.Value = lhrfRate["LH10000"].ToString();

                        xlWorkbook.Save();

                    }
                }
                catch (System.Exception ex)
                {
                    errormessage = ex.Message;
                }

                //2. LH RATES FROM TOOL & LHRF forms for GLHG DUAL
                //Open Single LH Template
                Excel._Worksheet LHDualTemplate = xlWorkbook.Sheets["LH Dual Template"];
                //var xlSheets = xlWorkbook.Sheets as Excel.Sheets;

                try
                {
                    foreach (DataRow lhrfRate in dtLHRFDual.Rows)
                    {
                        lhcount = lhcount + 1;
                        string newsheetname = "LH Dual_" + lhcount.ToString();

                        int sheetnum = xlWorkbook.Sheets.Count;
                        LHDualTemplate.Copy(xlWorkbook.Sheets[sheetnum]);
                        xlWorkbook.Sheets[sheetnum].Name = newsheetname;

                        xlWorkbook.Save();

                        Excel._Worksheet LHDualWorksheet = xlWorkbook.Sheets[newsheetname];

                        //Fill In CARR Data - Location 1
                        var LHheadingcell = LHDualWorksheet.Evaluate("LHSingleHeader");
                        LHheadingcell.Value = "Northbound Linehaul Pricing, Single Induction, " + lhrfRate["LocationName"].ToString();

                        //ORIGIN DEST - Location 1
                        var DualOrigincell1 = LHDualWorksheet.Evaluate("LHDualOrigin1");
                        DualOrigincell1.Value = lhrfRate["Origin"].ToString();
                        var DualDestinationcell1 = LHDualWorksheet.Evaluate("LHDualDestination1");
                        DualDestinationcell1.Value = lhrfRate["Destination"].ToString();
                        var DualTransitDayscell1 = LHDualWorksheet.Evaluate("LHDualTransitDays1");
                        DualTransitDayscell1.Value = lhrfRate["TransitDays"].ToString();

                        //Rates - Location 1
                        var LHmincell1 = LHDualWorksheet.Evaluate("LHMin1");
                        LHmincell1.Value = lhrfRate["LHMin"].ToString();
                        var LHl5ccell1 = LHDualWorksheet.Evaluate("LHl5c1");
                        LHl5ccell1.Value = lhrfRate["LH100"].ToString();
                        var LH500lbscell1 = LHDualWorksheet.Evaluate("LH500lbs1");
                        LH500lbscell1.Value = lhrfRate["LH500"].ToString();
                        var LH1000lbscell1 = LHDualWorksheet.Evaluate("LH1000lbs1");
                        LH1000lbscell1.Value = lhrfRate["LH1000"].ToString();
                        var LH2000lbscell1 = LHDualWorksheet.Evaluate("LH2000lbs1");
                        LH2000lbscell1.Value = lhrfRate["LH2000"].ToString();
                        var LH5000lbscell1 = LHDualWorksheet.Evaluate("LH5000lbs1");
                        LH5000lbscell1.Value = lhrfRate["LH5000"].ToString();
                        var LH10000lbscell1 = LHDualWorksheet.Evaluate("LH10000lbs1");
                        LH10000lbscell1.Value = lhrfRate["LH10000"].ToString();

                        //ORIGIN DEST - Location 1                       
                        var DualOrigincell2 = LHDualWorksheet.Evaluate("LHDualOrigin2");
                        DualOrigincell2.Value = lhrfRate["DOrigin"].ToString();
                        var Destinationcell2 = LHDualWorksheet.Evaluate("LHDualDestination2");
                        Destinationcell2.Value = lhrfRate["DDestination"].ToString();
                        var TransitDayscell2 = LHDualWorksheet.Evaluate("LHDualTransitDays2");
                        TransitDayscell2.Value = lhrfRate["DTransitDays"].ToString();

                        //Rates - Location 2
                        var LHmincell2 = LHDualWorksheet.Evaluate("LHMin2");
                        LHmincell2.Value = lhrfRate["DMin"].ToString();
                        var LHl5ccell2 = LHDualWorksheet.Evaluate("LHl5c2");
                        LHl5ccell2.Value = lhrfRate["D100"].ToString();
                        var LH500lbscell2 = LHDualWorksheet.Evaluate("LH500lbs2");
                        LH500lbscell2.Value = lhrfRate["D500"].ToString();
                        var LH1000lbscell2 = LHDualWorksheet.Evaluate("LH1000lbs2");
                        LH1000lbscell2.Value = lhrfRate["D1000"].ToString();
                        var LH2000lbscell2 = LHDualWorksheet.Evaluate("LH2000lbs2");
                        LH2000lbscell2.Value = lhrfRate["D2000"].ToString();
                        var LH5000lbscell2 = LHDualWorksheet.Evaluate("LH5000lbs2");
                        LH5000lbscell2.Value = lhrfRate["D5000"].ToString();
                        var LH10000lbscell2 = LHDualWorksheet.Evaluate("LH10000lbs2");
                        LH10000lbscell2.Value = lhrfRate["D10000"].ToString();

                        xlWorkbook.Save();

                    }
                }
                catch (System.Exception ex)
                {
                    errormessage = ex.Message;
                }

                // 3. For each LHRF Form filled out
                // Do CASE on LHRFType: AIRCO, AIRD, LTLD, TL and select the right template
                foreach (DataRow lhrow in dtLHRFForms.Rows)
                {
                    try
                    {
                        string lhtype = lhrow["LHRFType"].ToString().Trim();
                        string templatename = "";
                        string sheetname = "";
                        string lhdesc = "";
                        string lhoriginname = "";
                        string lhdestinationname = "";
                        string lhheader = "";

                        lhcount = lhcount + 1;

                        switch (lhtype)
                        {
                            case "AIRCO":
                                templatename = "LH AIRCO Template";
                                sheetname = "LH AIRCO";
                                lhdesc = "Air Charter Pricing ";
                                lhheader = "LHAIRCOHeader";
                                lhoriginname = "LHAIRCOOrigin";
                                lhdestinationname = "LHAIRCODestination";
                                break;
                            case "AIRD":
                                templatename = "LH AIRD Template";
                                sheetname = "LH AIRD";
                                lhdesc = "Air Charter Pricing ";
                                lhheader = "LHAIRDHeader";
                                lhoriginname = "LHAIRDOrigin";
                                lhdestinationname = "LHAIRDDestination";
                                break;
                            case "GLHG":
                                //leave code here, but there should not be any GLHG
                                templatename = "LH Single Template";
                                sheetname = "LH Single";
                                lhdesc = "Northbound Linehaul Pricing, Single Induction ";
                                lhheader = "LHSingleHeader";
                                lhoriginname = "LHSingleOrigin";
                                lhdestinationname = "LHSingleDestination";
                                break;
                            case "LOC":
                                templatename = "LH Single Template";
                                sheetname = "LH Single";
                                lhdesc = "Customer Pickup";
                                lhheader = "LHSingleHeader";
                                lhoriginname = "LHSingleOrigin";
                                lhdestinationname = "LHSingleDestination";
                                break;
                            case "LTLD":
                                templatename = "LH Single Template";
                                sheetname = "LH Single";
                                lhdesc = "LTLD";
                                lhheader = "LHSingleHeader";
                                lhoriginname = "LHSingleOrigin";
                                lhdestinationname = "LHSingleDestination";
                                break;
                            case "TL":
                                templatename = "LH FTL Template";
                                sheetname = "LH FTL";
                                lhdesc = "Full Truck Load Pricing ";
                                lhheader = "LHFTLHeader";
                                lhoriginname = "LHFTLOrigin";
                                lhdestinationname = "LHFTLDestination";
                                break;

                        }
                        string newsheetname = sheetname + "_" + lhcount.ToString();
                        int sheetnum = xlWorkbook.Sheets.Count;
                        Excel._Worksheet LHTemplate = xlWorkbook.Sheets[templatename];

                        LHTemplate.Copy(xlWorkbook.Sheets[sheetnum]);
                        xlWorkbook.Sheets[sheetnum].Name = newsheetname;

                        xlWorkbook.Save();


                        Excel._Worksheet LHWorksheet = xlWorkbook.Sheets[newsheetname];

                        var lhheadercell = LHWorksheet.Evaluate(lhheader);
                        lhheadercell.Value = lhdesc + lhrow["PUCity"].ToString() + " to " + lhrow["DestCity"].ToString();
                        var lhorigincell = LHWorksheet.Evaluate(lhoriginname);
                        lhorigincell.Value = lhrow["Origin"].ToString();
                        var lhdestinationcell = LHWorksheet.Evaluate(lhdestinationname);
                        lhdestinationcell.Value = lhrow["Destination"].ToString();


                        xlWorkbook.Save();
                    }
                    catch (System.Exception ex)
                    {
                        errormessage = ex.Message;
                    }



                }

                // 4. For Returns -dtReturn
                if (returnsFlag == true)
                {
                    try
                    {

                        // 1. LH FORM
                        DataRow returnLHRF = dtReturn.Rows[0];
                        string origin = returnLHRF["Origin"].ToString();
                        string destination = returnLHRF["Destination"].ToString();

                        Excel._Worksheet LHReturnsTemplate = xlWorkbook.Sheets["LH Returns Template"];
                        string newsheetname = "LH_RETURNS";

                        int sheetnum = xlWorkbook.Sheets.Count;
                        LHReturnsTemplate.Copy(xlWorkbook.Sheets[sheetnum]);
                        xlWorkbook.Sheets[sheetnum].Name = newsheetname;

                        Excel._Worksheet ReturnsLHWorksheet = xlWorkbook.Sheets[newsheetname];
                        var returnsorigincell = ReturnsLHWorksheet.Evaluate("LHReturnsOrigin");
                        returnsorigincell.Value = origin;
                        var returnsdestinationcell = ReturnsLHWorksheet.Evaluate("LHReturnsDestination");
                        returnsdestinationcell.Value = destination;

                        // 2.CANADIAN RETURNS FORM
                        Excel._Worksheet CanadianReturnsTemplate = xlWorkbook.Sheets["Canadian Returns Template"];
                        string newsheetname2 = "Canadian Returns";

                        int sheetnum2 = xlWorkbook.Sheets.Count;
                        CanadianReturnsTemplate.Copy(xlWorkbook.Sheets[sheetnum2]);
                        xlWorkbook.Save();
                        xlWorkbook.Sheets[sheetnum2].Name = newsheetname2;

                        xlWorkbook.Save();
                    }
                    catch (System.Exception ex)
                    {
                        errormessage = ex.Message;
                    }


                }



                // WORKSHEETS
                // MasterData
                // Estimated Daily Revenue
                // Linehaul Single Origin Template                
                // Linehaul Dual Origin Template                
                // FTL Template                
                // Air Charter Template                
                // Commercial Air Template                
                // Pricing 
                // Courier SAP CAVY            
                // CPC Expedited 
                // CPC Xpresspost
                // LTL Charges-YYZ Data
                // LTL Charges-YVR Data
                // LTL Charges-YUL Data
                // LTL Charges-YWG Data
                // CDN LTL-YYZ
                // CDN LTL-YVR 
                // CDN LTL-YUL 
                // CDN LTL-YWG 
                // Courier Worksheet
                // Courier Accessorials
                // Freight Accessorials
                // CPC Accessorials

                //More sheets are added as the templates are copied


                //Hide Unneeded Worksheets

                //COURIER
                try
                {
                    if (courierFlag == false)
                    {
                        xlWorkbook.Sheets["Courier Worksheet"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    }
                    if (courierAccessorialFlag == false)
                    {
                        xlWorkbook.Sheets["Courier Accessorials"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    }
                    if (freightAccessorialFlag == false)
                    {
                        xlWorkbook.Sheets["Freight Accessorials"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    }
                    if (cpcAccessorialFlag == false)
                    {
                        xlWorkbook.Sheets["CPC Accessorials"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    }
                    //LTL YVR
                    if (ltlyvrFlag == false)
                    {
                        xlWorkbook.Sheets["CDN LTL-YVR"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    }
                    //LTL YYZ
                    if (ltlyyzFlag == false)
                    {
                        xlWorkbook.Sheets["CDN LTL-YYZ"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    }
                    //LTL YVR
                    if (ltlyulFlag == false)
                    {
                        xlWorkbook.Sheets["CDN LTL-YUL"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    }
                    //LTL YYZ
                    if (ltlywgFlag == false)
                    {
                        xlWorkbook.Sheets["CDN LTL-YWG"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    }
                    //LTL Pricing Sheets
                    xlWorkbook.Sheets["LTL Charges-YYZ Data"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    xlWorkbook.Sheets["LTL Charges-YVR Data"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    xlWorkbook.Sheets["LTL Charges-YUL Data"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    xlWorkbook.Sheets["LTL Charges-YWG Data"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    //CPC XpressPost
                    if (cpcxpresspostFlag == false)
                    {
                        xlWorkbook.Sheets["CPC Xpresspost"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    }
                    //CPC Expedited
                    if (cpcexpeditedFlag == false)
                    {
                        xlWorkbook.Sheets["CPC Expedited"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    }
                    //COURIER
                    if (courierFlag == false)
                    {
                        xlWorkbook.Sheets["Courier SAP CAVY"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    }
                    //else
                    //{
                    //    xlWorkbook.Sheets["Courier SAP CAVY"].Protect(uspricingpwd, true);
                    //}

                    //Pricing Data
                    xlWorkbook.Sheets["Pricing"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    //Templates  
                    xlWorkbook.Sheets["LH Single Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    xlWorkbook.Sheets["LH Dual Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    xlWorkbook.Sheets["LH FTL Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    xlWorkbook.Sheets["LH AIRD Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    xlWorkbook.Sheets["LH AIRCO Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    xlWorkbook.Sheets["LH Returns Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    xlWorkbook.Sheets["Canadian Returns Template"].Visible = Excel.XlSheetVisibility.xlSheetHidden;

                    //For Intr-Canada, no need to show the CAVY Sheet
                    if (courieryyzFlag == false && courieryvrFlag == false && courieryulFlag == false && courierywgFlag)
                    {
                        xlWorkbook.Sheets["Courier SAP CAVY"].Visible = Excel.XlSheetVisibility.xlSheetHidden;
                    }

                    //password protect all sheets
                    foreach (Excel.Worksheet sheet in xlWorkbook.Worksheets)
                    {
                        sheet.Protect(uspricingpwd, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false);
                    }

                    //Save and Close
                    //activate first sheet, masterdata
                    masterDataWorksheet.Activate();
                    xlWorkbook.Save();
                }
                catch (System.Exception ex)
                {
                    errormessage = ex.Message;
                }


                //Output - stream back to the browser
                try
                {

                    return File(exportFilepath, "application/vnd.ms-excel", exportFilename);

                }
                finally
                {
                    xlWorkbook.Close();
                    xlApp.Quit();
                }



            }

            catch (System.Exception ex)
            {

                xlWorkbook.Close();
                xlApp.Quit();

            }

            //return errormessage if it gets to here
            if (errormessage != "")
            {
                //json errormessage
            }
            return null;

        }



        public ActionResult DoPuroPostExport(int idCARRDetail)
        {

            //
            //GET DATA FIRST
            //
            //DATA ELEMENTS NEEDED
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
            var MyCarr = db.CARRs.Find(MyCarrDetail.idCARR);
            var MyDefaultExchange = db.KeyValuePairs.Where(x => x.Key == "DefaultExchangeRate").FirstOrDefault();


            //Details used in Partial Info Bar            
            ViewBag.MyCarr = MyCarr;
            ViewBag.MyCarrDetail = MyCarrDetail;



            //Check for PuroPost Plus
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            DataTable dtPPSTPlusCount = new DataTable();
            int PPSTPlusCount = 0;
            string sql = @"SELECT count(*) as ppstpluscount
                        FROM [PURO_APPS].[CAR].[CARRLocation] l
                        join CAR.Service s on l.idLocation=s.idLocation
                         join CAR.ShippingServices ss on s.idShippingService=ss.idShippingService
                         where AccessorialType='PPST'
                        and l.ActiveFlag = 1
                        and s.ActiveFlag = 1
                        and s.idShippingService in (7,12)
                        and l.idCARRDetail  = ";
            sql = sql + idCARRDetail;

            SqlDataAdapter daLoc = new SqlDataAdapter(sql, connection);
            daLoc.Fill(dtPPSTPlusCount);

            foreach (DataRow dgrow in dtPPSTPlusCount.Rows)
            {
                PPSTPlusCount = PPSTPlusCount + (int)dgrow["ppstpluscount"]; ;
            }
            string template = "";
            if (PPSTPlusCount > 0)
            {
                template = ConfigurationManager.AppSettings["USPuroPostPlusTemplate"].ToString();
            }
            else
            {
                template = ConfigurationManager.AppSettings["USPuroPostTemplate"].ToString();
            }


            //Data Needed in MASTERDATA sheet
            string customername = MyCarr.CustomerLegalName;
            string exchangerate = MyDefaultExchange.Value;
            string currencycode = MyCarrDetail.Currency;


            // string template = ConfigurationManager.AppSettings["USPuroPostTemplate"].ToString();
            string exportpath = ConfigurationManager.AppSettings["USPricingExport"].ToString();
            string exportFilename = "USPuroPost_" + MyCarr.CustomerLegalName + "_v" + MyCarrDetail.VersionNumber.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsb";
            string exportFilepath = exportpath + exportFilename;


            //
            //SECOND, CREATE NEW EXCEL FILE
            //
            //OPEN EXCEL TEMPLATE
            Excel.Application xlApp = new Excel.Application();
            xlApp.DisplayAlerts = false;
            Excel.Workbook templateWorkbook = xlApp.Workbooks.Open(template);

            //SAVE A NEW COPY TO WORK WITH
            templateWorkbook.SaveCopyAs(exportFilepath);
            //close and open new copy
            templateWorkbook.Close();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(exportFilepath);


            //READ EXCEL TEMPLATE
            try
            {
                //Read the Master Data Sheet
                Excel._Worksheet masterDataWorksheet = xlWorkbook.Sheets["Input"];

                //Fill In Master Data Values
                var customerNamecell = masterDataWorksheet.Evaluate("CustomerName");
                customerNamecell.Value = customername;
                var exchangeRatecell = masterDataWorksheet.Evaluate("ExchangeRate");
                exchangeRatecell.Value = "=1/" + exchangerate;
                var currencyCodecell = masterDataWorksheet.Evaluate("CurrencyCode");
                currencyCodecell.Value = currencycode;


                //Save and Close
                //activate summary sheet
                Excel._Worksheet SummaryWorksheet = xlWorkbook.Sheets["Summary"];
                SummaryWorksheet.Activate();
                xlWorkbook.Save();


                //Output - stream back to the browser
                try
                {

                    return File(exportFilepath, "application/vnd.ms-excel", exportFilename);

                }
                finally
                {
                    xlWorkbook.Close();
                    xlApp.Quit();
                }

            }

            catch (System.Exception ex)
            {

                xlWorkbook.Close();
                xlApp.Quit();

            }

            return null;
        }

    }
}