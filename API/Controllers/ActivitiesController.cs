
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers


// it derives from BaseApiController
{
  public class ActivitiesController : BaseApiController
    {
    private readonly DataContext _context;
 
        public ActivitiesController(DataContext context)
        {
         _context = context;
   
        }

        [HttpGet] //api/activities
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            return await _context.Activities.ToListAsync(); // it returns all 
        }


        [HttpGet("{id}")] //api/activities/someId
        public async Task<ActionResult<Activity>>GetActivity(Guid id) // this id gonna match this [HttpGet("{id}")] so this id /someId
        {
             return await _context.Activities.FindAsync(id);  // it returns only one record with appropriate id
        }
    }
}