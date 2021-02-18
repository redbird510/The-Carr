using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Portal.Models;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.DirectoryServices;

namespace PI_Portal.Controllers
{
    public class DataController : Controller
    {
        private Entities db = new Entities();

        [HttpPost]
        [Authorize]
        public JsonResult Save()
        {

            //TODO - if Session["accountname"] is null, redirect to login page;
            //if (Session["accountname"].ToString() == null)
            //{
            //    //return Redirect("/User/Login");
            //}
            string id = Request.Form["id"];            
            string table = Request.Form["table"];
            string field = Request.Form["field"];
            string idfield = Request.Form["idfield"];
            string value = Request.Form["value"];
            string username = Session["accountname"].ToString();
            //TODO - if username is null, redirect to login page
            int rowsAffected = 0;

            //handle single quotes which cause an issue replace single quote with double single quote
            if  (value != null)
            {
                value = value.Replace("'", "''");
            }
           

            if (id != null && table != null && field != null && value != null && idfield != null)
            {
                String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
                DataTable dt = new DataTable();
                SqlConnection conn = new SqlConnection(cs);
                value = value.Trim();
                string sql = "";
                if (username != null)
                {
                    if (field.Equals("DGFlag") && value.Equals("") || value.Equals(""))
                        sql = "UPDATE " + table + " SET " + field + " = null, UpdatedBy = '" + username + "' WHERE " + " " + idfield + " = " + id;     
                    else
                        sql = "UPDATE " + table + " SET " + field + " = '" + value + "', UpdatedBy = '" + username + "' WHERE " + " " + idfield + " = " + id;
                }
                else
                {
                    sql = "UPDATE " + table + " SET " + field + " = '" + value + "' WHERE " + " " + idfield + " = " + id;
                }
                   
                SqlCommand cmd = new SqlCommand(sql, conn);          
               

                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
            }
            JsonResult json = new JsonResult();
            return Json(rowsAffected, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveInt()
        {
            string id = Request.Form["id"];
            string table = Request.Form["table"];
            string field = Request.Form["field"];
            string idfield = Request.Form["idfield"];
            string value = Request.Form["value"];
            string username = Session["accountname"].ToString();
            int rowsAffected = 0;

            if (id != null && table != null && field != null && value != null && idfield != null)
            {
                String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
                DataTable dt = new DataTable();
                SqlConnection conn = new SqlConnection(cs);

                string sql = "";

                if (value == "")
                    value = "0";
                if (value == "true")
                    value = "1";
                if (value == "false")
                    value = "0";

                if (username != null)
                {
                    sql = "UPDATE " + table + " SET " + field + " = " + value + ", UpdatedBy = '" + username + "' WHERE " + " " + idfield + " = " + id;
                }
                else
                {
                    sql = "UPDATE " + table + " SET " + field + " = " + value + " WHERE " + " " + idfield + " = " + id;
                }


                SqlCommand cmd = new SqlCommand(sql, conn);

                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
            }




            JsonResult json = new JsonResult();
            return Json(rowsAffected, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Dropdown()
        {
            try
            {
                string id = Request.Form["id"];
                String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;

                DataTable dt = new DataTable();
                SqlConnection conn = new SqlConnection(cs);
                
                //string sql = "SELECT * FROM [PURO_APPS].[CAR].[DropdownsOptions] where id = " + id;

                string sql = "SELECT * FROM [PURO_APPS].[CAR].[DropdownsOptions] where id = " + id + " and isActive = 1 Order By SortOrder";


                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.Fill(dt);

                conn.Close();
                conn.Dispose();
                JsonResult json = new JsonResult();
                List<Dictionary<string, object>> lstPersons = GetTableRows(dt);
                return Json(lstPersons, JsonRequestBehavior.AllowGet);
                
            }
            catch 
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        //TODO: Can delete this, at some point, used it so i can convert list to json
        public List<Dictionary<string, object>> GetTableRows(DataTable dtData)
        {
            List<Dictionary<string, object>>
            lstRows = new List<Dictionary<string, object>>();
            Dictionary<string, object> dictRow = null;

            foreach (DataRow dr in dtData.Rows)
            {
                dictRow = new Dictionary<string, object>();
                foreach (DataColumn col in dtData.Columns)
                {
                    dictRow.Add(col.ColumnName, dr[col]);
                }
                lstRows.Add(dictRow);
            }
            return lstRows;
        }

        // update / insert / delete
        //conn.Open()
        //string sql = "select ...";
        //SqlCommand cmd = new SqlCommand(sql, con);
        //cmd.Parameters.AddWithValue("dateFrom", DateTime.Now);
        //cmd.Parameters.AddWithValue("dateFrom", DateTime.Now);
        //cmd.ExecuteReader();
        //return redirecttoaction("index")
        [HttpPost]
        [Authorize]
        public JsonResult SaveCorporateContact()
        {
            string id = Request.Form["idContact"];
            string firstname = Request.Form["hdnfirstname"];
            string lastname = Request.Form["hdnlastname"];
            string title = Request.Form["hdntitle"];
            string email = Request.Form["hdnemail"];
            string phonenumber = Request.Form["hdnphonenumber"];
            string address1 = Request.Form["hdnaddress1"];
            string address2 = Request.Form["hdnaddress2"];
            string postalcode = Request.Form["hdnpostalcode"];
            string city = Request.Form["hdncity"];
            string state = Request.Form["hdnstate"];
            string country = Request.Form["hdncountry"];
            string username = Session["accountname"].ToString();
            int rowsAffected = 0;

            if (id != null)
            {
                String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
                DataTable dt = new DataTable();
                SqlConnection conn = new SqlConnection(cs);
                string sql = "";

                sql = "UPDATE CAR.Contacts SET ";

                sql = sql + "FirstName = '" + firstname.Trim() + "'";
                sql = sql + ", LastName = '" + lastname.Trim() + "'";
                sql = sql + ", Title = '" + title.Trim() + "'";
                sql = sql + ", Email = '" + email.Trim() + "'";
                sql = sql + ", PhoneNumber = '" + phonenumber.Trim() + "'";
                sql = sql + ", Address1 = '" + address1.Trim() + "'";
                sql = sql + ", Address2 = '" + address2.Trim() + "'";
                sql = sql + ", PostalCode = '" + postalcode.Trim() + "'";
                sql = sql + ", City = '" + city.Trim() + "'";
                sql = sql + ", State = '" + state.Trim() + "'";
                sql = sql + ", Country = '" + country.Trim() + "'";


                if (username != null)
                {                    
                    sql = sql + ", UpdatedBy = '" + username.Trim() + "'";                    
                }
               
                sql = sql + " WHERE idContact = " + id;


                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();

                conn.Close();
                conn.Dispose();
            }


            return Json(rowsAffected, JsonRequestBehavior.AllowGet);
           

        }

        [HttpPost]
        [Authorize]
        public JsonResult GetInductionAddress()
        {
            string id = Request.Form["id"];
            try
            {

                if (id != null)
                {
                    String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
                    DataTable dt = new DataTable();
                    SqlConnection conn = new SqlConnection(cs);
                    string sql = "";

                    sql = "Select * from CAR.InductionPoints where idInduction = " + id;

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    da.Fill(dt);

                    conn.Close();
                    conn.Dispose();
                    JsonResult json = new JsonResult();
                    List<Dictionary<string, object>> address = GetTableRows(dt);
                    Dictionary<string, object> firstrow = address[0];
                    
                    return Json(firstrow, JsonRequestBehavior.AllowGet);
                    


                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }
        
        [HttpPost]
        [Authorize]
        public JsonResult GetLinehaulRates()
        {
            string zip = Request.Form["zip"].Trim();
            string induction = Request.Form["induction"].Trim();
            string custdropoffFlag = Request.Form["custdropoffFlag"].Trim();
            int idLocation = Convert.ToInt32(Request.Form["idLocation"].Trim());
            string branch = Request.Form["branch"].Trim();
            string via = Request.Form["via"].Trim();
            string avgSkids = Request.Form["avgskids"].Trim();           
            if (avgSkids == "")
                avgSkids = "0";
            string transitDays = Request.Form["transitdays"].Trim();
            if (transitDays == "")
                transitDays = "0";

            String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);

            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                cmd = new SqlCommand("sp_GetLHRates", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@zip", zip));
                cmd.Parameters.Add(new SqlParameter("@induction", induction));
                cmd.Parameters.Add(new SqlParameter("@custdropoffFlag", custdropoffFlag));
                cmd.Parameters.Add(new SqlParameter("@avgSkids", avgSkids));
                da.SelectCommand = cmd;
                da.Fill(dt);
                JsonResult json = new JsonResult();
                List<Dictionary<string, object>> row = GetTableRows(dt);
                if (row.Count > 0)
                {
                    Dictionary<string, object> firstrow = row[0];

                    Entities db = new Entities();
                    var existingRow = db.CARRLHRates.Where(x => x.idLocation == idLocation && x.ZipCode == zip && x.Induction == induction && x.Branch == branch && x.Via == via && x.ActiveFlag == true).FirstOrDefault();

                    string username = Session["accountname"].ToString();
                    if (existingRow == null)
                    {
                        CARRLHRate rate = new CARRLHRate();
                        rate.idLocation = idLocation;
                        rate.ZipCode = zip;
                        rate.Branch = branch;
                        rate.Via = via;
                        rate.Induction = induction;
                        rate.Min = Convert.ToDouble(firstrow["Min"]);
                        rate.C100 = Convert.ToDouble(firstrow["100"]);
                        rate.C500 = Convert.ToDouble(firstrow["500"]);
                        rate.C1000 = Convert.ToDouble(firstrow["1,000"]);
                        rate.C2000 = Convert.ToDouble(firstrow["2,000"]);
                        rate.C5000 = Convert.ToDouble(firstrow["5,000"]);
                        rate.C10000 = Convert.ToDouble(firstrow["10,000"]);
                        rate.CustDropOffFlag = Convert.ToBoolean(Convert.ToInt32(custdropoffFlag));
                        //if (branch.Equals("BUF"))
                        //{
                        rate.AvgSkids = Convert.ToInt32(firstrow["AvgSkids"]);
                        rate.SkidRatePer = Convert.ToDouble(firstrow["SkidRatePer"]);
                            rate.SkidRateTotal = Convert.ToDouble(firstrow["SkidRateTotal"]);
                        //}
                        rate.TransitDays = Convert.ToInt32(transitDays);
                        rate.ActiveFlag = true;
                        rate.UpdatedBy = username;
                        rate.UpdatedOn = DateTime.Now;
                        db.CARRLHRates.Add(rate);
                        db.SaveChanges();
                    }
                    else
                    {
                        existingRow.Min = Convert.ToDouble(firstrow["Min"]);
                        existingRow.C100 = Convert.ToDouble(firstrow["100"]);
                        existingRow.C500 = Convert.ToDouble(firstrow["500"]);
                        existingRow.C1000 = Convert.ToDouble(firstrow["1,000"]);
                        existingRow.C2000 = Convert.ToDouble(firstrow["2,000"]);
                        existingRow.C5000 = Convert.ToDouble(firstrow["5,000"]);
                        existingRow.C10000 = Convert.ToDouble(firstrow["10,000"]);
                        existingRow.CustDropOffFlag = Convert.ToBoolean(Convert.ToInt32(custdropoffFlag));
                        //if (branch.Equals("BUF"))
                        //{
                        existingRow.AvgSkids = Convert.ToInt32(avgSkids);
                        //existingRow.AvgSkids = Convert.ToInt32(firstrow"AvgSkids");
                        existingRow.SkidRatePer = Convert.ToDouble(firstrow["SkidRatePer"]);
                        existingRow.SkidRateTotal = Convert.ToDouble(firstrow["SkidRateTotal"]);
                        //}
                        existingRow.TransitDays = Convert.ToInt32(transitDays);
                        existingRow.ActiveFlag = true;
                        existingRow.UpdatedBy = username;
                        existingRow.UpdatedOn = DateTime.Now;
                        db.SaveChanges();
                    }

                    return getLinehaulRatesByLocID(idLocation);
                }
                else
                {
                    return Json(new EmptyResult(), JsonRequestBehavior.AllowGet);
                }

            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult getLinehaulRatesByLocID(int idLocation)
        {
            String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            try
            {
                var sql = "Select * from CAR.CARRLHRates where idLocation = " + idLocation + " and ActiveFlag = 1";


                conn.Open();
                SqlDataAdapter da2 = new SqlDataAdapter(sql, conn);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);

                conn.Close();
                conn.Dispose();
                JsonResult json2 = new JsonResult();
                List<Dictionary<string, object>> lstPersons = GetTableRows(dt2);

                return Json(lstPersons, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [Authorize]
        public DataTable GetIndCodesByLoc(int idLocation)
        {
            Entities db = new Entities();
            String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);

            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            try
            {
                cmd = new SqlCommand("sp_GetIndCodesByLoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idLocation", idLocation));
                da.SelectCommand = cmd;
                da.Fill(dt);
                dt.Columns.Add("CustDropOffFlag", typeof(String));
                dt.Columns.Add("AvgSkids", typeof(String));
                dt.Columns.Add("TransitDays", typeof(String));

                foreach (DataRow row in dt.Rows)
                {
                    string rowBranch = row.Field<string>("Branch").Trim();
                    string rowZip = row.Field<string>("ZipCode").Trim();
                    string rowVia = row.Field<string>("Via").Trim();
                    string rowInd = row.Field<string>("Airport Code").Trim();

                    var rate = db.CARRLHRates.Where(x => x.idLocation == idLocation && x.ZipCode == rowZip && x.Branch == rowBranch
                               && x.Via == rowVia && x.Induction == rowInd && x.ActiveFlag == true).FirstOrDefault();

                    row["CustDropOffFlag"] = rate != null ? (rate.CustDropOffFlag != null ? rate.CustDropOffFlag.ToString() : "") : "";
                    row["AvgSkids"] = rate != null ? (rate.AvgSkids != null ? rate.AvgSkids.ToString() : "") : "";
                    row["TransitDays"] = "0";

                }

                return dt;
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return null;
        }

        [HttpPost]
        [Authorize]
        public JsonResult processSubmit(int idCARRDetail, string routeTo, string emailMsg)
        {
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();

            //Remove spaces from routeTo name
            routeTo = routeTo.Replace(" ","");
            
            string username = Session["accountname"].ToString();
            
            //Enter Routing Row
            if (routeTo != "" && routeTo != "0")
            {
                Routing routedto = new Routing();
                routedto.RoutedTo = routeTo;
                routedto.CreatedBy = username;
                routedto.CreatedOn = DateTime.Now;
                routedto.DateRoutedTo = DateTime.Now;
                routedto.idCARRDetail = idCARRDetail;
                routedto.RoutingMsg = emailMsg;
                db.Routings.Add(routedto);
                MyCarrDetail.idRoutedTo = routedto.idRouting;
                db.SaveChanges();
            }



         
            try
            {
                //send email
                bool pricingflag = false;
                sendRoutingEmail(idCARRDetail, routeTo, MyCarrDetail, pricingflag, "", emailMsg);
            }
            catch (System.Exception ex)
            {
                recordError(1, ex.Message, "");
            }
            

            //var MyApprovals = db.Approvals.Where(x => x.idCARRDetail == idCARRDetail);
            //return Json(MyApprovals, JsonRequestBehavior.AllowGet);
            return Json(new
            {
                redirectUrl = Url.Action("Index","Home"), isRedirect = true
            });
        }

        [HttpPost]
        [Authorize]
        public JsonResult processApprove(int idCARRDetail, string routeTo, bool approveflag, string EmailMsg)
        {
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            //var MyCARR = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

            //Remove spaces from routeTo name
            routeTo = routeTo.Replace(" ", "");

            string username = Session["accountname"].ToString();

            if (Session["userrole"].ToString() == "DecisionSupport")
            {
                MyCarrDetail.DFLApproved = true;
            }

            //Enter Routing Row
            if (routeTo != "" && routeTo != "0")
            {
                Routing routedto = new Routing();
                routedto.RoutedTo = routeTo;
                routedto.CreatedBy = username;
                routedto.CreatedOn = DateTime.Now;
                routedto.DateRoutedTo = DateTime.Now;
                routedto.idCARRDetail = idCARRDetail;
                routedto.RoutingMsg = EmailMsg;
                db.Routings.Add(routedto);
                MyCarrDetail.idRoutedTo = routedto.idRouting;
                db.SaveChanges();
            }

            //approval row
            if (approveflag == true)
            {
                Approval approvalrow = new Approval();
                approvalrow.ApprovedBy = username;
                approvalrow.DateApproved = DateTime.Now;
                approvalrow.CreatedBy = username;
                approvalrow.CreatedOn = DateTime.Now;
                approvalrow.idCARRDetail = idCARRDetail;
                db.Approvals.Add(approvalrow);
                db.SaveChanges();
            }

          
            try
            {
                //send email
                bool pricingflag = false;
                string cc = "";
                sendRoutingEmail(idCARRDetail, routeTo, MyCarrDetail, pricingflag, cc, EmailMsg);
            }
            catch (System.Exception ex)
            {
                recordError(1, ex.Message, "");
            }
           

            //var MyApprovals = db.Approvals.Where(x => x.idCARRDetail == idCARRDetail);
            return Json(new
            {
                redirectUrl = Url.Action("Index", "Home"),
                isRedirect = true
            });
        }

        [HttpPost]
        [Authorize]
        public void processPricing(int idCARRDetail, bool routeflag)
        {
            string username = Session["accountname"].ToString();
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            


            MyCarrDetail.PricingReqCompleteFlag = true;
            MyCarrDetail.PricingReqCompleteDate = DateTime.Now;
            MyCarrDetail.PricingCompletedBy = username;
            MyCarrDetail.UpdatedBy = username;
            MyCarrDetail.UpdatedOn = DateTime.Now;
            db.SaveChanges();

            if (routeflag == true)
            {
                //Route Back to SalesPerson
                Routing routedto = new Routing();
                routedto.RoutedTo = MyCarrDetail.SalesProfessional;
                routedto.CreatedBy = username;
                routedto.CreatedOn = DateTime.Now;
                routedto.DateRoutedTo = DateTime.Now;
                routedto.idCARRDetail = idCARRDetail;
                routedto.RoutingMsg = "Rates Provided by Pricing";
                db.Routings.Add(routedto);
                MyCarrDetail.idRoutedTo = routedto.idRouting;
                db.SaveChanges();
            }
           

           
            try
            {
                //send email to Sales either way
                bool pricingflag = true;
                string EmailMsg = "";
                sendRoutingEmail(idCARRDetail, MyCarrDetail.SalesProfessional, MyCarrDetail, pricingflag, MyCarrDetail.SalesManager, EmailMsg);

            }
            catch (System.Exception ex)
            {
                recordError(1, ex.Message, "");
            }

        }

        

        [HttpPost]
        [Authorize]
        public bool processComplete(int idCARRDetail, string username)
        {
            var MyCarrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail && x.ActiveFlag == true).FirstOrDefault();
            var MyCARR = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();


            MyCarrDetail.UseForContractFlag = true;
            MyCarrDetail.UpdatedBy = username;
            MyCarrDetail.UpdatedOn = DateTime.Now;
           
            MyCARR.UpdatedBy = username;
            MyCARR.UpdatedOn = DateTime.Now;
            db.SaveChanges();

            //Set CompleteFlag for all versions, so they will move off of Active CARR list but still be viewed in All CARRs
            //IList<CARRDetail> MyCarrDetailList = db.CARRDetails.Where(x => x.idCARR == MyCarrDetail.idCARR && x.ActiveFlag == true).ToList();
            //MK - only mark the one version as complete
            IList<CARRDetail> MyCarrDetailList = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).ToList();
            foreach (CARRDetail detail in MyCarrDetailList)
            {
                detail.CompletedFlag = true;
                detail.CompletedDate = DateTime.Now;
            }            

            db.SaveChanges();

             return true;
        }

        public void sendRoutingEmail(int idCARRDetail,string routeTo,CARRDetail MyCarrDetail,bool pricingFlag,string ccTo,string emailMsg)
        {
            routeTo = routeTo.Trim();

            String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            //Get userrole of route to, if contracts or operations, cc the group email box
           
            //these are default values
            string toEmailAddress = routeTo + "@purolator.com";
            string toEmailAddress2 = MyCarrDetail.SalesProfessional + "@purolator.com";
            string ccEmailAddress = ccTo + "@purolator.com";
            //String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            string ApplicationURL = System.Configuration.ConfigurationManager.AppSettings["AppURLforEmail"].ToString();
            //SqlConnection conn = new SqlConnection(cs);
            //conn.Open();

            try
            {
                //SqlCommand cmd = new SqlCommand();
                //cmd = new SqlCommand("sp_GetEmailAddress", conn);
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.Add(new SqlParameter("@username", routeTo));
                //toEmailAddress = (string)cmd.ExecuteScalar();
                toEmailAddress = getEmailAddress(routeTo);
            }
            catch (System.Exception ex)
            {
                recordError(1, ex.Message, "");
            }
           

            //Get Sales Email Address and copy them
            try
            {
                //SqlCommand cmd2 = new SqlCommand();
                //cmd2 = new SqlCommand("sp_GetEmailAddress", conn);
                //cmd2.CommandType = CommandType.StoredProcedure;
                //cmd2.Parameters.Add(new SqlParameter("@username", MyCarrDetail.SalesProfessional));
                //toEmailAddress2 = (string)cmd2.ExecuteScalar();
                toEmailAddress2 = getEmailAddress(MyCarrDetail.SalesProfessional);
            }
            catch (System.Exception ex)
            {
                recordError(1, ex.Message, "");
            }
          

            //Optional CC which is used to CC the Sales Manager
            try
            {
                if (ccTo != "")
                {
                    //SqlCommand cmd = new SqlCommand();
                    //cmd = new SqlCommand("sp_GetEmailAddress", conn);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Add(new SqlParameter("@username", ccTo));
                    //ccEmailAddress = (string)cmd.ExecuteScalar();
                    ccEmailAddress = getEmailAddress(ccTo);
                }
               
            }
            catch (System.Exception ex)
            {
                recordError(1, ex.Message, "");
            }
            

            if (toEmailAddress2 != null && (toEmailAddress != toEmailAddress2))
            {
                toEmailAddress = toEmailAddress + ", " + toEmailAddress2;
            }
               
            if (ccTo != "" && ccEmailAddress != null)
            {
                toEmailAddress = toEmailAddress + ", " + ccEmailAddress;
            }


            //For Pricing Rates Email, CC the DM
            if (pricingFlag == true)
            {
                //sp_GetDMApprovalList
                string ccDMList = "";
                try
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter();
                    SqlCommand cmd = new SqlCommand();
                    cmd = new SqlCommand("sp_GetDMApprovalList", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));                   
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                    foreach (DataRow row in dt.Rows)
                    {
                        
                        string myDMEmail = getEmailAddress(row["DMName"].ToString());
                        if (myDMEmail != null && myDMEmail != "")
                        {
                            ccDMList = ccDMList + ",";
                            ccDMList = ccDMList + myDMEmail;
                        }                       
                    }
                }
                 catch (System.Exception ex)
                {
                    
                 }

                toEmailAddress = toEmailAddress +  ccDMList;
            }


            toEmailAddress = toEmailAddress + ", Michele.Kennedy@purolator.com";

            //CC group email boxes for Contracts and Operations 
            try
            {
                string routetoUserRole = "";
                string ccGroup = "";
                string sql = "select r.Role from COM.applicationUserRoles ur join COM.applicationRoles r on r.id = ur.idRole where ur.ActiveDirectory = '" + routeTo + "' and ur.idApplication = 1 and ur.ActiveFlag = 1";

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    routetoUserRole = row.Field<string>("Role").Trim();
                }

                if (routetoUserRole == "Contracts")
                {                   
                        ccGroup = "USContracts@purolator.com";                       
                }
                if (routetoUserRole == "Operations")
                {
                    //get all users for the group
                     sql = "SELECT ActiveDirectory FROM COM.applicationUserRoles where idApplication=1 and ActiveFlag = 1and idRole = 9  Order by ActiveDirectory";                    
                        using (SqlCommand com = new SqlCommand(sql, conn))
                        {                            
                            using (SqlDataReader sdr = com.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                  string opsUser = sdr["ActiveDirectory"].ToString().Trim();
                                  if (opsUser != routeTo)
                                  {
                                    //get email address and add it to the group
                                    string opsEmail = getEmailAddress(opsUser);
                                    if (opsEmail != null && opsEmail != "")
                                    {
                                        if (ccGroup != "")
                                            ccGroup = ccGroup + ",";
                                        ccGroup = ccGroup + opsEmail;
                                    }
                                }
                                }
                            }                            
                        }
                }

                if (ccGroup != "")
                {
                    toEmailAddress = toEmailAddress + ", " + ccGroup;
                }
            }
            catch (System.Exception ex)
            {

            }

            IList<dtoRoutingEmailCC> qRoutingCC = db.RoutingEmailCCs.Where(r => r.RouteToUser.Contains(routeTo)).Select(s => new dtoRoutingEmailCC() { id = s.id, RouteToUser = s.RouteToUser, ADUserToCC = s.ADUserToCC, UserToCC = s.UserToCC }).ToList();
            foreach (dtoRoutingEmailCC r in qRoutingCC)
            {
                ccEmailAddress = getEmailAddress(r.ADUserToCC);
                toEmailAddress = toEmailAddress + ", " + ccEmailAddress;
            }

            var MyCARR = db.CARRs.Where(x => x.idCarr == MyCarrDetail.idCARR).FirstOrDefault();

            try
            {
                string  routedFrom = Session["accountname"].ToString();
                string subject = MyCARR.CustomerLegalName + " CARR Version " + MyCarrDetail.VersionNumber.ToString() + " - " + MyCarrDetail.SalesProfessional;
                string msgBody = MyCARR.CustomerLegalName + " CARR Version " + MyCarrDetail.VersionNumber.ToString() + " has been routed to " + routeTo +  " from " + routedFrom + ".";

                msgBody = msgBody + "\n\n" + emailMsg;

                if (pricingFlag == true)
                {
                    msgBody = msgBody + "\n\nPricing Rates have been uploaded.";
                    subject = "Rates Available for " + subject;
                }

                msgBody = msgBody + "\n\nURL: " + ApplicationURL;
;                

                string host = System.Configuration.ConfigurationManager.AppSettings["host"].ToString();
                int port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["port"]);
                string userName = System.Configuration.ConfigurationManager.AppSettings["userName"];
                string password = System.Configuration.ConfigurationManager.AppSettings["password"];
                string fromEmail = System.Configuration.ConfigurationManager.AppSettings["fromEmail"];                
                string toEmail = toEmailAddress;


                SmtpClient client = new SmtpClient(host, port);               
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(userName, password);

                string errorMsg = "Error Sending Email";
                MailMessage message = new MailMessage (fromEmail, toEmail, subject, errorMsg);


                message.Body = msgBody;
              
                client.Send(message);


            }
            catch (System.Exception ex)
            {
                recordError(1, ex.Message, "");
            }
            conn.Close();

        }

        public string getEmailAddress(string username)
        {
            string emailaddress = "";
            String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("sp_GetEmailAddress", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@username", username));
            emailaddress =  (string)cmd.ExecuteScalar();
            conn.Close();
            return emailaddress;
        }

        [HttpPost]
        [Authorize]
        public JsonResult getCPCExpeditedRates(float markuppct, float exchangerate, DateTime effectivedate)
        {
            String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            try
            {

                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("sp_CalcCPCExpeditedWithAddl", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@markuppct", markuppct));
                cmd.Parameters.Add(new SqlParameter("@exchangerate", exchangerate));
                cmd.Parameters.Add(new SqlParameter("@effectivedate", effectivedate));
                da.SelectCommand = cmd;
                da.Fill(dt);
                List<Dictionary<string, object>> lstRates = GetTableRows(dt);

                return Json(lstRates, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult getCPCXpressPostRates(float markuppct, float exchangerate, DateTime effectivedate)
        {
            String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            try
            {

                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("sp_CalcCPCXpressPostWithAddl", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@markuppct", markuppct));
                cmd.Parameters.Add(new SqlParameter("@exchangerate", exchangerate));
                cmd.Parameters.Add(new SqlParameter("@effectivedate", effectivedate));
                da.SelectCommand = cmd;
                da.Fill(dt);
                List<Dictionary<string, object>> lstRates = GetTableRows(dt);

                return Json(lstRates, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            string desc = Request.Form["description"];
            string username = Session["accountname"].ToString();
            bool pricingOnly = Convert.ToBoolean(Request.Form["pricingOnly"]);
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string fileUploadFolderName = "FileUpload";
                        string fileName = Path.GetFileName(files[0].FileName);
                        string filenameWithTime = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", DateTime.Now.ToString("yyyyMMddHHmm"), Path.GetExtension(fileName));

                        string path = Path.Combine(Server.MapPath("~/" + fileUploadFolderName), filenameWithTime);

                        files[0].SaveAs(path);

                        CARRFileUpload fileUpload = new CARRFileUpload();
                        fileUpload.idCARRDetail = idCARRDetail;
                        fileUpload.Description = desc;
                        fileUpload.PricingOnlyFlag = pricingOnly;
                        fileUpload.FilePath = Path.Combine(fileUploadFolderName, filenameWithTime);
                        fileUpload.CreatedBy = username;
                        fileUpload.CreatedOn = DateTime.Now;
                        fileUpload.ActiveFlag = true;
                        db.CARRFileUploads.Add(fileUpload);

                        db.SaveChanges();
                        ViewBag.Message = "File uploaded successfully";
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully");
                }
                catch (System.Exception ex)
                {
                    ViewBag.Message = "Error occurred. Error details: " + ex.Message;
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                ViewBag.Message = "No files selected.";
                return Json("No files selected.");
            }
        }

        [HttpPost]
        public ActionResult UploadContractFiles()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string directory = "USContractFileUpload";
                        string fileName = Path.GetFileName(files[i].FileName);
                        fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", DateTime.Now.ToString("yyyyMMddHHmm"), Path.GetExtension(fileName));

                        // Save file to server file system
                        files[i].SaveAs(Path.Combine(Server.MapPath("~/" + directory), fileName));

                        // Add file location in server to DB
                        CARRVDAFile fileUpload = new CARRVDAFile();
                        fileUpload.idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
                        fileUpload.Description = Request.Form["description"];
                        fileUpload.FilePath = Path.Combine(directory, fileName);
                        fileUpload.CreatedBy = Session["accountname"].ToString();
                        fileUpload.CreatedOn = DateTime.Now;
                        fileUpload.ActiveFlag = true;
                        db.CARRVDAFiles.Add(fileUpload);

                        db.SaveChanges();

                        ViewBag.Message = "File uploaded successfully";
                    }
                }
                catch (System.Exception e)
                {
                    ViewBag.Message = "Error occurred. Error details: " + e.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "No files selected.";
            }

            return Json(ViewBag.Message);
        }

        [HttpPost]
        public ActionResult UploadShippingProfiles()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string directory = "FileUpload";
                        string fileName = Path.GetFileName(files[i].FileName);
                        fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", DateTime.Now.ToString("yyyyMMddHHmm"), Path.GetExtension(fileName));

                        // Save file to server file system
                        files[i].SaveAs(Path.Combine(Server.MapPath("~/" + directory), fileName));

                        // Add file location in server to DB
                        CARRPuroPostFile fileUpload = new CARRPuroPostFile();
                        fileUpload.idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
                        fileUpload.Description = Request.Form["description"];
                        fileUpload.FilePath = Path.Combine(directory, fileName);
                        fileUpload.CreatedBy = Session["accountname"].ToString();
                        fileUpload.CreatedOn = DateTime.Now;
                        fileUpload.ActiveFlag = true;
                        db.CARRPuroPostFiles.Add(fileUpload);

                        db.SaveChanges();

                        ViewBag.Message = "File uploaded successfully";
                    }
                }
                catch (System.Exception e)
                {
                    ViewBag.Message = "Error occurred. Error details: " + e.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "No files selected.";
            }

            return Json(ViewBag.Message);
        }

        [HttpPost]
        public ActionResult UploadRateFiles()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            string desc = Request.Form["description"];
            string username = Session["accountname"].ToString();
            bool routeflag = bool.Parse(Request.Form["routeflag"]);

            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string fileUploadFolderName = "RateFileUpload";
                        string fileName = Path.GetFileName(files[0].FileName);
                        string filenameWithTime = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", DateTime.Now.ToString("yyyyMMddHHmm"), Path.GetExtension(fileName));

                        string path = Path.Combine(Server.MapPath("~/" + fileUploadFolderName), filenameWithTime);

                        files[0].SaveAs(path);

                        DateTime timeNow = DateTime.Now;
                        
                        CARRRateFile rateUpload = new CARRRateFile();
                        rateUpload.idCARRDetail = idCARRDetail;
                        rateUpload.Description = desc;
                        rateUpload.FilePath = Path.Combine(fileUploadFolderName, filenameWithTime);
                        rateUpload.CreatedBy = username;
                        rateUpload.CreatedOn = timeNow;
                        rateUpload.ActiveFlag = true;
                        db.CARRRateFiles.Add(rateUpload);

                        var carrDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
                        carrDetail.PricingReqCompleteFlag = true;
                        carrDetail.PricingCompletedBy = username;
                        carrDetail.PricingReqCompleteDate = timeNow;

                        db.SaveChanges();
                        ViewBag.Message = "File uploaded successfully";
                    }

                    processPricing(idCARRDetail, routeflag);
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully");
                }
                catch (System.Exception ex)
                {
                    ViewBag.Message = "Error occurred. Error details: " + ex.Message;
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                ViewBag.Message = "No files selected.";
                return Json("No files selected.");
            }
        }

