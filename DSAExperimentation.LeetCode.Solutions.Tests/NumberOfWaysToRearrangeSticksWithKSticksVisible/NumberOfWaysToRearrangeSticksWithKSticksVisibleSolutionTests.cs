using DSAExperimentation.LeetCode.Solutions.NumberOfWaysToRearrangeSticksWithKSticksVisible;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfWaysToRearrangeSticksWithKSticksVisible;

// Harness only. Both strategies are
// NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution's - stick counts small
// enough to enumerate are stated once and replayed against each, so a failure names
// the strategy that broke rather than reporting a disagreement between an anonymous
// test helper and an anonymous benchmark arm. The permutation baseline was never
// asserted before this migration. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class NumberOfWaysToRearrangeSticksWithKSticksVisibleSolutionTests
{
    // Cases both arms are checked at. The permutation baseline lays out all n!
    // arrangements, so the set stops at the larger of the two stick counts the
    // benchmark measures.
    public static TheoryData<int, int, int> EnumerableStickCounts =>
        new()
        {
            // Two rows of the unsigned Stirling triangle.
            { 4, 2, 11 },
            { 5, 3, 35 },

            // The two stick counts the benchmark measures, at its visible count.
            { 8, 3, 13_132 },
            { 9, 3, 118_124 },
        };

    [Theory]
    [MemberData(nameof(EnumerableStickCounts))]
    public void RearrangeSticksByPermutationEnumeration_EnumerableStickCounts_CountsArrangementsWithKVisibleSticks(
        int stickCount, int visibleCount, int expected)
    {
        var arrangements = NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution
            .RearrangeSticksByPermutationEnumeration(stickCount, visibleCount);

        Assert.Equal(expected, arrangements);
    }

    [Theory]
    [MemberData(nameof(EnumerableStickCounts))]
    public void RearrangeSticksByMemoizedStirling_EnumerableStickCounts_CountsArrangementsWithKVisibleSticks(
        int stickCount, int visibleCount, int expected)
    {
        var arrangements = NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution
            .RearrangeSticksByMemoizedStirling(stickCount, visibleCount);

        Assert.Equal(expected, arrangements);
    }
}
