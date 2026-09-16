using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestNumberInInfiniteSetBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a List scan per operation against
// Heap + Set - so a harness whose arms disagree is replaying two different operation scripts.
// Both arms build their own mutable set inside the call from the factory they name, so neither
// arm leaves state behind for the other and one harness instance is safe to read twice in
// either order. Setup builds one fixed operation script from a single seed, so the same Length
// must rebuild the same script; otherwise two published numbers were never comparable.
//
// Each arm answers with the running sum of everything it popped, which is the whole of LeetCode
// 2336's observable output across a script, so the two sums agreeing is the two strategies
// agreeing on every pop in order.
public sealed partial class SmallestNumberInInfiniteSetBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameOperationScript() =>
        Assert.Equal(BuildHarness().ListScanPerOperation(), BuildHarness().ListScanPerOperation());

    [Fact]
    public void ListScanPerOperation_TwoHundredOperationScript_AgreesWithHeapAndSetPerOperation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HeapAndSetPerOperation(), harness.ListScanPerOperation());
    }

    [Fact]
    public void HeapAndSetPerOperation_TwoHundredOperationScript_AgreesWithListScanPerOperation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListScanPerOperation(), harness.HeapAndSetPerOperation());
    }

    private static SmallestNumberInInfiniteSetBenchmarks BuildHarness()
    {
        var harness = new SmallestNumberInInfiniteSetBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
