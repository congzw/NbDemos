using System;
using System.Collections.Generic;
using MvcDemo.Api.Libs;
using MvcDemo.Api.Libs.Rbac;
using MvcDemo.Api.Libs.Users;

namespace MvcDemo.Api.AppServices
{
    public interface IUserAppService
    {
        dynamic GetUserDynamic(int userId);
        UserVo GetUser(int userId);
        DictionaryVo GetUserDic(int userId);
    }

    public class UserVo
    {
        public UserVo()
        {
            UserRoles = new List<UserRole>();
        }
        public UserDto User { get; set; }
        public IList<UserRole> UserRoles { get; set; }
    }

    public class DictionaryVo : Dictionary<string, object>
    {
    }

    public class UserAppService : IUserAppService
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;

        public UserAppService(IUserService userService, IUserRoleService userRoleService)
        {
            _userService = userService;
            _userRoleService = userRoleService;
        }

        public dynamic GetUserDynamic(int userId)
        {
            var userDto = _userService.GetUser(userId);
            var userRoles = _userRoleService.GetUserRoles(userDto.LoginName);


            // Creating a dynamic dictionary.
            dynamic result = new DynamicDictionary();

            // Adding new dynamic properties. 
            // The TrySetMember method is called.
            result.User = userDto;
            result.UserRoles = userRoles;

            return result;

            //// Getting values of the dynamic properties.
            //// The TryGetMember method is called.
            //// Note that property names are case-insensitive.
            //Console.WriteLine(person.firstname + " " + person.lastname);

            //// Getting the value of the Count property.
            //// The TryGetMember is not called, 
            //// because the property is defined in the class.
            //Console.WriteLine("Number of dynamic properties:" + person.Count);

            //// The following statement throws an exception at run time.
            //// There is no "address" property,
            //// so the TryGetMember method returns false and this causes a
            //// RuntimeBinderException.
            //// Console.WriteLine(person.address);
        }

        public UserVo GetUser(int userId)
        {
            var userDto = _userService.GetUser(userId);
            var userRoles = _userRoleService.GetUserRoles(userDto.LoginName);
            return new UserVo(){User = userDto, UserRoles = userRoles};
        }

        public DictionaryVo GetUserDic(int userId)
        {
            var userDto = _userService.GetUser(userId);
            var userRoles = _userRoleService.GetUserRoles(userDto.LoginName);
            var dictionaryVo = new DictionaryVo();
            dictionaryVo["User"] = userDto;
            dictionaryVo["UserRoles"] = userRoles;
            return dictionaryVo;
        }
    }

    public class MyFactory
    {
        public static IUserAppService CreateUserAppService()
        {
            return new UserAppService(new UserService(), new UserRoleService());
        }
    }
}
