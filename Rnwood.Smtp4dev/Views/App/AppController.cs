using Microsoft.AspNetCore.Mvc;

namespace Rnwood.Smtp4dev.Views.App
{
    public class AppController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}