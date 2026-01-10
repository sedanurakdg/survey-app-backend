namespace SurveyApp.Core.Exceptions;

public sealed class DomainValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public DomainValidationException(string key, string message)
        : base("Validation failed.")
    {
        Errors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            [key] = new[] { message }
        };
    }
}
