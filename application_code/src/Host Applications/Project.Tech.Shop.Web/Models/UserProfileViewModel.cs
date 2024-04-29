namespace Project.Tech.Shop.Web.Models
{
    public class UserProfileViewModel
    {
        public Guid UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        // Add more properties as necessary, e.g., FirstName, LastName, etc.
    }
}
