using System.Net.Http;
using System.Threading.Tasks;

namespace GhostBTC
{
    public class TorClient
    {
        private readonly HttpClient _httpClient;
        
        public TorClient(int socksPort = 9050)
        {
            var handler = new SocketsHttpHandler
            {
                Proxy = new WebProxy($"socks5://localhost:{socksPort}")
            };
            
            _httpClient = new HttpClient(handler);
        }

        public async Task<string> GetAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostAsync(string url, HttpContent content)
        {
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}