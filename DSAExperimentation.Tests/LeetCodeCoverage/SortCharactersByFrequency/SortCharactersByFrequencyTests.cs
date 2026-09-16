using DSAExperimentation.LeetCode.SortCharactersByFrequency;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortCharactersByFrequency;

// Harness only: both strategies live in SortCharactersByFrequencySolution and are
// asserted against the same examples. LeetCode accepts any arrangement whose
// characters are grouped by non-increasing frequency, so the assertion checks that
// property directly rather than one fixed expected string.
public sealed partial class SortCharactersByFrequencyTests
{
    public static TheoryData<string> Examples =>
        new() { "tree", "cccaaa", "a" };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FrequencySortByDictionaryOrderBy_LeetCodeExamples_OrdersCharactersByDescendingFrequency(string text)
    {
        var result = new FrequencySortResult(
            text, SortCharactersByFrequencySolution.FrequencySortByDictionaryOrderBy(text));

        AssertOrderedByDescendingFrequency(result);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FrequencySortByHashMapHeap_LeetCodeExamples_OrdersCharactersByDescendingFrequency(string text)
    {
        var result = new FrequencySortResult(
            text, SortCharactersByFrequencySolution.FrequencySortByHashMapHeap(text));

        AssertOrderedByDescendingFrequency(result);
    }

    private static void AssertOrderedByDescendingFrequency(FrequencySortResult result)
    {
        AssertSameCharacters(result);
        AssertRunsDescendInLength(result.Sorted);
    }

    // The arrangement is built from the input's own characters, and nothing else:
    // same length, same multiset.
    private static void AssertSameCharacters(FrequencySortResult result)
    {
        Assert.Equal(result.Input.Length, result.Sorted.Length);
        Assert.Equal(result.Input.OrderBy(c => c), result.Sorted.OrderBy(c => c));
    }

    // The grouping property, read off the result alone: each run of one repeated
    // character is no longer than the run before it.
    private static void AssertRunsDescendInLength(string sorted)
    {
        var previousRunLength = int.MaxValue;
        var index = 0;

        while (index < sorted.Length)
        {
            var current = sorted[index];
            var runLength = 0;

            while (index < sorted.Length && sorted[index] == current)
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

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one checked case,
    // naming the input text and the arrangement the strategy produced. Passing the
    // two strings separately would let a caller transpose them silently.
    public readonly record struct FrequencySortResult(string Input, string Sorted);
}
