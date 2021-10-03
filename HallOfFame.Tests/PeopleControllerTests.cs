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
            new Person[]
            {
                new Person {
                Name = "",
                SkillsCollection =
                new Skill[]
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
                new Skill[]
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

        readonly Person person =
        new Person
        {
            Id = 1,
            Name = "",
            SkillsCollection =
            new Skill[]
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
            long id = 1;
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.GetPerson(id))
                .ReturnsAsync(person)
                .Verifiable();

            var controller = new PeopleController(mock.Object);
            // Act
            var result = await controller.GetPerson(id);

            // Assert
            var viewResult = Assert.IsType<ActionResult<Person>>(result);
            var model = Assert.IsAssignableFrom<ObjectResult>(
            viewResult.Result);
            Assert.Equal(person, model.Value);
            mock.Verify();
        }

        [Fact]
        public async Task GetPerson_WithBadId_ReturnsNotFound()
        {
            // Arrange
            Person person = null;

            int id = 10;
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.GetPerson(id))
                .ReturnsAsync(person)
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
            Person newPerson = null;

            // Act
            var result = await controller.CreatePerson(newPerson);

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
            Person newPerson = null;

            // Act
            var result = await controller.CreatePerson(newPerson);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        [Fact]
        public async Task UpdatePerson_WithPersonAndGoodId_ReturnsOk()
        {
            // Arrange
            long id = 1;
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.TryToUpdatePerson(id, It.IsAny<Person>()))
                .ReturnsAsync(true)
                .Verifiable();
            var controller = new PeopleController(mock.Object);

            // Act
            var result = await controller.UpdatePerson(id, person);

            // Assert
            var viewResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, viewResult?.StatusCode);
            mock.Verify();
        }

        [Fact]
        public async Task UpdatePerson_WithPersonAndBadId_ReturnsBadRequest()
        {
            // Arrange
            long id = 10;
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            // Act
            var result = await controller.UpdatePerson(id, person);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        [Fact]
        public async Task UpdatePerson_WithNonExistingPerson_ReturnsNotFound()
        {
            // Arrange
            long id = 1;
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.TryToUpdatePerson(id, It.IsAny<Person>()))
                .ReturnsAsync(false)
                .Verifiable();
            var controller = new PeopleController(mock.Object);

            // Act
            var result = await controller.UpdatePerson(id, person);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, viewResult?.StatusCode);
            mock.Verify();
        }

        [Fact]
        public async Task UpdatePerson_WithBadModel_ReturnsBadRequest()
        {
            // Arrange
            long id = 1;
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);
            controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await controller.UpdatePerson(id, person);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        [Fact]
        public async Task UpdatePerson_WithNullId_ReturnsBadRequest()
        {
            // Arrange
            long? id = null;
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            // Act
            var result = await controller.UpdatePerson(id, person);

            // Assert
            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        [Fact]
        public async Task DeletePerson_WithNonExisting_ReturnsNotFound()
        {
            // Arrange
            long id = 1;
            Person newPerson = null;
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.DeletePerson(id))
                .ReturnsAsync(newPerson)
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
            long id = 1;
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.DeletePerson(id))
                .ReturnsAsync(person)
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
