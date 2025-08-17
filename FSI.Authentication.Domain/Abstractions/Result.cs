namespace FSI.Authentication.Domain.Abstractions
{
    public sealed class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        private Result(bool ok, string? err) { IsSuccess = ok; Error = err; }
        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, error);
    }
}
