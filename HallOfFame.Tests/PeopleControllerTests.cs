namespace HallOfFame.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using HallOfFame.Web.Controllers;

    using Microsoft.AspNetCore.Mvc;

    using Moq;

    using Xunit;

    /// <summary>
    /// ����� ����������� <see cref="PeopleController"/>
    /// </summary>
    public class PeopleControllerTests
    {
        /// <summary>
        /// �������� �������� ������ �����������.
        /// </summary>
        /// <returns> ������ �����������. </returns>
        private static IEnumerable<Person> GetPeople()
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
        /// �������� ������.
        /// </summary>
        private readonly Person _person =
            new Person
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
        /// ���������� �������� ID-����������.
        /// </summary>
        private const long INVALID_TEST_ID = 10;

        /// <summary>
        /// �������� �������� ID-����������.
        /// </summary>
        private const long VALID_TEST_ID = 1;

        /// <summary>
        /// ���� ������ CreatePerson � �������� �������. 
        /// </summary>
        /// <returns> ��� 200. </returns>
        [Fact]
        public async Task CreatePerson()
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
            mock.Verify();
        }

        /// <summary>
        /// ���� ������ CreatePerson � ���������� �������. 
        /// </summary>
        /// <returns> ������ 400. </returns>
        [Fact]
        public async Task CreatePersonWithBadModel()
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
        /// ���� ������ CreatePerson � ��� ������������ ��������. 
        /// </summary>
        /// <returns> ��� 400. </returns>
        [Fact]
        public async Task CreatePersonWithExistingPerson()
        {
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            var result = await controller.CreatePerson(null);

            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        /// <summary>
        /// ���� ������ CreatePerson � ������ ��������. 
        /// </summary>
        /// <returns> ��� 400. </returns>
        [Fact]
        public async Task CreatePersonWithNull()
        {
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            var result = await controller.CreatePerson(null);

            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        /// <summary>
        /// ���� ������ DeletePerson � ������������ ��������.
        /// </summary>
        /// <returns> ��� 200. </returns>
        [Fact]
        public async Task DeletePerson()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.DeletePerson(VALID_TEST_ID))
                .ReturnsAsync(_person)
                .Verifiable();

            var controller = new PeopleController(mock.Object);

            var result = await controller.DeletePerson(VALID_TEST_ID);

            var viewResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, viewResult?.StatusCode);
            mock.Verify();
        }

        /// <summary>
        /// ���� ������ DeletePerson � �������������� ��������.
        /// </summary>
        /// <returns> ��� 404. </returns>
        [Fact]
        public async Task DeletePersonWithNonExistingPerson()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.DeletePerson(VALID_TEST_ID))
                .ReturnsAsync((Person)null)
                .Verifiable();

            var controller = new PeopleController(mock.Object);

            var result = await controller.DeletePerson(VALID_TEST_ID);

            var viewResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, viewResult?.StatusCode);
            mock.Verify();
        }

        /// <summary>
        /// ���� ������ GetPerson � �������� ID.
        /// </summary>
        /// <returns> ������ ����������. </returns>
        [Fact]
        public async Task GetPerson()
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
            mock.Verify();
        }

        /// <summary>
        /// ���� ������ GetPerson.
        /// </summary>
        /// <returns> ��������� �����������. </returns>
        [Fact]
        public async Task GetPersons()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.GetPeople())
                .Returns(Task.FromResult(GetPeople().ToArray()));

            var controller = new PeopleController(mock.Object);

            var result = await controller.GetPersons();

            var viewResult = Assert.IsAssignableFrom<Person[]>(result);
            Assert.Equal(2, viewResult.Length);
        }

        /// <summary>
        /// ���� ������ GetPerson � ���������� ID.
        /// </summary>
        /// <returns> ��� 404. </returns>
        [Fact]
        public async Task GetPersonWithBadId()
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
            mock.Verify();
        }

        /// <summary>
        /// ���� ������ UpdatePerson � ��������� �������.
        /// </summary>
        /// <returns> ��� 200. </returns>
        [Fact]
        public async Task UpdatePerson()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.TryToUpdatePerson(VALID_TEST_ID, It.IsAny<Person>()))
                .ReturnsAsync(true)
                .Verifiable();

            var controller = new PeopleController(mock.Object);

            var result = await controller.UpdatePerson(VALID_TEST_ID, _person);

            var viewResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, viewResult?.StatusCode);
            mock.Verify();
        }

        /// <summary>
        /// ���� ������ UpdatePerson � ���������� ID.
        /// </summary>
        /// <returns> ��� 400. </returns>
        [Fact]
        public async Task UpdatePersonWithBadId()
        {
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            var result = await controller.UpdatePerson(INVALID_TEST_ID, _person);

            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        /// <summary>
        /// ���� ������ UpdatePerson � ���������� �������.
        /// </summary>
        /// <returns> ��� 400. </returns>
        [Fact]
        public async Task UpdatePersonWithBadModel()
        {
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);
            controller.ModelState.AddModelError("Name", "Required");

            var result = await controller.UpdatePerson(VALID_TEST_ID, _person);

            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }

        /// <summary>
        /// ���� ������ UpdatePerson � �������������� ��������.
        /// </summary>
        /// <returns> ��� 404. </returns>
        [Fact]
        public async Task UpdatePersonWithNonExistingPerson()
        {
            var mock = new Mock<IPeopleRepository>();
            mock.Setup(repo => repo.TryToUpdatePerson(VALID_TEST_ID, It.IsAny<Person>()))
                .ReturnsAsync(false)
                .Verifiable();

            var controller = new PeopleController(mock.Object);

            var result = await controller.UpdatePerson(VALID_TEST_ID, _person);

            var viewResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, viewResult?.StatusCode);
            mock.Verify();
        }

        /// <summary>
        /// ���� ������ UpdatePerson � ������ ����� ID.
        /// </summary>
        /// <returns> ��� 400. </returns>
        [Fact]
        public async Task UpdatePersonWithNullId()
        {
            var mock = new Mock<IPeopleRepository>();
            var controller = new PeopleController(mock.Object);

            var result = await controller.UpdatePerson(null, _person);

            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult?.StatusCode);
        }
    }
}