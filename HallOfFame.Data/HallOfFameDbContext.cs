using Microsoft.EntityFrameworkCore;

namespace HallOfFame.Data
{
    public class HallOfFameDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Skill> Skills { get; set; }

        public HallOfFameDbContext(DbContextOptions<HallOfFameDbContext> options)
            : base(options)
        {
        }
    }

}
