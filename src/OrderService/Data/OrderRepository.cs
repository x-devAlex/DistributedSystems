using OrderService.Models;

namespace OrderService.Data;

public class OrderRepository
{
    private static int _id = 0;
    private static readonly List<Order> _orders = new();

    public Order? GetById(int id) => _orders.FirstOrDefault(o => o.Id == id);

    public Order Add(Order order)
    {
        order.Id = ++_id;
        _orders.Add(order);
        return order;
    }
}