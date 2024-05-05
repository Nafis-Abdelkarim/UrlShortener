using Microsoft.EntityFrameworkCore;
using UrlShortener;
using UrlShortener.Models;
using UrlShortener.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddScoped<UrlShortingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//app.MapPost("api/shorten", (
//    UrlShortnerRequest request,
//    UrlShortingService urlShortingService,
//    ApplicationDbContext dbContext,
//    HttpContext httpContext ) =>
//{
//    if(Uri.TryCreate(request.Url, UriKind.Absolute, out _))
//    {
//        return Results.BadRequest("the specified url is invalide"); 
//    }
//});


app.UseHttpsRedirection();


app.Run();
