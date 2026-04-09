using System.Net.Http.Json;
using System.Net;
using WarehouseManagement.Application.DTOs.Users;
using FluentAssertions;
using WarehouseManagement.Application.Shared;

namespace InterationTest
{
    public class UsersControllerTest(CustomWebApplicationFactory factory) : IntegrationTestBase(factory), IClassFixture<CustomWebApplicationFactory>
    {
        #region Regiester InterationTest
        [Fact]
        public async Task RegisterUser_ShouldReturnOk_WhenDataValid()
        {
            //Arrange
            await ResetDatabaseAsync();

            var request = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };

            //Act
            var response = await Client.PostAsJsonAsync("api/user/register", request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnBadRequest_WhenDataInValid()
        {
            //Arrange
            await ResetDatabaseAsync();
            
            var request = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/register", request);

            //Act
            var response = await Client.PostAsJsonAsync("api/user/register", request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnBadRequest_WhenEmailEmpty()
        {
            //Arrange
            await ResetDatabaseAsync();

            var request = new UserRegisterDto
            {
                Email = "",
                Password = "Test@123"
            };

            //Act
            var response = await Client.PostAsJsonAsync("api/user/register", request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnBadRequest_WhenPasswordEmpty()
        {
            //Arrange
            await ResetDatabaseAsync();

            var request = new UserRegisterDto
            {
                Email = "",
                Password = "Test@123"
            };

            //Act
            var response = await Client.PostAsJsonAsync("api/user/register", request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnBadRequest_WhenEmailWrongFormat()
        {
            //Arrange
            await ResetDatabaseAsync();

            var request = new UserRegisterDto
            {
                Email = "cuong123@cc.cc",
                Password = "Test@123"
            };

            //Act
            var response = await Client.PostAsJsonAsync("api/user/register", request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnBadRequest_WhenPasswordWrongFormat()
        {
            //Arrange
            await ResetDatabaseAsync();

            var request = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "123"
            };

            //Act
            var response = await Client.PostAsJsonAsync("api/user/register", request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        #endregion

        #region Login InterationTest
        [Fact]
        public async Task LoginUser_ShouldReturnOk_WhenDataValid()
        {
            //Arrange
            await ResetDatabaseAsync();
            var register = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/register", register);

            var request = new UserLoginDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };

            //Act
            var response = await Client.PostAsJsonAsync("api/user/login", request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnBadRequest_WhenEmailNotExist()
        {
            //Arrange
            await ResetDatabaseAsync();
            var register = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/register", register);

            var request = new UserLoginDto
            {
                Email = "test11@gmail.com",
                Password = "Test@123"
            };

            //Act
            var response = await Client.PostAsJsonAsync("api/user/login", request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnBadRequest_WhenPasswordInCorrect()
        {
            //Arrange
            await ResetDatabaseAsync();
            var register = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/register", register);

            var request = new UserLoginDto
            {
                Email = "test@gmail.com",
                Password = "Test@3333"
            };

            // Act
            var response = await Client.PostAsJsonAsync("api/user/login", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        #endregion

        #region Assign Role InterationTest
        [Fact]
        public async Task AssignRole_ShouldReturnOk_WhenUserIsAdmin()
        {
            //Arrange
            await ResetDatabaseAsync();
            await CreateUserAndAssignRoleAsync("admin@test.com", "Admin");
            var userId = await CreateUserAndAssignRoleAsync("user@test.com", "Staff");

            Authenticate("Admin");

            //Act
            var response = await Client.PostAsync(
                $"/api/user/assign-role/{userId}?role=Admin",
                null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AssignRole_ShouldReturnBadRequest_WhenUserNotAdmin()
        {
            //Arrange
            await ResetDatabaseAsync();
            await CreateUserAndAssignRoleAsync("admin@test.com", "Admin");
            var userId = await CreateUserAndAssignRoleAsync("user@test.com", "Staff");

            Authenticate("Staff");

            //Act
            var response = await Client.PostAsync(
                $"/api/user/assign-role/{userId}?role=Staff",
                null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AssignRole_ShouldReturnBadRequest_WhenUserNotFound()
        {
            //Arrange
            await ResetDatabaseAsync();
            var userAdmin = await CreateUserAndAssignRoleAsync("admin@test.com", "Admin");
            var userId = await CreateUserAndAssignRoleAsync("user@test.com", "Staff");

            Authenticate("Staff");

            //Act
            var response = await Client.PostAsync(
                $"/api/user/assign-role/{Guid.NewGuid}?role=Staff",
                null);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        #endregion

        #region Update User InterationTest
        [Fact]
        public async Task UpdateUser_ShouldReturnOk_WhenUserUpdatesSelf()
        {
            //Arrange
            await ResetDatabaseAsync();

            var register = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };

            var user = await Client.PostAsJsonAsync("api/user/register", register);
            var userId = await user.Content.ReadFromJsonAsync<ApiResponse<UserDto?>>();

            Authenticate("Staff");
            AuthenticateAsUser(userId!.Data!.Id);

            var dto = new UpdateUserDto
            {
                Role = "Admin",
                Email = "test1@gmail.com"
            };

            //Act
            var response = await Client.PostAsJsonAsync(
                $"/api/user/update/{userId.Data.Id}",
                dto);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto?>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.Equal(dto.Email, result?.Data?.Email);
            Assert.Equal("Staff", result?.Data?.Role);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnNotFound_WhenUserNotFound()
        {
            //Arrange
            await ResetDatabaseAsync();

            var register = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            var user = await Client.PostAsJsonAsync("api/user/register", register);

            var userId = await user.Content.ReadFromJsonAsync<ApiResponse<UserDto?>>();

            Authenticate("Staff");
            AuthenticateAsUser(userId!.Data!.Id);

            var dto = new UpdateUserDto
            {
                Role = "Admin",
                Email = "test1@gmail.com"
            };

            //Act
            var response = await Client.PostAsJsonAsync(
                $"/api/user/update/{Guid.NewGuid}",
                dto);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnForbiden_WhenUserUpdateOrtherUser()
        {
            //Arrange
            await ResetDatabaseAsync();

            var register = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            var user = await Client.PostAsJsonAsync("api/user/register", register);
            var userId = await user.Content.ReadFromJsonAsync<ApiResponse<UserDto?>>();

            var ortheruser = await CreateUserAndAssignRoleAsync("ortheruser@gmail.com", "Staff");

            Authenticate("Staff");
            AuthenticateAsUser(userId!.Data!.Id);

            var dto = new UpdateUserDto
            {
                Role = "Admin",
                Email = "test1@gmail.com"
            };

            //Act
            var response = await Client.PostAsJsonAsync(
                $"/api/user/update/{ortheruser}",
                dto);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnOk_WhenUserIsAdmin()
        {
            //Arrange
            await ResetDatabaseAsync();

            var user = await CreateUserAndAssignRoleAsync("test@gmail.com", "Admin");

            var ortheruser = await CreateUserAndAssignRoleAsync("user@gmail.com", "Staff");

            Authenticate("Admin");
            AuthenticateAsUser(user);

            var dto = new UpdateUserDto
            {
                Role = "Admin",
                Email = "test1@gmail.com"
            };

            //Act
            var response = await Client.PostAsJsonAsync(
                $"/api/user/update/{ortheruser}",
                dto);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto?>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.Equal(dto.Email, result?.Data?.Email);
            Assert.Equal(dto.Role, result?.Data?.Role);
        }
        #endregion

        #region Change Password InterationTest
        [Fact]
        public async Task ChangePassword_ShouldReturnNotFound_WhenUserNotExist()
        {
            //Arrange
            await ResetDatabaseAsync();

            var user = await CreateUserAndAssignRoleAsync("test@gmail.com", "Admin");

            var ortheruser = await CreateUserAndAssignRoleAsync("user@gmail.com", "Staff");

            Authenticate("Admin");
            AuthenticateAsUser(user);

            var dto = new ChangePasswordDto
            {
                CurrentPassword = "Test@123",
                NewPassword = "Test@333"
            };

            //Act
            var response = await Client.PostAsJsonAsync(
                $"/api/user/changepassword/{Guid.NewGuid}",
                dto);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnForbiden_WhenUserChangePasswordOrtherUser()
        {
            //Arrange
            await ResetDatabaseAsync();

            var user = await CreateUserAndAssignRoleAsync("test@gmail.com", "Staff");

            var ortheruser = await CreateUserAndAssignRoleAsync("user@gmail.com", "Staff");

            Authenticate("Staff");
            AuthenticateAsUser(user);

            var dto = new ChangePasswordDto
            {
                CurrentPassword = "Test@123",
                NewPassword = "Test@333"
            };

            //Act
            var response = await Client.PostAsJsonAsync(
                $"/api/user/changepassword/{ortheruser}",
                dto);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnBadRequest_WhenCurrentPasswordInCorrect()
        {
            //Arrange
            await ResetDatabaseAsync();

            var user = await CreateUserAndAssignRoleAsync("test@gmail.com", "Staff");

            var ortheruser = await CreateUserAndAssignRoleAsync("user@gmail.com", "Staff");

            Authenticate("Staff");
            AuthenticateAsUser(user);

            var dto = new ChangePasswordDto
            {
                CurrentPassword = "Test1",
                NewPassword = "Test@333"
            };

            //Act
            var response = await Client.PostAsJsonAsync(
                $"/api/user/changepassword/{ortheruser}",
                dto);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnOk_WhenUserIsAdmin()
        {
            //Arrange
            await ResetDatabaseAsync();

            var user = await CreateUserAndAssignRoleAsync("test@gmail.com", "Staff");

            var ortheruser = await CreateUserAndAssignRoleAsync("user@gmail.com", "Staff");

            Authenticate("Admin");
            AuthenticateAsUser(user);

            var dto = new ChangePasswordDto
            {
                CurrentPassword = "Test@123",
                NewPassword = "Test@333"
            };

            //Act
            var response = await Client.PostAsJsonAsync(
                $"/api/user/changepassword/{ortheruser}",
                dto);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        #endregion

        #region Delete User InterationTest
        [Fact]
        public async Task DeleteUser_ShouldReturnOk_WhenUserIsAdmin()
        {
            //Arrange
            await ResetDatabaseAsync();

            var user = await CreateUserAndAssignRoleAsync("test@gmail.com", "Staff");

            var ortheruser = await CreateUserAndAssignRoleAsync("user@gmail.com", "Staff");

            Authenticate("Admin");
            AuthenticateAsUser(user);

            //Act
            var response = await Client.DeleteAsync(
                $"/api/user/{ortheruser}");
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.Equal("Delete User Successfully", result?.Message);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnUnAuthorize_WhenUserDoNotLogin()
        {
            //Arrange
            await ResetDatabaseAsync();

            await CreateUserAndAssignRoleAsync("test@gmail.com", "Admin");

            var ortheruser = await CreateUserAndAssignRoleAsync("user@gmail.com", "Admin");

            Client.DefaultRequestHeaders.Authorization = null;

            //Act
            var response = await Client.DeleteAsync(
                $"/api/user/{ortheruser}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnForbidden_WhenUserIsNotAdmin()
        {
            //Arrange
            await ResetDatabaseAsync();

            var user = await CreateUserAndAssignRoleAsync("test@gmail.com", "Staff");

            var ortheruser = await CreateUserAndAssignRoleAsync("user@gmail.com", "Staff");

            Authenticate("Staff");
            AuthenticateAsUser(user);

            //Act
            var response = await Client.DeleteAsync(
                $"/api/user/{ortheruser}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        #endregion

        #region Get Users By Id InterationTest
        [Fact]
        public async Task GetUserById_ShouldReturnNotFound_WhenUserIdNotExist()
        {
            //Arrange
            await ResetDatabaseAsync();

            var user = await CreateUserAndAssignRoleAsync("test@gmail.com", "Staff");

            var ortheruser = await CreateUserAndAssignRoleAsync("user@gmail.com", "Staff");

            Authenticate("Staff");
            AuthenticateAsUser(user);

            //Act
            var response = await Client.GetAsync(
                $"/api/user/{Guid.NewGuid}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnOke_WhenUserIsAdmin()
        {
            //Arrange
            await ResetDatabaseAsync();

            await CreateUserAndAssignRoleAsync("test@gmail.com", "Admin");

            var ortheruser = await CreateUserAndAssignRoleAsync("user@gmail.com", "Staff");

            Authenticate("Admin");

            //Act
            var response = await Client.GetAsync(
                $"/api/user/{ortheruser}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUnthorize_WhenUserIsNotAdmin()
        {
            //Arrange
            await ResetDatabaseAsync();

            await CreateUserAndAssignRoleAsync("test@gmail.com", "Staff");

            var ortheruser = await CreateUserAndAssignRoleAsync("user@gmail.com", "Staff");

            Authenticate("Staff");

            //Act
            var response = await Client.GetAsync(
                $"/api/user/{ortheruser}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnOke_WhenUserIsNotAdmin()
        {
            //  Arrange
            await ResetDatabaseAsync();

            var user = await CreateUserAndAssignRoleAsync("test@gmail.com", "Staff");

            AuthenticateAsUser(user);
            Authenticate("Staff");

            // Act
            var response = await Client.GetAsync(
                $"/api/user/{user}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        #endregion

        #region Get All Users InterationTest
        [Fact]
        public async Task GetAllUser_ShouldReturnOke_WhenUserIsAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();

            var user = await CreateUserAndAssignRoleAsync("test@gmail.com", "Admin");

            await CreateUserAndAssignRoleAsync("test@gmail1.com", "Staff");
            await CreateUserAndAssignRoleAsync("test@gmail2.com", "Staff");

            AuthenticateAsUser(user);
            Authenticate("Admin");

            // Act
            var response = await Client.GetAsync(
                $"/api/user");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAllUser_ShouldReturnListEmpty_WhenNoUserExist()
        {
            // Arrange
            await ResetDatabaseAsync();

            Authenticate("Admin");

            // Act
            var response = await Client.GetAsync(
                $"/api/user");
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<UserDto?>>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.NotNull(result);
            result!.Data!.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllUser_ShouldReturnForbidden_WhenUserIsNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();

            await CreateUserAndAssignRoleAsync("test@gmail.com", "Staff");

            await CreateUserAndAssignRoleAsync("test@gmail1.com", "Staff");
            await CreateUserAndAssignRoleAsync("test@gmail2.com", "Staff");

            Authenticate("Staff");

            // Act
            var response = await Client.GetAsync(
                $"/api/user");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetAllUser_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();

            await CreateUserAndAssignRoleAsync("test@gmail.com", "Staff");

            await CreateUserAndAssignRoleAsync("test@gmail1.com", "Staff");
            await CreateUserAndAssignRoleAsync("test@gmail2.com", "Staff");

            // Act
            var response = await Client.GetAsync(
                $"/api/user");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        #endregion

        #region Refresh Token InterationTest
        [Fact]
        public async Task RefreshToken_ShouldReturnNewAccessToken_WhenRefreshTokenIsValid()
        {
            // Arrange
            await ResetDatabaseAsync();
            var user = await CreateUserAndAssignRoleAsync("test@gmail.com", "Staff");

            var login = new UserLoginDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };

            Authenticate("Staff");
            AuthenticateAsUser(user);

            var loginResponseMessage = await Client.PostAsJsonAsync("/api/user/login", login);

            var loginResponse = await loginResponseMessage.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();

            var dto = new RefreshTokenRequestDto
            {
                UserId = loginResponse!.Data!.Id,
                RefreshToken = loginResponse.Data.RefreshToken
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/user/refresh-token", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewAccessToken_WhenRefreshTokenInvalid()
        {
            // Arrange
            await ResetDatabaseAsync();
            var user = await CreateUserAndAssignRoleAsync("test@gmail.com", "Staff");

            Authenticate("Staff");
            AuthenticateAsUser(user);

            var dto = new RefreshTokenRequestDto
            {
                UserId = Guid.NewGuid(),
                RefreshToken = "invalid_refresh_token"
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/user/refresh-token", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        #endregion
    }
}
