using DSAExperimentation.LeetCode.SortCharactersByFrequency;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortCharactersByFrequency;

// Harness only: both strategies live in SortCharactersByFrequencySolution and are
// asserted against the same examples. LeetCode accepts any arrangement whose
// characters are grouped by non-increasing frequency, so the assertion checks that
// property directly rather than one fixed expected string.
public sealed class SortCharactersByFrequencyTests
{
    public static TheoryData<string> Examples =>
        new() { "tree", "cccaaa", "a" };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FrequencySortByDictionaryOrderBy_LeetCodeExamples_OrdersCharactersByDescendingFrequency(string s) =>
        AssertOrderedByDescendingFrequency(s, SortCharactersByFrequencySolution.FrequencySortByDictionaryOrderBy(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FrequencySortByHashMapHeap_LeetCodeExamples_OrdersCharactersByDescendingFrequency(string s) =>
        AssertOrderedByDescendingFrequency(s, SortCharactersByFrequencySolution.FrequencySortByHashMapHeap(s));

    private static void AssertOrderedByDescendingFrequency(string input, string result)
    {
        Assert.Equal(input.Length, result.Length);
        Assert.Equal(input.OrderBy(c => c), result.OrderBy(c => c));

        var previousRunLength = int.MaxValue;
        var index = 0;

        while (index < result.Length)
        {
            var current = result[index];
            var runLength = 0;

            while (index < result.Length && result[index] == current)
            {
                runLength++;
                index++;
            }

            Assert.True(
                runLength <= previousRunLength,
                $"Run of '{current}' (length {runLength}) follows a shorter run (length {previousRunLength}).");
            previousRunLength = runLength;
        }
    }
}
