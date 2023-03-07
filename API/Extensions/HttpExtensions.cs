using System.Text.Json;

namespace API.Extensions
{
  public static class HttpExtensions
  {
    public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
      var paginationHeader = new
      {
        currentPage,
        itemsPerPage,
        totalItems,
        totalPages
      };
      response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader)); // will be formatted as json string and given a key Pagination
      response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
  }
}

//  because this is a custom header, we need to specifically expose this so that our browser will be able to read it.
// And inside here, what we want to do is be able to add a pagination header to any response that we send back from our API.