using DSAExperimentation.LeetCode.IteratorForCombination;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IteratorForCombination;

// Harness only: both strategies live in IteratorForCombinationSolution. LeetCode's
// shape here is a stateful object driven by next()/hasNext(), so an example states
// the whole sequence the judge would observe and each [Theory] replays it - asserting
// HasNext before every Next and exhaustion at the end. The bitmask enumeration was
// previously only a benchmark's baseline arm and went unasserted; it is under test
// here for the first time.
public sealed partial class IteratorForCombinationTests
{
    public static TheoryData<string, int, string[]> Examples =>
        new()
        {
            { "abc", 2, ["ab", "ac", "bc"] },
            { "wxyz", 4, ["wxyz"] },
            { "abc", 1, ["a", "b", "c"] },
            { "abcd", 3, ["abc", "abd", "acd", "bcd"] },
            { "ab", 2, ["ab"] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByBitmaskEnumeration_LeetCodeExamples_YieldsInLexicographicalOrder(
        string characters, int combinationLength, string[] expected)
    {
        var iterator = IteratorForCombinationSolution.CreateByBitmaskEnumeration(characters, combinationLength);

        AssertYields(expected, iterator);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByBacktrackEngine_LeetCodeExamples_YieldsInLexicographicalOrder(
        string characters, int combinationLength, string[] expected)
    {
        var iterator = IteratorForCombinationSolution.CreateByBacktrackEngine(characters, combinationLength);

        AssertYields(expected, iterator);
    }

    private static void AssertYields(
        string[] expected,
        IteratorForCombinationSolution.CombinationIterator iterator)
    {
        foreach (var combination in expected)
        {
            Assert.True(iterator.HasNext());
            Assert.Equal(combination, iterator.Next());
        }

        Assert.False(iterator.HasNext());
    }
}
