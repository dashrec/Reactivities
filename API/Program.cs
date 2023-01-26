using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args); // to create something called a Kestrel server.

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();


// using statement means that when we're finished with this scope, anything inside it is going to be disposed or destroyed and cleaned up from memory. like garbage collector
// So we need to get access to a services and we create a scope specifically to do this because our service
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try //create database
{
var context = services.GetRequiredService<DataContext>();
await context.Database.MigrateAsync();// it will create db if it does not exist
await Seed.SeedData(context);
}
catch(Exception ex)
{
   var logger = services.GetRequiredService<ILogger<Program>>(); //  we told the logger what class we're going to be logging against.
   logger.LogError(ex, "An error occured during migration");
}

app.Run();


// DataContext is what class we use for our db context

// add sqlite connection string  in app settings.development.json file And SQLite just uses a file to store a database and typically it uses a DB extension.