using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Services.UsersAccounts.Entities
{
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AddressId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        [MaxLength(1024)]
        public string AddressLine { get; set; }

        [Required]
        [MaxLength(256)]
        public string City { get; set; }

        [Required]
        [MaxLength(256)]
        public string County { get; set; }

        [Required]
        [MaxLength(10)]
        public string PostCode { get; set; }

        [Required]
        [MaxLength(256)]
        public string Country { get; set; }

        public bool IsShippingAddress { get; set; }
        public bool IsBillingAddress { get; set; }
    }
}
