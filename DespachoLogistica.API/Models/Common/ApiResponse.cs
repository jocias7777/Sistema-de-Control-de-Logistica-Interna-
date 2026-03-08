namespace DespachoLogistica.API.Models.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Éxito")
            => new() { Success = true, Message = message, Data = data, StatusCode = 200 };

        public static ApiResponse<T> Fail(string message, int statusCode = 400)
            => new() { Success = false, Message = message, Data = default, StatusCode = statusCode };
    }
}