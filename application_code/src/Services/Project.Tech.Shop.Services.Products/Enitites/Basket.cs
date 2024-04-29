using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Services.Products.Enitites
{
    public class Basket
    {
        [Key]
        public Guid BasketId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        public BasketStatus Status { get; set; } = BasketStatus.Active;

        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        public virtual Sale Sale { get; set; }
    }

    public enum BasketStatus
    {
        Active,
        Completed,
        Canceled
    }
}
