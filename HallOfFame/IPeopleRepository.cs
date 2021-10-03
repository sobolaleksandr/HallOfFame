using System.Threading.Tasks;

namespace HallOfFame
{
    public interface IPeopleRepository
    {
        Task<Person[]> GetPeople();
        Task<Person> GetPerson(long id);
        Task<bool> TryToUpdatePerson(long id, Person person);
        Task<bool> TryToCreatePerson(Person person);
        Task<Person> DeletePerson(long id);
    }
}
