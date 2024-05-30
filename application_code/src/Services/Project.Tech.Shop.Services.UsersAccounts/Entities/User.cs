using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Project.Tech.Shop.Services.UsersAccounts.Entities
{
    public class User
    {
        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(256)]
        public string Username { get; set; }

        [Required]
        [MaxLength(256)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Surname { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public Role Role { get; set; }

        [Required]
        public AccountStatus Status { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }

        public SecuritySettings SecuritySettings { get; set; }

        public UserPreferences Preferences { get; set; }

        public virtual ICollection<AccountActivity> Activities { get; set; }

        public User()
        {
            Addresses = new HashSet<Address>();
            Activities = new HashSet<AccountActivity>();
        }
    }

    public enum AccountStatus
    {
        Active,
        Disabled,
        Unverified
    }

    public enum Role
    {
        Customer,
        Support,
        Admin
    }
}
