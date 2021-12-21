namespace HallOfFame.Tests
{
    using System.Threading.Tasks;

    using HallOfFame.Web.Controllers;

    using Microsoft.AspNetCore.Mvc;

    using Moq;

    using Xunit;

    /// <summary>
    /// Тесты контроллера <see cref="PeopleController"/>
    /// </summary>
    public class PeopleControllerTests
    {
        /// <summary>
        /// Получить тестовый список сотрудников.
        /// </summary>
        /// <returns> Список сотрудников. </returns>
        private static Person[] GetPeople()
        {
            return new[]
            {
                new Person
                {
                    Name = "",
                    SkillsCollection =
                        new[]
                        {
                            new Skill
                            {
                                Name = "agility",
                                Level = 9
                            },
                            new Skill
                            {
                                Name = "strength",
                                Level = 9
                            }
                        }
                },
                new Person
                {
                    Name = "asd",
                    SkillsCollection =
                        new[]
                        {
                            new Skill
                            {
                                Name = "agility",
                                Level = 9
                            },
                            new Skill
                            {
                                Name = "strength",
                                Level = 9
                            }
                        }
                }
            };
        }

        /// <summary>
        /// Тестовая модель.
        /// </summary>
        private readonly Person _person =
            new()
            {
                Id = VALID_TEST_ID,
                Name = "",
                SkillsCollection =
                    new[]
                    {
                        new Skill
                        {
                            Name = "agility",
                            Level = 9
                        },
                        new Skill
                        {
                            Name = "strength",
                            Level = 9
                        }
                    }
            };

        /// <summary>
        /// Невалидный тестовый ID-сотрудника.
        /// </summary>
        private const long INVALID_TEST_ID = 10;

        /// <summary>
        /// Валидный тестовый ID-сотрудника.
        /// </summary>
        private const long VALID_TEST_ID = 1;

        /// <summary>
        /// Тест метода CreatePerson с валидной моедлью. 
        /// </summary>
        /// <returns> Код 200. </returns>
        [Fact]
        public async Task CreatePerson_RepoReturnsFalse()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.TryToCreatePerson(It.IsAny<Person>()))
                .ReturnsAsync(false)
                .Verifiable();

            var controller = new PeopleController(mock.Object);
            var newPerson = new Person();

            var result = await controller.CreatePerson(newPerson);

            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
            mock.Verify(repo => repo.TryToCreatePerson(It.IsAny<Person>()), Times.Once());
        }

        /// <summary>
        /// Тест метода CreatePerson с валидной моедлью. 
        /// </summary>
        /// <returns> Код 200. </returns>
        [Fact]
        public async Task CreatePerson_Valid()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.TryToCreatePerson(It.IsAny<Person>()))
                .ReturnsAsync(true)
                .Verifiable();

            var controller = new PeopleController(mock.Object);
            var newPerson = new Person();

            var result = await controller.CreatePerson(newPerson);

            var viewResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, viewResult?.StatusCode);
            mock.Verify(repo => repo.TryToCreatePerson(It.IsAny<Person>()), Times.Once());
        }

        /// <summary>
        /// Тест метода CreatePerson с невалидной моделью. 
        /// </summary>
        /// <returns> Ошибку 400. </returns>
        [Fact]
        public async Task CreatePerson_WithBadModel()
        {
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);
            controller.ModelState.AddModelError("Name", "Required");
            var newPerson = new Person();

            var result = await controller.CreatePerson(newPerson);

            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        /// <summary>
        /// Тест метода CreatePerson с уже существующим объектом. 
        /// </summary>
        /// <returns> Код 400. </returns>
        [Fact]
        public async Task CreatePerson_WithExistingPerson()
        {
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            var result = await controller.CreatePerson(null);

            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        /// <summary>
        /// Тест метода CreatePerson с пустым запросом. 
        /// </summary>
        /// <returns> Код 400. </returns>
        [Fact]
        public async Task CreatePerson_WithNull()
        {
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            var result = await controller.CreatePerson(null);

            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        /// <summary>
        /// Тест метода DeletePerson с существующим объектом.
        /// </summary>
        /// <returns> Код 200. </returns>
        [Fact]
        public async Task DeletePerson_Valid()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.DeletePerson(VALID_TEST_ID))
                .ReturnsAsync(_person)
                .Verifiable();

            var controller = new PeopleController(mock.Object);

            var result = await controller.DeletePerson(VALID_TEST_ID);

            var viewResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, viewResult?.StatusCode);
            mock.Verify(repo => repo.DeletePerson(VALID_TEST_ID), Times.Once);
        }

        /// <summary>
        /// Тест метода DeletePerson с несуществующим объектом.
        /// </summary>
        /// <returns> Код 404. </returns>
        [Fact]
        public async Task DeletePerson_WithNonExistingPerson()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.DeletePerson(VALID_TEST_ID))
                .ReturnsAsync((Person)null)
                .Verifiable();

            var controller = new PeopleController(mock.Object);

            var result = await controller.DeletePerson(VALID_TEST_ID);

            var viewResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, viewResult?.StatusCode);
            mock.Verify(repo => repo.DeletePerson(VALID_TEST_ID), Times.Once);
        }

        /// <summary>
        /// Тест метода GetPerson с валидным ID.
        /// </summary>
        /// <returns> Модель сотрудника. </returns>
        [Fact]
        public async Task GetPerson_Valid()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.GetPerson(VALID_TEST_ID))
                .ReturnsAsync(_person)
                .Verifiable();

            var controller = new PeopleController(mock.Object);

            var result = await controller.GetPerson(VALID_TEST_ID);

            var viewResult = Assert.IsType<ActionResult<Person>>(result);
            var model = Assert.IsAssignableFrom<ObjectResult>(viewResult.Result);
            Assert.Equal(_person, model.Value);
            mock.Verify(repo => repo.GetPerson(VALID_TEST_ID), Times.Once);
        }

        /// <summary>
        /// Тест метода GetPerson с невалидным ID.
        /// </summary>
        /// <returns> Код 404. </returns>
        [Fact]
        public async Task GetPerson_WithBadId()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.GetPerson(INVALID_TEST_ID))
                .ReturnsAsync((Person)null)
                .Verifiable();

            var controller = new PeopleController(mock.Object);

            var result = await controller.GetPerson(INVALID_TEST_ID);

            var viewResult = Assert.IsType<ActionResult<Person>>(result);
            var model = Assert.IsAssignableFrom<NotFoundResult>(viewResult.Result);
            Assert.Equal(404, model?.StatusCode);
            mock.Verify(repo => repo.GetPerson(INVALID_TEST_ID), Times.Once);
        }

        /// <summary>
        /// Тест метода GetPerson.
        /// </summary>
        /// <returns> Коллекцию сотрудников. </returns>
        [Fact]
        public async Task GetPersons_Valid()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.GetPeople())
                .Returns(Task.FromResult(GetPeople()))
                .Verifiable();

            var controller = new PeopleController(mock.Object);

            var result = await controller.GetPersons();

            var viewResult = Assert.IsAssignableFrom<Person[]>(result);
            Assert.Equal(2, viewResult.Length);
            mock.Verify(repo => repo.GetPeople(), Times.Once);
        }

        /// <summary>
        /// Тест метода UpdatePerson с валидными данными.
        /// </summary>
        /// <returns> Код 200. </returns>
        [Fact]
        public async Task UpdatePerson_Valid()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.TryToUpdatePerson(VALID_TEST_ID, It.IsAny<Person>()))
                .ReturnsAsync(true)
                .Verifiable();

            var controller = new PeopleController(mock.Object);

            var result = await controller.UpdatePerson(VALID_TEST_ID, _person);

            var viewResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, viewResult?.StatusCode);
            mock.Verify(repo => repo.TryToUpdatePerson(VALID_TEST_ID, It.IsAny<Person>()), Times.Once);
        }

        /// <summary>
        /// Тест метода UpdatePerson с невалидным ID.
        /// </summary>
        /// <returns> Код 400. </returns>
        [Fact]
        public async Task UpdatePerson_WithBadId()
        {
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            var result = await controller.UpdatePerson(INVALID_TEST_ID, _person);

            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        /// <summary>
        /// Тест метода UpdatePerson с невалидной моделью.
        /// </summary>
        /// <returns> Код 400. </returns>
        [Fact]
        public async Task UpdatePerson_WithBadModel()
        {
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);
            controller.ModelState.AddModelError("Name", "Required");

            var result = await controller.UpdatePerson(VALID_TEST_ID, _person);

            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        /// <summary>
        /// Тест метода UpdatePerson с несуществующим объектом.
        /// </summary>
        /// <returns> Код 404. </returns>
        [Fact]
        public async Task UpdatePerson_WithNonExistingPerson()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.TryToUpdatePerson(VALID_TEST_ID, It.IsAny<Person>()))
                .ReturnsAsync(false)
                .Verifiable();

            var controller = new PeopleController(mock.Object);

            var result = await controller.UpdatePerson(VALID_TEST_ID, _person);

            var viewResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, viewResult?.StatusCode);
            mock.Verify(repo => repo.TryToUpdatePerson(VALID_TEST_ID, It.IsAny<Person>()), Times.Once);
        }

        /// <summary>
        /// Тест метода UpdatePerson с пустым полем ID.
        /// </summary>
        /// <returns> Код 400. </returns>
        [Fact]
        public async Task UpdatePerson_WithNullId()
        {
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            var result = await controller.UpdatePerson(null, _person);

            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }
    }
}