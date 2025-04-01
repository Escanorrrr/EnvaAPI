using System.ComponentModel.DataAnnotations;

namespace EnvaTest.DTO
{
    public class ChangePasswordDTO
    {
        [Required]
        [StringLength(100)]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100)]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(100)]
        [Compare("NewPassword", ErrorMessage = "Yeni şifre ve şifre tekrarı eşleşmiyor")]
        public string ConfirmNewPassword { get; set; }
    }
} 