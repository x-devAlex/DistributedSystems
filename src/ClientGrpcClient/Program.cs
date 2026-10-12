using ClientGrpcService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("http://localhost:5296");
var client = new ClientService.ClientServiceClient(channel);

// creation
var created = await client.CreateClientAsync(new CreateClientRequest
{
    FullName = "Шевченко Марія Сергіївна", Email = "maria@example.com",
    Phone = "+380931234567", City = "Одеса"
});
Console.WriteLine($"Створено клієнта з id={created.Id}");

// list
var list = await client.ListClientsAsync(new Empty());
foreach (var c in list.Clients)
    Console.WriteLine($"{c.Id}. {c.FullName} – {c.City}");

// get with error handling
try
{
    var one = await client.GetClientAsync(new ClientId { Id = 1 });
    Console.WriteLine($"Знайдено: {one.FullName}");
    await client.GetClientAsync(new ClientId { Id = 9999 });
}
catch (RpcException ex)
{
    Console.WriteLine($"Помилка: {ex.Status.StatusCode} – {ex.Status.Detail}");
}

// update and delete
var updated = await client.UpdateClientAsync(new UpdateClientRequest
{
    Id = 1, FullName = "Петренко Олена Іванівна", Email = "olena.new@example.com",
    Phone = "+380501112299", City = "Харків"
});
Console.WriteLine($"Оновлено: {updated.Id}. {updated.FullName} – {updated.City}");
await client.DeleteClientAsync(new ClientId { Id = 2 });
Console.WriteLine("Видалено клієнта з id=2");

// streaming
using var call = client.StreamClients(new Empty());
await foreach (var c in call.ResponseStream.ReadAllAsync())
    Console.WriteLine($"Потік: {c.Id} – {c.FullName}");