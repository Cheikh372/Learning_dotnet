using Microsoft.EntityFrameworkCore;

namespace ToDoListApi.Infrastructure
{
    public class ToDoListDbContext : DbContext
    {
        public DbSet<ToDoListApi.Entities.Task> Tasks { get; set; }

        public ToDoListDbContext(DbContextOptions<ToDoListDbContext> options)   : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
