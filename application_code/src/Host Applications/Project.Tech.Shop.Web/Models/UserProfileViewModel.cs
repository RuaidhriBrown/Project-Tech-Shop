namespace Project.Tech.Shop.Web.Models
{
    public class UserProfileViewModel
    {
        public Guid UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public IEnumerable<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
        public SecuritySettingsViewModel SecuritySettings { get; set; } = new SecuritySettingsViewModel();
        public UserPreferencesViewModel Preferences { get; set; } = new UserPreferencesViewModel();
        public IEnumerable<AccountActivityViewModel> Activities { get; set; } = new List<AccountActivityViewModel>();
        public byte[] RowVersion { get; set; } = new byte[0];

        public string passwordReplacement = string.Empty;
    }

    public class AddressViewModel
    {
        public Guid? AddressId { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? PostCode { get; set; }
        public string? Country { get; set; }
        public bool IsShippingAddress { get; set; } = false;
        public bool IsBillingAddress { get; set; } = false;
    }

    public class SecuritySettingsViewModel
    {
        public bool TwoFactorEnabled { get; set; } = false;
        public string? SecurityQuestion { get; set; }
        public string? SecurityAnswerHash { get; set; }
    }

    public class UserPreferencesViewModel
    {
        public bool ReceiveNewsletter { get; set; } = false;
        public string? PreferredPaymentMethod { get; set; }
    }

    public class AccountActivityViewModel
    {
        public Guid ActivityId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? ActivityType { get; set; }
        public string? Description { get; set; }
    }
}
