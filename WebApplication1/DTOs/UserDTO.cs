using System;
using System.ComponentModel.DataAnnotations;

namespace WEBAPI.DTOs
{
    public class UserDTO
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string UserEmail { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string UserPassword { get; set; } = null!;
    }
}
