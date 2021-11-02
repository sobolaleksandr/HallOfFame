namespace HallOfFame.Data
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Контекст данных.
    /// </summary>
    public class HallOfFameDbContext : DbContext
    {
        /// <summary>
        /// Контекст данных.
        /// </summary>
        public HallOfFameDbContext(DbContextOptions<HallOfFameDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Таблица сотрудников.
        /// </summary>
        public DbSet<Person> People { get; set; }

        /// <summary>
        /// Таблица навыков.
        /// </summary>
        public DbSet<Skill> Skills { get; set; }
    }
}