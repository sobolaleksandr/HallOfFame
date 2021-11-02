using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HallOfFame
{
    /// <summary>
    /// Модель навыка.
    /// </summary>
    public class Skill
    {
        /// <summary>
        /// ID.
        /// </summary>
        [JsonProperty(PropertyName = "Id")]
        public long Id { get; set; }

        /// <summary>
        /// Название навыка.
        /// </summary>
        [Required]
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Уровень навыка.
        /// </summary>
        [Range(1, 10)]
        [JsonProperty(PropertyName = "Level")]
        public byte Level { get; set; }

        /// <summary>
        /// ID-сотрудника которому принадлежит навык.
        /// </summary>
        [JsonProperty(PropertyName = "PersonId")]
        public long PersonId { get; set; }
    }
}
