namespace HallOfFame.IntegrationTest
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using HallOfFame.Web;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;

    using Newtonsoft.Json;

    using Xunit;

    /// <summary>
    /// Интеграционные тесты.
    /// </summary>
    public class PeopleTest
    {
        /// <summary>
        /// Интеграционные тесты.
        /// </summary>
        public PeopleTest()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true).Build();

            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .UseEnvironment("Development")
                .UseConfiguration(configuration));

            _client = server.CreateClient();
        }

        /// <summary>
        /// Клиент.
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>
        /// Модель для теста.
        /// </summary>
        private Person _testPerson;

        /// <summary>
        /// Строка запроса.
        /// </summary>
        private const string REQUEST_URI = "/api/v1/person/";

        /// <summary>
        /// Тест метода POST с валидной моделью.
        /// </summary>
        /// <returns> <see cref="HttpStatusCode.OK"/> </returns>
        private async Task CreatePersonWithValidModel()
        {
            var person =
                new Person
                {
                    Name = TEST_NAME,
                    SkillsCollection =
                        new[]
                        {
                            new Skill
                            {
                                Name = "TestSkill",
                                Level = 9
                            },
                            new Skill
                            {
                                Name = "TestSkill2",
                                Level = 9
                            }
                        }
                };

            var request = CreateRequest(person, HttpMethod.Post, REQUEST_URI);

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Создать запрос.
        /// </summary>
        /// <param name="person"> Модель. </param>
        /// <param name="method"> <see cref="HttpMethod"/></param>
        /// <param name="requestUri"> Строка запроса. </param>
        /// <returns> Возвращает запрос. </returns>
        private static HttpRequestMessage CreateRequest(Person person, HttpMethod method, string requestUri)
        {
            var body = JsonConvert.SerializeObject(person);
            return new HttpRequestMessage(method, requestUri)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
        }

        /// <summary>
        /// Тест метода GET с объектом в базе.
        /// </summary>
        /// <returns> <see cref="HttpStatusCode.OK"/> </returns>
        private async Task GetPeople()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/persons");

            var content = await GetContent(request);
            var jsonPersons = JsonConvert.DeserializeObject<List<Person>>(content);
            _testPerson = jsonPersons.FirstOrDefault(p => p.Name == TEST_NAME);

            Assert.NotNull(_testPerson);
        }

        /// <summary>
        /// Тестовый атрибут.
        /// </summary>
        private const string TEST_NAME = "TestsName";

        /// <summary>
        /// Тест метода PUT с валидным ID и объектом в базе.
        /// </summary>
        /// <returns> <see cref="HttpStatusCode.OK"/> </returns>
        private async Task UpdatePersonWithGoodModel()
        {
            var skillsArray = _testPerson.SkillsCollection.ToArray();
            skillsArray[0].Level = 1;
            skillsArray[1].Level = 1;
            var request = CreateRequest(_testPerson, HttpMethod.Put, ValidIdRequest);

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Тест метода GET с валидным ID и объектом в базе.
        /// </summary>
        /// <returns> <see cref="HttpStatusCode.OK"/> </returns>
        private async Task GetPersonWithExisting()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, ValidIdRequest);

            var content = await GetContent(request);
            var jsonPerson = JsonConvert.DeserializeObject<Person>(content);

            Assert.Equal(TEST_NAME, jsonPerson.Name);
            Assert.Null(jsonPerson.DisplayName);

            var skillsArray = jsonPerson.SkillsCollection.ToArray();

            Assert.Equal(2, skillsArray.Length);
            Assert.Equal("TestSkill", skillsArray[0].Name);
            Assert.Equal(1, skillsArray[0].Level);
            Assert.Equal("TestSkill2", skillsArray[1].Name);
            Assert.Equal(1, skillsArray[1].Level);
        }

        /// <summary>
        /// Получить содержимое ответа. 
        /// </summary>
        /// <param name="request"> Запрос. </param>
        /// <returns> Содержимое ответа. </returns>
        private async Task<string> GetContent(HttpRequestMessage request)
        {
            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return content;
        }

        /// <summary>
        /// Тест метода DELETE с валидным ID и объектом в базе.
        /// </summary>
        /// <returns> <see cref="HttpStatusCode.OK"/> </returns>
        private async Task DeletePersonWithGoodId()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, ValidIdRequest);

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Запрос с валидным ID.
        /// </summary>
        private string ValidIdRequest => REQUEST_URI + _testPerson.Id;

        /// <summary>
        /// Запрос с невалидным ID.
        /// </summary>
        private static string InValidIdRequest => REQUEST_URI + long.MaxValue;

        /// <summary>
        /// Тест метода POST c невалидной моделью.
        /// </summary>
        /// <returns> <see cref="HttpStatusCode.BadRequest"/> </returns>
        [Fact]
        internal async Task CreatePersonWithBadModel()
        {
            var person =
                new Person
                {
                    Name = "TestName",
                    SkillsCollection =
                        new[]
                        {
                            new Skill
                            {
                                Name = "TestSkill",
                                Level = 11
                            }
                        }
                };

            var request = CreateRequest(person, HttpMethod.Post, REQUEST_URI);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Тест метода POST c невалидной моделью.
        /// </summary>
        /// <returns> <see cref="HttpStatusCode.BadRequest"/> </returns>
        [Fact]
        internal async Task CreatePersonWithNull()
        {
            var request = CreateRequest(null, HttpMethod.Post, REQUEST_URI);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Тест метода DELETE c невалидным ID.
        /// </summary>
        /// <returns> <see cref="HttpStatusCode.NotFound"/> </returns>
        [Fact]
        internal async Task DeletePersonWithBadId()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, InValidIdRequest);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        /// Тест метода GET объекта, которого нет в базе.
        /// </summary>
        /// <returns> <see cref="HttpStatusCode.NotFound"/> </returns>
        [Fact]
        internal async Task GetNonExistingPerson()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, InValidIdRequest);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        /// CRUD-операции.
        /// </summary>
        [Fact]
        public async Task PeopleTests()
        {
            await CreatePersonWithValidModel();
            await GetPeople();
            await UpdatePersonWithGoodModel();
            await GetPersonWithExisting();
            await DeletePersonWithGoodId();
        }

        /// <summary>
        /// Тест метода UPDATE с невалидным ID.
        /// </summary>
        /// <returns> <see cref="HttpStatusCode.BadRequest"/> </returns>
        [Fact]
        internal async Task UpdatePersonWithBadId()
        {
            const long id = 1;
            var person =
                new Person
                {
                    Id = 12,
                    Name = "TestName",
                    SkillsCollection =
                        new[]
                        {
                            new Skill
                            {
                                Name = "TestSkill",
                                Level = 1
                            }
                        }
                };

            var request = CreateRequest(person, HttpMethod.Put, REQUEST_URI + id);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Тест метода PUT с невалидной моделью.
        /// </summary>
        /// <returns> <see cref="HttpStatusCode.BadRequest" /></returns>
        [Fact]
        internal async Task UpdatePersonWithBadModel()
        {
            const long id = 1;
            var person =
                new Person
                {
                    Id = id,
                    Name = "TestName",
                    SkillsCollection =
                        new[]
                        {
                            new Skill
                            {
                                Name = "TestSkill",
                                Level = 11
                            }
                        }
                };

            var request = CreateRequest(person, HttpMethod.Put, REQUEST_URI + id);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Тест метода PUT с несуществующим объектом. 
        /// </summary>
        /// <returns> <see cref="HttpStatusCode.NotFound" /></returns>
        [Fact]
        internal async Task UpdatePersonWithNonExistingPerson()
        {
            var person =
                new Person
                {
                    Id = long.MaxValue,
                    Name = TEST_NAME,
                    SkillsCollection =
                        new[]
                        {
                            new Skill
                            {
                                Name = "TestSkill",
                                Level = 9
                            },
                            new Skill
                            {
                                Name = "TestSkill2",
                                Level = 9
                            }
                        }
                };

            var request = CreateRequest(person, HttpMethod.Put, InValidIdRequest);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        /// Тест метода PUT с null в строке запроса
        /// </summary>
        /// <returns> <see cref="HttpStatusCode.BadRequest" /></returns>
        [Fact]
        internal async Task UpdatePersonWithNullId()
        {
            var person = new Person
            {
                Id = 1,
                Name = "TestName",
                SkillsCollection =
                    new[]
                    {
                        new Skill
                        {
                            Name = "TestSkill",
                            Level = 1
                        }
                    }
            };

            var request = CreateRequest(person, HttpMethod.Put, REQUEST_URI + (long?)null);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}