using AutoMapper;
using Microsoft.Extensions.Configuration;
using ZSports.Core.Enums;
using ZSports.Core.Interfaces.Repositories;
using ZSports.Core.Interfaces.Services;
using ZSports.Core.ViewModel;
using ZSports.Core.ViewModel.User;
using ZSports.Domain.User;
using ZSports.Keycloak.Client;
using ZSports.Keycloak.Request;
using ZSports.Services.Helpers;

namespace ZSports.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IKeycloakClient _keycloakClient;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository,
            IKeycloakClient keycloakClient,
            IMapper mapper) 
        {
            _userRepository = userRepository;
            _keycloakClient = keycloakClient;
            _mapper = mapper;
        }

        public async Task<PostResultViewModel> RegisterAsync(RegisterViewModel viewModel)
        {
            var existingUser = await _keycloakClient.GetUserByEmailAsync(viewModel.Email);
            if (existingUser != null)
                return new PostResultViewModel("User already exists");

            var user = _mapper.Map<KeycloakRegisterUserRequest>(viewModel);

            var registrationResult = await _keycloakClient.RegisterUserAsync(new KeycloakRegisterUserRequest
            {
                Email = viewModel.Email,
                Username = viewModel.Email,
                Password = viewModel.Password
            });

            if (!registrationResult)
                return new PostResultViewModel("Failed to register user in Keycloak");

            var model = _mapper.Map<User>(viewModel);

            existingUser = await _keycloakClient.GetUserByEmailAsync(model.Email);

            if (existingUser != null)
                model.KeycloakId = Guid.Parse(existingUser.Id);

            await _userRepository.AddAndSaveChangesAsync(model);

            var userViewModel = _mapper.Map<UserViewModel>(model);

            return new PostResultViewModel(userViewModel);
        }


        public async Task<LoginResponse> LoginAsync(LoginViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            if (string.IsNullOrEmpty(viewModel.Email))
                throw new ArgumentException("Email cannot be null or empty", nameof(viewModel));

            if (string.IsNullOrEmpty(viewModel.Password))
                throw new ArgumentException("Password cannot be null or empty", nameof(viewModel));

            var request = _mapper.Map<KeycloakLoginUserRequest>(viewModel);
            var token = await _keycloakClient.LoginUserAsync(request);

            if (token != null)
                return new LoginResponse { Token = token.AccessToken };

            return null!;
        }
    }
}
