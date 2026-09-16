using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ImplementStackUsingQueuesBenchmarks (ARCHITECTURE 17.9). Both arms are
// competing strategies for the same question - the rotate-on-push stack backed by the CLR's own
// Queue<int> against this repo's Queue<int> doing the identical rotation - so a harness whose
// arms disagree is timing two different problems. Each arm returns a single long, the sum of
// every value the fixed script returns, compared directly; that sum is a proxy for the whole
// replay rather than one decisive LIFO read, but it totals thousands of pops. Each arm
// constructs its own stack inside the call and the script is a fixed list of closures built in
// [GlobalSetup], so one harness is safe to call twice in either order. The script is seeded, so
// the same Count must rebuild the same push/pop traffic and therefore the same total.
public sealed partial class ImplementStackUsingQueuesBenchmarksTests
{
    private const int SmallestCount = 200;

    [Fact]
    public void Setup_SameCount_RebuildsTheSameScript() =>
        Assert.Equal(BuildHarness().BuiltInQueue(), BuildHarness().BuiltInQueue());

    [Fact]
    public void BuiltInQueue_ReplayTotal_AgreesWithQueuePrimitive()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.QueuePrimitive(), harness.BuiltInQueue());
    }

    [Fact]
    public void QueuePrimitive_ReplayTotal_AgreesWithBuiltInQueue()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BuiltInQueue(), harness.QueuePrimitive());
    }

    private static ImplementStackUsingQueuesBenchmarks BuildHarness()
    {
        var harness = new ImplementStackUsingQueuesBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
