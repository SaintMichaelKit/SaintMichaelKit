namespace SaintMichaelKit.Extensions;

public static class GuidExtensions
{
    /// <summary>
    /// Checks if the Guid is empty (all zeros).
    /// </summary>
    /// <param name="guid">The Guid to check.</param>
    /// <returns>True if the Guid is empty; otherwise, false.</returns>
    public static bool IsEmpty(this Guid guid)
    {
        return guid == Guid.Empty;
    }

    /// <summary>
    /// Converts the Guid to a string in "N" format (32 digits, no hyphens).
    /// </summary>
    /// <param name="guid">The Guid to convert.</param>
    /// <returns>String representation in "N" format.</returns>
    public static string ToStringN(this Guid guid)
    {
        return guid.ToString("N");
    }

    /// <summary>
    /// Converts the Guid to a string in "D" format (hyphen-separated).
    /// </summary>
    /// <param name="guid">The Guid to convert.</param>
    /// <returns>String representation in "D" format.</returns>
    public static string ToStringD(this Guid guid)
    {
        return guid.ToString("D");
    }

    /// <summary>
    /// Converts the Guid to a string in "B" format (enclosed in braces).
    /// </summary>
    /// <param name="guid">The Guid to convert.</param>
    /// <returns>String representation in "B" format.</returns>
    public static string ToStringB(this Guid guid)
    {
        return guid.ToString("B");
    }

    /// <summary>
    /// Converts the Guid to a string in "P" format (enclosed in parentheses).
    /// </summary>
    /// <param name="guid">The Guid to convert.</param>
    /// <returns>String representation in "P" format.</returns>
    public static string ToStringP(this Guid guid)
    {
        return guid.ToString("P");
    }

    /// <summary>
    /// Converts the Guid to a Base64 string, useful for compact storage or transmission.
    /// </summary>
    /// <param name="guid">The Guid to convert.</param>
    /// <returns>Base64 string representation of the Guid.</returns>
    public static string ToBase64(this Guid guid)
    {
        return Convert.ToBase64String(guid.ToByteArray());
    }

    /// <summary>
    /// Converts the Guid to a URL-safe Base64 string by removing padding and replacing URL-unsafe characters.
    /// </summary>
    /// <param name="guid">The Guid to convert.</param>
    /// <returns>URL-safe Base64 string representation of the Guid.</returns>
    public static string ToBase64UrlSafe(this Guid guid)
    {
        return Convert.ToBase64String(guid.ToByteArray())
                      .TrimEnd('=')
                      .Replace('+', '-')
                      .Replace('/', '_');
    }

    /// <summary>
    /// Attempts to parse a URL-safe Base64 encoded string back into a Guid.
    /// </summary>
    /// <param name="base64UrlSafe">The URL-safe Base64 encoded string.</param>
    /// <param name="guid">The parsed Guid if successful; otherwise, Guid.Empty.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    public static bool TryParseBase64UrlSafe(string base64UrlSafe, out Guid guid)
    {
        guid = Guid.Empty;

        if (string.IsNullOrEmpty(base64UrlSafe))
            return false;

        string base64 = base64UrlSafe
            .Replace('-', '+')
            .Replace('_', '/');

        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        try
        {
            var bytes = Convert.FromBase64String(base64);
            if (bytes.Length != 16)
                return false;

            guid = new Guid(bytes);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Parses the given string to a Guid. Throws an exception if the input is not a valid Guid.
    /// </summary>
    /// <param name="input">The string representation of a Guid.</param>
    /// <returns>The parsed Guid.</returns>
    /// <exception cref="ArgumentException">Thrown if the input string is not a valid Guid.</exception>
    public static Guid ParseGuidOrThrow(this string? input)
    {
        if (Guid.TryParse(input, out var guid))
            return guid;

        throw new ArgumentException($"Invalid GUID format: '{input}'", nameof(input));
    }

    /// <summary>
    /// Generates a new Guid with lowercase letters in "D" format.
    /// </summary>
    /// <returns>A new Guid.</returns>
    public static Guid GenerateNewGuid()
    {
        // Guid.NewGuid() already returns a new Guid; no need to parse strings.
        // If you want lowercase string representation, call ToString("d").ToLower() separately.
        return Guid.NewGuid();
    }
}
