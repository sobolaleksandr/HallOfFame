namespace HallOfFame.Tests
{
    using System;
    using System.Data.Common;

    using HallOfFame.Data;

    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    /// <summary>
    /// Объект для тестирования репозитория из памяти.
    /// </summary>
    public class SqliteInMemoryPeopleRepositoryTest : PeopleRepositoryTests, IDisposable
    {
        /// <summary>
        /// Подключение к базе данных.
        /// </summary>
        private readonly DbConnection _connection;

        /// <summary>
        /// Объект для тестирования репозитория из памяти.
        /// </summary>
        public SqliteInMemoryPeopleRepositoryTest() : base(new DbContextOptionsBuilder<HallOfFameDbContext>()
            .UseSqlite(CreateInMemoryDatabase())
            .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        /// <summary>
        /// Создать подключение к базе данных в памяти.
        /// </summary>
        /// <returns> Подключение к базе данных. </returns>
        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }
    }
}