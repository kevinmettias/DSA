using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InsertGreatestCommonDivisorsInLinkedListBenchmarks (ARCHITECTURE 17.9).
// Both arms are competing strategies for the same question - a fresh sequence rebuilt through a
// value buffer against one gcd node spliced between each original pair - so a harness whose arms
// disagree is timing two different problems. Both arms return the resulting list's head as object
// (the node type is internal, so a public [Benchmark] method cannot name it as a return type),
// which is the real answer rather than a proxy, and AnswerText cannot render it: a linked node is
// not an enumerable sequence, so each result is walked into its values first. The expected
// sequence is derived from the fixture, not read back out of an arm: [GlobalSetup] builds the
// values 1..Length in order, and two consecutive integers always have a greatest common divisor
// of one, so every inserted node holds one and the answer is fixed. Each arm clones the shared
// list inside the measured call (the splice rewrites .Next in place), so one harness is safe to
// call twice in either order.
public sealed partial class InsertGreatestCommonDivisorsInLinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    // gcd(n, n + 1) is one for every n, so the fixture's consecutive values fix every inserted
    // gcd without any Euclidean step.
    private const int ConsecutiveGcd = 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(RenderedValues(BuildHarness().ValueRebuild()), RenderedValues(BuildHarness().ValueRebuild()));
        Assert.Equal(RenderedValues(BuildHarness().NodeSplice()), RenderedValues(BuildHarness().NodeSplice()));
    }

    [Fact]
    public void ValueRebuild_InterleavedGcds_AgreesWithNodeSplice()
    {
        var harness = BuildHarness();

        Assert.Equal(RenderedValues(harness.NodeSplice()), RenderedValues(harness.ValueRebuild()));
        Assert.Equal(ExpectedGcdInterleaved(SmallestLength), ValuesOf(harness.ValueRebuild()));
    }

    [Fact]
    public void NodeSplice_InterleavedGcds_AgreesWithValueRebuild()
    {
        var harness = BuildHarness();

        Assert.Equal(RenderedValues(harness.ValueRebuild()), RenderedValues(harness.NodeSplice()));
        Assert.Equal(ExpectedGcdInterleaved(SmallestLength), ValuesOf(harness.NodeSplice()));
    }

    private static InsertGreatestCommonDivisorsInLinkedListBenchmarks BuildHarness()
    {
        var harness = new InsertGreatestCommonDivisorsInLinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    private static string RenderedValues(object? answer) => AnswerText.Of(ValuesOf(answer));

    private static int[] ValuesOf(object? answer)
    {
        var values = new List<int>();

        for (var node = (SinglyLinkedListNode<int>?)answer; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }

    // nums[0], gcd(nums[0], nums[1]), nums[1], ... - every value except the first is preceded by
    // the gcd of its pair, which the fixture's consecutive values make a constant one.
    private static int[] ExpectedGcdInterleaved(int length)
    {
        var values = new List<int> { 1 };

        for (var value = 2; value <= length; value++)
        {
            values.Add(ConsecutiveGcd);
            values.Add(value);
        }

        return [.. values];
    }
}
