namespace GourmetSpot.Models
{
    public class OrderItem
    {
        public MenuItem MenuItem { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice
        {
            get
            {
                return MenuItem.Price * Quantity;
            }
        }

        public OrderItem(MenuItem menuItem, int quantity)
        {
            MenuItem = menuItem;
            Quantity = quantity;
        }
    }
}
