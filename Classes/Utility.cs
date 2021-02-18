using System.Data;
using System.Data.SqlClient;

namespace PI_Portal.Classes
{
    public static class Utility
    {
        // TODO - Abstract functionality of these to just one function
        static public DataTable getShippingProfileData(string procedure, int idCARRDetail)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(procedure, cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
                catch (System.Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message.ToString());
                }
            }

            return dt;
        }

        static public DataTable getShippingTableData(string procedure, int idCARR)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(procedure, cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@idCARR", idCARR));
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
                catch (System.Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message.ToString());
                }
            }

            return dt;
        }

        static public DataTable getEstRevData(int idCARRDetail)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();

            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_getAllRevEst", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@idCARRDetail", idCARRDetail));
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
                catch (System.Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message.ToString());
                }
            }

            return dt;
        }

        static public DataTable getRateFileTableData(int idCARRDetail)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    string sql = $"select idRateFile, FilePath, Description, CreatedBy, CreatedOn from CAR.CARRRateFile where ActiveFlag = 1 and idCARRDetail = {idCARRDetail} order by CreatedOn desc";
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

            return dt;
        }

        static  public bool getPPSTFlag (int idCARRDetail)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection cnn = new SqlConnection(connectionString);
            DataTable dtppstFlag = new DataTable();
            string sqlppstFlag = @"select count(s.idShippingService) ppstCount
                                    from CAR.Service s
                                    join CAR.CARRLocation l on l.idLocation = s.idLocation
                                    where l.ActiveFlag = 1 and l.idCARRDetail = ";
            sqlppstFlag = sqlppstFlag + idCARRDetail + " and s.idShippingService in (6,7,9,12)";

            SqlDataAdapter dappstFlag = new SqlDataAdapter(sqlppstFlag, cnn);
            dappstFlag.Fill(dtppstFlag);
            int p = (int)dtppstFlag.Rows[0]["ppstCount"];
            bool PuroPostFlag = false;
            if (p > 0)
                PuroPostFlag = true;
            return PuroPostFlag;
        }

        static public bool getOtherFlag(int idCARRDetail)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            SqlConnection cnn = new SqlConnection(connectionString);
            DataTable dtotherFlag = new DataTable();
            string sqlotherFlag = @"select count(s.idShippingService) ppstCount
                                    from CAR.Service s
                                    join CAR.CARRLocation l on l.idLocation = s.idLocation
                                    where l.ActiveFlag = 1 and l.idCARRDetail = ";
            sqlotherFlag = sqlotherFlag + idCARRDetail + " and s.idShippingService not in (6,7,9,12)";

            SqlDataAdapter dappstFlag = new SqlDataAdapter(sqlotherFlag, cnn);
            dappstFlag.Fill(dtotherFlag);
            int p = (int)dtotherFlag.Rows[0]["ppstCount"];
            bool OtherFlag = false;
            if (p > 0)
                OtherFlag = true;
            return OtherFlag;
        }

        static public DataTable getVDAFileTableData(int idCARRDetail)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    string sql = $"select idVDAFile, FilePath, Description, CreatedBy, CreatedOn from CAR.CARRVDAFiles where ActiveFlag = 1 and idCARRDetail = {idCARRDetail} order by CreatedOn desc";
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

            return dt;
        }

        static public DataRow getVolumeRowByServiceID(int serviceID)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                try
                {
                    string sql = $@"select ind.code,ind.Description 'Induction',sv.idVolume,sv.idService,sv.idInduction,sv.NumberShipments,
                        sv.PickUpFrequency,sv.AvgWt,sv.AvgPcsOrSkids,sv.PkgLength,sv.PkgWidth,sv.PkgHeight,
                        sv.ProductDesc,sv.NumberPickups,sv.idTransitType,dd1.Name 'TransitType', sv.DimWeight
                        from CAR.ServiceVolume sv
                        left join CAR.DropDownsOptions dd1 on dd1.value = sv.idTransitType and dd1.id=7
                        left join CAR.InductionPoints ind on ind.idInduction = sv.idInduction
                        where sv.ActiveFlag = 1 and idService = {serviceID} order by sv.idVolume";
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
            try
            {
                return dt.Rows[0];
            }
            catch (System.Exception e)
            {
                return null;
            }
           
        }
    }
}