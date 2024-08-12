using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace WEBAPI.Models
{
    public partial class Service
    {
        public Service()
        {
            Reservations = new HashSet<Reservation>();
        }

        [Key]
        [Column("ServiceID")]
        public int ServiceId { get; set; }
        [StringLength(100)]
        public string ServiceName { get; set; } = null!;
        [StringLength(500)]
        public string? Description { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
        public int Duration { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore] // Ignora la propiedad en la serialización JSON

        [InverseProperty("Service")]
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
