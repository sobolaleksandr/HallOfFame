using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HallOfFame
{
    /// <summary>
    /// Модель сотрудника. 
    /// </summary>
    public class Person
    {
        /// <summary>
        /// ID.
        /// </summary>
        [JsonProperty(PropertyName = "Id")]
        public long Id { get; set; }

        /// <summary>
        /// Имя.
        /// </summary>
        [Required]
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Отображаемое имя.
        /// </summary>
        [JsonProperty(PropertyName = "DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Набор навыков.
        /// </summary>
        [Required]
        [JsonProperty(PropertyName = "SkillsCollection")]
        public IEnumerable<Skill> SkillsCollection { get; set; }        
    }
}
