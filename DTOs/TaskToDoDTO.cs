using ProjektZespolowy.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjektZespolowy.DTOs
{
    public class TaskToDoDTO
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public string? Description { get; set; }
        public Status Status { get; set; } = Status.ToDo;
        public TaskPriority Priority { get; set; } = TaskPriority.Średni;
        public DateTime? StartAt { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int? AssignedToId { get; set; }
        public User AssignedTo { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public int? CreatedById { get; set; }
        public User CreatedBy { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<TaskTag> TaskTags { get; set; }

    }
}
