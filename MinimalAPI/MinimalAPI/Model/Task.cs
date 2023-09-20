namespace MinimalAPI.Model
{
    public class Task
    {
        public string Id => Guid.NewGuid().ToString();

        public string? Libelle { get; set; }
        
        public DateTime Date { get; set; }
    }
}
