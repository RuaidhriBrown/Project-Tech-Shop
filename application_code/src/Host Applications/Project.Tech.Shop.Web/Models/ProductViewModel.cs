using Project.Tech.Shop.Services.Products.Enitites;

namespace Project.Tech.Shop.Web.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public Condition Condition { get; set; }

        public string Brand { get; set; } = string.Empty;

        public string? Series { get; set; }

        public string? ProcessorType { get; set; }

        public int? RAM { get; set; }

        public int? Storage { get; set; }

        public string? StorageType { get; set; }

        public string? GraphicsCard { get; set; }

        public double? ScreenSize { get; set; }

        public bool? TouchScreen { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public Category Category { get; set; }

        public int StockLevel { get; set; }

        public string? Image { get; set; }
    }
}
