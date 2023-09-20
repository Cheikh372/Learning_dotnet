using MinimalAPI.Data;
using MinimalAPI.Model;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// containt the task
var data = new TaskList();

app.MapGet("/", () => "Hello World!");

#region Map Route
//return all task in data
app.MapGet("/tasks", () => data.Items /*new Tache{ Libelle = "Nettoyage", Date = DateTime.Now.AddDays(2)}*/);

//return the task witch Libelle is libelle. ex : tasks/Tache 2 return task 2
app.MapGet("/tasks/{libelle}", (string libelle) =>  data.Items.Where(item => item.Libelle == libelle));
#endregion Map Route

#region  Route handlers for simple CRUD
//return all task in data
app.MapGet("/tasks", data.All /*new Tache{ Libelle = "Nettoyage", Date = DateTime.Now.AddDays(2)}*/);

//return the task witch Libelle is libelle. ex : tasks/Tache 2 return task 2
app.MapGet("/tasks/{libelle}", data.GateTaskByLibelle);

// you can use postman for testing Add and Update method
app.MapPost("/tasks/add", data.AddTask);

app.MapDelete("/tasks/{id}", data.RemoveTask);

#endregion Route handlers for simple CRUD

app.Run();
