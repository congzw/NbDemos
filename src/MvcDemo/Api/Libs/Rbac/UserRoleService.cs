using System.Collections.Generic;

namespace MvcDemo.Api.Libs.Rbac
{
    public interface IUserRoleService
    {
        IList<UserRole> GetUserRoles(string userLoginName);
    }

    public class UserRole
    {
        public string UserLoginName { get; set; }
        public string RoleCode { get; set; }
    }

    public class UserRoleService : IUserRoleService
    {
        public IList<UserRole> GetUserRoles(string userLoginName)
        {
            var userRoles = new List<UserRole>();
            userRoles.Add(new UserRole() { UserLoginName = "admin", RoleCode = "RoleA" });
            userRoles.Add(new UserRole() { UserLoginName = "admin", RoleCode = "RoleB" });
            userRoles.Add(new UserRole() { UserLoginName = "admin", RoleCode = "RoleC" });
            return userRoles;
        }
    }
}
