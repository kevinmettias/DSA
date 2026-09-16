using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IncrementalMemoryLeakBenchmarks (ARCHITECTURE 17.9). Both arms are
// competing strategies for the same question - two ints and an if/else against routing the same
// round-by-round allocation through this repo's own max-heap - so a harness whose arms disagree
// is timing two different problems. Both arms answer with LeetCode 1860's own triple
// [crashSecond, memory1, memory2], whose positions the problem pins, so AnswerText.Of is the
// right rendering. This class has no [GlobalSetup] and nothing to seed: both sticks start from
// the [Params] capacity, so each [Fact] constructs the harness with the smallest capacity and
// calls both arms. Each arm's own arithmetic is then checked against the accounting the problem
// statement fixes - every second before the crash gives away exactly that many megabytes, so the
// two capacities plus the leftovers must still add up to the total allocated - which is a claim
// derived from the problem rather than read back out of an arm.
public sealed partial class IncrementalMemoryLeakBenchmarksTests
{
    private const int SmallestCapacity = 1_000_000;

    // The two sticks always start equal, so the whole requested capacity is either given away or
    // still standing at the crash second.
    private const int StickCount = 2;

    // LeetCode 1860's answer shape, restated from the solution's own documented order.
    private const int CrashSecondIndex = 0;
    private const int FirstStickIndex = 1;
    private const int SecondStickIndex = 2;

    [Fact]
    public void Arithmetic_EqualSticks_AgreesWithHeapSimulation()
    {
        var harness = BuildHarness();
        var answer = harness.Arithmetic();

        Assert.Equal(AnswerText.Of(harness.HeapSimulation()), AnswerText.Of(answer));
        Assert.Equal(TotalCapacity(), AllocatedPlusRemaining(answer));
    }

    [Fact]
    public void HeapSimulation_EqualSticks_AgreesWithArithmetic()
    {
        var harness = BuildHarness();
        var answer = harness.HeapSimulation();

        Assert.Equal(AnswerText.Of(harness.Arithmetic()), AnswerText.Of(answer));
        Assert.Equal(TotalCapacity(), AllocatedPlusRemaining(answer));
    }

    private static IncrementalMemoryLeakBenchmarks BuildHarness() =>
        new() { Capacity = SmallestCapacity };

    private static long TotalCapacity() => (long)SmallestCapacity * StickCount;

    // Seconds 1..crashSecond - 1 each gave away their own number of megabytes, and the two
    // reported remainders are everything not yet given away.
    private static long AllocatedPlusRemaining(int[] answer)
    {
        long crashSecond = answer[CrashSecondIndex];

        return (crashSecond * (crashSecond - 1) / 2) + answer[FirstStickIndex] + answer[SecondStickIndex];
    }
}
