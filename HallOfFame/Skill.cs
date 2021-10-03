using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HallOfFame
{
    public class Skill
    {
        [JsonProperty(PropertyName = "Id")]
        public long Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [Range(1, 10)]
        [JsonProperty(PropertyName = "Level")]
        public byte Level { get; set; }

        [JsonProperty(PropertyName = "PersonId")]
        public long PersonId { get; set; }
    }
}
