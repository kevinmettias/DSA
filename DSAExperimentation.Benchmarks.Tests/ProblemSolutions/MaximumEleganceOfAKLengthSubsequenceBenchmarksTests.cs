using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumEleganceOfAKLengthSubsequenceBenchmarks (ARCHITECTURE 17.9): both arms
// are competing strategies for one question - the best elegance over every length-k subsequence - so
// a harness whose arms disagree is timing two different problems. Setup draws the items from a fixed
// seed and derives the subsequence length from the item count, so the same item count must rebuild
// the same workload; neither arm mutates it.
public sealed partial class MaximumEleganceOfAKLengthSubsequenceBenchmarksTests
{
    private const int SmallestItemCount = 500;

    [Fact]
    public void Setup_SameItemCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().Bcl(), BuildHarness().Bcl());

    [Fact]
    public void Bcl_AgreesWithRepoPrimitives()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Bcl(), harness.RepoPrimitives());
    }

    [Fact]
    public void RepoPrimitives_AgreesWithBcl()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepoPrimitives(), harness.Bcl());
    }

    private static MaximumEleganceOfAKLengthSubsequenceBenchmarks BuildHarness()
    {
        var harness = new MaximumEleganceOfAKLengthSubsequenceBenchmarks { ItemCount = SmallestItemCount };
        harness.Setup();

        return harness;
    }
}
