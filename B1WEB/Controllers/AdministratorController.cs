using Microsoft.AspNetCore.Mvc;

namespace B1WEB.Controllers
{
    public class AdministratorController : Controller
    {
        public IActionResult manageUser()
        {
            return View();
        } 
        public IActionResult CreateUser()
        {
            return View();
        }
    }
}
