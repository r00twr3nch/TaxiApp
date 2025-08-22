using Microsoft.EntityFrameworkCore;
using TaxiApp.Core;

namespace TaxiApp.Business.Abstract
{
    public interface IUserService
    {
        User RegisterPassenger(RegisterPassengerDto dto);
        User RegisterDriver(RegisterDriverDto dto);
        string Login(LoginDto dto);
        (bool Success, string Message) ForgotPassword(string email);
        (bool Success, string Message) ResetPassword(string token, string newPassword);
        (bool Success, string Message) VerifyEmail(string token);
        public User GetUserByEmail(string email);
        (bool Success, string Message) ConfirmEmail(string token);
        (bool Success, string Message) ChangePassword(string email, string currentPassword, string newPassword);

    }
}
