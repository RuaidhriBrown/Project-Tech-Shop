namespace Project.Tech.Shop.Web.Models
{
    public class BasketViewModel
    {
        public List<BasketItemViewModel> Items { get; set; } = new List<BasketItemViewModel>();
        public decimal TotalPrice => Items.Sum(item => item.TotalPrice);

        public class BasketItemViewModel
        {
            public int BasketItemId { get; set; }
            public int ProductId { get; set; }
            public ProductViewModel Product { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice => Price * Quantity;
        }
    }

    
}
