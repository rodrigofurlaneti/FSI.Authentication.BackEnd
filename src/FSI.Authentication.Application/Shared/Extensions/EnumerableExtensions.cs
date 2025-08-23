namespace FSI.Authentication.Application.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<string> WhereNotNull(this IEnumerable<string?> source) =>
            source.Where(s => !string.IsNullOrWhiteSpace(s))! // filtra null/empty/whitespace
                  .Select(s => s!);
    }
}
