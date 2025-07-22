using Microsoft.AspNetCore.Mvc;

namespace GestorPacientes.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
