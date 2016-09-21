using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Tresana.Data.Entities;
using Tresana.Data.Repository;
using Xunit;

namespace Tresana.Web.Services.Tests
{
    public class UserServiceTests
    {

        [Fact]
        public void GetAllUsersFromRepositoryTest()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            
            mockUnitOfWork.Setup(un => un.UserRepository.Get(null, null, ""));

            IUserService userService = new UserService(mockUnitOfWork.Object);

            //Act
            IEnumerable<User> returnedUsers = userService.GetAllUsers();

            //Assert
            mockUnitOfWork.VerifyAll();
        }

        [Fact]
        public void GetUserByIdReturnsUserWithId()
        {
            //Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            //Esperamos que se llame al metodo Get del userRepository con un int
            mockUnitOfWork.Setup(un => un.UserRepository.GetByID(It.IsAny<int>()));

            IUserService userService = new UserService(mockUnitOfWork.Object);

            //Act
            User returnedUser = userService.GetUserById(5);

            //Assert
            mockUnitOfWork.VerifyAll();
        }

        [Fact]
        public void CreateUserTest()
        {
            //Arrange
            //Creo el mock object del unitOfWork
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            //Esperamos que se llame al método Insert del userRepository con un Usuario y luego al Save();
            mockUnitOfWork.Setup(un => un.UserRepository.Insert(It.IsAny<User>()));
            mockUnitOfWork.Setup(un => un.Save());

            IUserService userService = new UserService(mockUnitOfWork.Object);
            

            //Act
           int id = userService.CreateUser(new User());


            //Assert
            mockUnitOfWork.VerifyAll();

        }

        [Fact]
        public void UpdatesExistingUser()
        {
            //Arrange 
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            //Para el Update, se utiliza el método ExistsUser(), el cual a su vez utiliza el método GetUserByID del repositorio.
            //En este test, querémos asegurarnos que, en caso que el usuario exista, se ejecute el Update() y el Save() en el repositorio.
            //Por lo tanto, debemos establecer que el GetUserByID devuelva algo distinto de null, de manera que el ExistsUser retorne true.
            mockUnitOfWork
                .Setup(un => un.UserRepository.GetByID(It.IsAny<int>()))
                .Returns(() => new User() { });

            //Además, seteamos las expectativas para los métodos que deben llamarse luego
            mockUnitOfWork.Setup(un => un.UserRepository.Update(It.IsAny<User>()));
            mockUnitOfWork.Setup(un => un.Save());

            IUserService userService = new UserService(mockUnitOfWork.Object);

            //act
            bool updated = userService.UpdateUser(0, new User() { });

            //Assert
            //En este caso, debemos asegurarnos que el Update y el Save se hayan llamado una vez.
            mockUnitOfWork.Verify(un=> un.UserRepository.Update(It.IsAny<User>()), Times.Exactly(1));
            mockUnitOfWork.Verify(un=> un.Save(), Times.Exactly(1));
            Assert.True(updated);

        }

        [Fact]
        public void DoesntUpdateNonExistingUser()
        {
            //Arrange 
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            //Para el Update, se utiliza el método ExistsUser(), el cual a su vez utiliza el método GetUserByID del repositorio.
            //En este test, querémos asegurarnos que, en caso que el usuario exista, se ejecute el Update() y el Save() en el repositorio.
            //Por lo tanto, debemos establecer que el GetUserByID devuelva algo distinto de null, de manera que el ExistsUser retorne true.
            mockUnitOfWork
                .Setup(un => un.UserRepository.GetByID(It.IsAny<int>()))
                .Returns(() => null);

            //Además, seteamos las expectativas para los métodos que deben llamarse luego
            mockUnitOfWork.Setup(un => un.UserRepository.Update(It.IsAny<User>()));
            mockUnitOfWork.Setup(un => un.Save());

            IUserService userService = new UserService(mockUnitOfWork.Object);

            //act
            bool updated = userService.UpdateUser(0, new User() { });

            //Assert
            mockUnitOfWork.Verify(un => un.UserRepository.Update(It.IsAny<User>()), Times.Never());
            mockUnitOfWork.Verify(un => un.Save(), Times.Never());
            Assert.False(updated);

        }


        private IEnumerable<User> GetUserList()
        {
            return new[]
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
        }
        private User GetUserById(int id)
        {
            return
                new User()
                {
                    Name = "Alejandro",
                    LastName = "Tocar",
                    UserName = "aletocar",
                    Mail = "aletocar@gmail.com",
                    Id = id
                };
        }
    }
}
