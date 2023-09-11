using GRPC.Models;
using Microsoft.EntityFrameworkCore;
using GRPC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Get ConnectionString
var conn = builder.Configuration.GetConnectionString("Default");

// Register AppDbContext
builder.Services.AddDbContext<AppDbContext>(db => db.UseSqlServer(conn));

// Register Category Service
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Register Grpc and json transcoding
builder.Services.AddGrpc().AddJsonTranscoding();

var app = builder.Build();

app.MapGrpcService<CategoryGrpcService>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
