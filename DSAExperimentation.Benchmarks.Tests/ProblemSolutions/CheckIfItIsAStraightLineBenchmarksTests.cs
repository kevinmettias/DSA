using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfItIsAStraightLineBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a cross product taken against the first two points
// for every point against checking every triple of points - so a harness whose arms disagree is
// timing two different problems. Both arms answer with a bare bool, so agreement between them says
// the two strategies reached the same verdict on the same coordinates.
public sealed partial class CheckIfItIsAStraightLineBenchmarksTests
{
    private const int SmallestLength = 20;

    // Setup lays every point on the diagonal, point i at (i, i), so all Length points are collinear
    // and the answer is yes. That is deliberately the shape where neither arm may exit early - the
    // brute force finds no bad triple to stop on and the anchored scan finds no point off the line -
    // and it is what makes a rebuilt workload's verdict decisive.
    private const bool ExpectedVerdict = true;

    [Fact]
    public void Setup_SameLength_RebuildsTheCollinearDiagonal()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(ExpectedVerdict, first.IsStraightLineByBruteForceEveryTriple());
        Assert.Equal(ExpectedVerdict, second.IsStraightLineByAnchoredCrossProductScan());
    }

    [Fact]
    public void IsStraightLineByBruteForceEveryTriple_DiagonalPoints_AgreesWithAnchoredCrossProductScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsStraightLineByAnchoredCrossProductScan(), harness.IsStraightLineByBruteForceEveryTriple());
    }

    [Fact]
    public void IsStraightLineByAnchoredCrossProductScan_DiagonalPoints_AgreesWithBruteForceEveryTriple()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsStraightLineByBruteForceEveryTriple(), harness.IsStraightLineByAnchoredCrossProductScan());
    }

    private static CheckIfItIsAStraightLineBenchmarks BuildHarness()
    {
        var harness = new CheckIfItIsAStraightLineBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
