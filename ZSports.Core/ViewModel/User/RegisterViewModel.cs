using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSports.Core.Enums;

namespace ZSports.Core.ViewModel.User
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        
        [Required]
        public string Name { get; set; } = default!;

        [MinLength(6)]
        [MaxLength(255)]
        [Required]
        public string Password { get; set; } = default!;
    }
}
