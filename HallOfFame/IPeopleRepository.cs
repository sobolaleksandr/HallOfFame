namespace HallOfFame
{
    using System.Threading.Tasks;

    /// <summary>
    /// Репозиторий для модели <see cref="Person"/>
    /// </summary>
    public interface IPeopleRepository
    {
        /// <summary>
        /// Удалить объект. 
        /// </summary>
        /// <param name="id"> ID-объекта.  </param>
        /// <returns> Null, если не получилось удалить. </returns>
        Task<Person> DeletePerson(long id);

        /// <summary>
        /// Получить все объекты.
        /// </summary>
        /// <returns></returns>
        Task<Person[]> GetPeople();

        /// <summary>
        /// Получить объект по ID.
        /// </summary>
        /// <param name="id"> ID-объекта. </param>
        /// <returns> Объект, если он существует, иначе null. </returns>
        Task<Person> GetPerson(long id);

        /// <summary>
        /// Создать объект.
        /// </summary>
        /// <param name="person"> Модель для создания. </param>
        /// <returns> True, если получилось создать. </returns>
        Task<bool> TryToCreatePerson(Person person);

        /// <summary>
        /// Обновить объект.
        /// </summary>
        /// <param name="id"> ID-объекта. </param>
        /// <param name="person"> Модель для обновления. </param>
        /// <returns> True, если получилось обновить. </returns>
        Task<bool> TryToUpdatePerson(long id, Person person);
    }
}