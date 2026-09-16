using DSAExperimentation.LeetCode.HandOfStraights;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HandOfStraights;

// Harness only. Both greedy strategies are HandOfStraightsSolution's - this file
// just pins them to LeetCode's published examples, including a hand that cannot
// divide evenly into groups at all and a hand whose duplicates have to be spread
// across parallel groups rather than stacked into one.
public sealed class HandOfStraightsTests
{
    public static TheoryData<StraightHandExample> Examples =>
        new()
        {
            { new StraightHandExample(Hand: [1, 2, 3, 6, 2, 3, 4, 7, 8], GroupSize: 3, Expected: true) },
            { new StraightHandExample(Hand: [1, 2, 3, 4, 5], GroupSize: 4, Expected: false) },
            { new StraightHandExample(Hand: [1, 2, 3, 4, 5, 6], GroupSize: 2, Expected: true) },
            { new StraightHandExample(Hand: [1, 1, 2, 2, 3, 3], GroupSize: 3, Expected: true) },
            { new StraightHandExample(Hand: [8, 10, 12], GroupSize: 3, Expected: false) },
            { new StraightHandExample(Hand: [1], GroupSize: 1, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsNStraightHandByBclDictionary_LeetCodeExamples_ReturnsWhetherHandSplitsIntoStraights(
        StraightHandExample example)
    {
        var actual = HandOfStraightsSolution.IsNStraightHandByBclDictionary(example.Hand, example.GroupSize);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsNStraightHandByHashMapMergeSort_LeetCodeExamples_ReturnsWhetherHandSplitsIntoStraights(
        StraightHandExample example)
    {
        var actual = HandOfStraightsSolution.IsNStraightHandByHashMapMergeSort(example.Hand, example.GroupSize);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the hand, the size each straight must have, and whether the
    // hand splits into them. The `bool` is the expected answer rather than a mode, so
    // the row names it instead of leaving a bare `true` in a position to be decoded.
    public readonly record struct StraightHandExample(int[] Hand, int GroupSize, bool Expected);
}
