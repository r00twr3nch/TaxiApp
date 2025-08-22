using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaxiApp.Business.Services;
using TaxiApp.Data.DbContext;

[ApiController]
[Route("api/[controller]")]
public class RidesController : ControllerBase
{
    private readonly IRideService _rideService;
    private readonly TaxiAppDbContext _context;

    public RidesController(IRideService rideService, TaxiAppDbContext context)
    {
        _rideService = rideService;
        _context = context;
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestRide([FromBody] RideRequestDto dto)
    {
        var ride = await _rideService.RequestRideAsync(dto);
        return Ok(ride);
    }
    [HttpPut("complete-ride/{rideId}")]
    public IActionResult CompleteRide(int rideId)
    {
        var ride = _context.Rides.FirstOrDefault(r => r.Id == rideId);

        if (ride == null)
            return NotFound("Sürüþ bulunamadý.");

        if (ride.Status != "Requested")
            return BadRequest("Sürüþ zaten tamamlanmýþ veya geçerli deðil.");

        ride.Status = "Completed";
        _context.SaveChanges();

        return Ok(new
        {
            Message = "Sürüþ baþarýyla tamamlandý.",
            RideId = ride.Id,
            Status = ride.Status
        });
    }
    [HttpGet("driver-completed-rides/{driverId}")]
    public IActionResult GetDriverCompletedRides(int driverId)
    {
        var completedRides = _context.Rides
            .Where(r => r.DriverId == driverId && r.Status == "Completed")
            .Select(r => new
            {
                r.Id,
                r.PassengerId,
                r.TaxiTypeId,
                r.StartLatitude,
                r.StartLongitude,
                r.EndLatitude,
                r.EndLongitude,
                r.DistanceKm,
                r.TotalPrice,
                r.RequestedAt
            })
            .ToList();
        return Ok(completedRides);
    }
    [HttpGet("passenger-completed-rides/{passengerId}")]
    public IActionResult GetPassengerCompletedRides(int passengerId)
    {
        var completedRides = _context.Rides
            .Where(r => r.PassengerId == passengerId && r.Status == "Completed")
            .Select(r => new
            {
                r.Id,
                r.DriverId,
                r.TaxiTypeId,
                r.StartLatitude,
                r.StartLongitude,
                r.EndLatitude,
                r.EndLongitude,
                r.DistanceKm,
                r.TotalPrice,
                r.RequestedAt
            })
            .ToList();
        return Ok(completedRides);
    }
    [HttpGet("driver-requested-rides/{driverId}")]
    public IActionResult GetDriverRequestedRides(int driverId)
    {
        var rides = _context.Rides
            .Where(r => r.DriverId == driverId && r.Status == "Requested")
            .Select(r => new
            {
                r.Id,
                r.PassengerId,
                r.StartLatitude,
                r.StartLongitude,
                r.EndLatitude,
                r.EndLongitude,
                r.DistanceKm,
                r.TotalPrice,
                r.RequestedAt
            })
            .ToList();
        if (!rides.Any())
            return NotFound("Ýstekte sürüþ bulunamadý.");
        return Ok(rides);
    }
    [HttpGet("passenger-requested-rides/{passengerId}")]
    public IActionResult GetPassengerRequestedRides(int passengerId)
    {
        var rides = _context.Rides
            .Where(r => r.PassengerId == passengerId && r.Status == "Requested")
            .Select(r => new
            {
                r.Id,
                r.DriverId,
                r.StartLatitude,
                r.StartLongitude,
                r.EndLatitude,
                r.EndLongitude,
                r.DistanceKm,
                r.TotalPrice,
                r.RequestedAt
            })
            .ToList();
        if (!rides.Any())
            return NotFound("Requested sürüþ bulunamadý.");
        return Ok(rides);
    }
}
