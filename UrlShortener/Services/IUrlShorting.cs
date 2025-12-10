namespace UrlShortener.Services
{
    public interface IUrlShorting
    {
        public Task<string> GenerateShortLink();
    }
}
