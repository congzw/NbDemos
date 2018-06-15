namespace MvcDemo.Api.Libs.Users
{
    public interface IUserService
    {
        UserDto GetUser(int userId);
    }

    public class UserService : IUserService
    {
        public UserDto GetUser(int userId)
        {
            return new UserDto() { Id = 1, LoginName = "admin", FullName = "[admin]" };
        }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string LoginName { get; set; }
        public string FullName { get; set; }
    }
}
