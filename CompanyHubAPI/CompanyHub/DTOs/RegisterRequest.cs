using System.ComponentModel.DataAnnotations;

namespace CompanyHub.DTOs
{
    public class RegisterRequest
    {
        [Required,EmailAddress]
        public string Email { get; set; }= string.Empty;
        public string Username { get; set; } = string.Empty;
        [Required,DataType(DataType.Password)]
        public string Password { get; set; }= string.Empty;
        [Required,DataType(DataType.Password),Compare(nameof(Password),ErrorMessage ="Passwors do not match")]
        public string ConfirmPassword {  get; set; }= string.Empty;
        [Required]
        public string Firstname { get; set; } = string.Empty;
        [Required]
        public string Lastname { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; }= string.Empty;

    }
}
