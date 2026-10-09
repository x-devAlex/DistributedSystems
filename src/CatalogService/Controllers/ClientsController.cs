using CatalogService.Data;
using CatalogService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly ClientRepository _repo;

    public ClientsController(ClientRepository repo) => _repo = repo;

    // GET api/clients
    [HttpGet]
    public ActionResult<IEnumerable<Client>> GetAll() => Ok(_repo.GetAll());

    // GET api/clients/5
    [HttpGet("{id:int}")]
    public ActionResult<Client> GetById(int id)
    {
        var client = _repo.GetById(id);
        return client is null ? NotFound() : Ok(client);
    }

    // POST api/clients
    [HttpPost]
    public ActionResult<Client> Create(Client client)
    {
        var created = _repo.Add(client);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT api/clients/5
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Client client)
        => _repo.Update(id, client) ? NoContent() : NotFound();

    // DELETE api/clients/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
        => _repo.Delete(id) ? NoContent() : NotFound();
}