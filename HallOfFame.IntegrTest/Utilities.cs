using HallOfFame.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallOfFame.IntegrTest
{
    public static class Utilities
    {
        #region snippet1
        public static void InitializeDbForTests(HallOfFameDbContext db)
        {
            db.People.AddRange(GetSeedingPeople());
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(HallOfFameDbContext db)
        {
            db.People.RemoveRange(db.People);
            InitializeDbForTests(db);
        }

        public static List<Person> GetSeedingPeople()
        {
            return new List<Person>()
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
        }
        #endregion
    }
}
