using System.Security.Cryptography;
using System.Text;

namespace Exceptionless;

/// <summary>Generates random data for use in unit tests and data seeding.</summary>
public static class RandomData {
    /// <summary>Gets the shared <see cref="Random"/> instance. Thread-safe.</summary>
    public static Random Instance => Random.Shared;

    /// <summary>Returns a random integer in the inclusive range [<paramref name="min"/>, <paramref name="max"/>].</summary>
    /// <remarks>Returns <paramref name="min"/> immediately when both values are equal.</remarks>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    public static int GetInt(int min, int max) {
        if (min == max)
            return min;

        ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);

        return Random.Shared.Next(min, max + 1);
    }

    /// <summary>Returns a random integer across the full <see cref="Int32"/> range.</summary>
    public static int GetInt() => GetInt(Int32.MinValue, Int32.MaxValue);

    /// <summary>
    /// Returns a random version string between <paramref name="min"/> and <paramref name="max"/>.
    /// Defaults to a range of "0.0.0.0" – "25.100.9999.9999" when either bound is null or empty.
    /// </summary>
    public static string GetVersion(string? min, string? max) {
        if (String.IsNullOrEmpty(min))
            min = "0.0.0.0";
        if (String.IsNullOrEmpty(max))
            max = "25.100.9999.9999";

        if (!Version.TryParse(min, out var minVersion))
            minVersion = new Version(0, 0, 0, 0);
        if (!Version.TryParse(max, out var maxVersion))
            maxVersion = new Version(25, 100, 9999, 9999);

        minVersion = new Version(
            minVersion.Major != -1 ? minVersion.Major : 0,
            minVersion.Minor != -1 ? minVersion.Minor : 0,
            minVersion.Build != -1 ? minVersion.Build : 0,
            minVersion.Revision != -1 ? minVersion.Revision : 0);

        maxVersion = new Version(
            maxVersion.Major != -1 ? maxVersion.Major : 0,
            maxVersion.Minor != -1 ? maxVersion.Minor : 0,
            maxVersion.Build != -1 ? maxVersion.Build : 0,
            maxVersion.Revision != -1 ? maxVersion.Revision : 0);

        var major = GetInt(minVersion.Major, maxVersion.Major);
        var minor = GetInt(minVersion.Minor, major == maxVersion.Major ? maxVersion.Minor : 100);
        var build = GetInt(minVersion.Build, minor == maxVersion.Minor ? maxVersion.Build : 9999);
        var revision = GetInt(minVersion.Revision, build == maxVersion.Build ? maxVersion.Revision : 9999);

        return new Version(major, minor, build, revision).ToString();
    }

    /// <summary>Returns a random <see cref="long"/> in the inclusive range [<paramref name="min"/>, <paramref name="max"/>].</summary>
    /// <remarks>Returns <paramref name="min"/> immediately when both values are equal.</remarks>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    public static long GetLong(long min, long max) {
        if (min == max)
            return min;

        ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);

        var buf = new byte[8];
        Random.Shared.NextBytes(buf);
        long longRand = BitConverter.ToInt64(buf, 0);

        return (Math.Abs(longRand % (max - min)) + min);
    }

    /// <summary>Returns a random <see cref="long"/> across the full <see cref="Int64"/> range.</summary>
    public static long GetLong() => GetLong(Int64.MinValue, Int64.MaxValue);

    /// <summary>Returns a random latitude/longitude coordinate string in the form <c>"lat,lng"</c>.</summary>
    public static string GetCoordinate() => $"{GetDouble(-90.0, 90.0)},{GetDouble(-180.0, 180.0)}";

    /// <summary>
    /// Returns a random <see cref="DateTime"/> between <paramref name="start"/> and <paramref name="end"/>.
    /// Defaults to <see cref="DateTime.MinValue"/> and <see cref="DateTime.MaxValue"/> when not specified.
    /// </summary>
    /// <remarks>Returns <paramref name="start"/> immediately when both values are equal.</remarks>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> is greater than <paramref name="end"/>.</exception>
    public static DateTime GetDateTime(DateTime? start = null, DateTime? end = null) {
        if (start.HasValue && end.HasValue && start.Value == end.Value)
            return start.Value;

        if (start.HasValue && end.HasValue)
            ArgumentOutOfRangeException.ThrowIfGreaterThan(start.Value, end.Value, nameof(start));

        start ??= DateTime.MinValue;
        end ??= DateTime.MaxValue;

        TimeSpan timeSpan = end.Value - start.Value;
        var newSpan = new TimeSpan(GetLong(0, timeSpan.Ticks));

        return start.Value + newSpan;
    }

    /// <summary>
    /// Returns a random <see cref="DateTimeOffset"/> between <paramref name="start"/> and <paramref name="end"/>.
    /// Defaults to <see cref="DateTimeOffset.MinValue"/> and <see cref="DateTimeOffset.MaxValue"/> when not specified.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> is greater than or equal to <paramref name="end"/>.</exception>
    public static DateTimeOffset GetDateTimeOffset(DateTimeOffset? start = null, DateTimeOffset? end = null) {
        if (start.HasValue && end.HasValue)
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(start.Value, end.Value, nameof(start));

        start ??= DateTimeOffset.MinValue;
        end ??= DateTimeOffset.MaxValue;

        TimeSpan timeSpan = end.Value - start.Value;
        var newSpan = new TimeSpan(GetLong(0, timeSpan.Ticks));

        return start.Value + newSpan;
    }

    /// <summary>
    /// Returns a random <see cref="TimeSpan"/> between <paramref name="min"/> and <paramref name="max"/>.
    /// Defaults to <see cref="TimeSpan.Zero"/> and <see cref="TimeSpan.MaxValue"/> when not specified.
    /// </summary>
    /// <remarks>Returns <paramref name="min"/> immediately when both values are equal.</remarks>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    public static TimeSpan GetTimeSpan(TimeSpan? min = null, TimeSpan? max = null) {
        if (min.HasValue && max.HasValue && min.Value == max.Value)
            return min.Value;

        if (min.HasValue && max.HasValue)
            ArgumentOutOfRangeException.ThrowIfGreaterThan(min.Value, max.Value, nameof(min));

        min ??= TimeSpan.Zero;
        max ??= TimeSpan.MaxValue;

        return min.Value + new TimeSpan((long)(new TimeSpan(max.Value.Ticks - min.Value.Ticks).Ticks * Random.Shared.NextDouble()));
    }

    /// <summary>Returns <c>true</c> with the given probability percentage.</summary>
    /// <param name="chance">Probability of returning <c>true</c>, from 0 (never) to 100 (always). Clamped to [0, 100].</param>
    public static bool GetBool(int chance = 50) {
        chance = Math.Clamp(chance, 0, 100);
        double c = 1 - (chance / 100.0);
        return Random.Shared.NextDouble() > c;
    }

    /// <summary>
    /// Returns a random <see cref="double"/> in the inclusive range [<paramref name="min"/>, <paramref name="max"/>].
    /// Defaults to <see cref="Double.MinValue"/> and <see cref="Double.MaxValue"/> when not specified.
    /// </summary>
    /// <remarks>Returns <paramref name="min"/> immediately when both values are equal.</remarks>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    public static double GetDouble(double? min = null, double? max = null) {
        if (min.HasValue && max.HasValue && min.Value == max.Value)
            return min.Value;

        if (min.HasValue && max.HasValue)
            ArgumentOutOfRangeException.ThrowIfGreaterThan(min.Value, max.Value, nameof(min));

        min ??= Double.MinValue;
        max ??= Double.MaxValue;

        return Random.Shared.NextDouble() * (max.Value - min.Value) + min.Value;
    }

    /// <summary>Returns a random <see cref="decimal"/> using two random integers as bounds.</summary>
    public static decimal GetDecimal() => GetDecimal(GetInt(), GetInt());

    /// <summary>Returns a random <see cref="decimal"/> in the range [<paramref name="min"/>, <paramref name="max"/>].</summary>
    public static decimal GetDecimal(int min, int max) => (decimal)GetDouble(min, max);

    /// <summary>Returns a random value from the enum type <typeparamref name="T"/>.</summary>
    public static T GetEnum<T>() where T : struct, Enum {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(GetInt(0, values.Length - 1))!;
    }

    /// <summary>Returns a random IPv4 address string in the form <c>"a.b.c.d"</c>.</summary>
    public static string GetIp4Address() => $"{GetInt(0, 255)}.{GetInt(0, 255)}.{GetInt(0, 255)}.{GetInt(0, 255)}";

    private const string DEFAULT_RANDOM_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    /// <summary>
    /// Returns a random string of the specified length using <paramref name="allowedChars"/> as the character pool.
    /// Uses a cryptographically secure source to eliminate modulo bias.
    /// </summary>
    /// <param name="minLength">Minimum length of the generated string.</param>
    /// <param name="maxLength">Maximum length of the generated string.</param>
    /// <param name="allowedChars">Pool of characters to pick from. Must contain 256 or fewer distinct characters.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="allowedChars"/> contains more than 256 distinct characters.</exception>
    public static string GetString(int minLength = 5, int maxLength = 20, string allowedChars = DEFAULT_RANDOM_CHARS) {
        int length = minLength != maxLength ? GetInt(minLength, maxLength) : minLength;

        const int byteSize = 0x100;
        var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
        ArgumentOutOfRangeException.ThrowIfGreaterThan(allowedCharSet.Length, byteSize, nameof(allowedChars));

        var result = new StringBuilder();
        var buf = new byte[128];

        while (result.Length < length) {
            RandomNumberGenerator.Fill(buf);
            for (var i = 0; i < buf.Length && result.Length < length; ++i) {
                var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                if (outOfRangeStart <= buf[i])
                    continue;
                result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
            }
        }

        return result.ToString();
    }

    // Some characters are left out because they are hard to tell apart.
    private const string DEFAULT_ALPHA_CHARS = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";

    /// <summary>Returns a random alpha string (no ambiguous characters such as l/1 or O/0).</summary>
    public static string GetAlphaString(int minLength = 5, int maxLength = 20) => GetString(minLength, maxLength, DEFAULT_ALPHA_CHARS);

    // Some characters are left out because they are hard to tell apart.
    private const string DEFAULT_ALPHANUMERIC_CHARS = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    /// <summary>Returns a random alphanumeric string (no ambiguous characters such as l/1 or O/0).</summary>
    public static string GetAlphaNumericString(int minLength = 5, int maxLength = 20) => GetString(minLength, maxLength, DEFAULT_ALPHANUMERIC_CHARS);

    /// <summary>Returns a title-cased phrase of random lorem ipsum words.</summary>
    public static string GetTitleWords(int minWords = 2, int maxWords = 10) => GetWords(minWords, maxWords, titleCaseAllWords: true);

    /// <summary>Returns a single random lorem ipsum word, optionally title-cased.</summary>
    public static string GetWord(bool titleCase = true) {
        var word = _words[GetInt(0, _words.Length - 1)];
        return titleCase ? UpperCaseFirstCharacter(word) : word;
    }

    /// <summary>Returns a space-separated phrase of random lorem ipsum words.</summary>
    /// <param name="minWords">Minimum number of words. Must be 2 or more.</param>
    /// <param name="maxWords">Maximum number of words. Must be 2 or more.</param>
    /// <param name="titleCaseFirstWord">Whether to title-case the first word.</param>
    /// <param name="titleCaseAllWords">Whether to title-case every word.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minWords"/> or <paramref name="maxWords"/> is less than 2.</exception>
    public static string GetWords(int minWords = 2, int maxWords = 10, bool titleCaseFirstWord = true, bool titleCaseAllWords = true) {
        ArgumentOutOfRangeException.ThrowIfLessThan(minWords, 2);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxWords, 2);

        var builder = new StringBuilder();
        int numberOfWords = GetInt(minWords, maxWords);
        for (int i = 1; i < numberOfWords; i++)
            builder.Append(' ').Append(GetWord(titleCaseAllWords || (i == 0 && titleCaseFirstWord)));

        return builder.ToString().Trim();
    }

    /// <summary>Returns a random lorem ipsum sentence ending with a period.</summary>
    /// <param name="minWords">Minimum number of words. Must be 3 or more.</param>
    /// <param name="maxWords">Maximum number of words. Must be 3 or more.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minWords"/> or <paramref name="maxWords"/> is less than 3.</exception>
    public static string GetSentence(int minWords = 5, int maxWords = 25) {
        ArgumentOutOfRangeException.ThrowIfLessThan(minWords, 3);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxWords, 3);

        var builder = new StringBuilder();
        builder.Append(UpperCaseFirstCharacter(_words[GetInt(0, _words.Length - 1)]));
        int numberOfWords = GetInt(minWords, maxWords);
        for (int i = 1; i < numberOfWords; i++)
            builder.Append(' ').Append(_words[GetInt(0, _words.Length - 1)]);

        builder.Append('.');
        return builder.ToString();
    }

    private static string UpperCaseFirstCharacter(string input) {
        if (String.IsNullOrEmpty(input))
            return input;

        Span<char> chars = stackalloc char[input.Length];
        input.AsSpan().CopyTo(chars);
        for (int i = 0; i < chars.Length; ++i) {
            if (chars[i] != ' ' && chars[i] != '\t') {
                chars[i] = Char.ToUpper(chars[i]);
                break;
            }
        }

        return new String(chars);
    }

    /// <summary>Returns one or more paragraphs of random lorem ipsum text.</summary>
    /// <param name="count">Number of paragraphs. Must be 1 or more.</param>
    /// <param name="minSentences">Minimum sentences per paragraph. Must be 1 or more.</param>
    /// <param name="maxSentences">Maximum sentences per paragraph.</param>
    /// <param name="minSentenceWords">Minimum words per sentence.</param>
    /// <param name="maxSentenceWords">Maximum words per sentence.</param>
    /// <param name="html">When <c>true</c>, wraps each paragraph in <c>&lt;p&gt;</c> tags.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> or <paramref name="minSentences"/> is less than 1.</exception>
    public static string GetParagraphs(int count = 3, int minSentences = 3, int maxSentences = 25, int minSentenceWords = 5, int maxSentenceWords = 25, bool html = false) {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(minSentences, 1);

        var builder = new StringBuilder();
        if (html)
            builder.Append("<p>");

        builder.Append("Lorem ipsum dolor sit amet. ");
        int sentenceCount = GetInt(minSentences, maxSentences) - 1;

        for (int i = 0; i < sentenceCount; i++)
            builder.Append(GetSentence(minSentenceWords, maxSentenceWords)).Append(' ');

        if (html)
            builder.Append("</p>");

        for (int i = 1; i < count; i++) {
            if (html)
                builder.Append("<p>");
            for (int x = 0; x < sentenceCount; x++)
                builder.Append(GetSentence(minSentenceWords, maxSentenceWords)).Append(' ');

            if (html)
                builder.Append("</p>");
            else
                builder.Append(Environment.NewLine).Append(Environment.NewLine);
        }

        return builder.ToString();
    }

    private static readonly string[] _words = [
        "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod",
        "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua",
        "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita",
        "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet",
        "lorem", "ipsum", "dolor", "sit", "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod",
        "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua",
        "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita",
        "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet",
        "lorem", "ipsum", "dolor", "sit", "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod",
        "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua",
        "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita",
        "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet", "duis",
        "autem", "vel", "eum", "iriure", "dolor", "in", "hendrerit", "in", "vulputate", "velit", "esse", "molestie",
        "consequat", "vel", "illum", "dolore", "eu", "feugiat", "nulla", "facilisis", "at", "vero", "eros", "et",
        "accumsan", "et", "iusto", "odio", "dignissim", "qui", "blandit", "praesent", "luptatum", "zzril", "delenit",
        "augue", "duis", "dolore", "te", "feugait", "nulla", "facilisi", "lorem", "ipsum", "dolor", "sit", "amet",
        "consectetuer", "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet",
        "dolore", "magna", "aliquam", "erat", "volutpat", "ut", "wisi", "enim", "ad", "minim", "veniam", "quis",
        "nostrud", "exerci", "tation", "ullamcorper", "suscipit", "lobortis", "nisl", "ut", "aliquip", "ex", "ea",
        "commodo", "consequat", "duis", "autem", "vel", "eum", "iriure", "dolor", "in", "hendrerit", "in", "vulputate",
        "velit", "esse", "molestie", "consequat", "vel", "illum", "dolore", "eu", "feugiat", "nulla", "facilisis", "at",
        "vero", "eros", "et", "accumsan", "et", "iusto", "odio", "dignissim", "qui", "blandit", "praesent", "luptatum",
        "zzril", "delenit", "augue", "duis", "dolore", "te", "feugait", "nulla", "facilisi", "nam", "liber", "tempor",
        "cum", "soluta", "nobis", "eleifend", "option", "congue", "nihil", "imperdiet", "doming", "id", "quod", "mazim",
        "placerat", "facer", "possim", "assum", "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer", "adipiscing",
        "elit", "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam",
        "erat", "volutpat", "ut", "wisi", "enim", "ad", "minim", "veniam", "quis", "nostrud", "exerci", "tation",
        "ullamcorper", "suscipit", "lobortis", "nisl", "ut", "aliquip", "ex", "ea", "commodo", "consequat", "duis",
        "autem", "vel", "eum", "iriure", "dolor", "in", "hendrerit", "in", "vulputate", "velit", "esse", "molestie",
        "consequat", "vel", "illum", "dolore", "eu", "feugiat", "nulla", "facilisis", "at", "vero", "eos", "et", "accusam",
        "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita", "kasd", "gubergren", "no", "sea",
        "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet", "lorem", "ipsum", "dolor", "sit",
        "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod", "tempor", "invidunt", "ut",
        "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua", "at", "vero", "eos", "et",
        "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita", "kasd", "gubergren", "no",
        "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet", "lorem", "ipsum", "dolor", "sit",
        "amet", "consetetur", "sadipscing", "elitr", "at", "accusam", "aliquyam", "diam", "diam", "dolore", "dolores",
        "duo", "eirmod", "eos", "erat", "et", "nonumy", "sed", "tempor", "et", "et", "invidunt", "justo", "labore",
        "stet", "clita", "ea", "et", "gubergren", "kasd", "magna", "no", "rebum", "sanctus", "sea", "sed", "takimata",
        "ut", "vero", "voluptua", "est", "lorem", "ipsum", "dolor", "sit", "amet", "lorem", "ipsum", "dolor", "sit",
        "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod", "tempor", "invidunt", "ut",
        "labore", "et", "dolore", "magna", "aliquyam", "erat", "consetetur", "sadipscing", "elitr", "sed", "diam",
        "nonumy", "eirmod", "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed",
        "diam", "voluptua", "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea",
        "rebum", "stet", "clita", "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum"
    ];
}

/// <summary>Extension methods for <see cref="IEnumerable{T}"/> providing random element selection.</summary>
public static class EnumerableExtensions {
    /// <summary>
    /// Returns a random element from <paramref name="items"/>, or <paramref name="defaultValue"/> if the sequence is null or empty.
    /// </summary>
    public static T? Random<T>(this IEnumerable<T>? items, T? defaultValue = default) {
        if (items is null)
            return defaultValue;

        var list = items.ToList();
        if (list.Count == 0)
            return defaultValue;

        return list.ElementAt(RandomData.Instance.Next(list.Count));
    }
}
