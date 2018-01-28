   using System.Collections.Generic;

namespace QT.Authentication
{
    public interface IAdminSystemService
    {
        IList<User> GetAllUsers();
        IEnumerable<Role> GetAllRoles();
        User GetUserById(int id);
        int AddUser(User userData);
        void EditUser(User userData);
        void EditUserRole(Role role, int userId);
        //List<Role> GetRolesForUser(int userId);
        VotyUserData LoginUser(string userName, string password);
    }
}