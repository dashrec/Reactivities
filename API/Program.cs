using Microsoft.EntityFrameworkCore;
using Persistence;
using API.Extensions;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using API.SignalR;

var builder = WebApplication.CreateBuilder(args); // to create something called a Kestrel server.

// Add services to the container.

// ad policy that can be applied as a whole
// And this effectively means that now every single controller endpoint is going to require authentication.
builder.Services.AddControllers(opt =>
{
  var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
  opt.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration); // related to authService

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();



app.UseXContentTypeOptions(); //  this one just prevents the Mime sniffing of the content type like json ..
app.UseReferrerPolicy(opt => opt.NoReferrer()); //  this one refers to the referrer policy that allows a site to control how much information the browser includes when navigating away from our app.
app.UseXXssProtection(opt => opt.EnabledWithBlockMode()); //  this is going to add a cross-site scripting protection header.
app.UseXfo(opt => opt.Deny()); //  this is going to prevent our application being used inside an iframe which protects against that click jacking.

//So some of our content that makes up our application is we're getting fonts from Google, for example, and our images are coming from cloudinary. And so we need to white source those sources of content.
app.UseCsp(opt => opt // this is our main defense against cross-site scripting attacks because this allows us to white source content And we also need to specifically allow any content that served from our application.
    .BlockAllMixedContent() //  this is going to force our app only to load HTTPS content. It won't load a mixture of HTTP and HTTPS.
    .StyleSources(s => s.Self().CustomSources("https://fonts.googleapis.com")) // So we're effectively saying that all of this content is allowed to be served from what we're returning to the client from our wwwfolder.
    .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:"))
    .FormActions(s => s.Self())
    .FrameAncestors(s => s.Self())
    .ImageSources(s => s.Self().CustomSources("blob:", "https://res.cloudinary.com", "https://platform-lookaside.fbsbx.com"))
    .ScriptSources(s => s.Self())
);




if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
else
{
  app.Use(async (context, next) =>
  {
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000"); // one year
    await next.Invoke();
  });
}



app.UseCors("CorsPolicy");
app.UseAuthentication();

//app.UseHttpsRedirection();
app.UseAuthorization();

app.UseDefaultFiles(); // look inside wwwroot folder and fish out index.html or index.htm to serve from kestrel server
app.UseStaticFiles(); // serve content from wwwroot folder by default

app.MapControllers();
app.MapHub<ChatHub>("/chat");
app.MapFallbackToController("Index", "Fallback"); // Index is a name of method inside FallbackController and the second param is first part of Fallback <-- Controller 


// using statement means that when we're finished with this scope, anything inside it is going to be disposed or destroyed and cleaned up from memory. like garbage collector
// So we need to get access to a services and we create a scope specifically to do this because our service
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try //create database
{
  var context = services.GetRequiredService<DataContext>();


  var userManager = services.GetRequiredService<UserManager<AppUser>>();
  await context.Database.MigrateAsync(); // it will create db if it does not exist

  await Seed.SeedData(context, userManager);
}
catch (Exception ex)
{
  var logger = services.GetRequiredService<ILogger<Program>>(); //  we told the logger what class we're going to be logging against.
  logger.LogError(ex, "An error occured during migration");
}

app.Run();


// DataContext is what class we use for our db context

// add sqlite connection string  in app settings.development.json file And SQLite just uses a file to store a database and typically it uses a DB extension.