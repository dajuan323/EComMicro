using OrderApi.Domain.Entities;

namespace OrderApi.App.DTOs.Conversions;

public static class OrderConversion
{
    public static Order ToEntity(OrderDTO order) => new Order()
    {
        Id = order.Id,
        ClientId = order.ClientId,
        ProductId = order.ProductId,
        OrderDate = order.OrderDate,
        PurchaseQuantity = order.PurchaseQuantity,
    };

    public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<Order>? orders)
    {
        if (order is not null && orders is null)
        {
            var singleOrder = new OrderDTO(order!.Id, order!.ClientId, order!.ProductId, order!.PurchaseQuantity, order!.OrderDate);
            return (singleOrder, null);
        }

        if (order is null && orders is not null)
        {
            var _orders = orders!.Select(_ =>
                new OrderDTO(_.Id,
                _.ClientId,
                _.ProductId,
                _.PurchaseQuantity,
                _.OrderDate
            ));

            return (null, _orders);
        }
        return (null, null);
    }
}
