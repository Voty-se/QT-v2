using System;
using System.Threading;
using System.Web;

//using log4net;

namespace QT.Authentication
{
    public static class SessionManager
    {
        //private static readonly ILog SessionCartLog = LogManager.GetLogger("CartLog");

        public static string Language => Thread.CurrentThread.CurrentCulture.Name;

        public static string CurrentDomain => HttpContext.Current.Request.Url.Scheme + 
            Uri.SchemeDelimiter + HttpContext.Current.Request.Url.Host + 
            (HttpContext.Current.Request.Url.IsDefaultPort ? "" : ":" + HttpContext.Current.Request.Url.Port);

        public static string AdminLanguage
        {
            get
            {
                var cookie = HttpContext.Current.Request.Cookies["AdminLanguage"];
                return cookie != null ? cookie.Value : Language;
            }
            set
            {
                var cookie = HttpContext.Current.Request.Cookies["AdminLanguage"];
                if (string.IsNullOrEmpty(value))
                {
                    if (cookie != null)
                    {
                        cookie.Value = "";
                        cookie.Expires = DateTime.Now.AddDays(-1D);
                        HttpContext.Current.Response.Cookies.Add(cookie);
                    }
                }
                else
                {
                    if (cookie == null)
                    {
                        cookie = new HttpCookie("AdminLanguage");
                    }
                    cookie.Value = value;
                    cookie.Expires = DateTime.Now.AddDays(20D);

                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
        }
        /// <summary>
        /// Returns country code from language as "SE" or "NO":
        /// SE
        /// </summary>
        public static string CountryCode
        {
            get
            {
                try
                {
                    return Language.Substring(3, 2).ToUpper();
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Returns language code using an underscore instead of dash
        /// </summary>
        public static string LanguageUnderscore
        {
            get { return Language.Replace("-", "_"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string Currency
        {
            get
            {
                var region = System.Globalization.RegionInfo.CurrentRegion;
                return "Sek";// new Currency(region.ISOCurrencySymbol, Language);
            }
        }

    }
}