using Microsoft.AspNetCore.Mvc;

namespace Veyrin.Vista.ViewComponents;

[ViewComponent(Name = "ModalConfirm")]
public class ModalConfirmViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(
        string id = "ModalConfirm",
        string header = "Confirm?",
        string msg = "Are you sure to save?",
        string btnLeftText = "Yes",
        string btnRightText = "Cancel",
        string btnLeftOnClick = "onClickModalBtnLeft()",
        string btnRightOnClick = "onClickModalBtnRight()")
    {
        ViewData["ModalId"] = id;
        ViewData["Header"] = header;
        ViewData["Message"] = msg;
        ViewData["BtnLeftText"] = btnLeftText;
        ViewData["BtnRightText"] = btnRightText;
        ViewData["BtnLeftOnClick"] = btnLeftOnClick;
        ViewData["BtnRightOnClick"] = btnRightOnClick;
        return View();
    }
}