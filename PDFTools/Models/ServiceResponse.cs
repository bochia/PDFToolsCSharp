namespace PDFTools.Models
{
    //TODO: Make method for returning new Service response with error message. Need to update all guard clauses to use it. They take up to much space.
    //TODO: Rename this to Attempt. Will also need to rename variable names that have "Response" in them.
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
