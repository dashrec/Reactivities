using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{


  public class DataContext : IdentityDbContext<AppUser> //IdentityDbContext it's gonna  scaffold a lot of tables in our database
  {
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Activity> Activities { get; set; }
    public DbSet<ActivityAttendee> ActivityAttendees { get; set; }

    // And what we also need to do is overwrite the model, creating methods from our IdentityDbContext.
    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<ActivityAttendee>(x => x.HasKey(aa => new { aa.AppUserId, aa.ActivityId })); // forms primary key in our table

      //configure Entity so ActivityAttendee has one user and might has manny avtivities
      builder.Entity<ActivityAttendee>().HasOne(u => u.AppUser).WithMany(u => u.Activities).HasForeignKey(aa => aa.AppUserId);
      builder.Entity<ActivityAttendee>().HasOne(u => u.Activity).WithMany(u => u.Attendees).HasForeignKey(aa => aa.ActivityId);

      

    }
  }
}


// if we hover over the  DbContext, this tells us that it represents a session with the database
// Activities is going to represent the table name inside our database when it gets created.
// So now that we have this DB context class, we need to tell our application about it and we'll do that inside our program class


// for example, if we want to query the table directly in our code, then we do need to have a DB set.
