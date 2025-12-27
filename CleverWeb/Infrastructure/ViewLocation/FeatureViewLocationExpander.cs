using Microsoft.AspNetCore.Mvc.Razor;

namespace CleverWeb.Infrastructure.ViewLocation
{
    public class FeatureViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context) { }

        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            return new[]
            {
                "/Features/{1}/Views/{0}.cshtml",
                "/Features/Shared/{0}.cshtml",
                "/Views/Shared/{0}.cshtml"
            }.Concat(viewLocations);
        }
    }
}
