using ZSports.Core.Enums;

namespace ZSports.Core.ViewModel.User
{
    public class UserViewModel
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public RolesEnum Role { get; set; }
    }
}
