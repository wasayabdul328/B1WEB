using B1WEB.ActionFilters;
using B1WEB.Common;
using B1WEB.DTO;
using B1WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace B1WEB.Controllers
{
    [SessionAuthorizationFilter]
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {


            ViewBag.BusinessPartners =GetBusinessPartners();
            ViewBag.CustomerGroup = GetCustomerGroupSeries();
            ViewBag.PriceList = GetPriceLists();
            return View();
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

        public QueryResponseDTO GetPriceLists()
        {
            QueryResponseDTO saleOrderResponseDTO = new QueryResponseDTO();
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetPriceLists')/List";

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

        
        public QueryResponseDTO GetCustomerGroupSeries()
        {
            QueryResponseDTO saleOrderResponseDTO = new QueryResponseDTO();
            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/SQLQueries('GetCustomerGroupSeries')/List";

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




        public String GetCustomerDetails(string cardCode)
        {

            var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
            var SessionID = HttpContext.Session.GetString("SessionID");
            string apiUrl = ConfiguredAPIUrl + "/b1s/v1/BusinessPartners('" + cardCode + "')";

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
            return "";
        }

        [HttpPost]
        public JsonResult PostBP([FromBody] BP bp)
        {
            ApiResponse<SaleOrderDto> apiResponse = new ApiResponse<SaleOrderDto>();
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/BusinessPartners";




                string jsonRequestBody = JsonConvert.SerializeObject(bp);

                // Make the request
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                httpWebRequest.ContentType = "application/json";
                //if (bp.CardCode == "" || bp.CardCode == null)
                //{
                    httpWebRequest.Method = "POST";
                //}
                //else
                //{
                //    httpWebRequest.Method = "PATCH";
                //}
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
                    apiResponse.Message = "Customer Created Successfully";

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
        
        [HttpPost]
        public JsonResult updateBP([FromBody] BP bp)
        {
            ApiResponse<SaleOrderDto> apiResponse = new ApiResponse<SaleOrderDto>();
            try
            {
                var ConfiguredAPIUrl = HttpContext.Session.GetString("ServiceLayerURL");
                var SessionID = HttpContext.Session.GetString("SessionID");
                string apiUrl = ConfiguredAPIUrl + "/b1s/v1/BusinessPartners('"+bp.CardCode+"')";




                string jsonRequestBody = JsonConvert.SerializeObject(bp);

                // Make the request
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
                httpWebRequest.ContentType = "application/json";
                //if (bp.CardCode == "" || bp.CardCode == null)
                //{
                    httpWebRequest.Method = "PATCH";
                //}
                //else
                //{
                //    httpWebRequest.Method = "PATCH";
                //}
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
                    apiResponse.Message = "Customer Updated Successfully";

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


    }
}
