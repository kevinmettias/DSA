using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountWaysToChooseCoprimeIntegersFromRowsBenchmarks (ARCHITECTURE 17.9): the
// generic arm check compares the arms only with each other, so what stays here is a bound derived
// without either arm. The matrix is private and the way count is the only thing either arm reports,
// so the documented shape is asserted through that: a way is one value chosen from each of the Size
// rows, and a Size x Size matrix offers only Size^Size such selections in total.
public sealed partial class CountWaysToChooseCoprimeIntegersFromRowsBenchmarksTests
{
    private const int SmallestSize = 5;

    private const int SelectionCount = 3_125; // SmallestSize ^ SmallestSize

    [Fact]
    public void BruteForce_SmallestSquareMatrix_CountsNoMoreWaysThanSelections() =>
        Assert.InRange(BuildHarness().BruteForce(SmallestSize), 0, SelectionCount);

    private static CountWaysToChooseCoprimeIntegersFromRowsBenchmarks BuildHarness()
    {
        var harness = new CountWaysToChooseCoprimeIntegersFromRowsBenchmarks();
        harness.Setup();

        return harness;
    }
}
