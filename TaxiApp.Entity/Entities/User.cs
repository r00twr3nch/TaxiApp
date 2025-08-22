using System.ComponentModel.DataAnnotations;
using TaxiApp.Core;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    [MaxLength(128)]
    public string Password { get; set; }

    [Required]
    [MaxLength(200)]
    [EmailAddress] 
    public string Email { get; set; }

    public UserRole Role { get; set; }
    public string? LicenseNumber { get; set; } 

    public DateTime CreatedAt { get; set; }
    public bool? EmailConfirmed { get; set; } = false;  
    public string? EmailVerificationToken { get; set; } 

    public string? ResetPasswordToken { get; set; }   
    public DateTime? ResetPasswordTokenExpires { get; set; }
}
