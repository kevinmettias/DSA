using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfSquarefulArraysBenchmarks (ARCHITECTURE 17.9): both arms count the same
// squareful permutations of the array - generating every distinct permutation and filtering at the
// leaves against Backtrack.Search with the perfect-square adjacency folded into candidate enumeration
// - so a harness whose arms disagree is timing two different questions. The count is the problem's
// whole answer rather than a proxy. Setup draws and sorts the values from a fixed seed, so the same
// Length must rebuild the same array.
public sealed partial class NumberOfSquarefulArraysBenchmarksTests
{
    private const int SmallestLength = 8;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().GenerateThenFilter(), BuildHarness().GenerateThenFilter());

    [Fact]
    public void GenerateThenFilter_AgreesWithPrunedBacktrack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrunedBacktrack(), harness.GenerateThenFilter());
    }

    [Fact]
    public void PrunedBacktrack_AgreesWithGenerateThenFilter()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GenerateThenFilter(), harness.PrunedBacktrack());
    }

    private static NumberOfSquarefulArraysBenchmarks BuildHarness()
    {
        var harness = new NumberOfSquarefulArraysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
