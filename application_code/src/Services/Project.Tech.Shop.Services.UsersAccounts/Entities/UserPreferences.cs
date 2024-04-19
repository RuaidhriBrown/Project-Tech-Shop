using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Services.UsersAccounts.Entities
{
    public class UserPreferences
    {
        [Key]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public bool ReceiveNewsletter { get; set; }
        public string PreferredPaymentMethod { get; set; }
    }
}
