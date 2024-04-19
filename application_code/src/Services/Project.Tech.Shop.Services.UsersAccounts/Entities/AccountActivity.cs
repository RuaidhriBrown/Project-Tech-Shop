using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Services.UsersAccounts.Entities
{
    public class AccountActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ActivityId { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime Timestamp { get; set; }
        public string ActivityType { get; set; }
        public string Description { get; set; }
    }
}
