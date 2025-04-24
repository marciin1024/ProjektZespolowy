using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjektZespolowy.Entities
{
    public class UserProject
    {
        [ForeignKey(nameof(User))]
        public int UsernameId { get; set; }
        public User Username { get; set; }

        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public UserProjectRole Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum UserProjectRole
    {
        Owner,
        Collaborator
    }
}
