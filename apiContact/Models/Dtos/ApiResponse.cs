namespace apiContact.Models.Dtos
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int? Total { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success", int? total = null)
            => new() { Success = true, Data = data, Message = message, Total = total };

        public static ApiResponse<T> Fail(string message)
            => new() { Success = false, Message = message };
    }
}
