using Microsoft.AspNetCore.Mvc;

namespace Project.Tech.Shop.Web.Infrastructure
{
    public static class ControllerExtensions
    {
        public const string ConfirmationMessageKey = "ConfirmationMessage";
        public const string UserGuideKey = "UserGuide";
        public const string UserGuideLinkKey = "UserGuideLink";
        public const string ErrorMessageKey = "ErrorMessage";
        public const string ErrorFieldNameKey = "ErrorFieldName";

        public static void AddConfirmationMessage(this Controller controller, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            controller.TempData.TryAdd(ConfirmationMessageKey, value);
        }

        public static void AddErrorMessage(this Controller controller, string value, string? fieldName = default)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            controller.TempData.TryAdd(ErrorMessageKey, value);
            if (fieldName is not null)
            {
                controller.TempData.TryAdd(ErrorFieldNameKey, fieldName);
            }
        }
    }
}
