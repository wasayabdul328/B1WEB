using B1WEB.AppModels;
using B1WEB.DBContext;
using B1WEB.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        public IActionResult RedirectToDashbord(CompanyViewModel company)
        {
            if(company != null)
            {
                int companyId = Convert.ToInt32(company.CompanyID);

                var companyy = _context.CompanyConfiguration.Where(x => x.ID == companyId).FirstOrDefault();
                HttpContext.Session.SetString("CompanyID", Convert.ToString(companyId));
                HttpContext.Session.SetString("CompanyName", Convert.ToString(companyy.DatabaseName));

            }
          

            return RedirectToAction("Index","Home");
        }   
        public IActionResult UserCompany()
        {
            var userId = Convert.ToInt64(HttpContext.Session.GetString("ID"));

            var UserCompanies=_context.UserCompany.Where(x=>x.PortalUsersId== userId ).ToList();

         List<CompanyConfiguration> Companies = new List<CompanyConfiguration>();




            foreach (var company in UserCompanies)
            {
                CompanyConfiguration companyConfiguration= _context.CompanyConfiguration.Where(x => x.ID == company.DatabaseId).FirstOrDefault();
                Companies.Add(companyConfiguration);
            }
            ViewBag.CompanyConfiguration = Companies;
        

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
                    HttpContext.Session.SetString("ID", Convert.ToString(userexist.ID));
                    HttpContext.Session.SetString("UserName", userexist.UserName);
                    HttpContext.Session.SetString("image", userexist.Image);
                    return RedirectToAction("UserCompany");
                }
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] =ex.Message;
                return RedirectToAction("Login");
            }
       
        }

        public IActionResult SignOut()
        {

            HttpContext.Session.Remove("ID");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("image");
            HttpContext.Session.Remove("CompanyID");
            HttpContext.Session.Remove("CompanyName");



            return RedirectToAction("Login");
        }


    }
}
