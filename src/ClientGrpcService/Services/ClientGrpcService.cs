using ClientGrpcService;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;

namespace ClientGrpcService.Services;

public class ClientsGrpcService : ClientService.ClientServiceBase
{
    private static int _id = 0;

    private static readonly List<ClientReply> _clients = new()
    {
        new ClientReply { Id = ++_id, FullName = "Петренко Олена Іванівна",
            Email = "olena@example.com", Phone = "+380501112233", City = "Київ" },
        new ClientReply { Id = ++_id, FullName = "Коваль Андрій Миколайович",
            Email = "andriy@example.com", Phone = "+380671234567", City = "Львів" }
    };

    public override Task<ListClientsReply> ListClients(Empty request, ServerCallContext context)
    {
        var reply = new ListClientsReply();
        reply.Clients.AddRange(_clients);
        return Task.FromResult(reply);
    }

    public override Task<ClientReply> GetClient(ClientId request, ServerCallContext context)
    {
        var client = _clients.FirstOrDefault(c => c.Id == request.Id)
            ?? throw new RpcException(new Status(StatusCode.NotFound,
                $"Клієнта {request.Id} не знайдено"));
        return Task.FromResult(client);
    }

    public override Task<ClientReply> CreateClient(CreateClientRequest request, ServerCallContext context)
    {
        var client = new ClientReply
        {
            Id = ++_id, FullName = request.FullName, Email = request.Email,
            Phone = request.Phone, City = request.City
        };
        _clients.Add(client);
        return Task.FromResult(client);
    }

    public override Task<ClientReply> UpdateClient(UpdateClientRequest request, ServerCallContext context)
    {
        var client = _clients.FirstOrDefault(c => c.Id == request.Id)
            ?? throw new RpcException(new Status(StatusCode.NotFound,
                $"Клієнта {request.Id} не знайдено"));
        client.FullName = request.FullName;
        client.Email = request.Email;
        client.Phone = request.Phone;
        client.City = request.City;
        return Task.FromResult(client);
    }

    public override Task<ClientReply> DeleteClient(ClientId request, ServerCallContext context)
    {
        var client = _clients.FirstOrDefault(c => c.Id == request.Id)
            ?? throw new RpcException(new Status(StatusCode.NotFound,
                $"Клієнта {request.Id} не знайдено"));
        _clients.Remove(client);
        return Task.FromResult(client);
    }

    // server streaming RPC
    public override async Task StreamClients(Empty request,
        IServerStreamWriter<ClientReply> responseStream, ServerCallContext context)
    {
        foreach (var client in _clients.ToList())
        {
            if (context.CancellationToken.IsCancellationRequested) break;
            await responseStream.WriteAsync(client);
            await Task.Delay(500);
        }
    }
}