using CatalogService.Models;

namespace CatalogService.Data;

public class ClientRepository
{
    private static int _id = 0;

    private static readonly List<Client> _clients = new()
    {
        new Client { Id = ++_id, FullName = "Петренко Олена Іванівна",
            Email = "olena@example.com", Phone = "+380501112233", City = "Київ" },
        new Client { Id = ++_id, FullName = "Коваль Андрій Миколайович",
            Email = "andriy@example.com", Phone = "+380671234567", City = "Львів" }
    };

    public List<Client> GetAll() => _clients;

    public Client? GetById(int id) => _clients.FirstOrDefault(c => c.Id == id);

    public Client Add(Client client)
    {
        client.Id = ++_id;
        _clients.Add(client);
        return client;
    }

    public bool Update(int id, Client data)
    {
        var client = GetById(id);
        if (client is null) return false;

        client.FullName = data.FullName;
        client.Email = data.Email;
        client.Phone = data.Phone;
        client.City = data.City;
        return true;
    }

    public bool Delete(int id)
    {
        var client = GetById(id);
        if (client is null) return false;

        _clients.Remove(client);
        return true;
    }
}