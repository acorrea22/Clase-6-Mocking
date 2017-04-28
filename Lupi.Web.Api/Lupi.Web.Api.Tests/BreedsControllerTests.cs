using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Lupi.Data.Entities;
using Moq;
using Lupi.BusinessLogic;
using Lupi.Web.Api.Controllers;
using System.Web.Http;
using System.Web.Http.Results;
using System.Linq;
using System.Net.Http;

namespace Lupi.Web.Api.Tests
{
    [TestClass]
    public class BreedsControllerTests
    {

        private IEnumerable<Breed> GetFakeBreeds()
        {
            return new List<Breed>
            {
                new Breed
                {
                    Id = new Guid("e5020d0b-6fce-4b9f-a492-746c6c8a1bfa"),
                    Name = "Pug",
                    HairType  = "short fur",
                    HairColors = new List<string>
                    {
                        "blonde"
                    }
                },
                new Breed
                {
                    Id = new Guid("6b718186-fa8c-4e14-9af8-2601e153db71"),
                    Name = "Golden Retriever",
                    HairType  = "hairy fur",
                    HairColors = new List<string>
                    {
                        "blonde"
                    }
                }
            };
        }

        [TestMethod]
        public void GetAllBreedsOkTest()
        {
            //Arrange
            var expectedBreeds = GetFakeBreeds();

            var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
            mockBreedsBusinessLogic
                .Setup(bl => bl.GetAllBreeds())
                .Returns(expectedBreeds);

            var controller = new BreedsController(mockBreedsBusinessLogic.Object);

            //Act
            IHttpActionResult obtainedResult = controller.Get();
            var contentResult = obtainedResult as OkNegotiatedContentResult<IEnumerable<Breed>>;

            //Assert
            mockBreedsBusinessLogic.VerifyAll();
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(expectedBreeds, contentResult.Content);
        }

        [TestMethod]
        public void GetAllBreedsErrorNotFoundTest()
        {
            //Arrange
            List<Breed> expectedBreeds = null;

            var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
            mockBreedsBusinessLogic
                .Setup(bl => bl.GetAllBreeds())
                .Returns(expectedBreeds);

            var controller = new BreedsController(mockBreedsBusinessLogic.Object);

            //Act
            IHttpActionResult obtainedResult = controller.Get();

            //Assert
            mockBreedsBusinessLogic.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetBreedByIdOkTest()
        {
            //Arrange
            var fakeBreed = GetAFakeBreed();
            var fakeGuid = GetARandomFakeGuid();

            var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
            mockBreedsBusinessLogic
                .Setup(bl => bl.GetByID(fakeGuid))
                .Returns(fakeBreed);

            var controller = new BreedsController(mockBreedsBusinessLogic.Object);

            //Act
            IHttpActionResult obtainedResult = controller.Get(fakeGuid);
            var contentResult = obtainedResult as OkNegotiatedContentResult<Breed>;

            //Assert
            mockBreedsBusinessLogic.VerifyAll();
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(fakeGuid, contentResult.Content.Id);
        }

        [TestMethod]
        public void GetBreedByIdNotFoundErrorTest()
        {
            //Arrange
            var fakeGuid = Guid.NewGuid();

            var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
            mockBreedsBusinessLogic
                .Setup(bl => bl.GetByID(fakeGuid))
                .Returns((Breed)null);

            // Debemos retornar null, es lo que le exigimos al Mock para lograr
            // que el controller nos de NotFound

            var controller = new BreedsController(mockBreedsBusinessLogic.Object);

            //Act
            IHttpActionResult obtainedResult = controller.Get(fakeGuid);

            //Assert
            mockBreedsBusinessLogic.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public void CreateNewBreedTest()
        {
            //Arrange
            var fakeBreed = GetAFakeBreed();

            var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
            mockBreedsBusinessLogic
                .Setup(bl => bl.Add(fakeBreed))
                .Returns(fakeBreed.Id);

            var controller = new BreedsController(mockBreedsBusinessLogic.Object);

            //Act
            IHttpActionResult obtainedResult = controller.Post(fakeBreed);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<Breed>;

            //Assert
            mockBreedsBusinessLogic.VerifyAll();
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.AreEqual(fakeBreed.Id, createdResult.RouteValues["id"]);
            Assert.AreEqual(fakeBreed, createdResult.Content);
        }

        [TestMethod]
        public void CreateNullBreedErrorTest()
        {
            //Arrange
            Breed fakeBreed = null;

            var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
            mockBreedsBusinessLogic
                .Setup(bl => bl.Add(fakeBreed))
                .Throws(new ArgumentNullException());

            var controller = new BreedsController(mockBreedsBusinessLogic.Object);

            //Act
            IHttpActionResult obtainedResult = controller.Post(fakeBreed);

            //Assert
            mockBreedsBusinessLogic.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void UpdateExistingBreedOkTest()
        {
            //Arrange
            var fakeBreed = GetAFakeBreed();
            var expectedResult = true;

            var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
            mockBreedsBusinessLogic
                .Setup(bl => bl.Update(It.IsAny<Guid>(), It.IsAny<Breed>()))
                .Returns(true);

            var controller = new BreedsController(mockBreedsBusinessLogic.Object);

            //Act
            IHttpActionResult obtainedResult = controller.Put(new Guid(), fakeBreed);
            var createdResult = obtainedResult as CreatedAtRouteNegotiatedContentResult<Breed>;

            //Assert
            mockBreedsBusinessLogic.VerifyAll();
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.AreEqual(expectedResult, createdResult.RouteValues["updated"]);
            Assert.AreEqual(fakeBreed, createdResult.Content);
        }

        [TestMethod]
        public void UpdateBreedWithNullIdErrorTest()
        {
            //Arrange
            Breed fakeBreed = null;

            var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
            mockBreedsBusinessLogic
                .Setup(bl => bl.Update(new Guid(), It.IsAny<Breed>()))
                .Throws(new ArgumentNullException());

            var controller = new BreedsController(mockBreedsBusinessLogic.Object);

            //Act
            IHttpActionResult obtainedResult = controller.Put(new Guid(), fakeBreed);

            //Assert
            mockBreedsBusinessLogic.VerifyAll();
            Assert.IsInstanceOfType(obtainedResult, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void DeleteBreedOkTest()
        {
            //Arrange
            var fakeBreed = GetAFakeBreed();

            var mockBreedsBusinessLogic = new Mock<IBreedsBusinessLogic>();
            mockBreedsBusinessLogic
                .Setup(bl => bl.Delete(It.IsAny<Guid>()))
                .Returns(It.IsAny<bool>());

            var controller = new BreedsController(mockBreedsBusinessLogic.Object);
            // Configuramos la Request (dado que estamos haciendo HTTPResponseMessage)
            // Y usando el objeto Request adentro.
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            //Act
            HttpResponseMessage obtainedResult = controller.Delete(new Guid());

            //Assert
            mockBreedsBusinessLogic.VerifyAll();
            Assert.IsNotNull(obtainedResult);
        }

        private Breed GetAFakeBreed()
        {
            List<Breed> breeds = GetFakeBreeds().ToList();
            return breeds.FirstOrDefault();
        }

        private Guid GetARandomFakeGuid()
        {
            return GetAFakeBreed().Id;
        }
    }
}
