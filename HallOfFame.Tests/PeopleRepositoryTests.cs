namespace HallOfFame.Tests
{
    using System.Linq;
    using System.Threading.Tasks;

    using HallOfFame.Data;

    using Microsoft.EntityFrameworkCore;

    using Xunit;

    /// <summary>
    /// Тесты репозитория <see cref="PeopleRepository"/>
    /// </summary>
    public abstract class PeopleRepositoryTests
    {
        /// <summary>
        /// Тесты репозитория <see cref="PeopleRepository"/>
        /// </summary>
        protected PeopleRepositoryTests(DbContextOptions<HallOfFameDbContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Seed();
        }

        protected DbContextOptions<HallOfFameDbContext> ContextOptions { get; }

        /// <summary>
        /// Заполнить базу данных.
        /// </summary>
        private void Seed()
        {
            using var context = new HallOfFameDbContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var firstPersonSkills = new[]
            {
                new Skill { Name = "testSkillPerson1", Level = 1 },
                new Skill { Name = "testSkillPerson2", Level = 2 }
            };

            object[] personArray =
            {
                new Person { Id = 1, Name = "testPerson1", SkillsCollection = firstPersonSkills }, new Person
                {
                    Id = VALID_TEST_ID,
                    Name = "testPerson2",
                    SkillsCollection =
                        new[]
                        {
                            new Skill
                            {
                                Name = "testSkillPerson12",
                                Level = 3
                            },
                            new Skill
                            {
                                Name = "testSkillPerson22",
                                Level = 4
                            }
                        }
                }
            };

            context.AddRange(personArray);
            context.SaveChanges();
        }

        /// <summary>
        /// Невалидный тестовый ID.
        /// </summary>
        private const long INVALID_TEST_ID = 12;

        /// <summary>
        /// Валидный тестовый ID.
        /// </summary>
        private const long VALID_TEST_ID = 2;

        /// <summary>
        /// Тест метода DeletePerson с валидным ID.
        /// </summary>
        /// <returns> Модель сотрудника. </returns>
        [Fact]
        public async Task DeletePerson()
        {
            await using var context = new HallOfFameDbContext(ContextOptions);
            var repo = new PeopleRepository(context);

            var newPerson = new Person
            {
                Id = VALID_TEST_ID,
                Name = "testPerson2",
            };

            var result = await repo.DeletePerson(newPerson.Id);

            Assert.Equal(newPerson.Id, result.Id);
            Assert.Equal(newPerson.Name, result.Name);
        }

        /// <summary>
        /// Тест метода DeletePerson с невалидным ID.
        /// </summary>
        /// <returns> Null. </returns>
        [Fact]
        public async Task DeletePersonWithBadId()
        {
            await using var context = new HallOfFameDbContext(ContextOptions);
            var repo = new PeopleRepository(context);
            var newPerson = new Person
            {
                Id = INVALID_TEST_ID,
                Name = "testPerson2",
            };

            var result = await repo.DeletePerson(newPerson.Id);

            Assert.Null(result);
        }

        /// <summary>
        /// Тест метода GetPeople.
        /// </summary>
        /// <returns> Коллекцию сотрудников. </returns>
        [Fact]
        public async Task GetPeople()
        {
            await using var context = new HallOfFameDbContext(ContextOptions);
            var repo = new PeopleRepository(context);

            var items = await repo.GetPeople();

            Assert.Equal(2, items.Length);

            Assert.Equal(1, items[0].Id);
            Assert.Equal("testPerson1", items[0].Name);
            Assert.Null(items[0].DisplayName);
            var skillsArray = items[0].SkillsCollection.ToArray();
            Assert.Equal(2, skillsArray.Length);

            Assert.Equal("testSkillPerson1", skillsArray[0].Name);
            Assert.Equal(1, skillsArray[0].Level);
            Assert.Equal("testSkillPerson2", skillsArray[1].Name);
            Assert.Equal(2, skillsArray[1].Level);

            Assert.Equal(2, items[1].Id);
            Assert.Equal("testPerson2", items[1].Name);
            Assert.Null(items[1].DisplayName);
            skillsArray = items[1].SkillsCollection.ToArray();
            Assert.Equal(2, skillsArray.Length);

            Assert.Equal("testSkillPerson12", skillsArray[0].Name);
            Assert.Equal(3, skillsArray[0].Level);
            Assert.Equal("testSkillPerson22", skillsArray[1].Name);
            Assert.Equal(4, skillsArray[1].Level);
        }

        /// <summary>
        /// Тест метода GetPerson с невалидным ID.
        /// </summary>
        /// <returns> Null. </returns>
        [Fact]
        public async Task GetPersonWithBadId()
        {
            await using var context = new HallOfFameDbContext(ContextOptions);
            var repo = new PeopleRepository(context);

            var person = await repo.GetPerson(INVALID_TEST_ID);

            Assert.Null(person);
        }

        /// <summary>
        /// Тест метода GetPerson с валидным ID.
        /// </summary>
        /// <returns> Модель сотрудника. </returns>
        [Fact]
        public async Task GetPersonWithGoodId()
        {
            await using var context = new HallOfFameDbContext(ContextOptions);
            var repo = new PeopleRepository(context);

            var person = await repo.GetPerson(1);

            Assert.Equal(1, person.Id);
            Assert.Equal("testPerson1", person.Name);
            Assert.Null(person.DisplayName);
            var skillsArray = person.SkillsCollection.ToArray();
            Assert.Equal(2, skillsArray.Length);

            Assert.Equal("testSkillPerson1", skillsArray[0].Name);
            Assert.Equal(1, skillsArray[0].Level);
            Assert.Equal("testSkillPerson2", skillsArray[1].Name);
            Assert.Equal(2, skillsArray[1].Level);
        }

        /// <summary>
        /// Тест метода TryToCreatePerson с новым объектом.
        /// </summary>
        /// <returns> True. </returns>
        [Fact]
        public async Task TryToCreatePerson()
        {
            await using var context = new HallOfFameDbContext(ContextOptions);
            var repo = new PeopleRepository(context);

            var newPerson = new Person
            {
                Id = 3,
                Name = "testPerson3",
                SkillsCollection =
                    new[]
                    {
                        new Skill
                        {
                            Name = "testSkillPerson12",
                            Level = 9
                        },
                        new Skill
                        {
                            Name = "testSkillPerson22",
                            Level = 9
                        }
                    }
            };

            var result = await repo.TryToCreatePerson(newPerson);

            Assert.True(result);
        }

        /// <summary>
        /// Тест метода TryToCreatePerson с существующим объектом.
        /// </summary>
        /// <returns> False. </returns>
        [Fact]
        public async Task TryToCreatePersonWithExistingId()
        {
            await using var context = new HallOfFameDbContext(ContextOptions);
            var repo = new PeopleRepository(context);

            var newPerson = new Person
            {
                Id = VALID_TEST_ID,
                Name = "testPerson2",
                SkillsCollection =
                    new[]
                    {
                        new Skill
                        {
                            Name = "testSkillPerson12",
                            Level = 11
                        },
                        new Skill
                        {
                            Name = "testSkillPerson22",
                            Level = 9
                        }
                    }
            };

            var result = await repo.TryToCreatePerson(newPerson);

            Assert.False(result);
        }

        /// <summary>
        /// Тест метода TryToUpdatePerson с валидным ID.
        /// </summary>
        /// <returns> True. </returns>
        [Fact]
        public async Task TryToUpdatePerson()
        {
            await using var context = new HallOfFameDbContext(ContextOptions);
            var repo = new PeopleRepository(context);

            var newPerson = new Person
            {
                Id = VALID_TEST_ID,
                Name = "testPerson2",
                SkillsCollection =
                    new[]
                    {
                        new Skill
                        {
                            Name = "testSkillPerson12",
                            Level = 9
                        },
                        new Skill
                        {
                            Name = "testSkillPerson22",
                            Level = 9
                        }
                    }
            };

            var result = await repo.TryToUpdatePerson(newPerson.Id, newPerson);

            Assert.True(result);
        }

        /// <summary>
        /// Тест метода TryToUpdatePerson с невалидным ID.
        /// </summary>
        /// <returns> False. </returns>
        [Fact]
        public async Task TryToUpdatePersonWithBadId()
        {
            await using var context = new HallOfFameDbContext(ContextOptions);
            var repo = new PeopleRepository(context);

            var newPerson = new Person
            {
                Id = INVALID_TEST_ID,
                Name = "testPerson2",
                SkillsCollection =
                    new[]
                    {
                        new Skill
                        {
                            Name = "testSkillPerson12",
                            Level = 9
                        },
                        new Skill
                        {
                            Name = "testSkillPerson22",
                            Level = 4
                        }
                    }
            };

            var result = await repo.TryToUpdatePerson(newPerson.Id, newPerson);

            Assert.False(result);
        }
    }
}