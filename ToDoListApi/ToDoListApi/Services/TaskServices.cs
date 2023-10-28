using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDoListApi.Dtos;
using ToDoListApi.ExtensionServices;
using ToDoListApi.Infrastructure;

namespace ToDoListApi.Services
{
    public class TaskServices : ITaskServices
    {
        private readonly ToDoListDbContext _dbContext;
        private readonly IMapper _mapper;

        public TaskServices(ToDoListDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;

        }

        public async Task<int> CreateTask(TaskDto input)
        {

            var task = _mapper.Map<ToDoListApi.Entities.Task>(input);

            await _dbContext.Tasks.AddAsync(task);

            await _dbContext.SaveChangesAsync();

            return task.TaskID;
        }

        public async Task<int> DeleteTask(int taskID)
        {
            var task = await _dbContext.Tasks.FindAsync(taskID);

            //if (task == null) return;

            _dbContext.Tasks.Remove(task);

            await _dbContext.SaveChangesAsync();

            return task.TaskID;
        }

        public  IEnumerable<TaskDto> GetAll(TaskFilterDto input)
        {
            var tasks = _dbContext.Tasks.AsNoTracking()
                                        .WhereIf(input.IsFilter, tsk => tsk.IsCompleted == input.IsCompletedFilter || tsk.DateCreated == input.DateCreatedFilter);

            var result = new  List<TaskDto>();

            foreach (var item in tasks) 
            {
                result.Add(
                    new TaskDto{
                        TaskID = item.TaskID,
                        Description = item.Description,
                        DateCreated = item.DateCreated,
                        IsCompleted = item.IsCompleted
                    });
            }

            return result ;
        }

        public async Task<int> UpdateTask(TaskDto input)
        {
            var task = _mapper.Map<ToDoListApi.Entities.Task>(input);

           _dbContext.Tasks.Update(task);

            await _dbContext.SaveChangesAsync();

            return task.TaskID;

        }
    }
}
