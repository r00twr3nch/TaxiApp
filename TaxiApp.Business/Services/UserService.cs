using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxiApp.Business.Abstract;
using TaxiApp.Core;
using TaxiApp.Data.DbContext;
using System.Net;
using System.Net.Mail;

namespace TaxiApp.Business.Services
{
    public class UserService : IUserService
    {
        private readonly TaxiAppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailSettings _emailSettings;


        public UserService(TaxiAppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();

        }
        private void SendEmail(string to, string subject, string body)
        {
            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);
            client.Send(mailMessage);
        }
        private void SendVerificationEmail(User user)
        {
            var verificationLink = $"https://localhost:7053/api/auth/verify-email?token={user.EmailVerificationToken}";

            SendEmail(user.Email, "Email Doğrulama",
                $"Merhaba {user.Username},<br/>Emailinizi doğrulamak için <a href='{verificationLink}'>buraya tıklayın</a>.");
        }

        public (bool Success, string Message) VerifyEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return (false, "Bu email ile kayıtlı kullanıcı bulunamadı.");

            // Token yoksa oluştur
            if (string.IsNullOrWhiteSpace(user.EmailVerificationToken))
                user.EmailVerificationToken = Guid.NewGuid().ToString();

            _context.SaveChanges();

            var verificationLink = $"https://localhost:7053/api/auth/verify-email?token={user.EmailVerificationToken}";
            SendEmail(user.Email, "Email Doğrulama",
                $"Merhaba {user.Username},<br/>Emailinizi doğrulamak için <a href='{verificationLink}'>buraya tıklayın</a>.");

            return (true, "Doğrulama linki email adresinize gönderildi.");
        }

        public (bool Success, string Message) ConfirmEmail(string token)
        {
            var user = _context.Users.FirstOrDefault(u => u.EmailVerificationToken == token);
            if (user == null)
                return (false, "Geçersiz veya süresi dolmuş doğrulama linki.");

            user.EmailConfirmed = true;
            user.EmailVerificationToken = null;
            _context.SaveChanges();

            return (true, "Email başarıyla doğrulandı.");
        }

        public User RegisterPassenger(RegisterPassengerDto dto)
        {
            var passenger = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
                Role = UserRole.Passenger,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = false,
                EmailVerificationToken = Guid.NewGuid().ToString()
            };

            _context.Users.Add(passenger);
            _context.SaveChanges();

            SendVerificationEmail(passenger);

            return passenger;
        }


        public User RegisterDriver(RegisterDriverDto dto)
        {
            var driver = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
                Role = UserRole.Driver,
                LicenseNumber = dto.LicenseNumber,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = false,
                EmailVerificationToken = Guid.NewGuid().ToString()
            };

            _context.Users.Add(driver);
            _context.SaveChanges();

            SendVerificationEmail(driver);

            return driver;
        }



        public string Login(LoginDto dto)
        {
            var user = _context.Users
                .FirstOrDefault(u =>
                    (u.Username == dto.Username || u.Email == dto.Email) &&
                    u.Password == dto.Password);

            if (user == null)
                return null;

            var secretKey = _configuration["Jwt:secretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public (bool Success, string Message) ForgotPassword(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return (false, "Bu email adresiyle kayıtlı kullanıcı bulunamadı.");

            user.ResetPasswordToken = Guid.NewGuid().ToString();
            user.ResetPasswordTokenExpires = DateTime.UtcNow.AddHours(1);
            _context.SaveChanges();

            var resetLink = $"https://localhost:7053/api/auth/reset-password?token={user.ResetPasswordToken}";
            SendEmail(user.Email, "Şifre Sıfırlama", $"Şifrenizi sıfırlamak için <a href='{resetLink}'>buraya tıklayın</a>");

            return (true, "Şifre sıfırlama linki email adresinize gönderildi.");
        }

        public (bool Success, string Message) ResetPassword(string token, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.ResetPasswordToken == token && u.ResetPasswordTokenExpires > DateTime.UtcNow);
            if (user == null)
                return (false, "Geçersiz veya süresi dolmuş token.");

            user.Password = newPassword;
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpires = null;

            _context.SaveChanges();

            return (true, "Şifre başarıyla güncellendi.");
        }
        public (bool Success, string Message) ChangePassword(string email, string currentPassword, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return (false, "Bu email ile kullanıcı bulunamadı.");

            if (user.Password != currentPassword)
                return (false, "Mevcut şifre yanlış.");

            user.Password = newPassword;
            _context.SaveChanges();

            return (true, "Şifre başarıyla değiştirildi.");
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

    }
}
