using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfItIsAStraightLineBenchmarks (ARCHITECTURE 17.9): the generic check
// holds the arms to each other, but both answer with a bare bool, so agreement says only that the
// two strategies reached the same verdict on the same coordinates. This pins the baseline's verdict
// to the one Setup's construction decides without either arm.
public sealed partial class CheckIfItIsAStraightLineBenchmarksTests
{
    private const int SmallestLength = 20;

    // Setup lays every point on the diagonal, point i at (i, i), so all Length points are collinear
    // and the answer is yes. That is deliberately the shape where neither arm may exit early - the
    // brute force finds no bad triple to stop on and the anchored scan finds no point off the line -
    // and it is what makes the workload's verdict decisive.
    private const bool ExpectedVerdict = true;

    [Fact]
    public void IsStraightLineByBruteForceEveryTriple_DiagonalPoints_ReturnsTrue() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().IsStraightLineByBruteForceEveryTriple());

    private static CheckIfItIsAStraightLineBenchmarks BuildHarness()
    {
        var harness = new CheckIfItIsAStraightLineBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
