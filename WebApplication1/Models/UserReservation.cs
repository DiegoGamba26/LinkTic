using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBAPI.Models
{
    [Table("UserReservations")]  // Asegúrate de que el nombre de la tabla coincida con el de la base de datos
    public class UserReservation
    {
        [Key, Column(Order = 0)]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Reservation")]
        public int ReservationID { get; set; }

        // Navegación (opcional)
        public Users User { get; set; }
        public Reservation Reservation { get; set; }
    }
}
