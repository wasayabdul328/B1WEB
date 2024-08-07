﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Net;
using B1WEB.Models;
using Newtonsoft.Json;
using B1WEB.DTO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using B1WEB.Common;
using B1WEB.ActionFilters;
using B1WEB.DBContext;

namespace B1WEB.Controllers
{

    [SessionAuthorizationFilter]
    public class SalesController : Controller
    {

        private readonly MyAppContext _context;

        public SalesController(MyAppContext context)
        {
            _context = context;
        }
        public IActionResult SalesOrder()
        {
            var salesorder = GetSalesOrders();
            ViewBag.SalesOrders= salesorder.value;
            var logintype = HttpContext.Session.GetString("logintype");
            if (logintype == "0")
            {
                 var userid =Convert.ToInt32 (HttpContext.Session.GetString("ID"));
                var addAllowed = _context.UserPermission.Where(x => x.UserID == userid && x.FormId == 1).Select(x => x.CanCreate).FirstOrDefault();
                  var deleteAllowed = _context.UserPermission.Where(x => x.UserID == userid && x.FormId == 1).Select(x => x.CanDelete).FirstOrDefault();
                ViewBag.CreateAllowed = addAllowed;
                ViewBag.DeleteAllowed = deleteAllowed;
            }
            else
            {
                ViewBag.CreateAllowed = true;
                ViewBag.DeleteAllowed = true;
            }
          
           
            return View();
        }  
        public IActionResult AddSaleOrder()
        {
            ViewBag.BusinessPartners = GetBusinessPartners();
            ViewBag.Items = GetSapItems();
            ViewBag.Series = GetSapSereies();
            ViewBag.SalesPersons = GetSapSalesPersons();
            ViewBag.TaxCodes = GetTaxCodes();
           // ViewBag.GetImagePath = GetImagePath();

            return View();
        }
        public IActionResult ViewSaleOrder(int docNum)
        {
            var singleSaleOrder = GetSingleSalesOrders( docNum);
            ViewBag.Series = GetSapSereies();
            ViewBag.SalesPersons = GetSapSalesPersons();

            ViewBag.SingleSaleOrder = singleSaleOrder;
           // ViewBag.GetImagePath = GetImagePath();


            return View();
        }
        public IActionResult SalesReturn()
        {
            var salesReturn = GetSalesReturn();
            ViewBag.SalesReturn = salesReturn.value;
            return View();
        }
        public IActionResult ARInvoice()
        {
            var salesInvoice = GetSalesInvoice();
            ViewBag.SalesInvoice = salesInvoice.value;
            return View();
        } 
        public IActionResult ARCreditNote()
        {
            var salesInvoice = GetARCreditNotes();
            ViewBag.SalesInvoice = salesInvoice.value;
            return View();
        }
        public IActionResult APCreditNote()
        {
            var salesInvoice = GetAPCreditNotes();
            ViewBag.SalesInvoice = salesInvoice.value;
            return View();
        }
        public IActionResult Inventory()
        {
            ViewBag.ItemsList = GetSapInventoryItems();
            return View();
        }
        public QueryResponseDTO GetSalesOrders()
        {
            String apiUrl = "";
            QueryResponseDTO saleOrderResponseDTO = new QueryResponseDTO();
            var logintype = HttpContext.Session.GetString("logintype");
            var cardCode = HttpContext.Session.GetString("ID");
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            if (logintype == "0")
            {
                apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetSalesOrder')/List";
            }
            else
            {
                apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetSalesOrderCustomer')/List?cardcode='"+ cardCode+"'";
            }
            
            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");
            httpWebRequest.Headers.Add("Prefer", $"odata.maxpagesize=100000");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var responseInstance = JsonConvert.DeserializeObject<QueryResponseDTO>(result);
                    return responseInstance;
                    // Process the response as needed
                }
            }
            return  saleOrderResponseDTO;
        }
         public QueryResponseDTO GetSalesReturn()
        {
            string apiUrl = "";
            QueryResponseDTO saleOrderResponseDTO = new QueryResponseDTO();
            var logintype = HttpContext.Session.GetString("logintype");
            var cardCode = HttpContext.Session.GetString("ID");
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
      
            if (logintype == "0")
            {
                apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetSalesReturn')/List";
            }
            else
            {
                apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetSalesReturnCustomer')/List?cardcode='" + cardCode + "'";
            }
            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");
            httpWebRequest.Headers.Add("Prefer", $"odata.maxpagesize=10000");
            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var responseInstance = JsonConvert.DeserializeObject<QueryResponseDTO>(result);
                    return responseInstance;
                    // Process the response as needed
                }
            }
            return  saleOrderResponseDTO;
        }
         public QueryResponseDTO GetSalesInvoice()
        {
            string apiUrl = "";
            var logintype = HttpContext.Session.GetString("logintype");
            var cardCode = HttpContext.Session.GetString("ID");
            QueryResponseDTO saleOrderResponseDTO = new QueryResponseDTO();
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            if (logintype == "0")
            {
                apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetSalesInvoices')/List";
            }
            else
            {
                apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetSalesInvoicesCustomer')/List?cardcode='" + cardCode + "'";
            }
            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");
            httpWebRequest.Headers.Add("Prefer", $"odata.maxpagesize=10000");
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var responseInstance = JsonConvert.DeserializeObject<QueryResponseDTO>(result);
                    return responseInstance;
                    // Process the response as needed
                }
            }
            return  saleOrderResponseDTO;
        }

        public QueryResponseDTO GetARCreditNotes()
        {
            string apiUrl = "";
            var logintype = HttpContext.Session.GetString("logintype");
            var cardCode = HttpContext.Session.GetString("ID");
            QueryResponseDTO saleOrderResponseDTO = new QueryResponseDTO();
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            if (logintype == "0")
            {
                apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetARCreditNoteQuery')/List";
            }
            else
            {
                apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetARCreditNoteCustomerQuery')/List?cardcode='" + cardCode + "'";
            }
            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");
            httpWebRequest.Headers.Add("Prefer", $"odata.maxpagesize=10000");
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var responseInstance = JsonConvert.DeserializeObject<QueryResponseDTO>(result);
                    return responseInstance;
                    // Process the response as needed
                }
            }
            return saleOrderResponseDTO;
        }
        
        public QueryResponseDTO GetAPCreditNotes()
        {
            string apiUrl = "";
            var logintype = HttpContext.Session.GetString("logintype");
            var cardCode = HttpContext.Session.GetString("ID");
            QueryResponseDTO saleOrderResponseDTO = new QueryResponseDTO();
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            if (logintype == "0")
            {
                apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetAPCreditNoteQuery')/List";
            }
            else
            {
                apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetAPCreditNoteCustomerQuery')/List?cardcode='" + cardCode + "'";
            }
            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");
            httpWebRequest.Headers.Add("Prefer", $"odata.maxpagesize=10000");
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var responseInstance = JsonConvert.DeserializeObject<QueryResponseDTO>(result);
                    return responseInstance;
                    // Process the response as needed
                }
            }
            return saleOrderResponseDTO;
        }

        public JsonResult GetContactPersonAgainstCustomer(string cardCode)
        {
            QueryResponseForContactPersonDTO saleOrderResponseDTO = new QueryResponseForContactPersonDTO();
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetContactaPersonAgainstBP')/List?cardcode='"+ cardCode+"'";

            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var responseInstance = JsonConvert.DeserializeObject<QueryResponseForContactPersonDTO>(result);

                 
                    return Json(responseInstance.value);
                    // Process the response as needed
                }
            }
            return Json(null);
        }
        public JsonResult GetPriceOfItem(string itemcode,string pricelist)
        {
            QueryResponseForContactPersonDTO saleOrderResponseDTO = new QueryResponseForContactPersonDTO();
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetPriceOfItems')/List?itemcode='" + itemcode + "'&pricelist='"+ pricelist + "'";

            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var responseInstance = JsonConvert.DeserializeObject<QueryResponseForPrice>(result);

                 
                    return Json(responseInstance);
                    // Process the response as needed
                }
            }
            return Json(null);
        }

        [HttpPost]
        public JsonResult PostsaleOrder([FromBody]SaleOrderDto sorder)
        {
            ApiResponse<SaleOrderDto> apiResponse = new ApiResponse<SaleOrderDto>();
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/Orders";




                string jsonRequestBody = JsonConvert.SerializeObject(sorder);

                // Make the request
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.KeepAlive = true;
                httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                httpWebRequest.ServicePoint.Expect100Continue = false;
                httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonRequestBody);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    

                    apiResponse.Code = ResponseCode.Success;
                    apiResponse.Message = "Sale Order Created Successfully";

                    return Json(apiResponse);

                }

            }
            catch (WebException webEx)
            {
                if (webEx.Response != null && webEx.Response is HttpWebResponse httpWebResponse)
                {
                    if (httpWebResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        // Handle bad request (HTTP 400) error
                        // You can retrieve more details from the response if needed
                        using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            var errorResponse = streamReader.ReadToEnd();
                            var responseInstance = JsonConvert.DeserializeObject<ErrorResponse>(errorResponse);
                          
                            apiResponse.Code = ResponseCode.Error;
                            apiResponse.Message =Convert.ToString( responseInstance.error.message.value);
                        }
                    }
                    else
                    {
                        // Handle other HTTP errors
                    }
                }
                else
                {
                    // Handle other types of exceptions or network errors
                }
            }

            return Json(apiResponse);
        }

        [HttpGet]
        public JsonResult ClosesaleOrder(int docNum)
        {
            ApiResponse<SaleOrderDto> apiResponse = new ApiResponse<SaleOrderDto>();
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/Orders("+ docNum + ")/Close";



                // Make the request
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.KeepAlive = true;
                httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                httpWebRequest.ServicePoint.Expect100Continue = false;
                httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();



                    apiResponse.Code = ResponseCode.Success;
                    apiResponse.Message = "Sale Order Closed Successfully";

                    return Json(apiResponse);

                }

            }
            catch (WebException webEx)
            {
                if (webEx.Response != null && webEx.Response is HttpWebResponse httpWebResponse)
                {
                    if (httpWebResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        // Handle bad request (HTTP 400) error
                        // You can retrieve more details from the response if needed
                        using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            var errorResponse = streamReader.ReadToEnd();
                            var responseInstance = JsonConvert.DeserializeObject<ErrorResponse>(errorResponse);

                            apiResponse.Code = ResponseCode.Error;
                            apiResponse.Message = Convert.ToString(responseInstance.error.message.value);
                        }
                    }
                    else
                    {
                        // Handle other HTTP errors
                    }
                }
                else
                {
                    // Handle other types of exceptions or network errors
                }
            }

            return Json(apiResponse);
        }





        public QueryResponseDTO GetSapItems()
        {
            QueryResponseDTO saleOrderResponseDTO = new QueryResponseDTO();
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetSapItems')/List";

            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");
            httpWebRequest.Headers.Add("Prefer", $"odata.maxpagesize=10000");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var responseInstance = JsonConvert.DeserializeObject<QueryResponseDTO>(result);
                    return responseInstance;
                    // Process the response as needed
                }
            }
            return saleOrderResponseDTO;
        }


        public object GetSapInventoryItems()
        {
            QueryResponseDTO saleOrderResponseDTO = new QueryResponseDTO();
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetSapInventoryItems')/List";

            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");
            httpWebRequest.Headers.Add("Prefer", $"odata.maxpagesize=2000");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var responseInstance = JsonConvert.DeserializeObject<QueryResponseDTO>(result);
                    return responseInstance.value;
                    // Process the response as needed
                }
            }
            return saleOrderResponseDTO;
        }

        public QueryResponseDTO GetTaxCodes()
        {
            QueryResponseDTO saleOrderResponseDTO = new QueryResponseDTO();
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetTaxCodes')/List";

            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var responseInstance = JsonConvert.DeserializeObject<QueryResponseDTO>(result);
                    return responseInstance;
                    // Process the response as needed
                }
            }
            return saleOrderResponseDTO;
        }
        
        public QueryResponseDTO GetSapSereies()
        {
            QueryResponseDTO saleOrderResponseDTO = new QueryResponseDTO();
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetSapSaleOrderSeries')/List";

            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var responseInstance = JsonConvert.DeserializeObject<QueryResponseDTO>(result);
                    return responseInstance;
                    // Process the response as needed
                }
            }
            return saleOrderResponseDTO;
        }
        
        public QueryResponseDTO GetSapSalesPersons()
        {
            QueryResponseDTO saleOrderResponseDTO = new QueryResponseDTO();
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetSapSalesPersons')/List";

            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var responseInstance = JsonConvert.DeserializeObject<QueryResponseDTO>(result);
                    return responseInstance;
                    // Process the response as needed
                }
            }
            return saleOrderResponseDTO;
        }

     
        
        public QueryResponseDTO GetBusinessPartners()
        {
            string apiUrl = "";
            var logintype = HttpContext.Session.GetString("logintype");
            var cardCode = HttpContext.Session.GetString("ID");
            QueryResponseDTO saleOrderResponseDTO = new QueryResponseDTO();
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");

            if (logintype == "0")
            {
                apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetBusinessPartner')/List";
            }
            else
            {
                apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetBusinessPartnerCustomer')/List?cardcode='" + cardCode + "'";
            }

           

            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");
            httpWebRequest.Headers.Add("Prefer", $"odata.maxpagesize=10000");
            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var responseInstance = JsonConvert.DeserializeObject<QueryResponseDTO>(result);
                    return responseInstance;
                    // Process the response as needed
                }
            }
            return saleOrderResponseDTO;
        }





        public object GetSingleSalesOrders(int docNum)
        {
            
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/Orders("+ docNum + ")";
            
            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    dynamic obj = JsonConvert.DeserializeObject(result);
                 //   var responseInstance = JsonConvert.DeserializeObject<SaleOrderResponseDTO>(result);
                    return obj;
                    // Process the response as needed
                }
            }
            return  null;
        }

       
        public String GetSingleSalesReturn(int docNum)
        {
            
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/Returns("+ docNum + ")";
            
            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    
                 //   var responseInstance = JsonConvert.DeserializeObject<SaleOrderResponseDTO>(result);
                    return result;
                    // Process the response as needed
                }
            }
            return  "";
        }
        
       
        public String GetSingleArCreditNote(int docNum)
        {
            
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/CreditNotes(" + docNum + ")";
            
            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    
                 //   var responseInstance = JsonConvert.DeserializeObject<SaleOrderResponseDTO>(result);
                    return result;
                    // Process the response as needed
                }
            }
            return  "";
        }
        
        public String GetSingleAPCreditNote(int docNum)
        {
            
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/PurchaseCreditNotes(" + docNum + ")";
            
            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    
                 //   var responseInstance = JsonConvert.DeserializeObject<SaleOrderResponseDTO>(result);
                    return result;
                    // Process the response as needed
                }
            }
            return  "";
        }
        
        public String GetSingleSalesInvoice(int docNum)
        {
            
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/Invoices(" + docNum + ")";
            
            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    
                 //   var responseInstance = JsonConvert.DeserializeObject<SaleOrderResponseDTO>(result);
                    return result;
                    // Process the response as needed
                }
            }
            return  "";
        }

       
       
        public String GetItemAgainstBarCodeQuery(string barcode)
        {
            
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetItemAgainstBarCodeQuery')/List?barcode='" + barcode + "'";

            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    
                 //   var responseInstance = JsonConvert.DeserializeObject<SaleOrderResponseDTO>(result);
                    return result;
                    // Process the response as needed
                }
            }
            return  "";
        }

        
       
        public String GetPriceListAgainstCustomerQuery(string cardCode)
        {
            
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('SavePriceListAgainstCustomerQuery')/List?cardCode='"+ cardCode + "'";

            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    
                 //   var responseInstance = JsonConvert.DeserializeObject<SaleOrderResponseDTO>(result);
                    return result;
                    // Process the response as needed
                }
            }
            return  "";
        }

       
        public String GetItemDetails(string itemcode)
        {
            
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/Items('" + itemcode + "')";
            
            // Make the request
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(jsonRequestBody);
            //}

            // Get the response or handle the error if any
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    
                 //   var responseInstance = JsonConvert.DeserializeObject<SaleOrderResponseDTO>(result);
                    return result;
                    // Process the response as needed
                }
            }
            return  "";
        }

       
    }
}
