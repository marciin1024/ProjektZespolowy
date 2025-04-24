using Microsoft.AspNetCore.Identity;
using ProjektZespolowy.Enums;
using ProjektZespolowy.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProjektZespolowy.Entities
{
    public class User : IdentityUser<int>
    {
        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        public LabelId LabelId { get; set; } = LabelId.Light;

        public string FullName => $"{FirstName} {LastName}";

        public ICollection<Comment> Comments { get; set; }
        public ICollection<Project> OwnedProjects { get; set; }
        public ICollection<UserProject> CollaboratedProjects { get; set; }
        public ICollection<TaskToDo> AssignedTasks { get; set; }
        public ICollection<TaskToDo> CreatedTasks { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}
