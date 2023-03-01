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
    //we add DbSet in case we need to query the photo collection directly 
    public DbSet<Photo> photos { get; set; } // table name gets from here  ref to "photos"

    public DbSet<Comment> Comments { get; set; }

    // And what we also need to do is overwrite the model, creating methods from our IdentityDbContext.
    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<ActivityAttendee>(x => x.HasKey(aa => new { aa.AppUserId, aa.ActivityId })); // forms primary key in our table

      //configure Entity so ActivityAttendee has one user and might has many activities
      builder.Entity<ActivityAttendee>().HasOne(u => u.AppUser).WithMany(u => u.Activities).HasForeignKey(aa => aa.AppUserId);
      builder.Entity<ActivityAttendee>().HasOne(u => u.Activity).WithMany(u => u.Attendees).HasForeignKey(aa => aa.ActivityId);

      // cascade will mean if we delete an activity, it will cascade that delete down to the comments that were associated with that activity
      builder.Entity<Comment>().HasOne(a => a.Activity).WithMany(c => c.Comments).OnDelete(DeleteBehavior.Cascade); 
      

    }
  }
}


// if we hover over the  DbContext, this tells us that it represents a session with the database
// Activities is going to represent the table name inside our database when it gets created.
// So now that we have this DB context class, we need to tell our application about it and we'll do that inside our program class


// for example, if we want to query the table directly in our code, then we do need to have a DB set.
