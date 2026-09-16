using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfWaysOfCuttingAPizzaBenchmarks (ARCHITECTURE 17.9): both arms count the
// ways to cut the same all-apple pizza into the fixed piece count - the unmemoized recursion against
// the memoized one over the same state - so a harness whose arms disagree is timing two different
// questions. The count modulo 1e9+7 is the problem's whole answer rather than a proxy. Setup builds the
// AppleGrid the hoisted overload takes; it is a readonly struct, so each arm works on its own copy of
// the field and one harness can be called twice in either order.
public sealed partial class NumberOfWaysOfCuttingAPizzaBenchmarksTests
{
    private const int SmallestSize = 6;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().UnmemoizedRecursion(), BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    private static NumberOfWaysOfCuttingAPizzaBenchmarks BuildHarness()
    {
        var harness = new NumberOfWaysOfCuttingAPizzaBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
