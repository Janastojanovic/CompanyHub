using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CompanyHub.Models
{
    public class AppUser:IdentityUser
    {
        [Required]
        public string Firstname { get; set; } = string.Empty;
        [Required]
        public string Lastname { get; set; } = string.Empty;
        [Required]
        public string Phone {  get; set; }=string.Empty;
        public bool Online { get; set; } = false;
        public bool Approved { get; set; } = false;

    }
}
