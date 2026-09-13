using DSAExperimentation.LeetCode.HandOfStraights;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HandOfStraights;

// Harness only. Both greedy strategies are HandOfStraightsSolution's - this file
// just pins them to LeetCode's published examples, including a hand that cannot
// divide evenly into groups at all and a hand whose duplicates have to be spread
// across parallel groups rather than stacked into one.
public sealed class HandOfStraightsTests
{
    public static TheoryData<int[], int, bool> Examples =>
        new()
        {
            { [1, 2, 3, 6, 2, 3, 4, 7, 8], 3, true },
            { [1, 2, 3, 4, 5], 4, false },
            { [1, 2, 3, 4, 5, 6], 2, true },
            { [1, 1, 2, 2, 3, 3], 3, true },
            { [8, 10, 12], 3, false },
            { [1], 1, true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsNStraightHandByBclDictionary_LeetCodeExamples_ReturnsWhetherHandSplitsIntoStraights(
        int[] hand, int groupSize, bool expected) =>
        Assert.Equal(expected, HandOfStraightsSolution.IsNStraightHandByBclDictionary(hand, groupSize));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsNStraightHandByHashMapMergeSort_LeetCodeExamples_ReturnsWhetherHandSplitsIntoStraights(
        int[] hand, int groupSize, bool expected) =>
        Assert.Equal(
            expected, HandOfStraightsSolution.IsNStraightHandByHashMapMergeSort(hand, groupSize));
}
