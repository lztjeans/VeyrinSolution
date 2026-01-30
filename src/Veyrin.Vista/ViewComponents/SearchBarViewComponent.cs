using Microsoft.AspNetCore.Mvc;
using Veyrin.Core.Html;

namespace Veyrin.Vista.ViewComponents;

[ViewComponent(Name = "SearchBar")]
public class SearchBarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string titleText, Func<Dictionary<int, List<AbstractElements>>?> createMethod)
    {
        ViewData["Elements"] = createMethod();
        ViewData["ViewTitle"] = titleText;
        return View();
    }
}



//private static List<AbstractElements>? GetICP_MSDSElements()
//{
//    var ret = CreateList<AbstractElements>();
//    ret.Add(SelectElement.Create(id: "Catg", name: "catg", label: "Category"));
//    ret.Add(SelectElement.Create(id: "Matl", name: "matl", label: "Material"));
//    ret.Add(TextElement.Create(id: "Malt", name: "malt", label: "Material Type"));
//    ret.Add(TextElement.Create(id: "Vndr", name: "vndr", label: "Vendor"));
//    return ret;
//}
//private static List<AbstractElements>? GetBomElements()
//{
//    var ret = CreateList<AbstractElements>();
//    ret.Add(SelectElement.Create(id: "ProdNo", name: "prodNo", label: "Product No"));
//    ret.Add(SelectElement.Create(id: "PkgFrm", name: "pkgFrm", label: "Package Form"));
//    ret.Add(SelectElement.Create(id: "Pin", name: "pin", label: "Pin"));
//    ret.Add(SelectElement.Create(id: "BdSz", name: "bdSz", label: "Body Size"));
//    ret.Add(TextElement.Create(id: "PartNo", name: "partNo", label: "Part No"));
//    ret.Add(TextElement.Create(id: "BomNo", name: "bomNo", label: "BOM No"));
//    return ret;
//}
//private static List<AbstractElements>? GetBDElements()
//{
//    var ret = CreateList<AbstractElements>();
//    ret.Add(SelectElement.Create(id: "ProdNo", name: "prodNo", label: "Product No"));
//    ret.Add(SelectElement.Create(id: "PkgFrm", name: "pkgFrm", label: "Package Form"));
//    ret.Add(SelectElement.Create(id: "Pin", name: "pin", label: "Pin"));
//    ret.Add(SelectElement.Create(id: "BdSz", name: "bdSz", label: "Body Size"));
//    ret.Add(TextElement.Create(id: "BdId", name: "bdId", label: "BD ID"));
//    return ret;
//}
