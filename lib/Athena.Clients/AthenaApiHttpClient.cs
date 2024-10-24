using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK;
using Athena.SDK.Models;

namespace Athena.Clients
{
    public sealed class AthenaApiHttpClient : IAthenaApi
    {
        private readonly HttpClient _httpClient;
        
        public AthenaApiHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<PantheonIdentity> CreateUserFromDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default)
        {
            _httpClient.GetAsync("athena/api/v1/")
        }

        public Task<PantheonIdentity?> GetUserByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DoesUsernameExistAsync(string username, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<PantheonIdentity> CreateUserFromUsernameAsync(string username, string password, CancellationToken stoppingToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<PantheonIdentity?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> VerifyPasswordAsync(byte[] passwordHash, string password, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}