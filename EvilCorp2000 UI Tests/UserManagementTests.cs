using EvilCorp2000.Pages.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MockQueryable;
using Moq;
using Serilog;
using System.Diagnostics;


namespace EvilCorp2000_UI_Tests
{

    public class UserManagementTests
    {
        

        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<ILogger<ManageUsersModel>> _loggerMock;
        private readonly ManageUsersModel _pageModel;

        public UserManagementTests()
        {
            // Setting up UserManager<IdentityUser> mock
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null
            );

            // Setting up RoleManager<IdentityRole> mock
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                roleStoreMock.Object, null, null, null, null
            );

            _loggerMock = new Mock<ILogger<ManageUsersModel>>();

            // Create the PageModel under test
            _pageModel = new ManageUsersModel(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task LoadDataAsync_ShouldPopulateUsersWithRoles()
        {
            // Arrange
            var mockUserStore = new Mock<IUserStore<IdentityUser>>();
            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                mockUserStore.Object,
                null, null, null, null, null, null, null, null
            );

            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                mockRoleStore.Object,
                null, null, null, null
            );

            var mockLogger = new Mock<ILogger<ManageUsersModel>>();

            // Prepare test data
            var identityUsers = new List<IdentityUser>
            {
                new IdentityUser { UserName = "User1", Email = "user1@test.com" },
                new IdentityUser { UserName = "User2", Email = "user2@test.com" },
            };

            var mockUsers = identityUsers.AsQueryable().BuildMock();

            // When .ToListAsync() is called, return our identityUsers list
            mockUserManager
                .Setup(um => um.Users)
                .Returns(mockUsers);

