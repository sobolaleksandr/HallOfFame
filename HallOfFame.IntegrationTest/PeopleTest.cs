using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using HallOfFame.Web;
using Xunit;
using Newtonsoft.Json;

namespace HallOfFame.IntegrationTest
{
    public class PeopleTest
    {
        private readonly HttpClient _client;
        private Person testPerson;

        public PeopleTest()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());
            _client = server.CreateClient();
        }

        [Fact] 
        public async Task PeopleTests()
        {
            await CreatePerson_WithGoodModel_ReturnsOk();
            await GetPersons_ReturnsPersons();
            await UpdatePerson_WithGoodModel_ReturnsOk();
            await GetPerson_ReturnsPerson();
            await DeletePerson_WithGoodId_ReturnsOk();
        }

        internal async Task CreatePerson_WithGoodModel_ReturnsOk()
        {
            // Arrange
            Person person =
            new Person
            {
                Name = "TestsName",
                SkillsCollection =
                new Skill[]
                {
                    new Skill
                    {
                        Name="TestSkill",
                        Level = 9
                    },
                    new Skill
                    {
                        Name="TestSkill2",
                        Level = 9
                    }
                }
            };

            var body = JsonConvert.SerializeObject(person);
            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/person/")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        internal async Task CreatePerson_WithBadModel_ReturnsBadRequest()
        {
            // Arrange
            Person person =
                    new Person
                    {
                        Name = "TestName",
                        SkillsCollection =
                        new Skill[]
                        {
                            new Skill
                            {
                                Name="TestSkill",
                                Level = 11
                            }
                        }
                    };

            var body = JsonConvert.SerializeObject(person);
            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/person/")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        internal async Task CreatePerson_WithNull_ReturnsBadRequest()
        {
            // Arrange
            Person person = null;

            var body = JsonConvert.SerializeObject(person);
            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/person/")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        internal async Task GetPersons_ReturnsPersons()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/persons");

            // Act
            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            List<Person> jsonPersons = JsonConvert.DeserializeObject<List<Person>>(content);
            testPerson = jsonPersons.Where(p => p.Name == "TestsName").FirstOrDefault();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        internal async Task UpdatePerson_WithGoodModel_ReturnsOk()
        {
            // Arrange
            var skillsArray = testPerson.SkillsCollection.ToArray();
            skillsArray[0].Level = 1;
            skillsArray[1].Level = 1;
            long id = testPerson.Id;

            var body = JsonConvert.SerializeObject(testPerson);
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/person/{id}")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        internal async Task UpdatePerson_WithNullId_ReturnBadRequest()
        {
            // Arrange
            long? id = null;

            Person person =
                    new Person
                    {
                        Id = 1,
                        Name = "TestName",
                        SkillsCollection =
                        new Skill[]
                        {
                            new Skill
                            {
                                Name="TestSkill",
                                Level = 1
                            }
                        }
                    };

            var body = JsonConvert.SerializeObject(person);
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/person/{id}")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        internal async Task UpdatePerson_WithBadId_ReturnBadRequest()
        {
            // Arrange
            long id = 1;

            Person person =
                    new Person
                    {
                        Id=12,
                        Name = "TestName",
                        SkillsCollection =
                        new Skill[]
                        {
                            new Skill
                            {
                                Name="TestSkill",
                                Level = 1
                            }
                        }
                    };

            var body = JsonConvert.SerializeObject(person);
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/person/{id}")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        internal async Task UpdatePerson_WithBadModel_ReturnsBadRequest()
        {
            // Arrange
            long id = 1;

            Person person =
                    new Person
                    {
                        Id = 1,
                        Name = "TestName",
                        SkillsCollection =
                        new Skill[]
                        {
                            new Skill
                            {
                                Name="TestSkill",
                                Level = 11
                            }
                        }
                    };

            var body = JsonConvert.SerializeObject(person);
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/person/{id}")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        internal async Task UpdatePerson_WithNonExistingPerson_ReturnsNotFound()
        {
            // Arrange
            long id = long.MaxValue;

            Person person =
            new Person
            {
                Id = id,
                Name = "TestsName",
                SkillsCollection =
                new Skill[]
                {
                    new Skill
                    {
                        Name="TestSkill",
                        Level = 9
                    },
                    new Skill
                    {
                        Name="TestSkill2",
                        Level = 9
                    }
                }
            };

            var body = JsonConvert.SerializeObject(person);
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/person/{id}")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        internal async Task GetPerson_ReturnsPerson()
        {
            // Arrange
            long id = testPerson.Id;
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/person/{id}");

            // Act
            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            Person jsonPerson = JsonConvert.DeserializeObject<Person>(content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal("TestsName", jsonPerson.Name);
            Assert.Null(jsonPerson.DisplayName);

            var skillsArray = jsonPerson.SkillsCollection.ToArray();

            Assert.Equal(2, skillsArray.Length);
            Assert.Equal("TestSkill", skillsArray[0].Name);
            Assert.Equal(1, skillsArray[0].Level);
            Assert.Equal("TestSkill2", skillsArray[1].Name);
            Assert.Equal(1, skillsArray[1].Level);
        }

        [Fact]
        internal async Task GetPerson_ReturnsNotFound()
        {
            // Arrange
            long id = long.MaxValue;
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/person/{id}");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        internal async Task DeletePerson_WithGoodId_ReturnsOk()
        {
            // Arrange
            long id = testPerson.Id;
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/person/{id}");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        internal async Task DeletePerson_WithBadId_ReturnsNotFound()
        {
            // Arrange
            long id = long.MaxValue;
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/person/{id}");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
