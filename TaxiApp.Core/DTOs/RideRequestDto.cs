public class RideRequestDto
{
    public int PassengerId { get; set; }
    public int? DriverId { get; set; } 
    public int TaxiTypeId { get; set; }
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public double EndLatitude { get; set; }
    public double EndLongitude { get; set; }
    public decimal DistanceKm { get; set; }
}
