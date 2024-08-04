namespace PDFTools.Models
{
    //TODO: Make method for returning new Service response with error message. Need to update all guard clauses to use it. They take up to much space.
    public class Attempt
    {
        public int ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public bool Success { get; set; } = false; // default to failure.
    }

    public class Attempt<T> : Attempt
    {
        public T? Data { get; set; }
    }
}
