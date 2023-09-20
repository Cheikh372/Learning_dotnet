using MinimalAPI.Model;
using Task = MinimalAPI.Model.Task;

namespace MinimalAPI.Data
{
    public class TaskList
    {
        public TaskList() 
        {
            Items = new List<Task>
            {
                new Task{ Libelle = "Tache 1", Date = new DateTime()},
                new Task{ Libelle = "Tache 2", Date = new DateTime()},
                new Task{ Libelle = "Tache 3", Date = new DateTime()},
                new Task{ Libelle = "Tache 4", Date = new DateTime()},
                new Task{ Libelle = "Tache 5", Date = new DateTime()},
                new Task{ Libelle = "Tache 6", Date = new DateTime()},
                new Task{ Libelle = "Tache 7", Date = new DateTime()},
                new Task{ Libelle = "Tache 8", Date = new DateTime()}
            };
        }

        public List<Task> Items { get; set; }

        public List<Task> All()
        {
            return Items;
        }

        public Task? GateTaskByLibelle(string libelle)
        {
            return Items.FirstOrDefault(item => item.Libelle == libelle);
        }

        public void AddTask(Task task)
        {
            if (task == null)
                return;
            
            Items.Add(task);
        }

        public void RemoveTask(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            var task = Items.FirstOrDefault(item => item.Id == id);

            if (task == null) return;

            Items.Remove(task); 
        }


        
    }
}
