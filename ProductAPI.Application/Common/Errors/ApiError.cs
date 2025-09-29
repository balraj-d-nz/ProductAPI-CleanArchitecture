namespace ProductAPI.Application.Common.Errors
{
    public class ApiError
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; } = string.Empty;
    }
}
