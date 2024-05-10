using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener;
using UrlShortener.Entities;
using UrlShortener.Extensions;
using UrlShortener.Models;
using UrlShortener.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("UrlShortenerDbConnectionString")));

builder.Services.AddScoped<UrlShortingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}


app.MapPost("api/shorten", async (
    UrlShortnerRequest request,
    UrlShortingService urlShortingService,
    ApplicationDbContext dbContext,
    HttpContext httpContext) =>
{
    if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("the specified url is invalide");
    }

    string code = await urlShortingService.GenerateShortLink();

    var shortenedUrl = new ShortenedUrl
    {
        Id = Guid.NewGuid(),
        LongUrl = request.Url,
        Code = code,
        ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}",
        CreatetedOn = DateTime.Now,
    };

    dbContext.Add(shortenedUrl);

    await dbContext.SaveChangesAsync();

    return Results.Ok(shortenedUrl.ShortUrl);
});


app.MapGet("api/{code}", async (string code, ApplicationDbContext dbContext) =>
{
    var urlShortened = await dbContext.ShortenedUrls.FirstOrDefaultAsync(s => s.Code == code);

    //TODO: Introducing cache with redis this can imporve the perofmance when system came to scale 

    if(urlShortened == null) return Results.NotFound();

    return Results.Redirect(urlShortened.LongUrl);
});


app.UseHttpsRedirection();


app.Run();
