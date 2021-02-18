using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PI_Portal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Volume",
                url: "Contract/Volume/{idvol}",
                defaults: new { controller = "Contract", action = "Volume", idvol = UrlParameter.Optional }
            );
            routes.MapRoute(
              name: "Location",
              url: "Contract/Location/{idloc}",
              defaults: new { controller = "Contract", action = "Location", idloc = UrlParameter.Optional }
            );
            routes.MapRoute(
              name: "Service",
              url: "Contract/Service/{idsvc}",
              defaults: new { controller = "Contract", action = "Service", idsvc = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
           

        }
    }
}
