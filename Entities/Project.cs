using DevExpress.Blazor;
using ProjektZespolowy.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjektZespolowy.Entities
{
    public class Project
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(Owner))]
        public int OwnerId { get; set; }
        public User Owner { get; set; }

        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<TaskToDo> Tasks { get; set; }
        public ICollection<UserProject> UserProjects { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
