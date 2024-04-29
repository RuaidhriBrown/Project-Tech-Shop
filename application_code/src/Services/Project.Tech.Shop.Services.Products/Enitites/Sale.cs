using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace Project.Tech.Shop.Services.Products.Enitites
{
    public class Sale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaleId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalSaleAmount { get; set; }

        [Required]
        [MaxLength(256)]
        public string Status { get; set; } = "Ordered";

        // Reference to the Basket
        [ForeignKey("Basket")]
        public Guid BasketId { get; set; }
        public virtual Basket Basket { get; set; }
    }
}