            // Mock getting roles for each user
            mockUserManager
                .Setup(um => um.GetRolesAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync((IdentityUser user) =>
                {
                    if (user.Email == "user1@test.com") return new List<string> { "Overseer" };
                    if (user.Email == "user2@test.com") return new List<string> { "TaskDrone" };
                    return new List<string>();
                });

            var pageModel = new ManageUsersModel(
                mockUserManager.Object,
                mockRoleManager.Object,
                mockLogger.Object
            );

            // Act
            await pageModel.LoadDataAsync();

            // Assert
            Assert.Equal(2, pageModel.UsersWithRoles.Count);

            Assert.Contains(pageModel.UsersWithRoles, u =>
                u.Email == "user1@test.com" && u.Role == "Overseer");

            Assert.Contains(pageModel.UsersWithRoles, u =>
                u.Email == "user2@test.com" && u.Role == "TaskDrone");

            // Optionally verify no errors were logged
            mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<System.Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never
            );
        }


        [Fact]
        public async Task OnGet_ShouldReturnPageResult()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            var loggerMock = new Mock<ILogger<ManageUsersModel>>();

            var model = new ManageUsersModel(userManagerMock.Object, roleManagerMock.Object, loggerMock.Object);

            // Act
            var result = await model.OnGet();

            // Assert
            Assert.IsType<PageResult>(result);
        }


        [Fact]
        public async Task OnPostNewUser_ShouldCreateUser_WhenDataIsValid()
        {
            
            // Arrange
            _pageModel.NewUserEmail = "newuser@valid.com";
            _pageModel.SelectedRole = "TaskDrone"; // valid role

            //Mock: identityUsers should not be empty
            var existingUser = new IdentityUser { Email = "irrelevant@evilcorp.com", Id = Guid.NewGuid().ToString() };
            var identityUsers = new List<IdentityUser> { existingUser };
            var mockUsers = identityUsers.AsQueryable().BuildMock();

            _userManagerMock.Setup(um => um.Users).Returns(mockUsers);
            _userManagerMock.Setup(um => um.GetRolesAsync(existingUser))
                            .ReturnsAsync(new List<string> { "TaskDrone" });

            // Mock: user does NOT exist yet
            _userManagerMock
                .Setup(um => um.FindByEmailAsync("newuser@valid.com"))
                .ReturnsAsync((IdentityUser)null);

            // Mock: create user -> success
            _userManagerMock
                .Setup(um => um.CreateAsync(
                    It.IsAny<IdentityUser>(),
                    It.IsAny<string>() // auto-generated password
                ))
                .ReturnsAsync(IdentityResult.Success);

            // Mock: add-to-role -> success
            _userManagerMock
                .Setup(um => um.AddToRoleAsync(
                    It.IsAny<IdentityUser>(),
                    "TaskDrone"
                ))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _pageModel.OnPostNewUser();

            // Assert
            // 1. Verify we got no ModelState errors
            Assert.True(_pageModel.ModelState.IsValid);

            // 2. Check the result is a PageResult (the method calls "return await OnGet();")
            var pageResult = Assert.IsType<PageResult>(result);

            // 3. Verify CreateAsync was called exactly once
            _userManagerMock.Verify(um => um.CreateAsync(
                It.Is<IdentityUser>(u => u.Email == "newuser@valid.com"),
                It.IsAny<string>()),
                Times.Once
            );

            // 4. Verify AddToRoleAsync was called with the correct role
            _userManagerMock.Verify(um => um.AddToRoleAsync(
                It.Is<IdentityUser>(u => u.Email == "newuser@valid.com"),
                "TaskDrone"),
                Times.Once
            );
        }

        [Fact]
        public async Task OnPostNewUser_ShouldReturnError_WhenEmailIsInvalid()
        {
            // Arrange
            _pageModel.NewUserEmail = "notAnEmail"; // invalid
            _pageModel.SelectedRole = "TaskDrone";

            // Act
            var result = await _pageModel.OnPostNewUser();

            // Assert
            // The method likely calls `ModelState.AddModelError` and then returns OnGet()
            var pageResult = Assert.IsType<PageResult>(result);
            Assert.False(_pageModel.ModelState.IsValid);
            Assert.True(_pageModel.ModelState.ContainsKey(nameof(_pageModel.NewUserEmail)));

            // Also verify that CreateAsync was never called
            _userManagerMock.Verify(
                um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task OnPostNewUser_ShouldReturnError_WhenUserAlreadyExists()
        {
            // Arrange
            _pageModel.NewUserEmail = "existinguser@evilcorp.com";
            _pageModel.SelectedRole = "TaskDrone";

            // Mock: user already exists
            _userManagerMock
                .Setup(um => um.FindByEmailAsync("existinguser@evilcorp.com"))
                .ReturnsAsync(new IdentityUser
                {
                    Email = "existinguser@evilcorp.com"
                });

            // Act
            var result = await _pageModel.OnPostNewUser();

            // Assert
            var pageResult = Assert.IsType<PageResult>(result);
            Assert.False(_pageModel.ModelState.IsValid);
            Assert.True(_pageModel.ModelState.ContainsKey(nameof(_pageModel.NewUserEmail)));

            // CreateAsync should never be called if user already exists
            _userManagerMock.Verify(
                um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task OnPostNewUser_ShouldReturnError_WhenRoleIsInvalid()
        {
            // Arrange
            _pageModel.NewUserEmail = "newuser@evilcorp.com";
            _pageModel.SelectedRole = "InvalidRole"; // neither TaskDrone nor Overseer

            //Mock: identityUsers should not be empty
            var existingUser = new IdentityUser { Email = "irrelevant@evilcorp.com", Id = Guid.NewGuid().ToString() };
            var identityUsers = new List<IdentityUser> { existingUser };
            var mockUsers = identityUsers.AsQueryable().BuildMock();

            _userManagerMock.Setup(um => um.Users).Returns(mockUsers);
            _userManagerMock.Setup(um => um.GetRolesAsync(existingUser))
                            .ReturnsAsync(new List<string> { "TaskDrone" });


            // Mock: user does NOT exist
            _userManagerMock
                .Setup(um => um.FindByEmailAsync("newuser@evilcorp.com"))
                .ReturnsAsync((IdentityUser)null);

            // Act
            var result = await _pageModel.OnPostNewUser();

            // Assert

            var pageResult = Assert.IsType<PageResult>(result);
            Assert.False(_pageModel.ModelState.IsValid);
            // An error on the SelectedRole key is expected
            Assert.True(_pageModel.ModelState.ContainsKey(nameof(_pageModel.SelectedRole)));

            // CreateAsync should not be called if role is invalid
            _userManagerMock.Verify(
                um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task OnPostNewUser_ShouldReturnError_WhenCreationFails()
        {
            // Arrange
            _pageModel.NewUserEmail = "newuser@evilcorp.com";
            _pageModel.SelectedRole = "TaskDrone";

            //Mock: identityUsers should not be empty
            var existingUser = new IdentityUser { Email = "irrelevant@evilcorp.com", Id = Guid.NewGuid().ToString() };
            var identityUsers = new List<IdentityUser> { existingUser };
            var mockUsers = identityUsers.AsQueryable().BuildMock();

            _userManagerMock.Setup(um => um.Users).Returns(mockUsers);
            _userManagerMock.Setup(um => um.GetRolesAsync(existingUser))
                            .ReturnsAsync(new List<string> { "TaskDrone" });


            // Mock: user does NOT exist
            _userManagerMock
                .Setup(um => um.FindByEmailAsync("newuser@evilcorp.com"))
                .ReturnsAsync((IdentityUser)null);

            // Mock: CreateAsync fails
            var identityErrors = new List<IdentityError>
            {
                new IdentityError { Description = "Something went wrong creating user." }
            };
            var failedResult = IdentityResult.Failed(identityErrors.ToArray());

            _userManagerMock
                .Setup(um => um.CreateAsync(
                    It.IsAny<IdentityUser>(),
                    It.IsAny<string>())
                )
                .ReturnsAsync(failedResult);

            // Act
            var result = await _pageModel.OnPostNewUser();

            // Assert
            var pageResult = Assert.IsType<PageResult>(result);
            Assert.False(_pageModel.ModelState.IsValid);

            // Confirm the error(s) got added to ModelState
            Assert.Single(_pageModel.ModelState[string.Empty].Errors);
            Assert.Contains("Something went wrong creating user.",
                _pageModel.ModelState[string.Empty].Errors.First().ErrorMessage);

            // Verify CreateAsync was called once
            _userManagerMock.Verify(
                um => um.CreateAsync(
                    It.Is<IdentityUser>(u => u.Email == "newuser@evilcorp.com"),
                    It.IsAny<string>()),
                Times.Once
            );
            // Because creation failed, AddToRoleAsync should not be called
            _userManagerMock.Verify(
                um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()),
                Times.Never
            );
        }


        [Fact]
        public async Task OnPostDeleteUser_ShouldDeleteUser_WhenExists()
        {
            // Arrange
            _pageModel.UserEmail = "existing.user@evilcorp.com";

            //Mocking des IdentityUsrr
            var existingUser = new IdentityUser { Email = _pageModel.UserEmail, Id = Guid.NewGuid().ToString() };
            var identityUsers = new List<IdentityUser> { existingUser };
            var mockUsers = identityUsers.AsQueryable().BuildMock();

            //Definieren der Verhalten aller relevanten Methoden
            _userManagerMock
            .Setup(um => um.FindByEmailAsync("existing.user@evilcorp.com"))
            .ReturnsAsync(existingUser);
            _userManagerMock.Setup(um => um.Users).Returns(mockUsers);
            _userManagerMock.Setup(um => um.GetRolesAsync(existingUser)).ReturnsAsync(new List<string> { "TaskDrone" });
            _userManagerMock
                .Setup(um => um.DeleteAsync(existingUser))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _pageModel.OnPostDeleteUser();


            // Assert
            // 1. No model errors
            Assert.True(_pageModel.ModelState.IsValid);

            // 2. We expect to call OnGet() => returns a PageResult
            var pageResult = Assert.IsType<PageResult>(result);

            // 3. Verify user deletion was called
            _userManagerMock.Verify(um => um.DeleteAsync(existingUser), Times.Once);

            // 4. Verify we did NOT log an error
            _loggerMock.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never
            );
        }

        [Fact]
        public async Task OnPostDeleteUser_ShouldReturnError_WhenUserDoesNotExist()
        {
            // Arrange
            _pageModel.UserEmail = "nonexistent.user@evilcorp.com";

            //Mocking des IdentityUsrr
            var nonExistentUser = new IdentityUser { Email = _pageModel.UserEmail, Id = Guid.NewGuid().ToString() };
            var identityUsers = new List<IdentityUser> ();
            var mockUsers = identityUsers.AsQueryable().BuildMock();

            //Definieren der Verhalten aller relevanten Methoden
            _userManagerMock
                .Setup(um => um.FindByEmailAsync("nonexistent.user@evilcorp.com"))
                .ReturnsAsync((IdentityUser)null);
            _userManagerMock
                .Setup(um => um.Users)
                .Returns(mockUsers);

            // Act
            var result = await _pageModel.OnPostDeleteUser();

            // Assert
            var pageResult = Assert.IsType<PageResult>(result); // calls OnGet() => PageResult
            Assert.False(_pageModel.ModelState.IsValid);
            Assert.True(_pageModel.ModelState.ContainsKey(nameof(_pageModel.UserEmail)));

            // Verify DeleteAsync never called
            _userManagerMock.Verify(um => um.DeleteAsync(It.IsAny<IdentityUser>()), Times.Never);

            // Verify error log
            _loggerMock.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task OnPostDeleteUser_ShouldReturnError_WhenUserEmailIsInvalid()
        {
            //Mocking des IdentityUsrr
            var nonExistentUser = new IdentityUser { Email = _pageModel.UserEmail, Id = Guid.NewGuid().ToString() };
            var identityUsers = new List<IdentityUser>();
            var mockUsers = identityUsers.AsQueryable().BuildMock();

            //Definieren der Verhalten aller relevanten Methoden
            _userManagerMock
                .Setup(um => um.FindByEmailAsync("nonexistent.user@evilcorp.com"))
                .ReturnsAsync((IdentityUser)null);
            _userManagerMock
                .Setup(um => um.Users)
                .Returns(mockUsers);

            _pageModel.UserEmail = null;

            // ACT
            var result = await _pageModel.OnPostDeleteUser();

            // ASSERT
            var pageResult = Assert.IsType<PageResult>(result); // OnGet() => PageResult
            Assert.False(_pageModel.ModelState.IsValid);
            Assert.True(_pageModel.ModelState.ContainsKey(nameof(_pageModel.UserEmail)));
            var errorMessage = _pageModel.ModelState[nameof(_pageModel.UserEmail)].Errors.First().ErrorMessage;
            Assert.Equal("Invalid user.", errorMessage);

            // Verify user was never looked up or deleted
            _userManagerMock.Verify(um => um.FindByEmailAsync(It.IsAny<string>()), Times.Never);
            _userManagerMock.Verify(um => um.DeleteAsync(It.IsAny<IdentityUser>()), Times.Never);

            // Verify error log occurred
            _loggerMock.Verify(
                logger => logger.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task OnPostDeleteUser_ShouldHandleException_AndLogError()
        {
            // Arrange
            _pageModel.UserEmail = "throw.user@evilcorp.com";

            // Mock user does exist
            var existingUser = new IdentityUser { Email = "throw.user@evilcorp.com", Id = Guid.NewGuid().ToString() };

            _userManagerMock
                .Setup(um => um.FindByEmailAsync("throw.user@evilcorp.com"))
                .ReturnsAsync(existingUser);

            // Now cause an exception when calling DeleteAsync
            _userManagerMock
                .Setup(um => um.DeleteAsync(existingUser))
                .ThrowsAsync(new Exception("Test delete exception"));

            // Act
            var result = await _pageModel.OnPostDeleteUser();

            // Assert
            // The code catches the exception and returns Page()
            var pageResult = Assert.IsType<PageResult>(result);

            // Verify it logged an error
            _loggerMock.Verify(
                logger => logger.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );

            // Model error expected, just an exception -> log -> return Page()
            Assert.False(_pageModel.ModelState.IsValid);
        }

    }
}
