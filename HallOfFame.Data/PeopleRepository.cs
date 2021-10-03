using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HallOfFame.Data
{
    public class PeopleRepository : IPeopleRepository
    {
        private readonly HallOfFameDbContext _context;

        public PeopleRepository(HallOfFameDbContext context)
        {
            _context = context;
        }

        public async Task<Person[]> GetPeople()
        {
            var people = await _context.People.Include(p => p.SkillsCollection)
                                              .ToArrayAsync();
            return people;
        }

        public async Task<Person> GetPerson(long id)
        {
            var person = await _context.People.Where(p=>p.Id == id)
                                              .Include(p=>p.SkillsCollection)
                                              .FirstOrDefaultAsync();                
            return person;
        }

        public async Task<bool> TryToUpdatePerson(long id, Person person)
        {
            _context.People.Update(person);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        public async Task<Person> DeletePerson(long id)
        {
            var person = await _context.People.FindAsync(id);

            if (person == null)
                return null;

            _context.People.Remove(person);
            await _context.SaveChangesAsync();

            return person;
        }

        public async Task<bool> TryToCreatePerson(Person person)
        {
            _context.People.Add(person);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }
    }

}
