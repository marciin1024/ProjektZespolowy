using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ProjektZespolowy.Entities;
using Microsoft.VisualBasic;
//using DevExpress.XtraRichEdit.Import.Html;

namespace ProjektZespolowy.Services
{
    public class TaskService
    {
        IDbContextFactory<ProjectContext> _contextFactory;

        public TaskService(IDbContextFactory<ProjectContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // 1. Dodawanie zadania
        public async Task AddTaskAsync(TaskToDo task)
        {
            using ProjectContext context = _contextFactory.CreateDbContext();

            DateOnly dateOnlyNow = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            TimeOnly timeOnlyStartAt = new TimeOnly(DateTime.Now.Hour + 1, 0, 0);
            TimeOnly timeOnlyDayEnd = new TimeOnly(23, 59, 0);

            TaskToDo taskToDo = new TaskToDo();
            taskToDo.Name = task.Name;
            taskToDo.Description = task.Description;
            taskToDo.Status = task.Status;
            taskToDo.Priority = task.Priority;
            taskToDo.AssignedToId = task.AssignedTo?.Id;
            taskToDo.CreatedById = task.CreatedBy?.Id;
            taskToDo.ProjectId = 2;

            if (task.StartAt is null)
            {
                taskToDo.StartAt = new DateTime(dateOnlyNow, timeOnlyStartAt);
            }
            else
            {
                taskToDo.StartAt = task.StartAt;
            }

            if (task.DueDate is null)
            {
                taskToDo.DueDate = new DateTime(dateOnlyNow, timeOnlyDayEnd);
            }
            else
            {
                taskToDo.DueDate = task.DueDate;
            }

            taskToDo.CreatedAt = DateTime.UtcNow;
            context.TasksToDo.Add(taskToDo);
            await context.SaveChangesAsync();
        }

        // 2. Pobranie wszystkich zadań
        public async Task<List<TaskToDo>> GetAllTasksToDoAsync()
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            return await context.TasksToDo
                .Include(x => x.AssignedTo)
                .Include(x => x.TaskTags)
                .ThenInclude(x => x.Tag)
                .Include(x => x.Project)
                .Include(x => x.Comments)
                .OrderBy(t => t.Status)
                .ThenByDescending(t => t.Id)
                .ToListAsync();
        }

        public List<TaskToDo> GetAllTasksToDo()
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            return context.TasksToDo
                .Include(x => x.AssignedTo)
                .Include(x => x.TaskTags)
                .ThenInclude(x => x.Tag)
                .Include(x => x.Project)
                .Include(x => x.Comments)
                .OrderBy(t => t.Status)
                .ThenByDescending(t => t.Id)
                .ToList();
        }

        // 3. Pobranie zadania po ID
        public async Task<TaskToDo> GetTaskToDoByIdAsync(int id)
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            return await context.TasksToDo
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .Include(x => x.TaskTags)
                .ThenInclude(x => x.Tag)
                .Include(t => t.Comments)
                .Include(t => t.TaskTags)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // 4. Aktualizacja zadania
        public async Task UpdateTaskToDoAsync(TaskToDo task, List<Tag> tags = default)
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            TaskToDo existingTask = await context.TasksToDo
                .Include(x => x.Comments)
                .Include(x => x.TaskTags)
                .FirstOrDefaultAsync(x => x.Id == task.Id);

            if (existingTask == null)
            {
                throw new Exception("TaskToDo not found.");
            }

            existingTask.Name = task.Name;
            existingTask.Description = task.Description;
            existingTask.Status = task.Status;
            existingTask.Priority = task.Priority;
            existingTask.DueDate = task.DueDate;
            existingTask.UpdatedAt = DateTime.UtcNow;
            existingTask.ProjectId = task.ProjectId;
            //existingTask.AssignedToId = task.AssignedToId;

            if (task.AssignedTo is not null)
            {
                existingTask.AssignedTo = await context.Users.FindAsync(task.AssignedTo.Id);
            }
            else
            {
                existingTask.AssignedTo = null;
            }

