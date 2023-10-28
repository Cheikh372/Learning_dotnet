using ToDoListApi.Dtos;

namespace ToDoListApi.Services
{
    public interface ITaskServices
    {
        IEnumerable<TaskDto> GetAll(TaskFilterDto input);

        Task<int> CreateTask(TaskDto input);

        Task<int> UpdateTask(TaskDto input);
        
        Task<int> DeleteTask(int taskID);

    }
}
