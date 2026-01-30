using Microsoft.AspNetCore.Mvc;

namespace Veyrin.Vista.ViewComponents;

public class HelloViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string name) => View("Default", name);
}