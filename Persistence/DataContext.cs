using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{


  public class DataContext:DbContext
  {
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Activity>Activities { get; set; }
  }
}


// if we hover over the  DbContext, this tells us that it represents a session with the database
// Activities is going to represent the table name inside our database when it gets created.
// So now that we have this DB context class, we need to tell our application about it and we'll do that inside our program class