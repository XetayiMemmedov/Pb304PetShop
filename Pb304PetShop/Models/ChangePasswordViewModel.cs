using System.ComponentModel.DataAnnotations;

namespace Pb304PetShop.Models
{
    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]

        public required string OldPassword { get; set; }
        [DataType(DataType.Password)]

        public required string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmNewPassword { get; set; }
    }
}
