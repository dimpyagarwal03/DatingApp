using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class UserForRegisterDTo
    {
        [Required]
        public string username { get; set; }

        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="You must specify password between 4 & 8 characters.")]
         public string password { get; set; }
    }
}