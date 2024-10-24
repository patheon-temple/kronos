using System;
using System.Threading;
using System.Threading.Tasks;
using Athena.SDK;
using Google.Protobuf;
using Pantheon.Athena.Grpc;
using PantheonIdentity = Athena.SDK.Models.PantheonIdentity;

namespace Pantheon.Athena.Clients
{
    public sealed class AthenaApiGrpcClient : IAthenaApi
    {
        private readonly AthenaRequestContext _requestContext;
        private readonly AthenaApiGrpc.AthenaApiGrpcClient _client;

        public AthenaApiGrpcClient(AthenaApiGrpc.AthenaApiGrpcClient client, AthenaRequestContext requestContext)
        {
            _client = client;
            _requestContext = requestContext;
        }

        public async Task<PantheonIdentity> CreateUserFromDeviceIdAsync(string deviceId,
            CancellationToken cancellationToken = default)
        {
            var response = await _client.CreateUserFromDeviceIdAsync(new CreateUserFromDeviceIdRequest
            {
                DeviceId = deviceId,
            }, _requestContext.CreateMetadata(), _requestContext.CallDeadline, cancellationToken);

            if (response == null) throw new NullReferenceException();

            return Mappers.FromGrpc(response);
        }

        public async Task<PantheonIdentity?> GetUserByDeviceIdAsync(string deviceId,
            CancellationToken cancellationToken = default)
        {
            var response = await _client.GetUserByDeviceIdAsync(new GetUserByDeviceIdRequest
            {
                DeviceId = deviceId,
            }, _requestContext.CreateMetadata(), _requestContext.CallDeadline, cancellationToken);

            if (response == null) throw new NullReferenceException();

            return Mappers.FromGrpc(response);
        }

        public async Task<bool> DoesUsernameExistAsync(string username, CancellationToken cancellationToken = default)
        {
            var response = await _client.DoesUsernameExistAsync(new DoesUsernameExistRequest()
            {
                Username = username
            }, _requestContext.CreateMetadata(), _requestContext.CallDeadline, cancellationToken);
            if (response == null) throw new NullReferenceException();

            return response.DoesExist;
        }

        public async Task<PantheonIdentity> CreateUserFromUsernameAsync(string username, string password,
            CancellationToken stoppingToken = default)
        {
            var response = await _client.CreateUserFromUsernameAsync(new CreateUserFromUsernameRequest
            {
                Username = username,
                Password = password,
            }, _requestContext.CreateMetadata(), _requestContext.CallDeadline, stoppingToken);

            if (response == null) throw new NullReferenceException();

            return Mappers.FromGrpc(response);
        }

        public async Task<PantheonIdentity?> GetUserByUsernameAsync(string username,
            CancellationToken cancellationToken = default)
        {
            var response = await _client.GetUserByUsernameAsync(new GetUserByUsernameRequest()
            {
                Username = username,
            }, _requestContext.CreateMetadata(), _requestContext.CallDeadline, cancellationToken);

            if (response == null) throw new NullReferenceException();

            return Mappers.FromGrpc(response);
        }

        public async Task<bool> VerifyPasswordAsync(Guid userId, string password,
            CancellationToken cancellationToken = default)
        {
            var response = await _client.VerifyPasswordAsync(new VerifyPasswordRequest
            {
                UserId = ByteString.CopyFrom(userId.ToByteArray()),
                Password = password
            }, _requestContext.CreateMetadata(), _requestContext.CallDeadline, cancellationToken);

            if (response == null) throw new NullReferenceException();

            return response.IsValid;
        }

        public async Task<PantheonIdentity?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var response = await _client.GetUserByIdAsync(new GetUserByIdRequest()
            {
                UserId = ByteString.CopyFrom(userId.ToByteArray()),
            }, _requestContext.CreateMetadata(), _requestContext.CallDeadline, cancellationToken);

            if (response == null) throw new NullReferenceException();

            return Mappers.FromGrpc(response);       
        }
    }
}