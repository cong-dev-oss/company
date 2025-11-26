namespace BuildingBlocks.Guards;

/// <summary>
/// Guard clauses for input validation and fail-fast principle
/// </summary>
public static class Guard
{
    public static void AgainstNull<T>(T? value, string parameterName) where T : class
    {
        if (value is null)
            throw new ArgumentNullException(parameterName);
    }

    public static void AgainstNullOrEmpty(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"Value cannot be null or empty: {parameterName}", parameterName);
    }

    public static void AgainstNullOrEmpty<T>(IEnumerable<T>? value, string parameterName)
    {
        if (value is null || !value.Any())
            throw new ArgumentException($"Collection cannot be null or empty: {parameterName}", parameterName);
    }

    public static void AgainstOutOfRange(int value, int min, int max, string parameterName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(parameterName, $"Value must be between {min} and {max}");
    }

    public static void AgainstNegative(decimal value, string parameterName)
    {
        if (value < 0)
            throw new ArgumentException($"Value cannot be negative: {parameterName}", parameterName);
    }

    public static void AgainstNegative(int value, string parameterName)
    {
        if (value < 0)
            throw new ArgumentException($"Value cannot be negative: {parameterName}", parameterName);
    }

    public static void AgainstInvalidEmail(string email, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException($"Invalid email format: {parameterName}", parameterName);
    }
}






