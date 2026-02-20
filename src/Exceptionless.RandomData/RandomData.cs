using System.Security.Cryptography;
using System.Text;

namespace Exceptionless;

public static class RandomData {
    public static Random Instance => Random.Shared;

    public static int GetInt(int min, int max) {
        if (min == max)
            return min;

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(min, max);

        return Random.Shared.Next(min, max + 1);
    }

    public static int GetInt() => GetInt(Int32.MinValue, Int32.MaxValue);

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

    public static long GetLong(long min, long max) {
        if (min == max)
            return min;

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(min, max);

        var buf = new byte[8];
        Random.Shared.NextBytes(buf);
        long longRand = BitConverter.ToInt64(buf, 0);

        return (Math.Abs(longRand % (max - min)) + min);
    }

    public static long GetLong() => GetLong(Int64.MinValue, Int64.MaxValue);

    public static string GetCoordinate() => $"{GetDouble(-90.0, 90.0)},{GetDouble(-180.0, 180.0)}";

    public static DateTime GetDateTime(DateTime? start = null, DateTime? end = null) {
        if (start.HasValue && end.HasValue && start.Value == end.Value)
            return start.Value;

        if (start.HasValue && end.HasValue)
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(start.Value, end.Value, nameof(start));

        start ??= DateTime.MinValue;
        end ??= DateTime.MaxValue;

        TimeSpan timeSpan = end.Value - start.Value;
        var newSpan = new TimeSpan(GetLong(0, timeSpan.Ticks));

        return start.Value + newSpan;
    }

    public static DateTimeOffset GetDateTimeOffset(DateTimeOffset? start = null, DateTimeOffset? end = null) {
        if (start.HasValue && end.HasValue)
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(start.Value, end.Value, nameof(start));

        start ??= DateTimeOffset.MinValue;
        end ??= DateTimeOffset.MaxValue;

        TimeSpan timeSpan = end.Value - start.Value;
        var newSpan = new TimeSpan(GetLong(0, timeSpan.Ticks));

        return start.Value + newSpan;
    }

    public static TimeSpan GetTimeSpan(TimeSpan? min = null, TimeSpan? max = null) {
        if (min.HasValue && max.HasValue && min.Value == max.Value)
            return min.Value;

        if (min.HasValue && max.HasValue)
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(min.Value, max.Value, nameof(min));

        min ??= TimeSpan.Zero;
        max ??= TimeSpan.MaxValue;

        return min.Value + new TimeSpan((long)(new TimeSpan(max.Value.Ticks - min.Value.Ticks).Ticks * Random.Shared.NextDouble()));
    }

    public static bool GetBool(int chance = 50) {
        chance = Math.Clamp(chance, 0, 100);
        double c = 1 - (chance / 100.0);
        return Random.Shared.NextDouble() > c;
    }

    public static double GetDouble(double? min = null, double? max = null) {
        if (min.HasValue && max.HasValue && min.Value == max.Value)
            return min.Value;

        if (min.HasValue && max.HasValue)
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(min.Value, max.Value, nameof(min));

        min ??= Double.MinValue;
        max ??= Double.MaxValue;

        return Random.Shared.NextDouble() * (max.Value - min.Value) + min.Value;
    }

    public static decimal GetDecimal() => GetDecimal(GetInt(), GetInt());

    public static decimal GetDecimal(int min, int max) => (decimal)GetDouble(min, max);

    public static T GetEnum<T>() where T : struct, Enum {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(GetInt(0, values.Length - 1))!;
    }

    public static string GetIp4Address() => $"{GetInt(0, 255)}.{GetInt(0, 255)}.{GetInt(0, 255)}.{GetInt(0, 255)}";

    private const string DEFAULT_RANDOM_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
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
    public static string GetAlphaString(int minLength = 5, int maxLength = 20) => GetString(minLength, maxLength, DEFAULT_ALPHA_CHARS);

    // Some characters are left out because they are hard to tell apart.
    private const string DEFAULT_ALPHANUMERIC_CHARS = "abcdefghjkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    public static string GetAlphaNumericString(int minLength = 5, int maxLength = 20) => GetString(minLength, maxLength, DEFAULT_ALPHANUMERIC_CHARS);

    public static string GetTitleWords(int minWords = 2, int maxWords = 10) => GetWords(minWords, maxWords, titleCaseAllWords: true);

    public static string GetWord(bool titleCase = true) {
        var word = _words[GetInt(0, _words.Length - 1)];
        return titleCase ? UpperCaseFirstCharacter(word) : word;
    }

    public static string GetWords(int minWords = 2, int maxWords = 10, bool titleCaseFirstWord = true, bool titleCaseAllWords = true) {
        ArgumentOutOfRangeException.ThrowIfLessThan(minWords, 2);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxWords, 2);

        var builder = new StringBuilder();
        int numberOfWords = GetInt(minWords, maxWords);
        for (int i = 1; i < numberOfWords; i++)
            builder.Append(' ').Append(GetWord(titleCaseAllWords || (i == 0 && titleCaseFirstWord)));

        return builder.ToString().Trim();
    }

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

public static class EnumerableExtensions {
    public static T? Random<T>(this IEnumerable<T>? items, T? defaultValue = default) {
        if (items is null)
            return defaultValue;

        var list = items.ToList();
        if (list.Count == 0)
            return defaultValue;

        return list.ElementAt(RandomData.Instance.Next(list.Count));
    }
}
