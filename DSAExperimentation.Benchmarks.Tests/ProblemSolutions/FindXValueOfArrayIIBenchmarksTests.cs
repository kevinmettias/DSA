using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindXValueOfArrayIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different
// problems. Setup draws both the array and the query list from a fixed seed, so the same Length must
// rebuild the same workload.
//
// Both arms answer the queries in order and return one count per query, so the outer order is pinned
// by the query list and AnswerText.Of's order-sensitive rendering is the right comparison.
public sealed partial class FindXValueOfArrayIIBenchmarksTests
{
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AgreesWithSegmentTreeAutomaton()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SegmentTreeAutomaton()),
            AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void SegmentTreeAutomaton_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForce()),
            AnswerText.Of(harness.SegmentTreeAutomaton()));
    }

    private static FindXValueOfArrayIIBenchmarks BuildHarness()
    {
        var harness = new FindXValueOfArrayIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
