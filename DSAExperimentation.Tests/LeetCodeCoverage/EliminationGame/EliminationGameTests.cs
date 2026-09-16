using DSAExperimentation.LeetCode.EliminationGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EliminationGame;

// Harness only. Both strategies live in EliminationGameSolution. One test method
// per strategy over one shared set of examples - LeetCode's own two published cases
// (1 and 9) plus the larger counts the pre-migration test cross-validated the
// closed-form arithmetic against a list simulation for - so a failure names the
// strategy that broke.
public sealed partial class EliminationGameTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 1, 1 },
            { 2, 2 },
            { 3, 2 },
            { 4, 2 },
            { 9, 6 },
            { 17, 6 },
            { 64, 22 },
            { 100, 54 },
            { 257, 86 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LastRemainingByHeadStepArithmetic_Examples_ReturnsSurvivingNumber(int numberCount, int expected) =>
        Assert.Equal(expected, EliminationGameSolution.LastRemainingByHeadStepArithmetic(numberCount));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LastRemainingByListSimulation_Examples_ReturnsSurvivingNumber(int numberCount, int expected) =>
        Assert.Equal(expected, EliminationGameSolution.LastRemainingByListSimulation(numberCount));
}
