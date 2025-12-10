using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Services
{
    public class UrlShortingService : IUrlShorting
    {
        public const int NumberOfCharsInShortLink = 7;

        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private readonly Random _random = new Random();

        private readonly ApplicationDbContext _dbcontext;

        public UrlShortingService(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<string> GenerateShortLink()
        {
            char[] CodeChars = new char[NumberOfCharsInShortLink];

            while(true)
            {
                for (int i = 0; i < NumberOfCharsInShortLink; i++)
                {
                    int randomIndex = _random.Next(Alphabet.Length - 1);

                    CodeChars[i] = Alphabet[randomIndex];
                }

                try
                {
                string code = new string(CodeChars);

                if (!await _dbcontext.ShortenedUrls.AnyAsync(x => x.Code == code))
                {
                    return code;
                }
                }
                catch(Exception ex) 
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
            };
            
        }
    }
}
