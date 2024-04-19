using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Services.UsersAccounts.Entities
{
    public class SecuritySettings
    {
        [Key]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public bool TwoFactorEnabled { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswerHash { get; set; }
    }
}
