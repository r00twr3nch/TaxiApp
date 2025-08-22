using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaxiApp.Business.Abstract;
using TaxiApp.Business.Services;
using TaxiApp.Core.DTOs;

namespace TaxiApp.API.Controllers
{ 
        [ApiController]
        [Route("api/[controller]")]
        public class AuthController : ControllerBase
        {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register-passenger")]
            public IActionResult RegisterPassenger([FromBody] RegisterPassengerDto dto)
            {
                var user = _userService.RegisterPassenger(dto);
                return Ok(new { Message = "Yolcu başarıyla kaydedildi. Emailinizi doğrulayın.", UserId = user.Id });
            }

            [HttpPost("register-driver")]
            public IActionResult RegisterDriver([FromBody] RegisterDriverDto dto)
            {
                var user = _userService.RegisterDriver(dto);
                return Ok(new { Message = "Şoför başarıyla kaydedildi. Emailinizi doğrulayın.", UserId = user.Id });
            }
        [HttpPost("verify-email")]
        public IActionResult VerifyEmail([FromBody] GetVerificationTokenDto dto)
        {
            var result = _userService.VerifyEmail(dto.Email);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { Message = result.Message });
        }
        [HttpGet("verify-email-link")]
        public IActionResult ConfirmEmail([FromQuery] string token)
        {
            var result = _userService.ConfirmEmail(token);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { Message = result.Message });
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Şifre boş olamaz.");

            if (string.IsNullOrWhiteSpace(dto.Username) && string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Kullanıcı adı veya e-posta girilmelidir.");

            var user = _userService.Login(dto);

            if (user == null)
                return Unauthorized("Kullanıcı adı/e-posta veya şifre yanlış.");

            return Ok(new { Message = "Giriş başarılı", User = user });
        }
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = _userService.ForgotPassword(dto.Email);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { Message = "Şifre sıfırlama linki email adresinize gönderildi." });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = _userService.ResetPassword(dto.Token, dto.NewPassword);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { Message = "Şifreniz başarıyla güncellendi." });
        }
        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email)
                || string.IsNullOrWhiteSpace(dto.CurrentPassword)
                || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("Email, mevcut şifre ve yeni şifre girilmelidir.");

            var result = _userService.ChangePassword(dto.Email, dto.CurrentPassword, dto.NewPassword);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { Message = result.Message });
        }

    }

}
