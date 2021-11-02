using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HallOfFame.Web.Controllers
{
    /// <summary>
    /// Контроллер для модели <see cref="Person"/>
    /// </summary>
    [Produces("application/json")]
    [Route("")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        /// <summary>
        /// Репозиторий.
        /// </summary>
        private readonly IPeopleRepository _peopleRepository;

        /// <summary>
        /// Контроллер для модели <see cref="Person"/>
        /// </summary>
        /// <param name="peopleRepository"> Репозиторий. </param>
        public PeopleController(IPeopleRepository peopleRepository)
        {
            _peopleRepository = peopleRepository;
        }

        /// <summary>
        /// Получить всех сотрудников.
        /// </summary>
        /// <returns> Коллекцию сотрудников. </returns>
        [Route("api/v1/persons")]
        [HttpGet]
        public async Task<Person[]> GetPersons()
        {
            return await _peopleRepository.GetPeople();
        }

        /// <summary>
        /// Получить сотрудника.
        /// </summary>
        /// <param name="id"> ID-сотрудника. </param>
        /// <returns> Модель сотрудника, если получилось создать, иначе <see cref="NotFoundResult"/> </returns>
        [HttpGet("api/v1/person/{id}")]
        public async Task<ActionResult<Person>> GetPerson(long id)
        {
            var person = await _peopleRepository.GetPerson(id);
            if (person != null)
            {
                FileLogger.Debug($"Get {id}", person);
                return new ObjectResult(person);
            }

            FileLogger.Warn($"{id} not found");
            return NotFound();
        }

        /// <summary>
        /// Создать сотрудника. 
        /// </summary>
        /// <param name="person"> Модель сотрудника. </param>
        /// <returns> <see cref="OkResult"/>, если получилось создать сотрудника, иначе <see cref="BadRequestResult"/> </returns>
        [Route("api/v1/person")]
        [HttpPost]
        public async Task<IActionResult> CreatePerson(Person person)
        {
            if (person != null && ModelState.IsValid)
            {
                if (await _peopleRepository.TryToCreatePerson(person))
                {
                    FileLogger.Debug("Post", person);
                    return Ok();
                }
            }

            FileLogger.Warn($"Post failed");
            return BadRequest();
        }

        [HttpPut("api/v1/person/{id?}")]
        public async Task<IActionResult> UpdatePerson(long? id, Person person)
        {
            if (!ModelState.IsValid || !id.HasValue)
                return BadRequest();

            if (id != person.Id)
                return BadRequest();

            if (await _peopleRepository.TryToUpdatePerson(id.Value, person))
                return Ok();

            return NotFound();
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
