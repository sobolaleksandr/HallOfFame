using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HallOfFame
{
    public class Person
    {
        [JsonProperty(PropertyName = "Id")]
        public long Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "DisplayName")]
        public string DisplayName { get; set; }

        [Required]
        [JsonProperty(PropertyName = "SkillsCollection")]
        public IEnumerable<Skill> SkillsCollection { get; set; }        
    }
}
