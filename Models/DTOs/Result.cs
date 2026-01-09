namespace Models.DTOs
{
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; }=null!;
        public object? Data { get; set; }
    }
    public class Result<T>
    {
        public bool Success{ get; set; }
        public string Message { get; set; }=null!;
        public T? Data { get; set; }
    }
}