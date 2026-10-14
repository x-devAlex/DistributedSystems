using System.ComponentModel.DataAnnotations;

namespace OrderService.Models;

public record CreateOrderRequest(
    [Range(1, int.MaxValue)] int ClientId,
    [Required, StringLength(300)] string Description,
    [Range(0.01, 1_000_000)] decimal Amount);