        [HttpPost]
        public ActionResult UploadSignatureFile()
        {
            string activeDirectory = Request.Form["activeDirectory"];

            var existingSignatures = db.Signatures.Where(x => x.ActiveDirectory == activeDirectory).ToList();
            foreach (var u in existingSignatures)
            {
                db.Signatures.Remove(u);
            }
            db.SaveChanges();

            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string fileUploadFolderName = "Contract\\Signatures";
                        string fileName = Path.GetFileName(files[0].FileName);
                        string filenameWithTime = string.Concat(activeDirectory, "_", DateTime.Now.ToString("yyyyMMddHHmm"), Path.GetExtension(fileName));

                        string path = Path.Combine(Server.MapPath("~/" + fileUploadFolderName), filenameWithTime);

                        files[0].SaveAs(path);

                        DateTime timeNow = DateTime.Now;

                        Signature signature = new Signature();
                        signature.ActiveDirectory = activeDirectory;
                        signature.ImageURL = Path.Combine(fileUploadFolderName, filenameWithTime);
                        db.Signatures.Add(signature);

                        db.SaveChanges();
                        ViewBag.Message = "File uploaded successfully";
                    }

                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully");
                }
                catch (System.Exception ex)
                {
                    ViewBag.Message = "Error occurred. Error details: " + ex.Message;
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                ViewBag.Message = "No files selected.";
                return Json("No files selected.");
            }
        }

        [HttpPost]
        public ActionResult UPSComparisonUpload()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            string desc = Request.Form["description"];
            string username = Session["accountname"].ToString();

            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string fileUploadFolderName = "RateFileUpload";
                        string fileName = Path.GetFileName(files[0].FileName);
                        string filenameWithTime = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", DateTime.Now.ToString("yyyyMMddHHmm"), Path.GetExtension(fileName));

                        string path = Path.Combine(Server.MapPath("~/" + fileUploadFolderName), filenameWithTime);

                        files[0].SaveAs(path);

                        DateTime timeNow = DateTime.Now;

                        UPSComparisonFile comparisonUpload = new UPSComparisonFile();
                        comparisonUpload.idCARRDetail = idCARRDetail;
                        comparisonUpload.Description = desc;
                        comparisonUpload.FilePath = Path.Combine(fileUploadFolderName, filenameWithTime);
                        comparisonUpload.CreatedBy = username;
                        comparisonUpload.CreatedOn = timeNow;
                        comparisonUpload.ActiveFlag = true;
                        db.UPSComparisonFiles.Add(comparisonUpload);

                        db.SaveChanges();
                        ViewBag.Message = "File uploaded successfully";
                    }

                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully");
                }
                catch (System.Exception ex)
                {
                    ViewBag.Message = "Error occurred. Error details: " + ex.Message;
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                ViewBag.Message = "No files selected.";
                return Json("No files selected.");
            }
        }

        [HttpPost]
        public ActionResult USContractUploadFiles()
        {
            int idCARRDetail = int.Parse(Request.Form["idcarrdetail"]);
            string desc = Request.Form["description"];
            string username = Session["accountname"].ToString();

            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string fileUploadFolderName = "USContractFileUpload";
                        string fileName = Path.GetFileName(files[0].FileName);
                        string filenameWithTime = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", DateTime.Now.ToString("yyyyMMddHHmm"), Path.GetExtension(fileName));

                        string path = Path.Combine(Server.MapPath("~/" + fileUploadFolderName), filenameWithTime);

                        files[0].SaveAs(path);

                        DateTime timeNow = DateTime.Now;

                        USContractsUpload fileUpload = new USContractsUpload();
                        fileUpload.idCARRDetail = idCARRDetail;
                        fileUpload.Description = desc;
                        fileUpload.FilePath = Path.Combine(fileUploadFolderName, filenameWithTime);
                        fileUpload.CreatedBy = username;
                        fileUpload.CreatedOn = timeNow;
                        fileUpload.ActiveFlag = true;
                        db.USContractsUploads.Add(fileUpload);

                        db.SaveChanges();
                        ViewBag.Message = "File uploaded successfully";
                    }

                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully");
                }
                catch (System.Exception ex)
                {
                    ViewBag.Message = "Error occurred. Error details: " + ex.Message;
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                ViewBag.Message = "No files selected.";
                return Json("No files selected.");
            }
        }


        [HttpPost]
        [Authorize]
        public JsonResult validateADNames(string username)
        {
            String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            try
            {

                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                cmd = new SqlCommand("sp_ValidateADName", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@username", username));
                da.SelectCommand = cmd;
                da.Fill(dt);
                List<Dictionary<string, object>> validationResult = GetTableRows(dt);

                return Json(validationResult, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message.ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateUser(string username, int idRole)
        {
            JsonResult json = new JsonResult();
            String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            SqlDataReader rdr = null;

            List<applicationUserRole> duplicateList = db.applicationUserRoles.Where(x => x.ActiveDirectory.Equals(username) && x.idApplication == 1 && x.ActiveFlag == true).ToList();
            if (duplicateList.Count() > 0)
            {

                json.Data = new { success = false, error = "Duplicate" };
                return json;
            }
            else
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataTable dt = new DataTable();
                    cmd = new SqlCommand("sp_ValidateADName", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@username", username));

                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if ((int)rdr["ADCount"] == 0)
                        {
                            json.Data = new { success = false, error = "InvalidUsername" };
                            return json;
                        }
                        else
                        {
                            applicationUserRole aur = new applicationUserRole();
                            aur.ActiveDirectory = username;
                            aur.idRole = idRole;
                            aur.idApplication = 1;
                            aur.ActiveFlag = true;
                            db.applicationUserRoles.Add(aur);
                            db.SaveChanges();
                        }
                    }

                }
                catch (System.Exception ex)
                {
                    string errMsg = ex.Message.ToString();
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                    rdr.Close();
                }
            }

            json.Data = new { success = true };
            return json;

        }

        [HttpPost]
        [Authorize]
        public ActionResult EditUserAccess(String[] selectedDistricts, String[] selectedBranches, String[] selectedDirectReports, string activeDirectory, int userRole)
        {
            JsonResult json = new JsonResult();
            string username = Session["accountname"].ToString();

            DateTime timeNow = DateTime.Now;

            try
            {
                // Delete any existing records for this user before adding new ones
                var existingDistricts = db.ApplicationDistrictsAlloweds.Where(x => x.ActiveDirectory.Equals(activeDirectory)).ToList();
                foreach (var d in existingDistricts)
                {
                    db.ApplicationDistrictsAlloweds.Remove(d);
                }

                var existingBranches = db.ApplicationRegionsAlloweds.Where(x => x.ActiveDirectory.Equals(activeDirectory)).ToList();
                foreach (var b in existingBranches)
                {
                    db.ApplicationRegionsAlloweds.Remove(b);
                }
                
                var existingDirectReports = db.ApplicationUsersAlloweds.Where(x => x.ActiveDirectory.Equals(activeDirectory)).ToList();
                foreach (var u in existingDirectReports)
                {
                    db.ApplicationUsersAlloweds.Remove(u);
                }
                db.SaveChanges();
                
                if (selectedDistricts != null)
                {
                    foreach(String district in selectedDistricts)
                    {
                        ApplicationDistrictsAllowed ada = new ApplicationDistrictsAllowed();
                        ada.ActiveDirectory = activeDirectory;
                        ada.District = district;
                        ada.UpdatedBy = username;
                        ada.UpdatedOn = timeNow;
                        db.ApplicationDistrictsAlloweds.Add(ada);
                    }
                }

                if (selectedBranches != null)
                {
                    foreach(String branch in selectedBranches)
                    {
                        ApplicationRegionsAllowed ara = new ApplicationRegionsAllowed();
                        ara.ActiveDirectory = activeDirectory;
                        ara.Region = branch;
                        ara.UpdatedBy = username;
                        ara.UpdatedOn = timeNow;
                        db.ApplicationRegionsAlloweds.Add(ara);
                    }
                }
              
                if (selectedDirectReports != null)
                {
                    foreach(String directReports in selectedDirectReports)
                    {
                        ApplicationUsersAllowed aur = new ApplicationUsersAllowed();
                        aur.ActiveDirectory = activeDirectory;
                        aur.UserAD = directReports;
                        aur.UpdatedBy = username;
                        aur.UpdatedOn = timeNow;
                        db.ApplicationUsersAlloweds.Add(aur);
                    }
                }

                var userrole = db.applicationUserRoles.Where(x => x.ActiveDirectory.Equals(activeDirectory) && x.idApplication == 1).FirstOrDefault();
                userrole.idRole = userRole;

                db.SaveChanges();
                json.Data = new { success = true };
                return json;
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message };
                return json;
            }
            
        }

        [HttpPost]
        [Authorize]
        public ActionResult LocationExistsCheck(string locationName, int idCARRDetail)
        {
            JsonResult json = new JsonResult();
            try
            {
                if (db.CARRLocations.Any(x => x.LocationName.Equals(locationName) && x.idCARRDetail == idCARRDetail && x.ActiveFlag == true))
                    json.Data = new { success = false};
                else
                    json.Data = new { success = true};
            }
            catch (System.Exception ex)
            {
                json.Data = new { success = false, error = ex.Message };
            }
            return json;
        }

        public int updatePricingComments(int idCARRDetail, string pricingComments)
        {
            int result = 0;
            try
            {
                var CARRDetail = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
                CARRDetail.PricingComments = pricingComments;
                db.SaveChanges();
                result = 1;
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message.ToString());
            }

            return result;
        }

        public int updateEstRevChanges(int idCARRDetail, string type, int service, string price)
        {
            int result = 0;
            try
            {
                CARREstRevSalesPricing estRev = db.CARREstRevSalesPricings.Where(x => x.idCARRDetail == idCARRDetail && x.idRevType == service).FirstOrDefault();

               


                float? temp;

                // Check if value is empty string or negative
                try
                {
                    temp = float.Parse(price);

                    if (temp < 0)
                    {
                        temp = null;
                    }
                }
                catch (System.Exception)
                {
                    temp = null;
                }

                switch (type)
                {
                    case "Sales":
                        estRev.EstWklyRevSales = temp;
                        break;
                    case "SalesDSM":
                        estRev.EstWklyRevSales = temp;
                        break;
                    case "Pricing":
                        estRev.EstWklyRevPricing = temp;
                        break;
                }

                db.SaveChanges();
                result = 1;
            }
            catch (System.Exception e)
            {
                
            }

            return result;
        }

        public int updateCarrDetailAccessorialFlags(int idCARRDetail, bool courierChecked, bool freightChecked, bool cpcChecked, bool ppstChecked, bool ltlyyzChecked, bool ltlyvrChecked, bool ltlywgChecked, bool ltlyulChecked)
        {
            int result = 0;

            try
            {
                CARRDetail temp = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
                temp.courierAccessorialFlag = courierChecked;
                temp.freightAccessorialFlag = freightChecked;
                temp.cpcAccessorialFlag = cpcChecked;
                temp.ppstAccessorialFlag = ppstChecked;
                temp.ltlyyzFlag = ltlyyzChecked;
                temp.ltlyvrFlag = ltlyvrChecked;
                temp.ltlywgFlag = ltlywgChecked;
                temp.ltlyulFlag = ltlyulChecked;

                db.SaveChanges();
                result = 1;
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message.ToString());
            }

            return result;
        }

        public int updateCarrDetailAccessorialFlags2(int idCARRDetail, bool courierChecked, bool freightChecked, bool cpcChecked)
        {
            int result = 0;

            try
            {
                CARRDetail temp = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault();
                temp.courierAccessorialFlag = courierChecked;
                temp.freightAccessorialFlag = freightChecked;
                temp.cpcAccessorialFlag = cpcChecked;               

                db.SaveChanges();
                result = 1;
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message.ToString());
            }

            return result;
        }


        public void recordError(int idApplication, string errMsg, string username)
    {
        String cs = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
        SqlConnection conn = new SqlConnection(cs);
            conn.Open();
            try
        {

            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            cmd = new SqlCommand("sp_RecordErrMsg", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idApplication", idApplication));
            cmd.Parameters.Add(new SqlParameter("@errMsg", errMsg));
            cmd.Parameters.Add(new SqlParameter("@username", username));
            cmd.ExecuteNonQuery();        
            
        }
        catch (System.Exception ex)
        {
            
        }
        finally
        {
            conn.Close();
            conn.Dispose();
        }
        
    }


    }
}