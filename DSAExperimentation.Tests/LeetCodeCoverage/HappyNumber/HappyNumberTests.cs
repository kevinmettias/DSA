using DSAExperimentation.LeetCode.HappyNumber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HappyNumber;

// Harness only: both strategies are HappyNumberSolution's - this file pins them
// to LeetCode's published examples plus a couple of extra cases proving the
// cycle-detection actually terminates instead of looping forever.
public sealed class HappyNumberTests
{
    public static TheoryData<int, bool> Examples =>
        new()
        {
            { 19, true }, // LC's example 1
            { 2, false }, // LC's example 2
            { 1, true }, // already happy, no iteration needed
            { 7, true }, // reaches 1 after several steps
            { 4, false }, // the canonical 4 -> 16 -> 37 -> 58 -> 89 -> 145 -> 42 -> 20 -> 4 cycle
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsHappyByVisitedSet_LeetCodeExamples_ReturnsWhetherDigitSquareSumReachesOne(
        int n, bool expected) =>
        Assert.Equal(expected, HappyNumberSolution.IsHappyByVisitedSet(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsHappyByFloydCycleDetection_LeetCodeExamples_ReturnsWhetherDigitSquareSumReachesOne(
        int n, bool expected) =>
        Assert.Equal(expected, HappyNumberSolution.IsHappyByFloydCycleDetection(n));
}
