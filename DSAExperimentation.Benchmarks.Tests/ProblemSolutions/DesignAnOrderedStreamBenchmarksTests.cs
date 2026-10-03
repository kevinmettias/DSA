using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAnOrderedStreamBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin:
// what the stream hands out across the whole script, known from Setup's construction rather than from either arm.
// Setup builds an arrival order covering every id 1..Size and gives id i the value "v{i}", and each arm returns the
// chunk every Insert handed back, in arrival order.
public sealed partial class DesignAnOrderedStreamBenchmarksTests
{
    private const int SmallestSize = 200;

    [Fact]
    public void ListBacked_ShuffledArrivalOrder_HandsOutEveryValueOnceInIdOrder() =>
        AssertHandsOutEveryValueOnceInIdOrder(BuildHarness().ListBacked());

    [Fact]
    public void DynamicArrayBacked_ShuffledArrivalOrder_HandsOutEveryValueOnceInIdOrder() =>
        AssertHandsOutEveryValueOnceInIdOrder(BuildHarness().DynamicArrayBacked());

    // The cursor only ever advances, so each slot is handed out at most once over the whole script,
    // and in id order: the chunks laid end to end are every value exactly once, which is what a script
    // covering every id 1..Size produces once the last gap closes, and what a script that dropped or
    // mistyped an id - leaving a slot permanently null and the cursor stuck short of the end - could not.
    private static void AssertHandsOutEveryValueOnceInIdOrder(List<string>[] chunks) =>
        Assert.Equal(
            Enumerable.Range(1, SmallestSize).Select(id => $"v{id}"),
            chunks.SelectMany(chunk => chunk));

    private static DesignAnOrderedStreamBenchmarks BuildHarness()
    {
        var harness = new DesignAnOrderedStreamBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
