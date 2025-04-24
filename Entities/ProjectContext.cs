using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjektZespolowy.Entities;

namespace ProjektZespolowy.Entities
{
    public class ProjectContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TaskToDo> TasksToDo { get; set; }
        public DbSet<TaskTag> TaskTags { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<Category> Category { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TaskTag>()
               .HasKey(tt => new { tt.TaskId, tt.TagId });

            modelBuilder.Entity<UserProject>()
                .HasKey(up => new { up.UsernameId, up.ProjectId });

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
               .HasOne(c => c.Task)
               .WithMany(t => t.Comments)
               .HasForeignKey(c => c.TaskId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserProject>()
                .HasOne(up => up.Username)
                .WithMany()
                .HasForeignKey(up => up.UsernameId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProject>()
                .HasOne(up => up.Project)
                .WithMany(p => p.UserProjects)
                .HasForeignKey(up => up.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskToDo>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
