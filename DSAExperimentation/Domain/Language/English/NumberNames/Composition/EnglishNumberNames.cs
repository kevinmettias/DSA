using System.Globalization;
using System.Text;
using DSAExperimentation.Domain.Language.English.NumberNames.Ordinals;
using DSAExperimentation.Domain.Language.English.NumberNames.Scales;
using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Domain.Language.English.NumberNames.Composition;

// English's rules for putting number words together, in a scale. A number's digits split into the
// scale's groups; each nonzero group is read as hundreds, then tens and ones, and followed by its
// group word, the highest group first. Below twenty a number has a word of its own, and a tens word
// takes its ones after a hyphen (twenty-one). American usage is followed: no "and" after hundred,
// where British English says "one hundred and one". The group sizes are the scale witness's
// constants and its words its own, so the short, long and Indian scales are three configurations of
// one composer.
//
// The rest are built on that: a negative number is read "minus" and its size; an ordinal takes its
// last word's ordinal (EnglishOrdinalWords); a fraction is its numerator and its denominator's
// parts (EnglishFractionWords), a lone power read without its "one" (three hundredths); a mixed
// number joins a whole number and a fraction with "and", one part read "a" or, before a vowel
// sound, "an"; a decimal reads its digits after the point one by one; a percentage is a decimal
// and "percent". "Twentieth percent" and the like are not English, so no ordinal percentage is
// offered: a rank by percent is statistics' percentile.
/// <summary>
/// English number names composed in a scale: cardinals, ordinals, fractions, decimals.
/// </summary>
/// <typeparam name="TScale">The scale whose groups and words name large numbers.</typeparam>
public static class EnglishNumberNames<TScale>
    where TScale : struct, INumberScale
{
    private const string Article = "a";
    private const string ArticleBeforeVowel = "an";
    private const char Separator = EnglishOperatorWords.WordSeparator;

    // English number words that begin with a vowel sound all begin with e: eight, eleven, eighteen,
    // eighty. One begins with a w sound.
    private const char VowelSoundStart = 'e';

    private static readonly ulong FirstGroupUnit = PowerOfTen(TScale.FirstGroupDigits);
    private static readonly ulong LaterGroupUnit = PowerOfTen(TScale.LaterGroupDigits);

    /// <summary>Writes a number's words into a sink, in reading order.</summary>
    /// <typeparam name="TSink">The sink, a struct that keeps what it gathers.</typeparam>
    /// <param name="magnitude">The number.</param>
    /// <param name="sink">Where the words go.</param>
    public static void Compose<TSink>(ulong magnitude, ref TSink sink)
        where TSink : struct, INumberWordSink
    {
        if (magnitude == 0)
        {
            sink.Append(EnglishNumberWords.Zero);
            return;
        }

        AppendNonzero(magnitude, ref sink);
    }

    /// <summary>A number in words, such as minus twenty-one.</summary>
    /// <param name="value">The number.</param>
    /// <returns>Its words, lowercase, a tens word joined to its ones by a hyphen.</returns>
    public static string Cardinal(long value)
    {
        var text = new StringBuilder();

        if (value < 0)
        {
            text.Append(EnglishOperatorWords.Minus);
        }

        var magnitude = Magnitude(value);
        AppendWords(text, WordsOf(magnitude));
        return text.ToString();
    }

    private static ulong Magnitude(long value)
    {
        // long.MinValue has no long negative, so its size is taken one short and added back.
        if (value < 0)
        {
            return (ulong)-(value + 1) + 1;
        }

        return (ulong)value;
    }

    /// <summary>A number's ordinal in words, such as one hundred twenty-first.</summary>
    /// <param name="value">The number, at least zero.</param>
    /// <returns>Its words with the last word's ordinal.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> is negative.
    /// </exception>
    public static string Ordinal(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        var words = WordsOf((ulong)value);
        var ordinal = EnglishOrdinalWords.OrdinalOf(words[^1]);
        var text = new StringBuilder();
        Spell(text, words, ordinal);
        return text.ToString();
    }

    /// <summary>A fraction in words, such as three quarters or minus two fifths.</summary>
    /// <param name="numerator">The parts counted.</param>
    /// <param name="denominator">The parts a whole is split into, at least two.</param>
    /// <returns>The numerator's words and the denominator's parts, plural unless one.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="denominator"/> is below two.
    /// </exception>
    public static string Fraction(long numerator, long denominator)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(denominator, EnglishNumberWords.Two.Value);
        var ordinal = DenominatorOrdinal(denominator);
        var parts = EnglishFractionWords.PluralOf(denominator, ordinal);

        if (numerator is 1 or -1)
        {
            parts = EnglishFractionWords.SingularOf(denominator, ordinal);
        }

        return string.Join(Separator, Cardinal(numerator), parts);
    }

    /// <summary>
    /// A mixed number in words, such as two and a half or one and three quarters.
    /// </summary>
    /// <param name="whole">The whole number.</param>
    /// <param name="numerator">
    /// The fraction's parts, more than zero and fewer than the denominator.
    /// </param>
    /// <param name="denominator">The parts a whole is split into, at least two.</param>
    /// <returns>
    /// The whole number, "and", then the fraction, one part read with an article.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The fraction is not proper and positive.
    /// </exception>
    public static string MixedNumber(long whole, long numerator, long denominator)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(numerator, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(numerator, denominator);

        var fraction = Fraction(numerator, denominator);

        if (numerator == 1)
        {
            fraction = OnePart(denominator);
        }

        return string.Join(Separator, Cardinal(whole), EnglishOperatorWords.And, fraction);
    }

    // One part of a denominator with its article: a half, an eighth.
    private static string OnePart(long denominator)
    {
        var ordinal = DenominatorOrdinal(denominator);
        var part = EnglishFractionWords.SingularOf(denominator, ordinal);
        return string.Join(Separator, ArticleFor(part), part);
    }

    // "an" before a vowel sound, "a" otherwise.
    private static string ArticleFor(string word)
    {
        var isVowelSound = word[0] == VowelSoundStart;
        return isVowelSound ? ArticleBeforeVowel : Article;
    }

    /// <summary>A decimal number in words, its digits after the point read one by one.</summary>
    /// <param name="value">The number, its integer part within an unsigned long.</param>
    /// <returns>
    /// Its words, such as three point one four, keeping the digits as written, a trailing zero
    /// among them.
    /// </returns>
    /// <exception cref="OverflowException">
    /// The integer part does not fit an unsigned long.
    /// </exception>
    public static string Decimal(decimal value)
    {
        var text = new StringBuilder();

        if (value < 0)
        {
            text.Append(EnglishOperatorWords.Minus);
        }

        var written = Math.Abs(value).ToString(CultureInfo.InvariantCulture);
        AppendDecimalDigits(text, written);
        return text.ToString();
    }

    private static void AppendDecimalDigits(StringBuilder text, string written)
    {
        var decimalPoint = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;
        var point = written.IndexOf(decimalPoint, StringComparison.Ordinal);
        var hasPoint = point >= 0;
        var integerDigits = written;

        if (hasPoint)
        {
            integerDigits = written[..point];
        }

        var integerPart = ulong.Parse(integerDigits, CultureInfo.InvariantCulture);
        AppendWords(text, WordsOf(integerPart));

        if (hasPoint)
        {
            AppendFractionDigits(text, written[(point + decimalPoint.Length)..]);
        }
    }

    private static void AppendFractionDigits(StringBuilder text, string digits)
    {
        text.Append(Separator).Append(EnglishOperatorWords.Point);

        foreach (var digit in digits)
        {
            var digitValue = DecimalDigits.IndexOf(digit);
            text.Append(Separator).Append(EnglishNumberWords.OwnWords[digitValue].Word);
        }
    }

    /// <summary>A percentage in words, such as twelve point five percent.</summary>
    /// <param name="value">The number of parts in a hundred.</param>
    /// <returns>The decimal's words and "percent".</returns>
    /// <exception cref="OverflowException">
    /// The integer part does not fit an unsigned long.
    /// </exception>
    public static string Percentage(decimal value) =>
        string.Join(Separator, Decimal(value), EnglishOperatorWords.Percent);

    private static ulong PowerOfTen(int digits)
    {
        var power = 1UL;

        for (var digit = 0; digit < digits; digit++)
        {
            power *= DecimalDigits.Size;
        }

        return power;
    }

    private static void AppendNonzero<TSink>(ulong magnitude, ref TSink sink)
        where TSink : struct, INumberWordSink
    {
        var above = magnitude / FirstGroupUnit;

        if (above > 0)
        {
            AppendGroups(above, 0, ref sink);
        }

        AppendGroup(magnitude % FirstGroupUnit, ref sink);
    }

    // The groups from the one a group word names upward, the highest first.
    private static void AppendGroups<TSink>(ulong groups, int wordIndex, ref TSink sink)
        where TSink : struct, INumberWordSink
    {
        var words = TScale.GroupWords;

        if (wordIndex == words.Length - 1)
        {
            AppendTopGroup(groups, words[wordIndex], ref sink);
            return;
        }

        var above = groups / LaterGroupUnit;

        if (above > 0)
        {
            AppendGroups(above, wordIndex + 1, ref sink);
        }

        AppendNamedGroup(groups % LaterGroupUnit, words[wordIndex], ref sink);
    }

    // The last group word takes all the digits above it, read again in the scale.
    private static void AppendTopGroup<TSink>(ulong groups, NumberWord word, ref TSink sink)
        where TSink : struct, INumberWordSink
    {
        AppendNonzero(groups, ref sink);
        sink.Append(word);
    }

    // A group and its word, or nothing for a group of zero.
    private static void AppendNamedGroup<TSink>(ulong group, NumberWord word, ref TSink sink)
        where TSink : struct, INumberWordSink
    {
        if (group == 0)
        {
            return;
        }

        AppendGroup(group, ref sink);
        sink.Append(word);
    }

    // One group's value: thousands, if a group is that wide (the long scale's are), then hundreds,
    // tens and ones. Zero adds nothing.
    private static void AppendGroup<TSink>(ulong group, ref TSink sink)
        where TSink : struct, INumberWordSink
    {
        var thousand = (ulong)EnglishNumberWords.Thousand.Value;

        if (group >= thousand)
        {
            AppendGroup(group / thousand, ref sink);
            sink.Append(EnglishNumberWords.Thousand);
        }

        AppendBelowAThousand(group % thousand, ref sink);
    }

    private static void AppendBelowAThousand<TSink>(ulong value, ref TSink sink)
        where TSink : struct, INumberWordSink
    {
        var hundred = (ulong)EnglishNumberWords.Hundred.Value;

        if (value >= hundred)
        {
            sink.Append(EnglishNumberWords.OwnWords[(int)(value / hundred)]);
            sink.Append(EnglishNumberWords.Hundred);
        }

        var belowAHundred = value % hundred;

        if (belowAHundred >= (ulong)EnglishNumberWords.OwnWords.Length)
        {
            var ten = (ulong)EnglishNumberWords.Ten.Value;
            var tensPlace = (belowAHundred - (ulong)EnglishNumberWords.Twenty.Value) / ten;
            sink.Append(EnglishNumberWords.Tens[(int)tensPlace]);
            belowAHundred %= ten;
        }

        if (belowAHundred > 0)
        {
            sink.Append(EnglishNumberWords.OwnWords[(int)belowAHundred]);
        }
    }

    private static List<NumberWord> WordsOf(ulong magnitude)
    {
        var sink = new NumberWordList();
        Compose(magnitude, ref sink);
        return sink.Words;
    }

    private static void AppendWords(StringBuilder text, List<NumberWord> words)
    {
        if (text.Length > 0)
        {
            text.Append(Separator);
        }

        Spell(text, words, words[^1].Word);
    }

    // The words in order, the last spelled as given: itself, or its ordinal.
    private static void Spell(StringBuilder text, List<NumberWord> words, string lastSpelled)
    {
        NumberWord? previous = null;

        for (var at = 0; at < words.Count; at++)
        {
            var word = words[at];
            var isLast = at == words.Count - 1;
            AppendWord(text, previous, word, isLast ? lastSpelled : word.Word);
            previous = word;
        }
    }

    // A word after the one before it: a hyphen between a tens word and its ones, a space otherwise.
    private static void AppendWord(
        StringBuilder text, NumberWord? previous, NumberWord word, string spelled)
    {
        if (previous is { } before)
        {
            var isCompound = IsTensWord(before) && word.Value < EnglishNumberWords.Ten.Value;
            text.Append(isCompound ? EnglishOperatorWords.CompoundJoint : Separator);
        }

        text.Append(spelled);
    }

    private static bool IsTensWord(NumberWord word) =>
        word.Value >= EnglishNumberWords.Twenty.Value
        && word.Value < EnglishNumberWords.Hundred.Value;

    // A denominator's ordinal, without the "one" before a lone hundred or group word: three
    // hundredths, a thousandth, not "three one hundredths".
    private static string DenominatorOrdinal(long denominator)
    {
        var words = WordsOf((ulong)denominator);

        if (TryLonePower(words, out var power))
        {
            return EnglishOrdinalWords.OrdinalOf(power);
        }

        return Ordinal(denominator);
    }

    // "one" and a word of a hundred or more, nothing else: the power is that word.
    private static bool TryLonePower(List<NumberWord> words, out NumberWord power)
    {
        power = default;

        if (words is not [var first, var second] || first != EnglishNumberWords.One)
        {
            return false;
        }

        power = second;
        return second.Value >= EnglishNumberWords.Hundred.Value;
    }

}
