using B1WEB.Models;

namespace B1WEB.Common
{
    public static class UHelper
    {

        public static ApiResponse<object> ApiExceptionResponse(string message)
        {
            return new ApiResponse<object>
            {
                Code = ResponseCode.Error,
                Message = message,
                Data = null
            };
        }

    }
}
