using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using PI_Portal.Models;

namespace PI_Portal.Classes
{
    public class HelperDropdowns
    {
        private static Entities db = new Entities();

        static public List<SelectListItem> getSalesList()
        {
            List<SelectListItem> salesList = new List<SelectListItem>();
            salesList.Add(new SelectListItem { Text = "Select Sales Professional", Value = "0" });
            string sql = "SELECT id,ActiveDirectory FROM COM.applicationUserRoles where idApplication=1 and idRole = 1 and ActiveFlag = 1 Order by ActiveDirectory";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            salesList.Add(new SelectListItem
                            {
                                Text = sdr["ActiveDirectory"].ToString().Trim(),
                                Value = sdr["id"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return salesList;
        }

        static public List<SelectListItem> getSalesDMList()
        {
            List<SelectListItem> salesList = new List<SelectListItem>();
            salesList.Add(new SelectListItem { Text = "Select District Manager", Value = "0" });
            string sql = "SELECT id,ActiveDirectory FROM COM.applicationUserRoles where  idApplication=1 and idRole = 6 and ActiveFlag = 1 Order by ActiveDirectory";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            salesList.Add(new SelectListItem
                            {
                                Text = sdr["ActiveDirectory"].ToString().Trim(),
                                Value = sdr["id"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return salesList;
        }

        static public List<SelectListItem> getSalesDSMList()
        {
            List<SelectListItem> salesList = new List<SelectListItem>();
            salesList.Add(new SelectListItem { Text = "Select Sales Manager", Value = "0" });
            string sql = "SELECT id,ActiveDirectory FROM COM.applicationUserRoles where  idApplication=1 and  idRole = 5 and ActiveFlag = 1 Order by ActiveDirectory";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            salesList.Add(new SelectListItem
                            {
                                Text = sdr["ActiveDirectory"].ToString().Trim(),
                                Value = sdr["id"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return salesList;
        }


        //static public List<SelectListItem> getLocalSRIDList()
        //{
        //    List<SelectListItem> salesList = new List<SelectListItem>();
        //    salesList.Add(new SelectListItem { Text = "Select Local Sales Professional", Value = "0" });
        //    string sql = "SELECT SalesRepID,SalesRep FROM tblSalesReps where BusinessType='Field' and ActiveFlag = 1 Order by SalesRep";
        //    String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PUMADB"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(conStr))
        //    {
        //        using (SqlCommand com = new SqlCommand(sql, con))
        //        {
        //            con.Open();
        //            using (SqlDataReader sdr = com.ExecuteReader())
        //            {
        //                while (sdr.Read())
        //                {
        //                    salesList.Add(new SelectListItem
        //                    {
        //                        Text = sdr["SalesRep"].ToString() + " (" + sdr["SalesRepID"].ToString() + ")",
        //                        Value = sdr["SalesRepID"].ToString()
        //                    });
        //                }
        //            }
        //            con.Close();
        //        }
        //    }
        //    return salesList;
        //}
        static public List<SelectListItem> getLocalSRIDList()
        {
            List<SelectListItem> salesList = new List<SelectListItem>();
            salesList.Add(new SelectListItem { Text = "Select Local Sales Professional", Value = "0" });
            string sql = "SELECT u.id,u.ActiveDirectory,s.LocalSRID FROM COM.applicationUserRoles u  join CAR.SRIDs s on s.ActiveDirectory = u.ActiveDirectory where u.idApplication=1 and u.idRole = 1 and u.ActiveFlag = 1 and ISNULL(s.LocalSRID,'') != '' Order by u.ActiveDirectory";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PURODB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            salesList.Add(new SelectListItem
                            {
                                Text = sdr["ActiveDirectory"].ToString().Trim() + " (" + sdr["LocalSRID"].ToString() + ")",
                                Value = sdr["LocalSRID"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return salesList;
        }



        //static public List<SelectListItem> getStrategicSRIDList()
        //{
        //    List<SelectListItem> salesList = new List<SelectListItem>();
        //    salesList.Add(new SelectListItem { Text = "Select Strategic Sales Professional", Value = "0" });
        //    string sql = "SELECT SalesRepID,SalesRep FROM tblSalesReps where BusinessType='Strategic' and ActiveFlag = 1 Order by SalesRep";
        //    String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PUMADB"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(conStr))
        //    {
        //        using (SqlCommand com = new SqlCommand(sql, con))
        //        {
        //            con.Open();
        //            using (SqlDataReader sdr = com.ExecuteReader())
        //            {
        //                while (sdr.Read())
        //                {
        //                    salesList.Add(new SelectListItem
        //                    {
        //                        Text = sdr["SalesRep"].ToString() + " (" + sdr["SalesRepID"].ToString() + ")",
        //                        Value = sdr["SalesRepID"].ToString()
        //                    });
        //                }
        //            }
        //            con.Close();
        //        }
        //    }
        //    return salesList;
        //}
        static public List<SelectListItem> getStrategicSRIDList()
        {
            List<SelectListItem> salesList = new List<SelectListItem>();
            salesList.Add(new SelectListItem { Text = "Select Strategic Sales Professional", Value = "0" });
            string sql = "SELECT u.id,u.ActiveDirectory,s.StrategicSRID FROM COM.applicationUserRoles u  join CAR.SRIDs s on s.ActiveDirectory = u.ActiveDirectory where u.idApplication=1 and u.idRole = 1 and u.ActiveFlag = 1 and ISNULL(s.StrategicSRID,'') != '' Order by u.ActiveDirectory";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PURODB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            salesList.Add(new SelectListItem
                            {
                                Text = sdr["ActiveDirectory"].ToString().Trim() + " (" + sdr["StrategicSRID"].ToString() + ")",
                                Value = sdr["StrategicSRID"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return salesList;
        }

        static public List<SelectListItem> getISPSRIDList()
        {
            List<SelectListItem> salesList = new List<SelectListItem>();
            salesList.Add(new SelectListItem { Text = "Select SDR Sales Professional", Value = "0" });
            //string sql = "SELECT SalesRepID,SalesRep FROM tblSalesReps where SalesRepTitle='IAE' and SalesRep not like '%open%' and ActiveFlag = 1 Order by SalesRep";
            //String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PUMADB"].ConnectionString;
          
            string sql = "SELECT ID ,ActiveDirectory FROM CAR.InternalSales where ActiveFlag = 1 Order by ActiveDirectory";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PURODB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            salesList.Add(new SelectListItem
                            {
                                Text = sdr["ActiveDirectory"].ToString(),
                                Value = sdr["ID"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return salesList;
        }

        static public List<SelectListItem> getUserList()
        {
            //eliminate contract users (12)
            List<SelectListItem> salesList = new List<SelectListItem>();
            salesList.Add(new SelectListItem { Text = "Select CARR User", Value = "0" });
            string sql = "SELECT ActiveDirectory FROM COM.applicationUserRoles where idApplication=1 and ActiveFlag = 1  Order by ActiveDirectory";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            salesList.Add(new SelectListItem
                            {
                                Text = sdr["ActiveDirectory"].ToString(),
                                Value = sdr["ActiveDirectory"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return salesList;
        }

        //static public List<SelectListItem> getRoutingUserList(int idCARRDetail, string role, bool pricingFlag, out string preselect)
        static public List<SelectListItem> getRoutingUserList(int idCARRDetail, string role, bool pricingFlag)
        {
            List<SelectListItem> userList = new List<SelectListItem>();

            var routeToRow = db.RoutingRules.Where(x => x.Role == role && x.BeforeFlag == pricingFlag).FirstOrDefault();          
            string routeTo = routeToRow.RouteToRoles;
            if (routeTo == null || routeTo.IsEmpty())
            {               
                return userList;
            }

            //preselect = db.RoutingRules.Where(x => x.Role.Equals(role) && x.BeforeFlag.Equals(pricingFlag)).FirstOrDefault().Preselect;
            //if (preselect != null || preselect.IsEmpty())
            //{
            //    switch (preselect)
            //    {
            //        case "SalesManager":
            //            preselect = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault().SalesManager.ToString();
            //            break;
            //        case "DistrictManager":
            //            preselect = db.CARRDetails.Where(x => x.idCARRDetail == idCARRDetail).FirstOrDefault().DistrictManager.ToString();
            //            break;
            //        case "DFL":
            //            preselect = db.RoutingDefaults.Where(x => x.Role == "DFL").FirstOrDefault().RouteToUser.ToString();
            //            break;
            //    }
            //}

            if (routeTo.Equals("all"))
            {
                foreach (applicationUserRole user in db.applicationUserRoles.Where(x => x.idApplication == 1 && x.ActiveFlag == true))
                {
                    userList.Add(new SelectListItem
                    {
                        Text = user.ActiveDirectory.ToString(),
                        Value = user.ActiveDirectory.ToString()
                    });
                }
            }
            else
            {
                foreach (string routedRole in routeTo.Split(','))
                {
                    foreach (applicationUserRole user in db.applicationUserRoles.Where(x =>
                    // User's role is within routed roles list
                    x.idRole == (db.applicationRoles.Where(z => z.Role == routedRole).FirstOrDefault().id)
                    // Active and correct application group
                    && x.idApplication == 1
                    && x.ActiveFlag == true))
                    {
                        userList.Add(new SelectListItem
                        {
                            Text = user.ActiveDirectory.ToString(),
                            Value = user.ActiveDirectory.ToString()
                        });
                    }
                }
            }

            return userList.OrderBy(x => x.Text).ToList();
        }

        static public List<SelectListItem> getUserListNoContracts()
        {
            //eliminate contract users (12)
            List<SelectListItem> salesList = new List<SelectListItem>();
            salesList.Add(new SelectListItem { Text = "Select CARR User", Value = "0" });
            string sql = "SELECT ActiveDirectory FROM COM.applicationUserRoles where idApplication=1 and ActiveFlag = 1 and idRole != 12  Order by ActiveDirectory";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            salesList.Add(new SelectListItem
                            {
                                Text = sdr["ActiveDirectory"].ToString(),
                                Value = sdr["ActiveDirectory"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return salesList;
        }

        static public List<SelectListItem> getUserListNoPricing()
        {
            //Eliminate operations (9) and pricing (8)
            List<SelectListItem> userList = new List<SelectListItem>();
            userList.Add(new SelectListItem { Text = "Select CARR User", Value = "0" });
            string sql = "SELECT ActiveDirectory FROM COM.applicationUserRoles where idApplication=1 and ActiveFlag = 1 and idRole not in (8,9) Order by ActiveDirectory";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            userList.Add(new SelectListItem
                            {
                                Text = sdr["ActiveDirectory"].ToString(),
                                Value = sdr["ActiveDirectory"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return userList;
        }

        static public List<SelectListItem> getContractsUserList()
        {
            //select only contract users (12)
            List<SelectListItem> salesList = new List<SelectListItem>();
            salesList.Add(new SelectListItem { Text = "Select CARR User", Value = "0" });
            string sql = "SELECT ActiveDirectory FROM COM.applicationUserRoles where idApplication=1 and ActiveFlag = 1and idRole = 12  Order by ActiveDirectory";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            salesList.Add(new SelectListItem
                            {
                                Text = sdr["ActiveDirectory"].ToString(),
                                Value = sdr["ActiveDirectory"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return salesList;
        }

        static public List<SelectListItem> getOperationsUserList()
        {
            //select only contract users (12)
            List<SelectListItem> salesList = new List<SelectListItem>();
            salesList.Add(new SelectListItem { Text = "Select CARR User", Value = "0" });
            string sql = "SELECT ActiveDirectory FROM COM.applicationUserRoles where idApplication=1 and ActiveFlag = 1and idRole = 9  Order by ActiveDirectory";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            salesList.Add(new SelectListItem
                            {
                                Text = sdr["ActiveDirectory"].ToString(),
                                Value = sdr["ActiveDirectory"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return salesList;
        }

        static public List<SelectListItem> getDFLUserList()
        {
            //select only contract users (12)
            List<SelectListItem> salesList = new List<SelectListItem>();
            salesList.Add(new SelectListItem { Text = "Select CARR User", Value = "0" });
            string sql = "SELECT ActiveDirectory FROM CAR.RoutingDefaults where Role = 'DFL' Order by ActiveDirectory";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            salesList.Add(new SelectListItem
                            {
                                Text = sdr["ActiveDirectory"].ToString(),
                                Value = sdr["ActiveDirectory"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return salesList;
        }
        static public List<SelectListItem> getRegionList()
        {
            List<SelectListItem> regionlist = new List<SelectListItem>();
            regionlist.Add(new SelectListItem { Text = "Select Branch", Value = "0" });
            //string sql = "SELECT Regionid,Airport FROM tblRegions Where Reported=1 and Airport not like 'Dell%' Order by Airport";
            string sql = "SELECT branchID,OriginBranch from CAR.OriginBranches order by OriginBranch";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PURODB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            regionlist.Add(new SelectListItem
                            {
                                Text = sdr["OriginBranch"].ToString().Trim(),
                                Value = sdr["OriginBranch"].ToString()
                            });
                        }
                    }
                   
                    con.Close();
                }
            }
            return regionlist;
        }

        static public List<SelectListItem> getOriginList()
        {
            List<SelectListItem> regionlist = getRegionList();
            regionlist.Add(new SelectListItem
            {
                Text = "Not Applicable",
                Value = "Not Applicable"
            });
            return regionlist;
        }
        static public List<SelectListItem> getDistrictList()
        {
            List<SelectListItem> regionlist = new List<SelectListItem>();
            regionlist.Add(new SelectListItem { Text = "Select District", Value = "0" });
            string sql = "Select distinct District from tblRegions where ActiveFlag = 1";
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
                            regionlist.Add(new SelectListItem
                            {
                                Text = sdr["District"].ToString().Trim(),
                                Value = sdr["District"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return regionlist;
        }

        static public List<SelectListItem> getGatewayList()
        {
            List<SelectListItem> gwlist = new List<SelectListItem>();
            gwlist.Add(new SelectListItem { Text = "Select Gateway", Value = "0" });
            // string sql = "SELECT gatewayid,gateway FROM tblGateways Where ActiveFlag=1 Order by gateway";
            string sql = "select gatewayID,gateway from CAR.Gateways where ActiveFlag=1 order by gateway";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PURODB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            gwlist.Add(new SelectListItem
                            {
                                Text = sdr["gateway"].ToString().Trim(),
                                Value = sdr["gateway"].ToString()
                            });
                        }
                    }
                    gwlist.Add(new SelectListItem
                    {
                        Text = "Not Applicable",
                        Value = "Not Applicable"
                    });
                    con.Close();
                }
            }
            return gwlist;
        }

        static public List<SelectListItem> getInductionList()
        {
            List<SelectListItem> indlist = new List<SelectListItem>();
            indlist.Add(new SelectListItem { Text = "Select Induction Address", Value = "0" });
            string sql = "SELECT idInduction,Description FROM CAR.InductionPoints where idInduction != 30 Order by Description";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            indlist.Add(new SelectListItem
                            {
                                Text = sdr["Description"].ToString().Trim(),
                                Value = sdr["idInduction"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return indlist;
        }

        static public List<SelectListItem> getLHRatingBranchList()
        {
            List<SelectListItem> indlist = new List<SelectListItem>();
            indlist.Add(new SelectListItem { Text = "Select Branch", Value = "0" });
            string sql = "SELECT DISTINCT Branch from CAR.LHBranchInfo";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            indlist.Add(new SelectListItem
                            {
                                Text = sdr["Branch"].ToString().Trim(),
                                Value = sdr["Branch"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return indlist;
        }

        static public List<SelectListItem> getLHRatingInductionList()
        {
            List<SelectListItem> indlist = new List<SelectListItem>();
            indlist.Add(new SelectListItem { Text = "Select Induction", Value = "0" });
            string sql = "SELECT DISTINCT Induction from CAR.LHBranchInfo";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            indlist.Add(new SelectListItem
                            {
                                Text = sdr["Induction"].ToString().Trim(),
                                Value = sdr["Induction"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return indlist;
        }

        static public List<SelectListItem> getLHRFTypes()
        {
            List<SelectListItem> salesList = new List<SelectListItem>();
            salesList.Add(new SelectListItem { Text = "Select LHRF Type", Value = "0" });
            string sql = "SELECT * FROM CAR.DropdownsOptions where id = 19 and isActive=1";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            salesList.Add(new SelectListItem
                            {
                                Text = sdr["Name"].ToString().Trim(),
                                Value = sdr["Value"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return salesList;
        }

        static public List<SelectListItem> getCPCExpEffDates()
        {
            List<SelectListItem> datelist = new List<SelectListItem>();
            datelist.Add(new SelectListItem { Text = "Select Effective Date", Value = "0" });
            string sql = @" select distinct EffectiveDate from CAR.DefaultRatesCPCExpedited order by EffectiveDate";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            datelist.Add(new SelectListItem
                            {
                                Text = sdr["EffectiveDate"].ToString().Trim(),
                                Value = sdr["EffectiveDate"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return datelist;
        }

        static public List<SelectListItem> getCPCXpressEffDates()
        {
            List<SelectListItem> datelist = new List<SelectListItem>();
            datelist.Add(new SelectListItem { Text = "Select Effective Date", Value = "0" });
            string sql = @" select distinct EffectiveDate from CAR.DefaultRatesCPCXpressPost order by EffectiveDate";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            datelist.Add(new SelectListItem
                            {
                                Text = sdr["EffectiveDate"].ToString().Trim(),
                                Value = sdr["EffectiveDate"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return datelist;
        }

        static public List<SelectListItem> getStateList()
        {
            List<SelectListItem> slist = new List<SelectListItem>();
            string sql = "SELECT StateAbbreviation,StateName FROM COM.States Order by id";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            slist.Add(new SelectListItem
                            {
                                Text = sdr["StateName"].ToString().Trim(),
                                Value = sdr["StateAbbreviation"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            slist = slist.OrderBy(x => x.Text).ToList();

            slist.Insert(0, new SelectListItem { Text = "Select State", Value = "0" });
            slist.Add(new SelectListItem { Text = "Other", Value = "" }); // TODO - Is this option needed?

            return slist;
        }

        static public List<SelectListItem> getUserRoles()
        {
            List<SelectListItem> slist = new List<SelectListItem>();
            slist.Add(new SelectListItem { Text = "Select User Role", Value = "0" });
            string sql = "Select * from COM.applicationRoles where idApplication = 1 order by Role";
            String conStr = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = com.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            slist.Add(new SelectListItem
                            {
                                Text = sdr["Role"].ToString().Trim(),
                                Value = sdr["id"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return slist;
        }
    }
}