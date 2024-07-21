namespace PDFTools.Models
{
    public class ServiceResponse
    {
        //TODO: Am I even using this for anything?
        public int ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public int HttpStatusCode { get; set; }
        public bool Success { get; set; } = false; // default to failure.
    }

    public class ServiceResponse<T> : ServiceResponse
    {
        public T? Data { get; set; }
    }
}
