using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace QT.Authentication
{
    public class AdminSystemService : IAdminSystemService
    {
        private QTransportModelContainer db = new QTransportModelContainer();

        public AdminSystemService()
        {
            
        }

        public IList<User> GetAllUsers()
        {
            return db.UserSet.ToList();
        }

        public VotyUserData LoginUser(string userName, string password)
        {
            if (!db.UserSet.Any())
                return null;

            var users = db.UserSet.Where(u => u.Username == userName && u.Password == password);
            if (!users.Any())
                return null;

            var user = users.First();
            var votyUser = new VotyUserData()
            {
                DisplayName = user.Name,
                Id = user.Id,
                //Roles = GetRolesForUser(user.Id),
                Role = user.Role,
                UserName = user.Username
            };

            return votyUser;
        }

        public User GetUserById(int id)
        {
            var user = db.UserSet.First(u => u.Id == id);
            //user.Role = GetRolesForUser(user.Id).ToList();

            return user;
        }

        //public List<Role> GetRolesForUser(int userId)
        //{
        //    var user = GetUserById(userId);
        //    var roles = user.Role.ToList();
        //    var list = roles.Select(role => (Role) Enum.Parse(typeof (Role), role)).ToList();
        //    return list;
           
        //}

        public int AddUser(User userData)
        {
            var users = GetAllUsers();

            if (users.Any(u => u.Username == userData.Username))
            {
                // User exists.
                return 0;
            }

            var i = db.UserSet.Add(userData);
            var r = db.SaveChanges();
            return r;
        }

        public void EditUser(User userData)
        {
            db.UserSet.AddOrUpdate(userData);
        }

        public void EditUserRole(Role role, int userId)
        {
            var user = db.UserSet.First(u => u.Id == userId);
            //var role =  db.RolesSet.First(r => r.Id == roleId);
            user.Role = role.ToString();
            
            db.SaveChanges();
        }
        
        public IEnumerable<Role> GetAllRoles()
        {
            const string cacheKey = "VotyCarsUserAllRoles";

            var cachedRoles = HttpRuntime.Cache.Get(cacheKey) as IEnumerable<Role>;
            if (cachedRoles != null)
            {
                return cachedRoles;
            }

            //var roles = db.RolesSet.ToList();
            //var list = roles.Select(r => (Role) Enum.Parse(typeof (Role), r.Name));
            var list = Enum.GetValues(typeof(Role)).Cast<Role>();

            HttpRuntime.Cache.Add(cacheKey, list,
            dependencies: null,
            absoluteExpiration: System.Web.Caching.Cache.NoAbsoluteExpiration,
            slidingExpiration: new TimeSpan(hours: 1, minutes: 0, seconds: 0),
            priority: CacheItemPriority.Normal,
            onRemoveCallback: null);

            return list;
        }
    }

}