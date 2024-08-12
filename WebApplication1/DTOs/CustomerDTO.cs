using System.ComponentModel.DataAnnotations;

namespace WEBAPI.DTOs
{
    public class CustomerDTO
    {
        public string FirstName { get; set; } = null!;
        [StringLength(100)]
        public string LastName { get; set; } = null!;
        [StringLength(255)]
        public string Email { get; set; } = null!;
        [StringLength(20)]
        public string? Phone { get; set; }
    }
}


