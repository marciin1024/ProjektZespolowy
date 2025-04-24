using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ProjektZespolowy.Entities;

namespace ProjektZespolowy.Services
{
    public class ProjectService
    {
        private readonly IDbContextFactory<ProjectContext> _contextFactory;

        public ProjectService(IDbContextFactory<ProjectContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // 1. Dodawanie projektu
        public async Task AddProjectAsync(Project project)
        {
            using ProjectContext context = await _contextFactory.CreateDbContextAsync();

            project.CreatedAt = DateTime.UtcNow;
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        // 2. Pobranie wszystkich projektów
        public async Task<List<Project>> GetAllProjectsAsync()
        {
            using ProjectContext context = await _contextFactory.CreateDbContextAsync();
            return await context.Projects
                .Include(project => project.Category)
                .Include(project => project.Owner)
                .Include(project => project.Owner)
                .Include(project => project.Category)
                .Include(project => project.UserProjects)
                .ThenInclude(up => up.Username)
                .Include(project => project.Tasks)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
        }

        public List<Project> GetAllProjects()
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            return context.Projects
                .Include(project => project.Category)
                .Include(project => project.Owner)
                .Include(project => project.Owner)
                .Include(project => project.Category)
                .Include(project => project.UserProjects)
                .ThenInclude(up => up.Username)
                .Include(project => project.Tasks)
                .OrderBy(t => t.CreatedAt)
                .ToList();
        }

        // 3. Aktualizacja projektu
        public async Task UpdateProjectAsync(Project project, IEnumerable<User> users)
        {
            using ProjectContext context = await _contextFactory.CreateDbContextAsync();
            var existingProject = await context.Projects
                .Include(x => x.UserProjects)
                .FirstOrDefaultAsync(x => x.Id == project.Id);

            if (existingProject == null)
            {
                throw new Exception("Project not found.");
            }

            existingProject.Name = project.Name;
            existingProject.Description = project.Description;
            existingProject.DueDate = project.DueDate;
            existingProject.CreatedAt = project.CreatedAt;
            existingProject.UpdatedAt = DateTime.UtcNow;
            existingProject.OwnerId = project.OwnerId;
            existingProject.CategoryId = project.CategoryId;
            existingProject.StartAt = project.StartAt;

            if (existingProject.UserProjects is not null
                && existingProject.UserProjects.Any())
            {
                context.RemoveRange(existingProject.UserProjects);
            }

            foreach (User user in users)
            {
                UserProject userProject = new UserProject
                {
                    ProjectId = project.Id,
                    UsernameId = user.Id
                };
                context.Add(userProject);
            }

            context.Projects.Update(existingProject);
            await context.SaveChangesAsync();
        }

        // 4. Usuwanie projektu
        public async Task DeleteProjectAsync(Project project)
        {
            using ProjectContext context = await _contextFactory.CreateDbContextAsync();
            var proj = await context.Projects.FindAsync(project.Id);
            if (proj == null)
            {
                throw new Exception("Project not found.");
            }
            context.Projects.Remove(proj);
            await context.SaveChangesAsync();
        }

        // 5. Pobranie wszystkich kategorii
        public async Task<List<Category>> GetCategoriesAsync()
        {
            using ProjectContext context = await _contextFactory.CreateDbContextAsync();
            return await context.Category
                .Include(x => x.Projects)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            using ProjectContext context = await _contextFactory.CreateDbContextAsync();
            context.Category.Add(category);
            await context.SaveChangesAsync();
        }

        public async Task<Project> GetProjectByIdAsync(int projectId)
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            return await context.Projects
                .Include(project => project.Category)
                .Include(project => project.Owner)
                .Include(project => project.Owner)
                .Include(project => project.Category)
                .Include(project => project.UserProjects)
                .ThenInclude(up => up.Username)
                .FirstOrDefaultAsync(project => project.Id == projectId);
        }

        public Project GetProjectById(int projectId)
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            return context.Projects
                .Include(project => project.Category)
                .Include(project => project.Owner)
                .Include(project => project.Owner)
                .Include(project => project.Category)
                .Include(project => project.Tasks)
                .Include(project => project.UserProjects)
                .ThenInclude(up => up.Username)
                .FirstOrDefault(project => project.Id == projectId);
        }
    }
}