using Azure.Core;
using B1WEB.AppModels;
using B1WEB.DBContext;
using B1WEB.DTO;
using B1WEB.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.ComponentModel.Design;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.IO;
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
        public async Task<IActionResult> RedirectToDashbord(CompanyViewModel company)
        {
            string FinalresponseData = string.Empty;
            if (company != null)
            {
                int companyId = Convert.ToInt32(company.CompanyID);

                var companyy = _context.CompanyConfiguration.Where(x => x.ID == companyId).FirstOrDefault();
                string ConfiguredAPIUrl = companyy.ServiceLayerURL;
                string apiUrl = ConfiguredAPIUrl+ "/b1s/v1/Login";

                    LoginRequestModel RequestModel = new LoginRequestModel();
                    RequestModel.CompanyDB = companyy.DatabaseName;
                    RequestModel.UserName = companyy.ServiceLayerUsername;
                    RequestModel.Password = companyy.ServiceLayerPassword;

               
                string jsonRequestBody = JsonConvert.SerializeObject(RequestModel);

                // Make the request
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.KeepAlive = true;
                httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                httpWebRequest.ServicePoint.Expect100Continue = false;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonRequestBody);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var responseInstance = JsonConvert.DeserializeObject<LoginResponse>(result);

                    HttpContext.Session.SetString("SessionID", Convert.ToString(responseInstance.SessionId));
                    HttpContext.Session.SetString("CompanyID", Convert.ToString(companyId));
                    HttpContext.Session.SetString("CompanyName", Convert.ToString(companyy.DatabaseName));
                    HttpContext.Session.SetString("ServiceLayerURL", Convert.ToString(ConfiguredAPIUrl));
                    SaveSaleOrderQuery();
                    SaveBusinessPartnerQuery();
                    SaveItemsQuery();
                    SaveContactPersonQuery();
                    SaveSeriesQuery();
                    SaveSapSalesPersons();
                    SaveQueryForImagePath();
                    SaveSaleReturnQuery();
                    SaveSaleInvoiceQuery();
                    SaveInventoryItemsQuery();
                    GetImagePath();


                }

               

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
        public void SaveSaleOrderQuery()
        {
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries";
                DTOCreateQuery createQuery = new DTOCreateQuery();
                createQuery.SqlName = "Get All Sales Order";
                createQuery.SqlText = "select  DocNum,DocEntry,CardCode,CardName,DocDate,DocDueDate,DocStatus ,DocTotal from ORDR Where DocStatus='O'";
                createQuery.SqlCode = "GetSalesOrder";

                string jsonRequestBody = JsonConvert.SerializeObject(createQuery);

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

                // Get the response or handle the error if any
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var responseInstance = JsonConvert.DeserializeObject<DTOResponseCreateQuery>(result);
                        // Process the response as needed
                    }
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
                            // Handle the error response
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
        }
        public void SaveSaleReturnQuery()
        {
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries";
                DTOCreateQuery createQuery = new DTOCreateQuery();
                createQuery.SqlName = "Get All Sales Return";
                createQuery.SqlText = "select  DocNum,DocEntry,CardCode,CardName,DocDate,DocDueDate,DocStatus ,DocTotal from ORDN Where DocStatus='O'";
                createQuery.SqlCode = "GetSalesReturn";

                string jsonRequestBody = JsonConvert.SerializeObject(createQuery);

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

                // Get the response or handle the error if any
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var responseInstance = JsonConvert.DeserializeObject<DTOResponseCreateQuery>(result);
                        // Process the response as needed
                    }
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
                            // Handle the error response
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
        }
             public void SaveSaleInvoiceQuery()
        {
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries";
                DTOCreateQuery createQuery = new DTOCreateQuery();
                createQuery.SqlName = "Get All Sales Invoices";
                createQuery.SqlText = "select  DocNum,DocEntry,CardCode,CardName,DocDate,DocDueDate,DocStatus ,DocTotal from OINV Where DocStatus='O'";
                createQuery.SqlCode = "GetSalesInvoices";

                string jsonRequestBody = JsonConvert.SerializeObject(createQuery);

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

                // Get the response or handle the error if any
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var responseInstance = JsonConvert.DeserializeObject<DTOResponseCreateQuery>(result);
                        // Process the response as needed
                    }
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
                            // Handle the error response
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
        }

        public void SaveBusinessPartnerQuery()
        {
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries";
                DTOCreateQuery createQuery = new DTOCreateQuery();
                createQuery.SqlName = "Get All Business Partner";
                createQuery.SqlText = "select CardCode,CardName from OCRD where CardType='C'";
                createQuery.SqlCode = "GetBusinessPartner";

                string jsonRequestBody = JsonConvert.SerializeObject(createQuery);

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

                // Get the response or handle the error if any
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var responseInstance = JsonConvert.DeserializeObject<DTOResponseCreateQuery>(result);
                        // Process the response as needed
                    }
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
                            // Handle the error response
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
        }
        public void SaveContactPersonQuery()
        {
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries";
                DTOCreateQuery createQuery = new DTOCreateQuery();
                createQuery.SqlName = "Get All ContactPersons Against BP";
                createQuery.SqlText = "select Name,CntctCode  from OCPR a where a.CardCode= :cardcode";
                createQuery.SqlCode = "GetContactaPersonAgainstBP";

                string jsonRequestBody = JsonConvert.SerializeObject(createQuery);

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

                // Get the response or handle the error if any
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var responseInstance = JsonConvert.DeserializeObject<DTOResponseCreateQuery>(result);
                        // Process the response as needed
                    }
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
                            // Handle the error response
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
        }
        public void SaveSeriesQuery()
        {
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries";
                DTOCreateQuery createQuery = new DTOCreateQuery();
                createQuery.SqlName = "Get Sap Sale Order Series";
                createQuery.SqlText = "select Series,SeriesName from NNM1 where ObjectCode='17'";
                createQuery.SqlCode = "GetSapSaleOrderSeries";

                string jsonRequestBody = JsonConvert.SerializeObject(createQuery);

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

                // Get the response or handle the error if any
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var responseInstance = JsonConvert.DeserializeObject<DTOResponseCreateQuery>(result);
                        // Process the response as needed
                    }
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
                            // Handle the error response
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
        }
        public void SaveSapSalesPersons()
        {
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries";
                DTOCreateQuery createQuery = new DTOCreateQuery();
                createQuery.SqlName = "Get Sap Sales person";
                createQuery.SqlText = "select SlpCode,SlpName from OSLP";
                createQuery.SqlCode = "GetSapSalesPersons";

                string jsonRequestBody = JsonConvert.SerializeObject(createQuery);

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

                // Get the response or handle the error if any
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var responseInstance = JsonConvert.DeserializeObject<DTOResponseCreateQuery>(result);
                        // Process the response as needed
                    }
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
                            // Handle the error response
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
        }
        public void SaveInventoryItemsQuery()
        {
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries";
                DTOCreateQuery createQuery = new DTOCreateQuery();
                createQuery.SqlName = "Get All Inventory Items";
                createQuery.SqlText = "Select a.ItemCode,a.ItemName,a.PicturName,b.ItmsGrpNam,a.LastPurPrc from OITM a inner join OITB b on a.ItmsGrpCod=b.ItmsGrpCod Where a.ValidFor='Y'";
                createQuery.SqlCode = "GetSapInventoryItems";

                string jsonRequestBody = JsonConvert.SerializeObject(createQuery);

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

                // Get the response or handle the error if any
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var responseInstance = JsonConvert.DeserializeObject<DTOResponseCreateQuery>(result);
                        // Process the response as needed
                    }
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
                            // Handle the error response
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
        }  
        public void SaveItemsQuery()
        {
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries";
                DTOCreateQuery createQuery = new DTOCreateQuery();
                createQuery.SqlName = "Get All Items";
                createQuery.SqlText = "select ItemCode,ItemName,PicturName from OITM where ValidFor='Y'";
                createQuery.SqlCode = "GetSapItems";

                string jsonRequestBody = JsonConvert.SerializeObject(createQuery);

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

                // Get the response or handle the error if any
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var responseInstance = JsonConvert.DeserializeObject<DTOResponseCreateQuery>(result);
                        // Process the response as needed
                    }
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
                            // Handle the error response
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
        }  
        public void SaveQueryForImagePath()
        {
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries";
                DTOCreateQuery createQuery = new DTOCreateQuery();
                createQuery.SqlName = "GetImagePath";
                createQuery.SqlText = "select BitmapPath from OADP";
                createQuery.SqlCode = "GetImageFolder";

                string jsonRequestBody = JsonConvert.SerializeObject(createQuery);

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

                // Get the response or handle the error if any
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var responseInstance = JsonConvert.DeserializeObject<DTOResponseCreateQuery>(result);
                        // Process the response as needed
                    }
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
                            // Handle the error response
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
        }

        public QueryResponseImagePathDTO GetImagePath()
        {
            QueryResponseImagePathDTO saleOrderResponseDTO = new QueryResponseImagePathDTO();
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetImageFolder')/List";

                // Make the request
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.KeepAlive = true;
                httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                httpWebRequest.ServicePoint.Expect100Continue = false;
                httpWebRequest.Headers.Add("Cookie", $"B1SESSION={SessionID}");

                // Get the response or handle the error if any
                using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        var responseInstance = JsonConvert.DeserializeObject<QueryResponseImagePathDTO>(result);

                        string sourceDirectory = responseInstance.value[0].BitmapPath;
                        string targetDirectory =@"wwwroot/ItemImages";

                        // Get all files in the source directory
                        string[] files = Directory.GetFiles(sourceDirectory);

                        // Copy each file to the target directory
                        foreach (string file in files)
                        {
                            string fileName = Path.GetFileName(file);
                            string destFile = Path.Combine(targetDirectory, fileName);
                            System.IO.File.Copy(file, destFile, true); // Set overwrite to true if you want to overwrite existing files
                        }

                        return responseInstance;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, log error, etc.
                Console.WriteLine("Error occurred: " + ex.Message);
                // You might want to return an error response DTO or re-throw the exception
            }

            // If something went wrong, return an empty or default DTO
            return saleOrderResponseDTO;
        }


    }
}
