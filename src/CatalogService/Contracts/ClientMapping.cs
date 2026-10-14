using CatalogService.Models;
using Shared.Contracts;

namespace CatalogService.Contracts;

public static class ClientMapping
{
    public static ClientDto ToDto(Client c) =>
        new(c.Id, c.FullName, c.Email, c.Phone, c.City);

    public static ClientDtoV2 ToDtoV2(Client c)
    {
        var parts = c.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var last = parts.Length > 0 ? parts[0] : string.Empty;
        var first = parts.Length > 1 ? parts[1] : string.Empty;
        return new(c.Id, last, first, c.Email, c.Phone.TrimStart('+'), c.City);
    }
}