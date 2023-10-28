using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoListApi.Entities
{
    [Table("Tasks")]
    public class Task
    {
        public int TaskID { get; set; }

        public string Description {  get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }

        public bool IsCompleted { get; set; }
    }
}
