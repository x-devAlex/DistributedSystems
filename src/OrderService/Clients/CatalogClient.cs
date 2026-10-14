using System.Net;
using Shared.Contracts;

namespace OrderService.Clients;

public class CatalogClient(HttpClient http)
{
    public async Task<ClientDto?> GetClientAsync(int id)
    {
        using var response = await http.GetAsync($"api/v1/clients/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ClientDto>();
    }
}