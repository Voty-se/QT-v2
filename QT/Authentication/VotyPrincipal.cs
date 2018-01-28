using System.Security.Principal;

namespace QT.Authentication
{
    public class VotyPrincipal : IPrincipal, IIdentity
    {
        public VotyPrincipal(VotyUserData userData)
        {
            UserData = userData;
        }

        public bool IsInRole(string role)
        {
            return UserData.Role == role;
            //if (!UserData.Roles.Any())
            //    return false;
            //return UserData.Roles != null && UserData.Roles.Any(r => string.Equals(r.ToString(), role));
        }

        public IIdentity Identity
        {
            get { return this; }
        }

        public VotyUserData UserData { get; private set; }

        public string Name
        {
            get { return UserData == null ? "" : UserData.UserName; }
        }

        public string AuthenticationType
        {
            get { return "Voty"; }
        }

        public bool IsAuthenticated
        {
            get { return UserData != null; }
        }
    }
}