using B1WEB.DBContext;
using B1WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace B1WEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyAppContext _context;

        public AccountController(MyAppContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }     
        public IActionResult LoginToPortal(UserViewModel model)
        {

            try
            {
                var userexist = _context.PortalUsers.Where(x => x.UserName == model.username && x.Password == model.password).FirstOrDefault();
                if (userexist == null)
                {
                    TempData["ErrorMessage"] = "Incorrect username or password.";
                    return RedirectToAction("Login");
                }
                else
                {
                    HttpContext.Session.SetString("UserName", userexist.UserName);
                    HttpContext.Session.SetString("image", userexist.Image);
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] =ex.Message;
                return RedirectToAction("Login");
            }
       
        }
    }
}
