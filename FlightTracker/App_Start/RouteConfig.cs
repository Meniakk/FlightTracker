using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FlightTracker
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{ip}/{port}/{time}",
                defaults: new { controller = "Home", action = "Index", ip = "127.0.0.1", port = "5400", time = UrlParameter.Optional  }
            );
        }
    }
}
