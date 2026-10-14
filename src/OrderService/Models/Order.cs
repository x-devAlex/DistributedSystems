namespace OrderService.Models;

public class Order
{
    public int Id { get; set; }
    public int ClientId { get; set; }                 
    public string ClientFullName { get; set; } = "";  
    public string ClientEmail { get; set; } = "";     
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
}