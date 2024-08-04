namespace PDFTools.Models
{
    //TODO: Make method for returning new Attempt with error message. Need to update all guard clauses to use it. They take up to much space.
    //TODO: look at variables with attempt and make sure to udpate them to sound better.
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
