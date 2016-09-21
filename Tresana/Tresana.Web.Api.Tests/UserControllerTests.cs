using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.Web.Http;
using System.Web.Http.Results;
using Tresana.Data.Entities;
using Tresana.Web.Services;
using Tresana.Web.Api.Controllers;
using Xunit;

namespace Tresana.Web.Api.Tests
{
    public class UserControllerTests
    {
        [Fact]
        public void GetAllUsersReturnsEverythingInRepository()
        {
            var allUsers = new[]
            {
                new User()
                {
                    Name = "Alejandro",
                    LastName = "Tocar",
                    UserName = "aletocar",
                    Mail = "aletocar@gmail.com",
                    Id = 1
                },
                new User()
                {
                    Name = "Nicolas",
                    LastName = "Fornaro",
                    UserName = "nfornaro",
                    Mail = "nfornaro@gmail.com",
                    Id = 2
                }
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetAllUsers()).Returns(allUsers);


            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult actionResult = controller.GetUsers();
            OkNegotiatedContentResult<IEnumerable<User>> contentResult = Assert.IsType<OkNegotiatedContentResult<IEnumerable<User>>>(actionResult);
            Assert.NotNull(contentResult);
            Assert.NotNull(contentResult.Content);
            Assert.Same(allUsers, contentResult.Content);

        }

        [Fact]
        public void UpdateUsersReturnsNoContent()
        {
            var mockUserService = new Mock<IUserService>();

            mockUserService.Setup(x => x.UpdateUser(It.IsAny<int>(), It.IsAny<User>())).Returns(true);
            var controller = new UsersController(mockUserService.Object);

            IHttpActionResult actionResult = controller.PutUser(0, new User() { Id = 0 });

            StatusCodeResult contentResult = Assert.IsType<StatusCodeResult>(actionResult);
            Assert.NotNull(contentResult);
            Assert.Equal(contentResult.StatusCode, HttpStatusCode.NoContent);

        }
    }
}
