namespace Invento.API.Common
{
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
    }
}
