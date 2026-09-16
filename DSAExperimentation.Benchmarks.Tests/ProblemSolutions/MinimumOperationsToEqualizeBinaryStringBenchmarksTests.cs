using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumOperationsToEqualizeBinaryStringBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the fewest flips of exactly flipCount indices
// that turn the string all-ones - so a harness whose arms disagree is timing two different problems.
// ReduceGraphBfs is handed the EqualizeStateGraph [GlobalSetup] already built, and is told the zero
// count [GlobalSetup] counted off the string; so the comparison also pins that the hoisted graph and
// the hoisted zero count describe the same string the queue arm is given. Setup draws that string from
// one fixed seed, so the same Length must rebuild the same string.
public sealed partial class MinimumOperationsToEqualizeBinaryStringBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().MutationQueueBfs(), BuildHarness().MutationQueueBfs());

    [Fact]
    public void MutationQueueBfs_SameBinaryString_AgreesWithReduceGraphBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraphBfs(), harness.MutationQueueBfs());
    }

    [Fact]
    public void ReduceGraphBfs_SameBinaryString_AgreesWithMutationQueueBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MutationQueueBfs(), harness.ReduceGraphBfs());
    }

    private static MinimumOperationsToEqualizeBinaryStringBenchmarks BuildHarness()
    {
        var harness = new MinimumOperationsToEqualizeBinaryStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
