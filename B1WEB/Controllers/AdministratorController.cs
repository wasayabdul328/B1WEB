using B1WEB.ActionFilters;
using B1WEB.AppModels;
using B1WEB.DBContext;
using Microsoft.AspNetCore.Mvc;

namespace B1WEB.Controllers
{
    [SessionAuthorizationFilter]
    public class AdministratorController : Controller
    {
        private readonly MyAppContext _context;
        private List<FormsModel> forms = new List<FormsModel>()
                                        {
                                         new FormsModel   { FormName = "SalesOrder", FormId = 1 },
                                         new FormsModel   { FormName = "SalesReturn", FormId = 2 },
                                         new FormsModel   { FormName = "ARInvoice", FormId = 3 },
                                         new FormsModel   { FormName = "ItemMasterData", FormId = 4 },
                                         new FormsModel   { FormName = "BusinessPartnerMasterDate", FormId = 5 },
                                         new FormsModel   { FormName = "ARCreditNote", FormId = 6 },
                                         new FormsModel   { FormName = "APCreditNote", FormId = 7 },
                                        };
        public AdministratorController(MyAppContext context)
        {
            _context = context;
        }
        public IActionResult manageUser()
        {
            var companyConnection=_context.CompanyConfiguration.Where(x=>x.IsActive==true).ToList();
            ViewBag.CompanyConfiguration = companyConnection;
            ViewBag.Forms = forms;
            return View();
        } 
        public IActionResult CreateUser()
        {
            return View();
        } 
        public IActionResult CompanyConfiguration()

        {
            return View();
        }
     
    }
    public class FormsModel
    {
        public int FormId { get; set; }
        public string FormName { get; set; }
    }
}
