using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HallOfFame.Web.Controllers
{
    using Newtonsoft.Json;

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

            FileLogger.Warn($"Get {id} not found");
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
            var serializedPerson = JsonConvert.SerializeObject(person);
            if (person != null && ModelState.IsValid)
            {
                if (await _peopleRepository.TryToCreatePerson(person))
                {
                    FileLogger.Debug("Post succeeded", serializedPerson);
                    return Ok();
                }
            }

            FileLogger.Warn($"Post failed {serializedPerson}");
            return BadRequest();
        }

        /// <summary>
        /// Обновить сотрудника. 
        /// </summary>
        /// <param name="id"> ID-сотрудника. </param>
        /// <param name="person"> Модель сотрудника. </param>
        /// <returns> <see cref="OkResult"/> если получилось обновить, <see cref="NotFoundResult"/> если сотрудника нет в базе данных, <see cref="BadRequestResult"/> если неверная модель запроса. </returns>
        [HttpPut("api/v1/person/{id?}")]
        public async Task<IActionResult> UpdatePerson(long? id, Person person)
        {
            if (!ModelState.IsValid || !id.HasValue || id != person.Id)
            {
                FileLogger.Warn($"Put BadRequest {id}");
                return BadRequest();
            }

            if (await _peopleRepository.TryToUpdatePerson(id.Value, person))
            {
                FileLogger.Debug($"Put succeeded {id}", person);
                return Ok();
            }

            FileLogger.Warn($"Put NotFound {id}");
            return NotFound();
        }

        /// <summary>
        /// Удалить сотрудника.
        /// </summary>
        /// <param name="id"> ID-сотрудника. </param>
        /// <returns> <see cref="OkResult"/> если получилось удалить, <see cref="NotFoundResult"/> если сотрудника нет в базе данных. </returns>
        [HttpDelete("api/v1/person/{id}")]
        public async Task<IActionResult> DeletePerson(long id)
        {
            var person = await _peopleRepository.DeletePerson(id);
            if (person == null)
            {
                FileLogger.Warn($"Delete NotFound {id}");
                return NotFound();
            }

            FileLogger.Debug($"Delete NotFound {id}");
            return Ok();
        }
    }
}
