using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Wrappers
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public static ApiResponse<T> SuccessResponse(T data, string message = "İşlem başarılı")
            => new() { IsSuccess = true, Data = data, Message = message };
        public static ApiResponse<T> Fail(string message)
      => new() { IsSuccess = false, Message = message, Data = default };
    }
}
