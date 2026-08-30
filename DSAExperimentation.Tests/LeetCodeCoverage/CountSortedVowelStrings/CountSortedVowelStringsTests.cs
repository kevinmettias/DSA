using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSortedVowelStrings;

// LeetCode 1641. Count Sorted Vowel Strings: Memoizer caches the recurrence
// counting non-decreasing vowel sequences by (remaining length, smallest
// allowed vowel index) - the same "state -> memoized recursive count" shape
// UniquePathsTests already uses for its own counting recurrence. A choice at
// vowel v may only be followed by choices >= v, which is exactly what keeps
// every produced string sorted without ever materializing one.
public sealed partial class CountSortedVowelStringsTests
{
    private const int VowelCount = 5;

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 15)]
    [InlineData(33, 66045)]
    public void CountVowelStrings_LeetCodeExamples_ReturnsExpectedCount(int n, int expected)
        => Assert.Equal(expected, CountVowelStrings(n));

    private static int CountVowelStrings(int n)
    {
        return Memoizer.Memoize<(int Remaining, int Start), int>((n, 0), Count);

        int Count((int Remaining, int Start) state, Func<(int Remaining, int Start), int> count)
        {
            var (remaining, start) = state;

            if (remaining == 0)
            {
                return 1;
            }

            var total = 0;

            for (var vowel = start; vowel < VowelCount; vowel++)
            {
                total += count((remaining - 1, vowel));
            }

            return total;
        }
    }
}
