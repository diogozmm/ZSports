using Xunit;
using Moq;
using System.Threading.Tasks;
using ZSports.Core.Interfaces.Repositories;
using ZSports.Core.Interfaces.Services;
using ZSports.Core.ViewModel;
using ZSports.Core.ViewModel.User;
using ZSports.Keycloak.Client;
using ZSports.Keycloak.Request;
using AutoMapper;
using ZSports.Domain.User;
using ZSports.Keycloak.Response;

namespace ZSports.Services.Test
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IKeycloakClient> _keycloakClientMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _keycloakClientMock = new Mock<IKeycloakClient>();
            _mapperMock = new Mock<IMapper>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _keycloakClientMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_SuccessfulRegistration_AddsUserToRepository()
        {
            // Arrange
            var viewModel = new RegisterViewModel { Email = "newuser@example.com", Password = "password" };
            var keycloakUser = new KeycloakUser { Id = Guid.NewGuid().ToString() };
            var user = new User { KeycloakId = Guid.Parse(keycloakUser.Id), Email = viewModel.Email };
            var userViewModel = new UserViewModel { Email = viewModel.Email };

            _keycloakClientMock
                .SetupSequence(k => k.GetUserByEmailAsync(viewModel.Email))
                .ReturnsAsync((KeycloakUser)null!)  
                .ReturnsAsync(keycloakUser);        

            // Setup registration
            _keycloakClientMock
                .Setup(k => k.RegisterUserAsync(It.Is<KeycloakRegisterUserRequest>(r =>
                    r.Email == viewModel.Email &&
                    r.Username == viewModel.Email &&
                    r.Password == viewModel.Password)))
                .ReturnsAsync(true)
                .Verifiable();

            // Setup mapper
            _mapperMock
                .Setup(m => m.Map<User>(viewModel))
                .Returns(user)
                .Verifiable();

            _mapperMock
                .Setup(m => m.Map<UserViewModel>(user))
                .Returns(userViewModel)
                .Verifiable();

            // Setup repository
            _userRepositoryMock
                .Setup(u => u.AddAndSaveChangesAsync(It.Is<User>(u =>
                    u.KeycloakId == Guid.Parse(keycloakUser.Id) &&
                    u.Email == viewModel.Email)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _userService.RegisterAsync(viewModel);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Model);
            Assert.IsType<UserViewModel>(result.Model);

            _keycloakClientMock.Verify();
            _mapperMock.Verify();
            _userRepositoryMock.Verify(
                r => r.AddAndSaveChangesAsync(It.Is<User>(u =>
                    u.KeycloakId == Guid.Parse(keycloakUser.Id) &&
                    u.Email == viewModel.Email)),
                Times.Once
            );
        }

        [Fact]
        public async Task RegisterAsync_UserAlreadyExists_ReturnsUserAlreadyExistsMessage()
        {
            // Arrange
            var viewModel = new RegisterViewModel { Email = "test@example.com", Password = "password" };

            _keycloakClientMock
                .Setup(k => k.GetUserByEmailAsync(viewModel.Email))
                .ReturnsAsync(new KeycloakUser { Id = "existing-user-id" });

            // Act
            var result = await _userService.RegisterAsync(viewModel);

            // Assert
            Assert.Equal("User already exists", result.Error);
            _keycloakClientMock.Verify(k => k.RegisterUserAsync(It.IsAny<KeycloakRegisterUserRequest>()), Times.Never);
            _userRepositoryMock.Verify(r => r.AddAndSaveChangesAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsTokenResponse()
        {
            // Arrange
            var viewModel = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var keycloakRequest = new KeycloakLoginUserRequest
            {
                Username = viewModel.Email,
                Password = viewModel.Password
            };

            var keycloakResponse = new KeycloakAccessTokenResponse
            {
                AccessToken = "valid-access-token",
            };

            _mapperMock
                .Setup(m => m.Map<KeycloakLoginUserRequest>(viewModel))
                .Returns(keycloakRequest)
                .Verifiable();

            _keycloakClientMock
                .Setup(k => k.LoginUserAsync(It.Is<KeycloakLoginUserRequest>(r =>
                    r.Username == viewModel.Email &&
                    r.Password == viewModel.Password)))
                .ReturnsAsync(keycloakResponse)
                .Verifiable();

            // Act
            var result = await _userService.LoginAsync(viewModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(keycloakResponse.AccessToken, result.Token);
            _mapperMock.Verify();
            _keycloakClientMock.Verify();
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var viewModel = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            var keycloakRequest = new KeycloakLoginUserRequest
            {
                Username = viewModel.Email,
                Password = viewModel.Password
            };

            _mapperMock
                .Setup(m => m.Map<KeycloakLoginUserRequest>(viewModel))
                .Returns(keycloakRequest)
                .Verifiable();

            _keycloakClientMock
                .Setup(k => k.LoginUserAsync(It.Is<KeycloakLoginUserRequest>(r =>
                    r.Username == viewModel.Email &&
                    r.Password == viewModel.Password)))
                .ReturnsAsync((KeycloakAccessTokenResponse)null!)
                .Verifiable();

            // Act
            var result = await _userService.LoginAsync(viewModel);

            // Assert
            Assert.Null(result);
            _mapperMock.Verify();
            _keycloakClientMock.Verify();
        }

        [Theory]
        [InlineData(null, "password123")]
        [InlineData("", "password123")]
        [InlineData("test@example.com", null)]
        [InlineData("test@example.com", "")]
        public async Task LoginAsync_InvalidInput_ThrowsArgumentException(string email, string password)
        {
            // Arrange
            var viewModel = new LoginViewModel
            {
                Email = email,
                Password = password
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.LoginAsync(viewModel));
        }

        [Fact]
        public async Task LoginAsync_KeycloakClientThrowsException_PropagatesException()
        {
            // Arrange
            var viewModel = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var keycloakRequest = new KeycloakLoginUserRequest
            {
                Username = viewModel.Email,
                Password = viewModel.Password
            };

            _mapperMock
                .Setup(m => m.Map<KeycloakLoginUserRequest>(viewModel))
                .Returns(keycloakRequest);

            _keycloakClientMock
                .Setup(k => k.LoginUserAsync(It.IsAny<KeycloakLoginUserRequest>()))
                .ThrowsAsync(new HttpRequestException("Keycloak service unavailable"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                _userService.LoginAsync(viewModel));
        }

    }
}