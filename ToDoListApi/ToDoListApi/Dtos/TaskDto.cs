namespace ToDoListApi.Dtos
{
    public class TaskDto
    {
        public int TaskID { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }

        public bool IsCompleted { get; set; }
    }
}
