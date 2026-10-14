using Microsoft.AspNetCore.Mvc;
using OrderService.Clients;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(OrderRepository repo, CatalogClient catalog) : ControllerBase
{
    // POST api/orders
    [HttpPost]
    public async Task<ActionResult<Order>> Create(CreateOrderRequest req)
    {
        try
        {
            var client = await catalog.GetClientAsync(req.ClientId);
            if (client is null)
                return Problem(
                    title: "Клієнта не знайдено в каталозі",
                    detail: $"Клієнт з id={req.ClientId} відсутній у сервісі каталогу",
                    statusCode: StatusCodes.Status400BadRequest);
            
            var order = repo.Add(new Order
            {
                ClientId = client.Id,
                ClientFullName = client.FullName,
                ClientEmail = client.Email,
                Description = req.Description,
                Amount = req.Amount
            });

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (HttpRequestException)
        {
            return Problem(
                title: "Сервіс каталогу недоступний",
                detail: "Не вдалося звернутися до CatalogService. Перевірте, що він запущений.",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }

    // GET api/orders/5
    [HttpGet("{id:int}")]
    public ActionResult<Order> GetById(int id)
        => repo.GetById(id) is { } order
            ? Ok(order)
            : Problem(title: "Замовлення не знайдено",
                detail: $"Замовлення з id={id} відсутнє",
                statusCode: StatusCodes.Status404NotFound);
}