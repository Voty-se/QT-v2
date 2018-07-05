using System;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using QT.Authentication;
using QT.Helpers;
using WebGrease.Configuration;

namespace QT
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            
            //Register(GlobalConfiguration.Configuration);
            //RegisterAuth();
            //System.Threading.LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        public MvcApplication()
        {
            PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
        }

        static void MvcApplication_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            AdminAuthenticationHelper.Current.AuthenticateRequest();
        }

        //protected override void OnApplicationStarted()
        //{
        //    ViewEngines.Engines.Clear();
        //    ViewEngines.Engines.Add(new RazorViewEngine());
        //    base.OnApplicationStarted();


        //    AreaRegistration.RegisterAllAreas();

        //    RegisterGlobalFilters(GlobalFilters.Filters);
        //    RegisterRoutes(RouteTable.Routes);
        //    RegisterBundles(BundleTable.Bundles);
        //    Register(GlobalConfiguration.Configuration);
        //    RegisterAuth();
        //    System.Threading.LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        //}
    }
}
