using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CircularArrayLoopBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a visited-set walk from each start against Floyd's two-pointer
// cycle detection - so a harness whose arms disagree is timing two different problems. Both arms
// answer with a bare bool, so agreement between them says the two strategies reached the same verdict
// on the same step array.
public sealed partial class CircularArrayLoopBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameSignedSteps()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // Each step's magnitude and its sign are both drawn from one seeded stream, and the array
        // itself is private while each arm answers with a single bit, so two harnesses built from the
        // same Length reporting the same verdict for each strategy is the reading the rebuild can be
        // pinned to: one seed means the same steps in the same order, and the same steps mean the same
        // pair of verdicts.
        Assert.Equal(first.HasLoopByHashSetWalk(), second.HasLoopByHashSetWalk());
        Assert.Equal(first.HasLoopByLinkedListFloyd(), second.HasLoopByLinkedListFloyd());
    }

    [Fact]
    public void HasLoopByHashSetWalk_SeededSignedSteps_AgreesWithLinkedListFloyd()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasLoopByLinkedListFloyd(), harness.HasLoopByHashSetWalk());
    }

    [Fact]
    public void HasLoopByLinkedListFloyd_SeededSignedSteps_AgreesWithHashSetWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasLoopByHashSetWalk(), harness.HasLoopByLinkedListFloyd());
    }

    private static CircularArrayLoopBenchmarks BuildHarness()
    {
        var harness = new CircularArrayLoopBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
