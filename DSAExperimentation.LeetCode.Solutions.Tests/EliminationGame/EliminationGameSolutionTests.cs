using DSAExperimentation.LeetCode.Solutions.EliminationGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.EliminationGame;

// Harness only. Both strategies live in EliminationGameSolution. One test method
// per strategy over one shared set of cases - the larger counts the pre-migration
// test cross-validated the closed-form arithmetic against a list simulation for - so
// a failure names the strategy that broke. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class EliminationGameSolutionTests
{
    public static TheoryData<int, int> Cases =>
        new()
        {
            { 17, 6 },
            { 64, 22 },
            { 257, 86 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LastRemainingByHeadStepArithmetic_Cases_ReturnsSurvivingNumber(int numberCount, int expected) =>
        Assert.Equal(expected, EliminationGameSolution.LastRemainingByHeadStepArithmetic(numberCount));

    [Theory]
    [MemberData(nameof(Cases))]
    public void LastRemainingByListSimulation_Cases_ReturnsSurvivingNumber(int numberCount, int expected) =>
        Assert.Equal(expected, EliminationGameSolution.LastRemainingByListSimulation(numberCount));
}