            //existingTask.CreatedById = task.CreatedById;

            if (task.CreatedBy is not null)
            {
                existingTask.CreatedBy = await context.Users.FindAsync(task.CreatedBy.Id);
            }
            else
            {
                existingTask.CreatedBy = null;
            }

            if (task.Project is not null)
            {
                existingTask.Project = await context.Projects.FindAsync(task.Project.Id);
            }
            else
            {
                existingTask.Project = null;
            }

            existingTask.StartAt = task.StartAt;

            List<Comment> commentsToAdd = task.Comments.Where(x => x.Id == 0).ToList();
            foreach (Comment comment in commentsToAdd)
            {
                comment.TaskId = existingTask.Id;
                context.Add(comment);
            }

            if (tags is List<Tag> tagsSelected)
            {
                List<TaskTag> taskTagsToDelete = existingTask.TaskTags
                    .Where(x => tagsSelected.All(t => t.Id != x.TagId))
                    .ToList();

                context.TaskTags.RemoveRange(taskTagsToDelete);

                List<Tag> tagsToAdd = tagsSelected
                    .Where(x => existingTask.TaskTags.All(t => t.TagId != x.Id))
                    .ToList();

                foreach (Tag tag in tagsToAdd)
                {
                    TaskTag taskTag = new TaskTag()
                    {
                        TagId = tag.Id,
                        TaskId = task.Id
                    };
                    context.TaskTags.Add(taskTag);
                }
            }

