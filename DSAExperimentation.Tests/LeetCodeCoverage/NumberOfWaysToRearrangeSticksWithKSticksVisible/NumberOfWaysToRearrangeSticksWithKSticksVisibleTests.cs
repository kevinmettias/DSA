using DSAExperimentation.LeetCode.NumberOfWaysToRearrangeSticksWithKSticksVisible;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToRearrangeSticksWithKSticksVisible;

// Harness only. Both strategies are
// NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution's - LeetCode's published
// examples are stated once and replayed against each, so a failure names the
// strategy that broke rather than reporting a disagreement between an anonymous
// test helper and an anonymous benchmark arm. The permutation baseline was never
// asserted before this migration.
public sealed class NumberOfWaysToRearrangeSticksWithKSticksVisibleTests
{
    // Cases both arms are checked at. The permutation baseline lays out all n!
    // arrangements, so the shared set stops at the larger of the two stick counts
    // the benchmark measures and the big modular case below is the recurrence's
    // alone.
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            // LC example 1.
            { 3, 2, 3 },

            // LC example 2: only the increasing arrangement shows every stick.
            { 5, 5, 1 },

            // One stick is always its own single visible stick.
            { 1, 1, 1 },

            // Two more rows of the unsigned Stirling triangle.
            { 4, 2, 11 },
            { 5, 3, 35 },

            // The two stick counts the benchmark measures, at its visible count.
            { 8, 3, 13_132 },
            { 9, 3, 118_124 },
        };

    // LC example 3. Its answer is the count reduced mod 1e9+7, and 20! arrangements
    // is far past what the permutation baseline can enumerate in test time, so only
    // the recurrence is asserted against it.
    public static TheoryData<int, int, int> LargeExamples =>
        new()
        {
            { 20, 11, 647_427_950 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RearrangeSticksByPermutationEnumeration_LeetCodeExamples_CountsArrangementsWithKVisibleSticks(
        int stickCount, int visibleCount, int expected) =>
        Assert.Equal(
            expected,
            NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution
                .RearrangeSticksByPermutationEnumeration(stickCount, visibleCount));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RearrangeSticksByMemoizedStirling_LeetCodeExamples_CountsArrangementsWithKVisibleSticks(
        int stickCount, int visibleCount, int expected) =>
        Assert.Equal(
            expected,
            NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution
                .RearrangeSticksByMemoizedStirling(stickCount, visibleCount));

    [Theory]
    [MemberData(nameof(LargeExamples))]
    public void RearrangeSticksByMemoizedStirling_TwentySticks_ReducesTheCountModuloOneBillionSeven(
        int stickCount, int visibleCount, int expected) =>
        Assert.Equal(
            expected,
            NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution
                .RearrangeSticksByMemoizedStirling(stickCount, visibleCount));
}
