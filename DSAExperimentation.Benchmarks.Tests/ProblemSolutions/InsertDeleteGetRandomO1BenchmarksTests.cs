using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InsertDeleteGetRandomO1Benchmarks (ARCHITECTURE 17.9). Both arms are
// competing strategies for the same question - a BCL List scanned on every insert and shifted on
// every removal against a hashmap plus swap-remove - so they are asserted to agree. Each arm
// replays the same fixed script against an instance it builds inside the call (a Design problem's
// whole shape is a sequence of mutating calls against one instance), so one harness is safe to
// call twice in either order. Each arm returns the set's surviving Count, which is a proxy rather
// than the set itself: the script inserts every value and then removes every value, so the honest
// claim is not merely that the two arms agree but that both drained the set completely - a
// harness that skipped its removals would report a large, agreed, wrong count. Both halves are
// asserted.
public sealed partial class InsertDeleteGetRandomO1BenchmarksTests
{
    private const int SmallestCount = 200;

    // The removal order is a permutation of the insertion order, so every inserted value is
    // removed again and nothing is left behind.
    private const int ExpectedSurvivingCount = 0;

    [Fact]
    public void Setup_SameCount_RebuildsTheSameScript()
    {
        Assert.Equal(BuildHarness().ListScan(), BuildHarness().ListScan());
        Assert.Equal(BuildHarness().HashMapSwapRemove(), BuildHarness().HashMapSwapRemove());
    }

    [Fact]
    public void ListScan_SurvivingCount_AgreesWithHashMapSwapRemove()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapSwapRemove(), harness.ListScan());
        Assert.Equal(ExpectedSurvivingCount, harness.ListScan());
    }

    [Fact]
    public void HashMapSwapRemove_SurvivingCount_AgreesWithListScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListScan(), harness.HashMapSwapRemove());
        Assert.Equal(ExpectedSurvivingCount, harness.HashMapSwapRemove());
    }

    private static InsertDeleteGetRandomO1Benchmarks BuildHarness()
    {
        var harness = new InsertDeleteGetRandomO1Benchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
