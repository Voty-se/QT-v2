using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QT.Authentication
{
    public class VotyAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly Role[] _acceptedRoles;

        public VotyAuthorizeAttribute()
        {
            
        }

        public VotyAuthorizeAttribute(params Role[] acceptedRoles)
        {
            _acceptedRoles = acceptedRoles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            ////Do an IP-check first to determine if user is on one of the correct IP-ranges
            //if (!IPRangeValidator.Current.IPRangeCheck(httpContext.Request.GetForwardedIPAdress()))
            //{
            //    // Invalid IP, dont redirect to loginpage.
            //    return false;
            //}

            if (AdminAuthenticationHelper.Current.IsAuthenticated(new[] { Role.Administrator }))
            {
                // Administrators is always allowed everywhere.
                return true;
            }

            if (AdminAuthenticationHelper.Current.IsAuthenticated(_acceptedRoles))
            {
                return true;
            }

            // Set redirect
            SetResponseRedirect(httpContext);
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Returns HTTP 401 by default - see HttpUnauthorizedResult.cs.
            filterContext.Result = new RedirectToRouteResult(
            new RouteValueDictionary
            {
                { "action", "GoToRoot" },
                { "controller", "Admin" }
            });
        }

        private static void SetResponseRedirect(HttpContextBase httpContext)
        {
            // If not from bygghemma.se
            //var ip = httpContext.Request.GetForwardedIPAdress();
            //if (!IPRangeValidator.Current.IPRangeCheck(ip))
            //{
            //    httpContext.Response.StatusCode = 404;
            //}
            //else
            //{
                // Redirect home if admin controller
                switch (httpContext.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower())
                {
                    case "admin":
                        httpContext.Response.Redirect("/admin/facebooklogin/?returnUrl=" + httpContext.Request.RawUrl);
                        break;
                    case "rotinvoice":
                        httpContext.Response.Redirect("/admin/facebooklogin/?returnUrl=" + httpContext.Request.RawUrl);
                        break;
                    default:
                        httpContext.Response.Redirect(httpContext.Request.UrlReferrer != null ? httpContext.Request.UrlReferrer.ToString() : "/");
                        break;
                }
            //}
        }
    }
}