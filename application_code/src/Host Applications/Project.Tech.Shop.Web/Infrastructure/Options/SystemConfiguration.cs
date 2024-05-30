namespace Project.Tech.Shop.Web.Infrastructure.Options
{
    public class SystemConfiguration
    {
        /// <summary>
        /// Gets and Sets the system name that is used when creating audit events from this web application.
        /// </summary>
        public string SystemName { get; set; } = "Project Tech Shop";

        /// <summary>
        /// Gets and Sets the sub-system name that is used when creating audit events from this web application.
        /// </summary>
        public string SubSystemName { get; set; } = "Web App";

        /// <summary>
        /// Gets and Sets a string identifier for the current version of this system.
        /// </summary>
        public string VersionTag { get; set; } = "v1";
    }
}
