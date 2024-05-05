using PROG6212.POE.ST10153536.Models;

namespace PROG6212.POE.ST10153536.Interfaces
{
    public interface IUserService
    {
        RegistrationResult RegisterUser(Users user);
        bool AuthenticateUser(string username, string passwordhash);
        object GetUserByUsername(string username);
    }

    public class RegistrationResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
