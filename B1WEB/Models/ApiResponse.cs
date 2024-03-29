using B1WEB.Common;

namespace B1WEB.Models
{
    public class ApiResponse<T>
    {
        public ResponseCode Code { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }
}
