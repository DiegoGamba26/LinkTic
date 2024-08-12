namespace WEBAPI.DTOs
{
    public class ReservationDTO
    {
        public int CustomerId { get; set; }
        public int ServiceId { get; set; }
        public DateTime ReservationDate { get; set; }
        public string? Status { get; set; }
        public int ReservationId { get; internal set; }
    }

}
