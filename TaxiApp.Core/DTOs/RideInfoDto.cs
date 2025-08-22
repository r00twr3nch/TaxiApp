public class RideInfoDto
{
    public int RideId { get; set; }
    public string PassengerName { get; set; }
    public string DriverName { get; set; }
    public DateTime Date { get; set; }
    public string PickupLocation { get; set; }
    public string DropoffLocation { get; set; }
    public decimal Fare { get; set; }
}