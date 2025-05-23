using System.ComponentModel.DataAnnotations;

namespace Pb304PetShop.Models
{
    public class LoginViewModel
    {
        public required string UserName { get; set; }
        [DataType(DataType.Password)]
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
        public string? ReturnUrl {  get; set; }
    }
}
