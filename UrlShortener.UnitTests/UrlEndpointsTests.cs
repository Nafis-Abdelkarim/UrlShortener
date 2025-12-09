using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Net.Http;
using UrlShortener.Endpoints;
using UrlShortener.Entities;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.UnitTests
{
    public class UrlEndpointsTests
    {
        private ApplicationDbContext GetDbContext(bool withData)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            if (withData)
            {
                // Add some default test data if needed
                context.ShortenedUrls.Add(new ShortenedUrl
                {
                    Id = Guid.NewGuid(),
                    LongUrl = "https://www.linkedin.com/in/abde-lkrim-nefis-9a80281b9/",
                    ShortUrl = "https://shorturl.at/wliMs85",
                    Code = "wliMs85",
                    CreatetedOn = DateTime.Now
                });
                context.SaveChanges();
            }

            return context;
        }

        public UrlEndpointsTests() { }

        [Fact]
        public void GetShortenUrl_ShouldReturnNotFound_WhenUrlShortenedIsNull()
        {
            // Arrange
            using ApplicationDbContext context = GetDbContext(false);

            // Act
            var act = UrlEndpoints.GetShortenUrl("Kri4on8", context);

            // Assert
            Assert.IsType<NotFound>(act.Result);
        }

        [Fact]
        public void GetShortenUrl_ShouldReturnRedirectUrl_WhenUrlFound()
        {
            // Arrange 
            using ApplicationDbContext context = GetDbContext(true);

            // Act
            var act = UrlEndpoints.GetShortenUrl("wliMs85", context);

            // Assert
            Assert.NotNull(act);
        }

        [Fact]
        public void CreateShortenUrl_ShouldReturnBadRequest_WhenUrlIsInValide()
        {
            // Arrange
            UrlShortnerRequest request = new UrlShortnerRequest { Url = "www.example" };
            using ApplicationDbContext context = GetDbContext(false);
            UrlShortingService urlShortingService = new UrlShortingService(context);
            var mockHttpContext = new Mock<HttpContext>();

            // Act
            var act = UrlEndpoints.CreateShortenUrl(request, urlShortingService, context, mockHttpContext.Object);
            
            // Assert
            Assert.IsType<BadRequest<string>>(act.Result);
        }

        [Fact]
        public void CreateShortenUrl_ShouldReturnOk_WhenUrlIsInValide()
        { 
            // Arrange
            UrlShortnerRequest request = new UrlShortnerRequest { Url = "https://www.example.com" };
            using ApplicationDbContext context = GetDbContext(false);
            var mockUrlShortingServiceMock = new Mock<UrlShortingService>(context);
            //mockUrlShortingServiceMock.Setup(s => s.GenerateShortLink()).ReturnsAsync("aBc1723");

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(r => r.Request.Scheme).Returns("https");
            mockHttpContext.Setup(r => r.Request.Host).Returns(new HostString("localhost"));    

            // Act
            var act = UrlEndpoints.CreateShortenUrl(request, mockUrlShortingServiceMock.Object, context, mockHttpContext.Object);

            // Assert
            Assert.IsType<Ok<string>>(act.Result);
        }

        //[Fact]
        //public void CreateShortenUrl_ShouldRunAtLeastOnceUrlShortingService_WhenUrlIsInValide()
        //{
            
        //    UrlShortnerRequest request = new UrlShortnerRequest { Url = "https://www.example.com" };
        //    using ApplicationDbContext context = GetDbContext(false);
        //    var mockUrlShortingServiceMock = new Mock<UrlShortingService>(context);
        //    mockUrlShortingServiceMock.Setup(s => s.GenerateShortLink()).ReturnsAsync("aBc1723");

        //    var mockHttpContext = new Mock<HttpContext>();
        //    mockHttpContext.Setup(r => r.Request.Scheme).Returns("https");
        //    mockHttpContext.Setup(r => r.Request.Host).Returns(new HostString("localhost"));    


        //    var act = UrlEndpoints.CreateShortenUrl(request, mockUrlShortingServiceMock.Object, context, mockHttpContext.Object);


        //    mockUrlShortingServiceMock.Verify(s => s.GenerateShortLink(), Times.Once);
        //    Assert.IsType<Ok<string>>(act.Result);
        //}


    }
}