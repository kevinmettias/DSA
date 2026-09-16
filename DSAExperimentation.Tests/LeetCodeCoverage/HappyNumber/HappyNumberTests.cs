using DSAExperimentation.LeetCode.HappyNumber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HappyNumber;

// Harness only: both strategies are HappyNumberSolution's - this file pins them
// to LeetCode's published examples plus a couple of extra cases proving the
// cycle-detection actually terminates instead of looping forever.
public sealed partial class HappyNumberTests
{
    public static TheoryData<HappyNumberExample> Examples =>
        new()
        {
            { new HappyNumberExample(N: 19, Expected: true) }, // LC's example 1
            { new HappyNumberExample(N: 2, Expected: false) }, // LC's example 2
            { new HappyNumberExample(N: 1, Expected: true) }, // already happy, no iteration needed
            { new HappyNumberExample(N: 7, Expected: true) }, // reaches 1 after several steps
            // the canonical 4 -> 16 -> 37 -> 58 -> 89 -> 145 -> 42 -> 20 -> 4 cycle
            { new HappyNumberExample(N: 4, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsHappyByVisitedSet_LeetCodeExamples_ReturnsWhetherDigitSquareSumReachesOne(
        HappyNumberExample example)
    {
        var actual = HappyNumberSolution.IsHappyByVisitedSet(example.N);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsHappyByFloydCycleDetection_LeetCodeExamples_ReturnsWhetherDigitSquareSumReachesOne(
        HappyNumberExample example)
    {
        var actual = HappyNumberSolution.IsHappyByFloydCycleDetection(example.N);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the number to classify, and whether its digit-square sum
    // reaches one. The `bool` is the expected answer rather than a mode, so the row
    // names it instead of leaving a bare `true` in a position to be decoded.
    public readonly record struct HappyNumberExample(int N, bool Expected);
}
