using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Pb304PetShop.Models
{
    public class RegisterViewModel
    {
        public required string UserName { get; set; }
        public required string FullName { get; set; }
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }
        [DataType(DataType.Password)]
        public required string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }
        public string? OldPassword { get; set; }

    }
}
