using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HallOfFame.Web.Controllers;
using Moq;
using Xunit;

namespace HallOfFame.Tests
{
    public class PeopleControllerTests
    {
        private static IEnumerable<Person> GetPeople()=>
            new[]
            {
                new Person {
                Name = "",
                SkillsCollection =
                new[]
                {
                    new Skill
                    {
                        Name="agility",
                        Level = 9
                    },
                    new Skill
                    {
                        Name="strength",
                        Level = 9
                    }
                }
            },
                new Person {
                Name = "asd",
                SkillsCollection =
                new[]
                {
                    new Skill
                    {
                        Name="agility",
                        Level = 9
                    },
                    new Skill
                    {
                        Name="strength",
                        Level = 9
                    }
                }
            }
            };

        readonly Person _person =
        new Person
        {
            Id = 1,
            Name = "",
            SkillsCollection =
            new[]
                {
                new Skill
                    {
                        Name="agility",
                        Level = 9
                    },
                new Skill
                {
                    Name="strength",
                    Level = 9
                }
            }
        };

        [Fact]
        public async Task GetPersons_ReturnsPersonArray()
        {
            // Arrange
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.GetPeople())
                .Returns(Task.FromResult(GetPeople().ToArray()));

            var controller = new PeopleController(mock.Object);
            // Act
            var result = await controller.GetPersons();

            // Assert
            var viewResult = Assert.IsAssignableFrom<Person[]>(result);
            Assert.Equal(2, viewResult.Length);
        }

        [Fact]
        public async Task GetPerson_WithGoodId_ReturnsPerson()
        {
            // Arrange
            const long id = 1;
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.GetPerson(id))
                .ReturnsAsync(_person)
                .Verifiable();

            var controller = new PeopleController(mock.Object);
            // Act
            var result = await controller.GetPerson(id);

            // Assert
            var viewResult = Assert.IsType<ActionResult<Person>>(result);
            var model = Assert.IsAssignableFrom<ObjectResult>(
            viewResult.Result);
            Assert.Equal(_person, model.Value);
            mock.Verify();
        }

        [Fact]
        public async Task GetPerson_WithBadId_ReturnsNotFound()
        {
            // Arrange
            const int id = 10;
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.GetPerson(id))
                .ReturnsAsync((Person)null)
                .Verifiable();

            var controller = new PeopleController(mock.Object);
            // Act
            var result = await controller.GetPerson(id);

            // Assert
            var viewResult = Assert.IsType<ActionResult<Person>>(result);
            var model = Assert.IsAssignableFrom<NotFoundResult>(
            viewResult.Result);
            Assert.Equal(404, model?.StatusCode);
            mock.Verify();
        }

        [Fact]
        public async Task CreatePerson_WithPerson_ReturnsOk()
        {
            // Arrange
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.TryToCreatePerson(It.IsAny<Person>()))
                .ReturnsAsync(true)
                .Verifiable();
            var controller = new PeopleController(mock.Object);
            Person newPerson = new Person();

            // Act
            var result = await controller.CreatePerson(newPerson);

            // Assert
            var viewResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, viewResult?.StatusCode);
            mock.Verify();
        }

        [Fact]
        public async Task CreatePerson_WithNull_ReturnsBadRequest()
        {
            // Arrange
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            // Act
            var result = await controller.CreatePerson(null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        [Fact]
        public async Task CreatePerson_WithBadModel_ReturnsBadRequest()
        {
            // Arrange
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);
            controller.ModelState.AddModelError("Name", "Required");
            Person newPerson = new Person();

            // Act
            var result = await controller.CreatePerson(newPerson);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        [Fact]
        public async Task CreatePerson_WithExistingPerson_ReturnsBadRequest()
        {
            // Arrange
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            // Act
            var result = await controller.CreatePerson(null);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        [Fact]
        public async Task UpdatePerson_WithPersonAndGoodId_ReturnsOk()
        {
            // Arrange
            const long id = 1;
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.TryToUpdatePerson(id, It.IsAny<Person>()))
                .ReturnsAsync(true)
                .Verifiable();
            var controller = new PeopleController(mock.Object);

            // Act
            var result = await controller.UpdatePerson(id, _person);

            // Assert
            var viewResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, viewResult?.StatusCode);
            mock.Verify();
        }

        [Fact]
        public async Task UpdatePerson_WithPersonAndBadId_ReturnsBadRequest()
        {
            // Arrange
            const long id = 10;
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            // Act
            var result = await controller.UpdatePerson(id, _person);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        [Fact]
        public async Task UpdatePerson_WithNonExistingPerson_ReturnsNotFound()
        {
            // Arrange
            const long id = 1;
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.TryToUpdatePerson(id, It.IsAny<Person>()))
                .ReturnsAsync(false)
                .Verifiable();
            var controller = new PeopleController(mock.Object);

            // Act
            var result = await controller.UpdatePerson(id, _person);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, viewResult?.StatusCode);
            mock.Verify();
        }

        [Fact]
        public async Task UpdatePerson_WithBadModel_ReturnsBadRequest()
        {
            // Arrange
            const long id = 1;
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);
            controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await controller.UpdatePerson(id, _person);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        [Fact]
        public async Task UpdatePerson_WithNullId_ReturnsBadRequest()
        {
            // Arrange
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            // Act
            var result = await controller.UpdatePerson(null, _person);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        [Fact]
        public async Task DeletePerson_WithNonExisting_ReturnsNotFound()
        {
            // Arrange
            const long id = 1;
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.DeletePerson(id))
                .ReturnsAsync((Person)null)
                .Verifiable();
            var controller = new PeopleController(mock.Object);

            // Act
            var result = await controller.DeletePerson(id);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, viewResult?.StatusCode);
            mock.Verify();
        }

        [Fact]
        public async Task DeletePerson_WithExisting_ReturnsOk()
        {
            // Arrange
            const long id = 1;
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.DeletePerson(id))
                .ReturnsAsync(_person)
                .Verifiable();
            var controller = new PeopleController(mock.Object);

            // Act
            var result = await controller.DeletePerson(id);

            // Assert
            var viewResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, viewResult?.StatusCode);
            mock.Verify();
        }
    }
}
