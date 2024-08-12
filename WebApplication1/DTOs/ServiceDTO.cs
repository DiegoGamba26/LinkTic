using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBAPI.DTOs
{
    public class ServiceDTO
    {
        public string ServiceName { get; set; } = null!;
        [StringLength(500)]
        public string? Description { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
        public int Duration { get; set; }
    }
}

