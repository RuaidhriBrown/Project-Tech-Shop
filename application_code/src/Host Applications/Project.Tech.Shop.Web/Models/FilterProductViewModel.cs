using Project.Tech.Shop.Services.Products.Enitites;

namespace Project.Tech.Shop.Web.Models
{
    public class FilterProductViewModel
    {
        public Condition? Condition { get; set; }
        public string? Brand { get; set; }
        public Category? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinStorage { get; set; }
        public int? MaxStorage { get; set; }
    }
}
