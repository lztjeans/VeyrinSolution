using Microsoft.AspNetCore.Mvc;

namespace newAPD.Infrastructure.ViewComponents;

[ViewComponent(Name = "ICPSDSFile")]
public class ICPSDSFileViewComponent : ViewComponent
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    public ICPSDSFileViewComponent() { }

    public IViewComponentResult Invoke()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            _logger.Fatal(ex);
            throw;
        }
    }
}