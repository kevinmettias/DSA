using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumRepeatingSubstring;

// LeetCode 1668. Maximum Repeating Substring: grows a candidate string
// word+word+... one repeat at a time and asks PrefixFunctionSearch (KMP) whether it
// still occurs in sequence - the same growing-candidate-plus-KMP shape
// RepeatedStringMatchTests already uses, just counting up from k=0 until the first
// repeat that fails instead of searching for the first repeat that succeeds.
public sealed partial class MaximumRepeatingSubstringTests
{
    [Theory]
    [InlineData("ababc", "ab", 2)]
    [InlineData("ababc", "ba", 1)]
    [InlineData("ababc", "ac", 0)]
    [InlineData("aaabaaaabaaabaaaabaaaabaaaabaaaaba", "aaaba", 5)]
    public void MaxRepeating_LeetCodeExamples_ReturnsMaximumRepeatCount(string sequence, string word, int expected)
    {
        var actual = MaxRepeating(sequence, word);
        Assert.Equal(expected, actual);
    }

    private static int MaxRepeating(string sequence, string word)
    {
        var repeats = 0;
        var candidate = "";

        while (true)
        {
            var next = candidate + word;
            if (next.Length > sequence.Length || PrefixFunctionSearch.FindAll(sequence, next).Count == 0)
            {
                return repeats;
            }

            candidate = next;
            repeats++;
        }
    }
}
