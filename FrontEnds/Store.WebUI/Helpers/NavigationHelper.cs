using Microsoft.AspNetCore.Mvc.Rendering;

namespace Store.WebUI.Helpers
{
    public static class NavigationHelper
    {
        public static string IsActive(this IHtmlHelper htmlHelper, string controller, string action, string cssClass = "active")
        {
            var routeData = htmlHelper.ViewContext.RouteData.Values;
            var currentController = routeData["controller"]?.ToString();
            var currentAction = routeData["action"]?.ToString();

            return (controller == currentController && action == currentAction) ? cssClass : "";
        }

        public static string IsMenuOpen(this IHtmlHelper htmlHelper, string controller, string cssClass = "pcoded-trigger active")
        {
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"]?.ToString();
            return controller == currentController ? cssClass : "";
        }
    }
}
