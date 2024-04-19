using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Services.UsersAccounts.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Username { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
        public AccountStatus Status { get; set; }
        public SecuritySettings SecuritySettings { get; set; }
        public UserPreferences Preferences { get; set; }
        public virtual ICollection<AccountActivity> Activities { get; set; }

        public User()
        {
            Addresses = new HashSet<Address>();
            Roles = new HashSet<Role>();
            Activities = new HashSet<AccountActivity>();
        }
    }

    public enum AccountStatus
    {
        Active,
        Disabled,
        Unverified
    }
}
