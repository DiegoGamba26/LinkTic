using System.ComponentModel.DataAnnotations;

namespace WEBAPI.DTOs
{
    public class UserLoginDTO
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }
}


