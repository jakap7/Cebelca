namespace Cebelica.Models
{
    public class OrderModel
    {
        public int ProductId { get; set; }
        public string Email { get; set; }
        public Dictionary<int, int> Quantities { get; set; } = new Dictionary<int, int>(); // Key: ProductId, Value: Quantity
        public string Address { get; set; }
        public string Contact {  get; set; }
    }
}