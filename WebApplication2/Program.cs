using Microsoft.EntityFrameworkCore;
using WebApplication2.model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<IncidentsDbContext>(op => op.UseSqlServer(
    builder.Configuration.GetConnectionString("IncidentsConnection")
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Route racine
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();