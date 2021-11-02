namespace HallOfFame.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Репозиторий для модели <see cref="Person"/>
    /// </summary>
    public class PeopleRepository : IPeopleRepository
    {
        /// <summary>
        /// Контекст данных.
        /// </summary>
        private readonly HallOfFameDbContext _context;

        /// <summary>
        ///  Репозиторий для модели <see cref="Person"/>
        /// </summary>
        /// <param name="context"> Контекст данных. </param>
        public PeopleRepository(HallOfFameDbContext context)
        {
            _context = context;
        }

        public async Task<Person> DeletePerson(long id)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                
                return null;
            }

            _context.People.Remove(person);
            await _context.SaveChangesAsync();
            return person;
        }

        public async Task<Person[]> GetPeople()
        {
            return await _context.People.Include(p => p.SkillsCollection)
                .ToArrayAsync();
        }

        public async Task<Person> GetPerson(long id)
        {
            return await _context.People.Where(p => p.Id == id)
                .Include(p => p.SkillsCollection)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> TryToCreatePerson(Person person)
        {
            await _context.People.AddAsync(person);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException exception)
            {
                FileLogger.Error("Create", exception);
                return false;
            }
        }

        public async Task<bool> TryToUpdatePerson(long id, Person person)
        {
            _context.People.Update(person);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException exception)
            {
                FileLogger.Error($"Update ID {id}", exception);
                return false;
            }
        }
    }
}