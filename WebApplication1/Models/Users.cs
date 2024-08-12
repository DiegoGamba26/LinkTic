using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace WEBAPI.Models
{
    public partial class Users
    {
        [Key]
        [Column("UserId")]
        public int UserId { get; set; }
        [StringLength(100)]
        public string UserName { get; set; } = null!;
        [StringLength(100)]
        public string UserEmail { get; set; } = null!;
        [StringLength(255)]
        public string UserPassword { get; set; } = null!;
        [StringLength(255)]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<UserReservation> UserReservations { get; set; } = new HashSet<UserReservation>();

    }
}
