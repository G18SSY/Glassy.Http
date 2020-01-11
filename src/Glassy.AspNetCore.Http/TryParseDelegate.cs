namespace Glassy.Http.AspNetCore
{
    /// <summary>
    ///     Tries to convert the specified string representation of a logical value to its <typeparamref name="T" />
    ///     equivalent. A return value indicates whether the conversion succeeded   or failed.
    /// </summary>
    /// <typeparam name="T">    The type to convert to. </typeparam>
    /// <param name="value">     A string containing the value to convert. </param>
    /// <param name="result">   [out] The result if the conversion succeeded. </param>
    /// <returns>
    ///     true if value was converted successfully; otherwise, false.
    /// </returns>
    public delegate bool TryParseDelegate<T>(string value, out T result);
}