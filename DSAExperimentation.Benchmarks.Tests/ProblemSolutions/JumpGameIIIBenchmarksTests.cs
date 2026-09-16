using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for JumpGameIIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - can index 0 reach an index holding 0 - so a harness whose arms
// disagree is timing two different problems.
//
// Setup draws every value from [1, Length), so no index holds 0 and LC 1306's only success
// condition is never met: the verdict the arms have to agree on is decisively false, and the
// class comment's "force the real worst case" intent means neither arm may short-circuit on an
// early hit - both must exhaust the whole reachable component before answering.
public sealed partial class JumpGameIIIBenchmarksTests
{
    private const int SmallestLength = 200;
    private const bool ExpectedVerdict = false;

    [Fact]
    public void Setup_SameLength_RebuildsTheZeroFreeArray()
    {
        Assert.Equal(ExpectedVerdict, BuildHarness().CanReachByStackWalk());
        Assert.Equal(ExpectedVerdict, BuildHarness().CanReachByDepthFirstSearch());
    }

    [Fact]
    public void CanReachByStackWalk_ZeroFreeArray_AgreesWithDepthFirstSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanReachByDepthFirstSearch(), harness.CanReachByStackWalk());
    }

    [Fact]
    public void CanReachByDepthFirstSearch_ZeroFreeArray_AgreesWithStackWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanReachByStackWalk(), harness.CanReachByDepthFirstSearch());
    }

    private static JumpGameIIIBenchmarks BuildHarness()
    {
        var harness = new JumpGameIIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
