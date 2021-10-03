using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HallOfFame.Web.Controllers
{
    [Produces("application/json")]
    [Route("")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly IPeopleRepository _peopleRepository;

        public PeopleController(IPeopleRepository peopleRepository)
        {
            _peopleRepository = peopleRepository;
        }

        [Route("api/v1/persons")]
        [HttpGet]
        public async Task<Person[]> GetPersons()
        {
            var persons = await _peopleRepository.GetPeople();

            return persons;
        }

        [HttpGet("api/v1/person/{id}")]
        public async Task<ActionResult<Person>> GetPerson(long id)
        {
            var person = await _peopleRepository.GetPerson(id);

            if (person == null)
                return NotFound();

            return new ObjectResult(person);
        }

        [Route("api/v1/person")]
        [HttpPost]
        public async Task<IActionResult> CreatePerson(Person person)
        {
            if (person != null && ModelState.IsValid)
                if (await _peopleRepository.TryToCreatePerson(person))
                    return Ok();

            return BadRequest();
        }

        [HttpPut("api/v1/person/{id?}")]
        public async Task<IActionResult> UpdatePerson(long? id, Person person)
        {
            if (ModelState.IsValid && id.HasValue)
                if (id == person.Id)
                    if (await _peopleRepository.TryToUpdatePerson(id.Value, person))
                        return Ok();
                    else
                        return NotFound();

            return BadRequest();
        }

        [HttpDelete("api/v1/person/{id}")]
        public async Task<IActionResult> DeletePerson(long id)
        {
            var person = await _peopleRepository.DeletePerson(id);
            if (person == null)
                return NotFound();

            return Ok();
        }
    }
}
