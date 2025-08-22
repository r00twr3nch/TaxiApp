using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TaxiApp.Data.DbContext;
using System.Security.Claims;
using TaxiApp.Business.Abstract;
using TaxiApp.Business.Services;
using TaxiApp.Core.DTOs;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly TaxiAppDbContext _context;
    private readonly IUserService _userService;

    public UsersController(TaxiAppDbContext context)
    {
        _context = context;
    }

    [HttpGet("passengers")]
    public IActionResult GetPassengers()
    {
        var passengers = _context.Users
            .Where(u => u.Role == UserRole.Passenger)
            .Select(u => new { u.Id, u.Username })
            .ToList();

        return Ok(passengers);
    }

    [HttpGet("drivers")]
    public IActionResult GetDrivers()
    {
        var drivers = _context.Users
            .Where(u => u.Role == UserRole.Driver)
            .Select(u => new { u.Id, u.Username })
            .ToList();

        return Ok(drivers);
    }
    [HttpGet("driver-details/{username}")]
    public IActionResult GetDriverDetails(string username)
    {
        var driver = _context.Users
            .Where(u => u.Role == UserRole.Driver && u.Username == username)
            .Select(u => new
            {
                u.Username,
                u.Email,
                u.LicenseNumber
                //Eklenme durumunda buraya eklenecek diğer bilgiler
            })
            .FirstOrDefault();

        if (driver == null)
            return NotFound("Böyle bir sürücü bulunamadı.");

        return Ok(driver);
    }
    [HttpGet("passenger-details/{username}")]
    public IActionResult GetPassengerDetails(string username)
    {
        var passenger = _context.Users
             .Where(u => u.Role == UserRole.Passenger && u.Username == username)
            .Select(u => new
            {
                u.Username,
                u.Email,
            })
            .FirstOrDefault();
        if (passenger == null)
            return NotFound("Yolcu bulunamadı.");

        var result = new
        {
            passenger.Username,
            passenger.Email,
        };

        return Ok(result);
    }
    [HttpPut("update-driver/{driverId}")]
    public IActionResult UpdateDriver(int driverId, [FromBody] DriverUpdateDto dto)
    {
        var driver = _context.Users.FirstOrDefault(u => u.Id == driverId && u.Role == UserRole.Driver);
        if (driver == null)
            return NotFound("Sürücü bulunamadı.");

        if (!string.IsNullOrEmpty(dto.Username))
            driver.Username = dto.Username;

        if (!string.IsNullOrEmpty(dto.Email))
            driver.Email = dto.Email;

        if (!string.IsNullOrEmpty(dto.LicenseNumber))
            driver.LicenseNumber = dto.LicenseNumber;

        _context.SaveChanges();

        return Ok(new
        {
            Message = "Sürücü bilgileri başarıyla güncellendi.",
            driver.Username,
            driver.Email,
            driver.LicenseNumber
        });
    }
    [HttpPut("update-passenger/{passengerId}")]
    public IActionResult UpdatePassenger(int passengerId, [FromBody] PassengerUpdateDto dto)
    {
        var passenger = _context.Users.FirstOrDefault(u => u.Id == passengerId && u.Role == UserRole.Passenger);
        if (passenger == null)
            return NotFound("Yolcu bulunamadı.");

        if (!string.IsNullOrEmpty(dto.Username))
            passenger.Username = dto.Username;

        if (!string.IsNullOrEmpty(dto.Email))
            passenger.Email = dto.Email;

        _context.SaveChanges();

        return Ok(new
        {
            Message = "Yolcu bilgileri başarıyla güncellendi.",
            passenger.Username,
            passenger.Email
        });
    }

}
