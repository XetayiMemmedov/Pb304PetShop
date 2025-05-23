using System.ComponentModel.DataAnnotations;

namespace Pb304PetShop.Models
{
    public class ForgotPasswordViewModel
    {
        public required string Email { get; set; }
        //public required string FullName { get; set; }
    }
}
