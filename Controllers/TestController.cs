using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PI_Portal.Models;

namespace PI_Portal.Controllers
{
    public class TestController : Controller
    {
        private Entities db = new Entities();


        // GET: Test
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Datatable()
        {
            string sql = @" select 
                            C.customerlegalname as 'Name',
                            CM.Version as 'Version', 
                            CD.SalesProfessional as 'SP', 
                            CD.SalesManager as 'SM',
                            CD.DistrictManager as 'DM',
                            CD.DecisionMakerBranch as 'DecisionMaker',
                            C.CreatedOn,
                            C.UpdatedOn
                            from Car.carr C
                            left join(select idcarr, Max(VersionNumber) as Version
                                    from car.carrdetail
                                    where car.carrdetail.activeflag = 1
                                    group by idcarr
		                            ) CM on C.idcarr = CM.idCarr
                            left join car.carrdetail CD on CM.idcarr = CD.idcarr and CM.Version = CD.VersionNumber
                            where C.activeflag = 1";

            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PuroDB"].ConnectionString;

            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlDataAdapter da = new SqlDataAdapter(sql, connection);
            da.Fill(dt);
            connection.Close();
            connection.Dispose();

            ViewBag.title = "Getting Started...";
            ViewBag.result = dt;

            return View();
        }

    }
}
