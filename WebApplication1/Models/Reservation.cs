using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WEBAPI.Models;

public partial class Reservation
{
    [Key]
    [Column("ReservationID")]
    public int ReservationId { get; set; }
    [Column("CustomerID")]
    public int CustomerId { get; set; }
    [Column("ServiceID")]
    public int ServiceId { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime ReservationDate { get; set; }
    [StringLength(50)]
    public string Status { get; set; } = null!;
    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Reservations")]
    public virtual Customer Customer { get; set; } = null!;
    [ForeignKey("ServiceId")]
    [InverseProperty("Reservations")]
    public virtual Service Service { get; set; } = null!;
    public virtual ICollection<UserReservation> UserReservations { get; set; } = new HashSet<UserReservation>();

}
