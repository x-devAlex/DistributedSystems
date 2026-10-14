using Asp.Versioning;
using CatalogService.Contracts;
using CatalogService.Data;
using CatalogService.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace CatalogService.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/clients")]
public class ClientsController(ClientRepository repo) : ControllerBase
{
    // GET api/v1/clients?city=Київ&sort=-id&page=1&pageSize=10
    [HttpGet]
    [MapToApiVersion("1.0")]
    public ActionResult<PagedResult<ClientDto>> GetAll(
        string? city, string? sort, int page = 1, int pageSize = 10)
    {
        var (items, total) = Query(city, sort, page, pageSize);
        var dtos = items.Select(ClientMapping.ToDto).ToList();
        return Ok(new PagedResult<ClientDto>(dtos, page, pageSize, total,
            (int)Math.Ceiling(total / (double)pageSize)));
    }

    // GET api/v1/clients/5
    [HttpGet("{id:int}")]
    [MapToApiVersion("1.0")]
    public ActionResult<ClientDto> GetById(int id)
    {
        var client = repo.GetById(id);
        if (client is null)
            return Problem(
                title: "Клієнта не знайдено",
                detail: $"Клієнт з id={id} відсутній",
                statusCode: StatusCodes.Status404NotFound);
        return Ok(ClientMapping.ToDto(client));
    }

    // POST api/v1/clients
    [HttpPost]
    [MapToApiVersion("1.0")]
    public ActionResult<ClientDto> Create(CreateClientRequest request)
    {
        var created = repo.Add(new Client
        {
            FullName = request.FullName, Email = request.Email,
            Phone = request.Phone, City = request.City
        });
        return CreatedAtAction(nameof(GetById), new { version = "1.0", id = created.Id },
            ClientMapping.ToDto(created));
    }

    // PUT api/v1/clients/5
    [HttpPut("{id:int}")]
    [MapToApiVersion("1.0")]
    public IActionResult Update(int id, UpdateClientRequest request)
    {
        var ok = repo.Update(id, new Client
        {
            FullName = request.FullName, Email = request.Email,
            Phone = request.Phone, City = request.City
        });
        return ok ? NoContent()
            : Problem(title: "Клієнта не знайдено",
                detail: $"Клієнт з id={id} відсутній",
                statusCode: StatusCodes.Status404NotFound);
    }

    // DELETE api/v1/clients/5
    [HttpDelete("{id:int}")]
    [MapToApiVersion("1.0")]
    public IActionResult Delete(int id)
        => repo.Delete(id) ? NoContent()
            : Problem(title: "Клієнта не знайдено",
                detail: $"Клієнт з id={id} відсутній",
                statusCode: StatusCodes.Status404NotFound);

    // GET api/v2/clients/5
    [HttpGet("{id:int}")]
    [MapToApiVersion("2.0")]
    public ActionResult<ClientDtoV2> GetByIdV2(int id)
    {
        var client = repo.GetById(id);
        if (client is null)
            return Problem(
                title: "Клієнта не знайдено",
                detail: $"Клієнт з id={id} відсутній",
                statusCode: StatusCodes.Status404NotFound);
        return Ok(ClientMapping.ToDtoV2(client));
    }

    private (IReadOnlyList<Client> Items, int Total) Query(
        string? city, string? sort, int page, int pageSize)
    {
        IEnumerable<Client> query = repo.GetAll();
        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(c =>
                c.City.Contains(city, StringComparison.OrdinalIgnoreCase));

        query = sort switch
        {
            "city" => query.OrderBy(c => c.City),
            "-city" => query.OrderByDescending(c => c.City),
            "-id" => query.OrderByDescending(c => c.Id),
            _ => query.OrderBy(c => c.Id)
        };

        var list = query.ToList();
        var items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return (items, list.Count);
    }
}