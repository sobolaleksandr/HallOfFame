using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using HallOfFame.Data;

namespace HallOfFame.Tests
{
    public class SqliteInMemoryPeopleRepositoryTest : PeopleRepositoryTests, IDisposable
    {
        private readonly DbConnection _connection;

        public SqliteInMemoryPeopleRepositoryTest()
            : base(
                new DbContextOptionsBuilder<HallOfFameDbContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose() => _connection.Dispose();
    }
}
