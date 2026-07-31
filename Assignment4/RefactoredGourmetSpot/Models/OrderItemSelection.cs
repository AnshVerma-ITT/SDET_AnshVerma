namespace GourmetSpot.Models
{
    public class OrderItemSelection
    {
        public MenuItem MenuItem { get; }
        public int Quantity { get; }

        public OrderItemSelection(MenuItem menuItem, int quantity)
        {
            MenuItem = menuItem;
            Quantity = quantity;
        }

        public OrderItem ToOrderItem()
        {
            return new OrderItem(MenuItem, Quantity);
        }
    }
}
