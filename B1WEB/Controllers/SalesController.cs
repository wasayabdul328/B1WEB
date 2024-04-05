using Microsoft.AspNetCore.Mvc;

namespace B1WEB.Controllers
{
    public class SalesController : Controller
    {
        public IActionResult SalesOrder()
        {
            return View();
        }  
        public IActionResult AddSaleOrder()
        {
            return View();
        }
        public IActionResult SalesReturn()
        {
            return View();
        }
        public IActionResult ARInvoice()
        {
            return View();
        }
        public IActionResult Inventory()
        {
            return View();
        }
    }
}
