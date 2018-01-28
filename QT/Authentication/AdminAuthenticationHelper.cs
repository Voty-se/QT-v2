using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;

namespace QT.Authentication
{
    public static class AdminAuthenticationHelper
    {
        public static IAdminAuthenticationHelper Current
        {
            get
            {
                return new FormsAdminAuthenticationHelper();
            }
        }

        public static VotyUserData GetUserData(User user)
        {
            return new VotyUserData()
            {
                Id = user.Id,
                Password = user.Password,
                Active = true,
                DisplayName = user.Name + " " + user.Lastname,
                Role = user.Role,
                UserName = user.Username
            };
        }

        public static User GetUserFromDatabase()
        {
            var id = Current.GetAdminUserData().Id;
            return new AdminSystemService().GetUserById(id);
        }
    }

    public interface IAdminAuthenticationHelper
    {
        bool IsAuthenticated(Role[] roles);
        bool IsAuthenticated();
        VotyUserData GetAdminUserData();
        void Logout();
        void UpdateUser(VotyUserData user);
        void Login(VotyUserData user);
        void AuthenticateRequest();
    }

    public class FormsAdminAuthenticationHelper : IAdminAuthenticationHelper
    {
        private static bool TryGetLoggedInCurrentUser(out VotyPrincipal principal)
        {
            principal = HttpContext.Current.User as VotyPrincipal;
            return principal != null && principal.Identity.IsAuthenticated;
        }

        public bool IsAuthenticated(Role[] roles)
        {
            VotyPrincipal principal;
            if (!TryGetLoggedInCurrentUser(out principal))
            {
                return false;
            }

            if (roles == null || !roles.Any())
            {
                return true;
            }

            return roles.Any(r => principal.IsInRole(r.ToString()));
        }

        public bool IsAuthenticated()
        {
            VotyPrincipal principal;
             return TryGetLoggedInCurrentUser(out principal);
        }

        public VotyUserData GetAdminUserData()
        {
            VotyPrincipal principal;
            if (!TryGetLoggedInCurrentUser(out principal))
            {
                return null;
            }

            return principal.UserData;
        }

        public User GetUser(string stringId)
        {
            var id = int.Parse(stringId);
            List<User> users;

            using (var context = new QTransportModelContainer())
            {
                users = context.UserSet.Where(u => u.Id == id).ToList();
            }
            
            return users.Any() ? users.First() : null;
        }

        public void Logout()
        {
            FormsAuthentication.SignOut();
        }

        public void UpdateUser(VotyUserData user)
        {
            Login(user);
        }

        public void Login(VotyUserData user)
        {
            var ticket = new FormsAuthenticationTicket(
                1,
                user.UserName,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                true,
                JsonConvert.SerializeObject(user),
                "/");

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
            HttpContext.Current.Response.Cookies.Add(cookie);
            HttpContext.Current.User = new VotyPrincipal(user);
            var a = HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public void AuthenticateRequest()
        {
            var httpContext = HttpContext.Current;
            var authCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null)
            {
                return;
            }

            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            if (authTicket != null)
            {
                var userData = JsonConvert.DeserializeObject<VotyUserData>(authTicket.UserData);
                httpContext.User = new VotyPrincipal(userData);
            }
            else
            {
                httpContext.User = new VotyPrincipal(null);
            }
        }
        
    }

}