using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QT
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "PasswordReset",
                url: "resetpasswordverified/{id}/{token}",
                defaults: new { controller = "Users", action = "ResetPasswordVerified", id = UrlParameter.Optional, token = UrlParameter.Optional }
            );
            routes.MapRoute(name: "loging", url: "login.aspx", defaults: new { controller = "Home", action = "Index" });
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
