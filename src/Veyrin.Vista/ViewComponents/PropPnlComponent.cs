using Microsoft.AspNetCore.Mvc;
using Veyrin.Core.Html;

namespace Veyrin.Vista.ViewComponents;

[ViewComponent(Name = "PropPnl")]
public class PropPnlComponent : ViewComponent
{
    public PropPnlComponent() { }

    public IViewComponentResult Invoke(string header, string elementSuffix, Func<Dictionary<int, List<AbstractElements>>?> createMethod, bool needToggle = true)
    {
        ViewData["Type"] = elementSuffix;
        ViewData["Toggle"] = needToggle;
        ViewData["Header"] = header;
        ViewData["Elements"] = createMethod();
        return View();
    }

}
