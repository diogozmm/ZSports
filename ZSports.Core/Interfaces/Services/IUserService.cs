using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZSports.Core.ViewModel;
using ZSports.Core.ViewModel.User;

namespace ZSports.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<PostResultViewModel> RegisterAsync(RegisterViewModel viewModel);
        Task<LoginResponse> LoginAsync(LoginViewModel viewModel);
    }
}
