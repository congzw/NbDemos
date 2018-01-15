namespace FormAuthDemo.Models
{

    public class YourAccountService
    {
        public static bool Validate(string username, string password)
        {
            if (username == "admin" && password == "123")
            {
                return true;
            }
            return false;
        }
    }
}