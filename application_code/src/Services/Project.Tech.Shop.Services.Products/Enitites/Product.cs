using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Xml.Linq;

namespace Project.Tech.Shop.Services.Products.Enitites
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Condition Condition { get; set; }

        [Required]
        public string Brand { get; set; } = string.Empty;

        public string? Series { get; set; }

        public string? ProcessorType { get; set; }

        public int? RAM { get; set; }

        public int? Storage { get; set; }

        public string? StorageType { get; set; }

        public string? GraphicsCard { get; set; }

        public double? ScreenSize { get; set; }

        public bool? TouchScreen { get; set; }

        [Required]
        [MaxLength(256)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required]
        public int StockLevel { get; set; }

        [AllowNull]
        public string Image { get; set; } //base64 of bytearray for image

        // Navigation property for related sales
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

        public virtual ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
    }

    public enum Category
    {
        Other,
        Laptop,
        Desktop,
        Mobile,
        Accessories
    }

    public enum Condition
    {
        New,
        Good,
        Okay,
        Broken
    }
}
