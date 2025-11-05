using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Entities;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.Endpoints
{
    public static class UrlEndpoints
    {
        public static void MapToUrlGroup(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/shorten"); //We can add here an attibute to this group ex: var group = app.MapGroup("api/shorten").RequireAuthorization

            group.MapPost("", CreateShortenUrl);

            group.MapGet("{code}", GetShortenUrl);
        }

        public static async Task<IResult> CreateShortenUrl(UrlShortnerRequest request, UrlShortingService urlShortingService, ApplicationDbContext dbContext, HttpContext httpContext)
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
        }

        public static async Task<IResult> GetShortenUrl(string code, ApplicationDbContext dbContext)
        {
            var urlShortened = await dbContext.ShortenedUrls.FirstOrDefaultAsync(s => s.Code == code);

            //TODO: Introducing cache with redis this can imporve the perofmance when system came to scale 

            if (urlShortened == null) return Results.NotFound();

            return Results.Redirect(urlShortened.LongUrl);
        }
    }
}
