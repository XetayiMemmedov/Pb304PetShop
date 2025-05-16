namespace Pb304PetShop.Models
{
    public class WantlistViewModel
    {
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public List<WantlistItemViewModel> Items { get; set; } = new();
    }

    public class WantlistItemViewModel
    {
        public int ProductId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
