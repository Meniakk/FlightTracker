﻿using System;
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
                url: "{action}/{ip}/{port}/{time}/{period}/{filename}",
                defaults: new { controller = "Home", action = "Index", ip = "127.0.0.1", port = "5402", time = "0", period = "0", filename = "" }
            );
        }
    }
}