            context.TasksToDo.Update(existingTask);
            await context.SaveChangesAsync();
        }


        public void UpdateTaskToDo(TaskToDo task, List<Tag> tags = default)
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            TaskToDo existingTask = context.TasksToDo
                .Include(x => x.Comments)
                .Include(x => x.TaskTags)
                .FirstOrDefault(x => x.Id == task.Id);

            if (existingTask == null)
            {
                throw new Exception("TaskToDo not found.");
            }

            existingTask.Name = task.Name;
            existingTask.Description = task.Description;
            existingTask.Status = task.Status;
            existingTask.Priority = task.Priority;
            existingTask.DueDate = task.DueDate;
            existingTask.UpdatedAt = DateTime.UtcNow;
            existingTask.ProjectId = task.ProjectId;
            //existingTask.AssignedToId = task.AssignedToId;

            if (task.AssignedTo is not null)
            {
                existingTask.AssignedTo = context.Users.Find(task.AssignedTo.Id);
            }
            else
            {
                existingTask.AssignedTo = null;
            }

            //existingTask.CreatedById = task.CreatedById;

            if (task.CreatedBy is not null)
            {
                existingTask.CreatedBy = context.Users.Find(task.CreatedBy.Id);
            }
            else
            {
                existingTask.CreatedBy = null;
            }

            if (task.Project is not null)
            {
                existingTask.Project = context.Projects.Find(task.Project.Id);
            }
            else
            {
                existingTask.Project = null;
            }

            existingTask.StartAt = task.StartAt;

            List<Comment> commentsToAdd = task.Comments.Where(x => x.Id == 0).ToList();
            foreach (Comment comment in commentsToAdd)
            {
                comment.TaskId = existingTask.Id;
                context.Add(comment);
            }

            if (tags is List<Tag> tagsSelected)
            {
                List<TaskTag> taskTagsToDelete = existingTask.TaskTags
                    .Where(x => tagsSelected.All(t => t.Id != x.TagId))
                    .ToList();

                context.TaskTags.RemoveRange(taskTagsToDelete);

                List<Tag> tagsToAdd = tagsSelected
                    .Where(x => existingTask.TaskTags.All(t => t.TagId != x.Id))
                    .ToList();

                foreach (Tag tag in tagsToAdd)
                {
                    TaskTag taskTag = new TaskTag()
                    {
                        TagId = tag.Id,
                        TaskId = task.Id
                    };
                    context.TaskTags.Add(taskTag);
                }
            }

            context.TasksToDo.Update(existingTask);
            context.SaveChanges();
        }

        // 5. Usuwanie zadania
        public async Task DeleteTaskToDoAsync(TaskToDo taskToDo)
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            var task = await context.TasksToDo.FindAsync(taskToDo.Id);
            if (task == null)
            {
                throw new Exception("TaskToDo not found.");
            }

            context.TasksToDo.Remove(task);
            await context.SaveChangesAsync();
        }

        // 6. Pobranie wszystkich statusów
        public async Task<List<Status>> GetStatusesAsync()
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            return await context.TasksToDo
                .Select(t => t.Status)
                .Distinct()
                .ToListAsync();
        }

        // 7. Pobranie wszystkich priorytetów
        public async Task<List<TaskPriority>> GetPrioritiesAsync()
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            return await context.TasksToDo
                .Select(t => t.Priority)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<TaskToDo>> GetTasksByProjectIdAsync(int projectId)
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            return await context.TasksToDo
                .Include(x => x.AssignedTo)
                .Include(x => x.TaskTags)
                .Include(x => x.Project)
                .Include(x => x.Comments)
                .Where(task => task.ProjectId == projectId)
                .OrderBy(t => t.Status)
                .ThenByDescending(t => t.Id)
                .ToListAsync();
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            return await context.Tags
                .Include(u => u.TaskTags)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public List<Tag> GetTags()
        {
            using ProjectContext context = _contextFactory.CreateDbContext();
            return context.Tags
                .Include(u => u.TaskTags)
                .OrderBy(u => u.Name)
                .ToList();
        }

        public async Task AddTagAsync(Tag tag)
        {
            using ProjectContext context = await _contextFactory.CreateDbContextAsync();
            context.Tags.Add(tag);
            await context.SaveChangesAsync();
        }

        public async Task<List<Tag>> GetTaskTagsAsync(int taskId)
        {
            using ProjectContext context = await _contextFactory.CreateDbContextAsync();

            var task = await context.TasksToDo
                .Include(t => t.TaskTags)
                .ThenInclude(tt => tt.Tag)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                throw new Exception($"Task with ID {taskId} not found.");
            }

            // Zwrócenie listy tagów przypisanych do zadania
            return task.TaskTags.Select(tt => tt.Tag).ToList();
        }

        public List<Tag> GetTaskTags(int taskId)
        {
            using ProjectContext context = _contextFactory.CreateDbContext();

            var task = context.TasksToDo
                .Include(t => t.TaskTags)
                .ThenInclude(tt => tt.Tag)
                .FirstOrDefault(t => t.Id == taskId);

            if (task == null)
            {
                throw new Exception($"Task with ID {taskId} not found.");
            }

            // Zwrócenie listy tagów przypisanych do zadania
            return task.TaskTags.Select(tt => tt.Tag).ToList();
        }

        public async Task UpdateTaskTagsAsync(int taskId, IEnumerable<Tag> tags)
        {
            using ProjectContext context = await _contextFactory.CreateDbContextAsync();

            var task = await context.TasksToDo
                .Include(t => t.TaskTags)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                throw new Exception($"Task with ID {taskId} not found.");
            }

            // Usunięcie istniejących powiązań tagów z zadaniem
            if (task.TaskTags != null && task.TaskTags.Any())
            {
                context.TaskTags.RemoveRange(task.TaskTags);
            }

            // Dodanie nowych tagów do zadania
            foreach (var tag in tags)
            {
                var existingTag = await context.Tags.FirstOrDefaultAsync(t => t.Id == tag.Id);
                if (existingTag == null)
                {
                    throw new Exception($"Tag with ID {tag.Id} not found.");
                }

                var taskTag = new TaskTag
                {
                    TaskId = taskId,
                    TagId = tag.Id
                };

                context.TaskTags.Add(taskTag);
            }

            // Zapisanie zmian
            await context.SaveChangesAsync();
        }


    }
}
