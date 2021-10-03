using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HallOfFame.Data;
using Xunit;

namespace HallOfFame.Tests
{
    public abstract class PeopleRepositoryTests
    {
        protected PeopleRepositoryTests(DbContextOptions<HallOfFameDbContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        protected DbContextOptions<HallOfFameDbContext> ContextOptions { get; }

        private void Seed()
        {
            using (var context = new HallOfFameDbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Person[] PersonArray = new Person[2];
                Skill[] firstPersonSkills = new Skill[2];
                firstPersonSkills[0] = new Skill { Name = "testSkillPerson1", Level = 1 };
                firstPersonSkills[1] = new Skill { Name = "testSkillPerson2", Level = 2 };

                PersonArray[0] = new Person { Id = 1, Name = "testPerson1", SkillsCollection = firstPersonSkills };
                PersonArray[1] = new Person
                {
                    Id = 2,
                    Name = "testPerson2",
                    SkillsCollection =
                    new Skill[]
                    {
                                new Skill
                                {
                                    Name="testSkillPerson12",
                                    Level = 3
                                },
                                new Skill
                                {
                                    Name="testSkillPerson22",
                                    Level = 4
                                }
                    }
                };

                context.AddRange(PersonArray);

                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetPeople_ReturnsPeople()
        {
            using (var context = new HallOfFameDbContext(ContextOptions))
            {
                var repo = new PeopleRepository(context);

                var items = await repo.GetPeople();

                Assert.Equal(2, items.Length);

                Assert.Equal(1, items[0].Id);
                Assert.Equal("testPerson1", items[0].Name);
                Assert.Null(items[0].DisplayName);
                Skill[] skillsArray = items[0].SkillsCollection.ToArray();
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
        }

        [Fact]
        public async Task GetPerson_WithGoodId_ReturnsPerson()
        {
            using (var context = new HallOfFameDbContext(ContextOptions))
            {
                var repo = new PeopleRepository(context);

                var person = await repo.GetPerson(1);

                Assert.Equal(1, person.Id);
                Assert.Equal("testPerson1", person.Name);
                Assert.Null(person.DisplayName);
                Skill[] skillsArray = person.SkillsCollection.ToArray();
                Assert.Equal(2, skillsArray.Length);

                Assert.Equal("testSkillPerson1", skillsArray[0].Name);
                Assert.Equal(1, skillsArray[0].Level);
                Assert.Equal("testSkillPerson2", skillsArray[1].Name);
                Assert.Equal(2, skillsArray[1].Level);
            }
        }

        [Fact]
        public async Task GetPerson_WithBadId_ReturnsNull()
        {
            using (var context = new HallOfFameDbContext(ContextOptions))
            {
                var repo = new PeopleRepository(context);

                var person = await repo.GetPerson(12);

                Assert.Null(person);
            }
        }

        [Fact]
        public async Task TryToUpdatePerson_WithBadId_ReturnsFalse()
        {
            using (var context = new HallOfFameDbContext(ContextOptions))
            {
                var repo = new PeopleRepository(context);

                var newPerson = new Person
                {
                    Id = 12,
                    Name = "testPerson2",
                    SkillsCollection =
                    new Skill[]
                    {
                                new Skill
                                {
                                    Name="testSkillPerson12",
                                    Level = 9
                                },
                                new Skill
                                {
                                    Name="testSkillPerson22",
                                    Level = 4
                                }
                    }
                };

                var result = await repo.TryToUpdatePerson(newPerson.Id, newPerson);

                Assert.False(result);
            }
        }

        [Fact]
        public async Task TryToUpdatePerson_WithGoodId_ReturnsTrue()
        {
            using (var context = new HallOfFameDbContext(ContextOptions))
            {
                var repo = new PeopleRepository(context);

                var newPerson = new Person
                {
                    Id = 2,
                    Name = "testPerson2",
                    SkillsCollection =
                    new Skill[]
                    {
                                new Skill
                                {
                                    Name="testSkillPerson12",
                                    Level = 9
                                },
                                new Skill
                                {
                                    Name="testSkillPerson22",
                                    Level = 9
                                }
                    }
                };

                var result = await repo.TryToUpdatePerson(newPerson.Id, newPerson);

                Assert.True(result);
            }
        }

        [Fact]
        public async Task TryToCreatePerson_WithGoodModel_ReturnsTrue()
        {
            using (var context = new HallOfFameDbContext(ContextOptions))
            {
                var repo = new PeopleRepository(context);

                var newPerson = new Person
                {
                    Id = 3,
                    Name = "testPerson3",
                    SkillsCollection =
                    new Skill[]
                    {
                                new Skill
                                {
                                    Name="testSkillPerson12",
                                    Level = 9
                                },
                                new Skill
                                {
                                    Name="testSkillPerson22",
                                    Level = 9
                                }
                    }
                };

                var result = await repo.TryToCreatePerson(newPerson);

                Assert.True(result);
            }
        }

        [Fact]
        public async Task TryToCreatePerson_WithExistingId_ReturnsFalse()
        {
            using (var context = new HallOfFameDbContext(ContextOptions))
            {
                var repo = new PeopleRepository(context);

                var newPerson = new Person
                {
                    Id = 2,
                    Name = "testPerson2",
                    SkillsCollection =
                    new Skill[]
                    {
                                new Skill
                                {
                                    Name="testSkillPerson12",
                                    Level = 11
                                },
                                new Skill
                                {
                                    Name="testSkillPerson22",
                                    Level = 9
                                }
                    }
                };

                var result = await repo.TryToCreatePerson(newPerson);

                Assert.False(result);
            }
        }

        [Fact]
        public async Task DeletePerson_WithGoodId_ReturnsPerson()
        {
            using (var context = new HallOfFameDbContext(ContextOptions))
            {
                var repo = new PeopleRepository(context);

                var newPerson = new Person
                {
                    Id = 2,
                    Name = "testPerson2",
                };

                Person result = await repo.DeletePerson(newPerson.Id);

                Assert.Equal(newPerson.Id, result.Id);
                Assert.Equal(newPerson.Name, result.Name);
            }
        }

        [Fact]
        public async Task DeletePerson_WithBadId_ReturnsNull()
        {
            using (var context = new HallOfFameDbContext(ContextOptions))
            {
                var repo = new PeopleRepository(context);

                var newPerson = new Person
                {
                    Id = 12,
                    Name = "testPerson2",
                };

                Person result = await repo.DeletePerson(newPerson.Id);

                Assert.Null(result);
            }
        }

    }
}
