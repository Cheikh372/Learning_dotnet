namespace ToDoListApi.Dtos
{
    public class TaskFilterDto
    {
        public bool IsFilter { get; set; }

        public DateTime DateCreatedFilter { get; set; }

        public bool IsCompletedFilter { get; set; }
    }
}
