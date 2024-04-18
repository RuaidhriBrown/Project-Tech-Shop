using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Services.UsersAccounts.Entities
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RoleId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
