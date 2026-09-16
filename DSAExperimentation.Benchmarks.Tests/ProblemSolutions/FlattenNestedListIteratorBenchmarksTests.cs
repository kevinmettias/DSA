using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FlattenNestedListIteratorBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. The nested list is rebuilt from LeafCount alone in [GlobalSetup], so the same
// LeafCount must rebuild the same workload, and each arm drains its own iterator over that shared
// input rather than consuming it.
//
// One iterator's answer is pinned to the fixture's own structure - [GlobalSetup] lays down leaves
// 0, 1, 2, ... in ascending pairs - which makes the agreement a check on the flattened values and
// their order, not just on both arms returning the same length.
public sealed partial class FlattenNestedListIteratorBenchmarksTests
{
    private const int SmallestLeafCount = 200;

    [Fact]
    public void Setup_SameLeafCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().EagerFlatten()),
            AnswerText.Of(BuildHarness().EagerFlatten()));

    [Fact]
    public void EagerFlatten_AgreesWithLazyStack()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLeaves(), harness.EagerFlatten());
        Assert.Equal(AnswerText.Of(harness.LazyStack()), AnswerText.Of(harness.EagerFlatten()));
    }

    [Fact]
    public void LazyStack_AgreesWithEagerFlatten()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLeaves(), harness.LazyStack());
        Assert.Equal(AnswerText.Of(harness.EagerFlatten()), AnswerText.Of(harness.LazyStack()));
    }

    // [GlobalSetup] fills the list with pairs of consecutive integers, so the flattened sequence is
    // the ascending run 0..SmallestLeafCount - 1.
    private static IEnumerable<int> ExpectedLeaves() => Enumerable.Range(0, SmallestLeafCount);

    private static FlattenNestedListIteratorBenchmarks BuildHarness()
    {
        var harness = new FlattenNestedListIteratorBenchmarks { LeafCount = SmallestLeafCount };
        harness.Setup();

        return harness;
    }
}
