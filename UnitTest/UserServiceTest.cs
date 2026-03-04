using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Users;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Interfaces;
using Xunit.Sdk;
using static WarehouseManagement.Domain.Common.DomainException;

namespace UnitTest
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IJwtService> _jwtServiceMock = new();
        private readonly Mock<ICurrentUserService> _currentUserService = new();

        private readonly UserService _userService;

        public UserServiceTest()
        {
            _userService = new UserService(
                _unitOfWorkMock.Object,
                _jwtServiceMock.Object,
                _passwordHasherMock.Object,
                _mapperMock.Object);
        }

        #region Register User Tests
        [Fact]
        public async Task RegisterUserAsync_ShouldRegisterUserSuccessfully()
        {
            //Arrange
            var dto = new UserRegisterDto { Email = "test@gmail.com", Password = "123546" };

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
            _passwordHasherMock.Setup(p => p.Hash(dto.Password)).Returns("hashedPassword");

            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Email = dto.Email });
            //Act
            var result = await _userService.RegisterAsync(dto);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Email, result.Email);
        }

        [Fact]
        public async Task RegisterUser_ShouldRegisterUserFail_WhenEmailExist()
        {
            //Arrange
            var dto = new UserRegisterDto { Email = "test1@cc.cc", Password = "123546" };

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync(new User(dto.Email, "hashedPassword"));

            //Act & Assert
            await Assert.ThrowsAsync<ConflictException>(async () => await _userService.RegisterAsync(dto));
        }
        #endregion

        #region Login User Tests
        [Fact]
        public async Task LoginUser_ShouldRegisterUserSuccessfully_WhenEmailAndPassWordCorrect()
        {
            //Arrange
            var dto = new UserRegisterDto { Email = "test1@cc.cc", Password = "123546" };
            var user = new User(dto.Email, "123546");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(p => p.Verify(dto.Password, user.Password)).Returns(true);
            _jwtServiceMock.Setup(j => j.GenerateToken(user.Id, user.Email, user.Role)).Returns(("accessToken", DateTime.UtcNow.AddMinutes(1)));

            //Act
            var result = await _userService.LoginAsync(new UserLoginDto { Email = dto.Email, Password = dto.Password });

            //Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Email, result.Email);
        }

        [Fact]
        public async Task LoginUser_ShouldRegisterUserFail_WhenEmailInCorrect()
        {
            //Arrange
            var dto = new UserRegisterDto { Email = "test1@cc.cc", Password = "123546" };
            var user = new User("test@gmail.com", "hashedPassword");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
            _passwordHasherMock.Setup(p => p.Verify(dto.Password, user.Password)).Returns(true);

            //Assert&Act
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.LoginAsync(new UserLoginDto { Email = dto.Email, Password = dto.Password }));
        }

        [Fact]
        public async Task LoginUser_ShouldRegisterUserFail_WhenPasswordInCorrect()
        {
            //Arrange
            var dto = new UserRegisterDto { Email = "test1@cc.cc", Password = "123546" };
            var user = new User("test@gmail.com", "hashedPassword");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync(user);
            _passwordHasherMock.Setup(p => p.Verify(dto.Password, user.Password)).Returns(false);

            //Assert&Act
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LoginAsync(new UserLoginDto { Email = dto.Email, Password = dto.Password }));
        }

        [Fact]
        public async Task LoginUser_ShouldRegisterUserFail_WhenEmailNotFound()
        {
            //Arrange
            var dto = new UserRegisterDto { Email = "test1@cc.cc", Password = "123546" };
            var user = new User("test@gmail.com", "123546");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
            _passwordHasherMock.Setup(p => p.Verify(dto.Password, user.Password)).Returns(true);

            //Assert&Act
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.LoginAsync(new UserLoginDto { Email = dto.Email, Password = dto.Password }));
        }
        #endregion

        #region Get User By Id Tests
        [Fact]
        public async Task GetUserById_ShouldReturnFail_WhenEmailNotExist()
        {
            //Arrange
            var dto = new UserRegisterDto { Email = "test1@cc.com", Password = "123546" };
            var user = new User("test@gmail.com", "123546");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetUserByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
            _passwordHasherMock.Setup(p => p.Verify(dto.Password, user.Password)).Returns(true);
            _currentUserService.Setup(c => c.UserId).Returns(user.Id);

            //Assert&Act
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.GetUserByIdAsync(user.Id, _currentUserService.Object));
        }

        [Fact]
        public async Task GetUserById_ShouldReturnSucces_WhenUser_Accesses_Themselves()
        {
            //Arrange
            var dto = new UserLoginDto { Email = "test@gmail.com", Password = "123546" };
            var user = new User("test@gmail.com", "123546");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _currentUserService.Setup(c => c.UserId).Returns(user.Id);
            _currentUserService.Setup(c => c.Roles).Returns("Staff");
            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Email = dto.Email, Id = user.Id, Role = "Admin" });

            //Act
            var result = await _userService.GetUserByIdAsync(user.Id, _currentUserService.Object);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result!.Email);
            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnSucces_WhenUser_Accesses_AnotherUser()
        {
            //Arrange
            var dto = new UserLoginDto { Email = "test1@gmail.com", Password = "123546" };
            var user = new User("test@gmail.com", "123546");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _currentUserService.Setup(c => c.UserId).Returns(Guid.NewGuid);
            _currentUserService.Setup(c => c.Roles).Returns("Staff");

            //Assert&Act
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.GetUserByIdAsync(user.Id, _currentUserService.Object));
        }

        [Fact]
        public async Task GetUserById_ShouldReturnSucces_WhenAdmin_Accesses_AnotherUser()
        {
            //Arrange
            var dto = new UserLoginDto { Email = "admid@system.com", Password = "123546" };
            var user = new User("test@gmail.com", "123546");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _currentUserService.Setup(c => c.UserId).Returns(Guid.NewGuid());
            _currentUserService.Setup(c => c.Roles).Returns("Admin");
            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Email = user.Email, Id = user.Id, Role = "Admin" });

            //Act
            var result = await _userService.GetUserByIdAsync(user.Id, _currentUserService.Object);

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Email);
            Assert.Equal(user.Id, result.Id);
        }
        #endregion

        #region Get All Users Tests
        [Fact]
        public async Task GetAllUser_ShouldReturnSuccess_WhenUsersExists()
        {
            //Arrange
            var dto = new UserLoginDto { Email = "admid@system.com", Password = "123546" };

            var user = new User("test@gmail.com", "123546");
            var user2 = new User("test2@gmail.com", "123456");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync([user, user2]);
            _currentUserService.Setup(c => c.UserId).Returns(Guid.NewGuid());
            _currentUserService.Setup(c => c.Roles).Returns("Admin");
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { Email = user.Email, Id = user.Id, Role = "Staff" });
            _mapperMock.Setup(m => m.Map<UserDto>(user2)).Returns(new UserDto { Email = user2.Email, Id = user.Id, Role = "Staff" });

            //Act
            var result = await _userService.GetAllUsersAsync();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllUser_ShouldReturnEmptyList_WhenNoUser()
        {
            //Arrange
            var dto = new UserLoginDto { Email = "admid@system.com", Password = "123546" };

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
            _currentUserService.Setup(c => c.UserId).Returns(Guid.NewGuid());
            _currentUserService.Setup(c => c.Roles).Returns("Admin");

            //Act
            var results = await _userService.GetAllUsersAsync();

            //Assert
            Assert.NotNull(results);
            Assert.Empty(results!);
        }
        #endregion

        #region Update User Tests
        [Fact]
        public async Task UpdateUser_ShouldReturnSuccess_WhenUserIsAdmin()
        {
            //Arrange
            var dto = new UserLoginDto { Email = "admid@system.com", Password = "123546" };
            var updateuser = new UpdateUserDto { Role = "Staff", Email = "test1@gmail.com" };

            var user = new User("test@gmail.com", "123456");


            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _currentUserService.Setup(c => c.UserId).Returns(user.Id);
            _currentUserService.Setup(c => c.Roles).Returns("Admin");
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { Email = updateuser.Email, Id = user.Id, Role = updateuser.Role });

            //Act
            var results = await _userService.UpdateUserAsync(user.Id, updateuser, _currentUserService.Object);

            //Assert
            Assert.Equal(results.Email, updateuser.Email);
            Assert.Equal(results.Role, updateuser.Role);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnFail_WhenUserUpdateOrtherUser()
        {
            //Arrange
            var dto = new UserLoginDto { Email = "test1@gmail.com", Password = "123546" };
            var updateuser = new UpdateUserDto { Role = "Staff", Email = "test1@gmail.com" };

            var user = new User("test@gmail.com", "123456");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _currentUserService.Setup(c => c.UserId).Returns(Guid.NewGuid());
            _currentUserService.Setup(c => c.Roles).Returns("Staff");

            //Assert&Act
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _userService.UpdateUserAsync(user.Id, updateuser, _currentUserService.Object));
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnSuccess_WhenUserUpdateThemselves()
        {
            //Arrange
            var updateuser = new UpdateUserDto { Role = "Admind", Email = "test1@gmail.com" };

            var user = new User("test@gmail.com", "123456");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _currentUserService.Setup(c => c.UserId).Returns(user.Id);
            _currentUserService.Setup(c => c.Roles).Returns("Staff");
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { Email = updateuser.Email, Role = "Staff" });

            var result = await _userService.UpdateUserAsync(user.Id, updateuser, _currentUserService.Object);

            //Assert&Act
            Assert.Equal(result.Role, user.Role);
            Assert.Equal(result.Email, updateuser.Email);
        }
        #endregion

        #region Delete User Tests
        [Fact]
        public async Task DeleteUser_ShouldReturnSuccess_WhenUserExist()
        {
            //Arrange
            var dto = new UserLoginDto { Email = "test1@gmail.com", Password = "123546" };
            var updateuser = new UpdateUserDto { Role = "Staff", Email = "test1@gmail.com" };

            var user = new User("test@gmail.com", "123456");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _currentUserService.Setup(c => c.UserId).Returns(Guid.NewGuid());
            _currentUserService.Setup(c => c.Roles).Returns("Admin");
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { Email = updateuser.Email, Role = "Admin" });

            //Act
            await _userService.DeleteUserAsync(user.Id);

            //Assert
            _userRepoMock.Verify(r => r.Delete(user), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnFail_WhenUserNotFound()
        {
            //Arrange
            var dto = new UserLoginDto { Email = "test1@gmail.com", Password = "123546" };
            var updateuser = new UpdateUserDto { Role = "Staff", Email = "test1@gmail.com" };

            var user = new User("test@gmail.com", "123456");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(Guid.NewGuid())).ReturnsAsync((User?)null);
            _currentUserService.Setup(c => c.UserId).Returns(Guid.NewGuid());
            _currentUserService.Setup(c => c.Roles).Returns("Admin");

            //Act&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _userService.DeleteUserAsync(user.Id));
        }
        #endregion

        #region Change Password Tests
        [Fact]
        public async Task ChangePassword_ShouldReturnSucces_WhenUserIsAdmin()
        {
            //Arrange
            var dto = new UserLoginDto { Email = "test@gmail.com", Password = "123456" };
            var changePassword = new ChangePasswordDto { CurrentPassword = "123456", NewPassword = "654321" };

            var user = new User("test@gmail.com", "123456");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _currentUserService.Setup(c => c.UserId).Returns(user.Id);
            _currentUserService.Setup(c => c.Roles).Returns("Admin");
            _passwordHasherMock.Setup(p => p.Verify(changePassword.CurrentPassword!, user.Password)).Returns(true);
            _passwordHasherMock.Setup(p => p.Hash(changePassword.NewPassword)).Returns(changePassword.NewPassword);

            //Act
            var result = await _userService.ChangPassword(user.Id, changePassword, _currentUserService.Object);

            //Assert
            Assert.Equal(changePassword.NewPassword, user.Password);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnFail_WhenUserChangePasswordOrtherUser()
        {
            //Arrange
            var dto = new UserLoginDto { Email = "test1@gmail.com", Password = "123" };
            var changePassword = new ChangePasswordDto { CurrentPassword = "123456", NewPassword = "654321" };

            var user = new User("test@gmail.com", "123456");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _currentUserService.Setup(c => c.UserId).Returns(Guid.NewGuid);
            _currentUserService.Setup(c => c.Roles).Returns("Staff");
            _passwordHasherMock.Setup(p => p.Verify(changePassword.CurrentPassword!, user.Password)).Returns(true);
            _passwordHasherMock.Setup(p => p.Hash(changePassword.NewPassword)).Returns(changePassword.NewPassword);

            //Act&&Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _userService.ChangPassword(user.Id, changePassword, _currentUserService.Object));
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnFail_WhenCurentPasswordIsEmptyOrNull()
        {
            //Arrange
            var dto = new UserLoginDto { Email = "test1@gmail.com", Password = "123" };
            var changePassword = new ChangePasswordDto { CurrentPassword = "", NewPassword = "123456" };

            var user = new User("test@gmail.com", "123456");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _currentUserService.Setup(c => c.UserId).Returns(user.Id);
            _currentUserService.Setup(c => c.Roles).Returns("Staff");
            _passwordHasherMock.Setup(p => p.Verify(changePassword.CurrentPassword!, user.Password)).Returns(true);
            _passwordHasherMock.Setup(p => p.Hash(changePassword.NewPassword)).Returns(changePassword.NewPassword);

            //Act&&Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _userService.ChangPassword(user.Id, changePassword, _currentUserService.Object));
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnFail_WhenCurentPasswordInCorrect()
        {
            //Arrange
            var dto = new UserLoginDto { Email = "test1@gmail.com", Password = "123" };
            var changePassword = new ChangePasswordDto { CurrentPassword = "789000", NewPassword = "123456" };

            var user = new User("test@gmail.com", "123456");

            _unitOfWorkMock.Setup(u => u.User).Returns(_userRepoMock.Object);
            _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _currentUserService.Setup(c => c.UserId).Returns(user.Id);
            _currentUserService.Setup(c => c.Roles).Returns("Staff");
            _passwordHasherMock.Setup(p => p.Verify(changePassword.CurrentPassword!, user.Password)).Returns(false);
            _passwordHasherMock.Setup(p => p.Hash(changePassword.NewPassword)).Returns(changePassword.NewPassword);

            //Act&&Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _userService.ChangPassword(user.Id, changePassword, _currentUserService.Object));
        }
        #endregion
    }
}