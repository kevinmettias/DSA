using DSAExperimentation.Domain.Language;
using DSAExperimentation.Domain.Language.English.NumberNames;
using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.LeetCode.Solutions.IntegerToEnglishWords;

// LC 273's spelling of English's number words - data both strategies read, neither
// carries. The words and the numbers they name are English's, in
// Domain/Language/English/NumberNames' EnglishNumberWords; what is LC 273's own is the notation:
// every word capitalized, single spaces between them, and no hyphen in a compound
// such as "Twenty One". The tables are built in that notation once, from the Domain
// words, and the numbers the strategies split by are read from the words that name
// them. A type of its own because the solution's subject is the pair of un-reversal
// strategies: the tables beside it would be that file's second subject. Named for
// what it holds rather than for the problem, so "where is the word for nineteen" has
// an answer that isn't "in the algorithm".
internal static class IntegerToEnglishWordsVocabulary
{
    internal const string WordSeparator = " ";

    // The words a group contributes when it has none - the empty string rather
    // than null, because every group's words are joined and a missing group must
    // join as nothing at all.
    internal const string EmptyWords = "";

    // A scale word names each power of a thousand, so the number splits into groups
    // of three digits; within a group the hundreds come first; and below twenty each
    // number has a word of its own, so a tens word is composed only from there up.
    internal static readonly int GroupSize = (int)EnglishNumberWords.Thousand.Value;
    internal static readonly int HundredsDivisor = (int)EnglishNumberWords.Hundred.Value;
    internal static readonly int TensThreshold = EnglishNumberWords.OwnWords.Length;

    internal static readonly string ZeroWord = Spelled(EnglishNumberWords.Zero);
    internal static readonly string HundredWord = Spelled(EnglishNumberWords.Hundred);

    // Indexed by number, zero to nineteen.
    internal static readonly string[] Below20 = [.. EnglishNumberWords.OwnWords.Select(Spelled)];

    // Indexed by tens digit, each word placed at the digit its number has, so the
    // first two entries stay empty: those are the positions below the threshold
    // AppendTensAndOnes reads Below20 for instead.
    internal static readonly string[] Tens = TensByDigit();

    // Indexed by group, ones first: the ones group has no scale word, and each group
    // above it the short scale's word for its power of a thousand.
    internal static readonly string[] Scales = [EmptyWords, .. EnglishNumberWords.ShortScale.Select(Spelled)];

    private static string[] TensByDigit()
    {
        var tens = new string[DecimalDigits.Size];
        Array.Fill(tens, EmptyWords);

        foreach (var word in EnglishNumberWords.Tens)
        {
            tens[word.Value / DecimalDigits.Size] = Spelled(word);
        }

        return tens;
    }

    // LC 273 capitalizes every word.
    private static string Spelled(NumberWord word) => char.ToUpperInvariant(word.Word[0]) + word.Word[1..];
}
