namespace DSAExperimentation.LeetCode.IntegerToEnglishWords;

// LC 273's fixed English words and the bases that index them - data both
// strategies read, neither carries. A type of its own because the solution's
// subject is the pair of un-reversal strategies: eleven values beside it would
// be that file's second subject, so somebody tuning a word would have to read an
// implementation to find it and somebody reading the implementation would step
// over the tables first. Named for what it holds rather than for the problem, so
// "where is the word for nineteen" has an answer that isn't "in the algorithm".
internal static class IntegerToEnglishWordsVocabulary
{
    internal const string ZeroWord = "Zero";
    internal const string HundredWord = "Hundred";
    internal const string WordSeparator = " ";

    // The words a group contributes when it has none - the empty string rather
    // than null, because every group's words are joined and a missing group must
    // join as nothing at all.
    internal const string EmptyWords = "";

    internal const int GroupSize = 1000;
    internal const int HundredsDivisor = 100;
    internal const int TensThreshold = 20;
    internal const int DigitBase = 10;

    internal static readonly string[] Below20 =
    [
        "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
        "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
        "Seventeen", "Eighteen", "Nineteen"
    ];

    // Indexed by tens digit, so the first two entries are the positions below the
    // threshold AppendTensAndOnes reads Below20 for instead.
    internal static readonly string[] Tens =
    [
        "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
    ];

    internal static readonly string[] Scales = ["", "Thousand", "Million", "Billion"];
}
