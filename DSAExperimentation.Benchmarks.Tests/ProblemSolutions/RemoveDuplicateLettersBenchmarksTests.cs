using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveDuplicateLettersBenchmarks (ARCHITECTURE 17.9): both arms are
// RemoveDuplicateLettersSolution's, competing strategies for the same question - recursive splitting
// on the rarest letter against one Stack-and-Set sweep - so a harness whose arms disagree returns two
// different subsequences. Setup draws the letters from one seeded Random over a fixed alphabet, so
// the same Length must rebuild the same string.
public sealed partial class RemoveDuplicateLettersBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().RecursiveSplit(), BuildHarness().RecursiveSplit());

    [Fact]
    public void RecursiveSplit_AgreesWithStackAndSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StackAndSet(), harness.RecursiveSplit());
    }

    [Fact]
    public void StackAndSet_AgreesWithRecursiveSplit()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecursiveSplit(), harness.StackAndSet());
    }

    private static RemoveDuplicateLettersBenchmarks BuildHarness()
    {
        var harness = new RemoveDuplicateLettersBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
