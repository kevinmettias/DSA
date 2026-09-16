using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NextGreaterNodeInLinkedListBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the per-node forward rescan against one
// monotonic-decreasing sweep over this repo's own Stack<int> of pending indices - so a harness whose
// arms disagree is answering two different linked-list questions. Setup charges the list's
// construction to [GlobalSetup] and draws the node values from the seeded permutation, so the same
// Length must rebuild the same list.
//
// Both arms only WALK the list (each materializes the node values into its own array), so the shared
// head node is read-only and one harness serves both arms in either order.
public sealed partial class NextGreaterNodeInLinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceScan()),
            AnswerText.Of(BuildHarness().BruteForceScan()));

    [Fact]
    public void BruteForceScan_AgreesWithMonotonicStackSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MonotonicStackSweep()),
            AnswerText.Of(harness.BruteForceScan()));
    }

    [Fact]
    public void MonotonicStackSweep_AgreesWithBruteForceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForceScan()),
            AnswerText.Of(harness.MonotonicStackSweep()));
    }

    private static NextGreaterNodeInLinkedListBenchmarks BuildHarness()
    {
        var harness = new NextGreaterNodeInLinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
