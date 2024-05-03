using Newtonsoft.Json;

namespace B1WEB.Models
{
    public class LoginRequestModel
    {
        public string CompanyDB { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }
    public class DTOCreateQuery
    {
        public string SqlCode { get; set; }
        public string SqlName { get; set; }
        public string SqlText { get; set; }
    }
    
    public class DTOResponseCreateQuery
    {
        [JsonProperty("odata.metadata")]
        public string odatametadata { get; set; }

        [JsonProperty("odata.etag")]
        public string odataetag { get; set; }
        public string SqlCode { get; set; }
        public string SqlName { get; set; }
        public string SqlText { get; set; }
        public object ParamList { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }


    public class LoginResponse
    {
        public string odatametadata { get; set; }
        public string SessionId { get; set; }
        public string Version { get; set; }
        public int SessionTimeout { get; set; }
    }
    public class Error
    {
        public int code { get; set; }
        public Message message { get; set; }
    }

    public class Message
    {
        public string lang { get; set; }
        public string value { get; set; }
    }

    public class ErrorResponse
    {
        public Error error { get; set; }
    }

}
