using Microsoft.EntityFrameworkCore;

namespace Application.Core
{
  public class PagedList<T> : List<T> // generic type
  {
    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize) // pageSize means how many list item we want to show on a single page
    {
      CurrentPage = pageNumber;
      TotalPages = (int)Math.Ceiling(count / (double)pageSize); // if a list had 20 items and a pageSize of ten, then TotalPages would be 2
      PageSize = pageSize;
      TotalCount = count;
      AddRange(items); // We also need to add the items we get passed in as a parameter into the class that we're going to be returning.


    }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }


    // This PagedList will be able to get the CurrentPage, the TotalPages, the PageSize and the TotalCount based on what's going on inside our static method here.
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize) // source gonna be a query thats going to go to database 
    {
      var count = await source.CountAsync(); // it does executes the query to database to find out how many items are in the list
      var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(); // it will skip 0 records on 0 page and takes 10 records, then it will skip 10 records on 1 page and takes 10 records again
      return new PagedList<T>(items, count, pageNumber, pageSize);
    }
  }
}
// this PagedList class is going to give us a paginated list of results along with the counts and the items as well.
// (page 1 - 1) * pageSize = 0 and for a second page (2-1) * 10(pagesize) gonna give us next 10 records
// List class we get from framework and PagedList inherits everything available inside the List class