using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NQueensIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies for the same
// question - the array-based recursion that counts placements against the backtracking search - so a harness whose
// arms disagree is timing two different problems. Both arms take the board size alone and hold no state between
// calls, so one harness instance is safe to call twice in either order, and both answer with LeetCode's own shape:
// the number of distinct solutions.
public sealed partial class NQueensIIBenchmarksTests
{
    private const int EightQueens = 8;

    [Fact]
    public void ArrayRecursion_EightQueens_AgreesWithBacktrackSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BacktrackSearch(), harness.ArrayRecursion());
    }

    [Fact]
    public void BacktrackSearch_EightQueens_AgreesWithArrayRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayRecursion(), harness.BacktrackSearch());
    }

    private static NQueensIIBenchmarks BuildHarness() => new() { Size = EightQueens };
}
