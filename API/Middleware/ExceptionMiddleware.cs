
using System.Net;
using System.Text.Json;
using Application.Core;

namespace API.Middleware
{
  public class ExceptionMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
    {
      _env = env;
      _logger = logger;
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {

        _logger.LogError(ex, ex.Message);
        context.Response.ContentType = "application/json"; //wen we are outside of the controller we need to specify json
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500

        var response = _env.IsDevelopment()
        ? new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()) //development
        : new AppException(context.Response.StatusCode, "Internal Server Error");// production 


        // Again, this is something that our API controllers enabled by default because that's how we format JSON when we return it.
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);// create json response
        await context.Response.WriteAsync(json);// return json

      }
    }
  }
}