using ProjektZespolowy.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProjektZespolowy.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }

        public ICollection<TaskTag> TaskTags { get; set; }
    }
}
