using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IncrementalMemoryLeakBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on the smallest capacity, so this asserts only the accounting
// the problem statement fixes independently of either arm. Both arms answer with LeetCode 1860's
// own triple [crashSecond, memory1, memory2], and every second before the crash gives away exactly
// that many megabytes, so the two capacities plus the leftovers must still add up to the total
// allocated.
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

    // Seconds 1..crashSecond - 1 give away 1 + 2 + ... + (crashSecond - 1) megabytes, a triangular
    // number, which is that product over this.
    private const int TriangularNumberDivisor = 2;

    [Fact]
    public void Arithmetic_EqualSticks_AccountsForTheWholeCapacity() =>
        Assert.Equal(TotalCapacity(), AllocatedPlusRemaining(BuildHarness().Arithmetic()));

    private static IncrementalMemoryLeakBenchmarks BuildHarness() =>
        new() { Capacity = SmallestCapacity };

    private static long TotalCapacity() => (long)SmallestCapacity * StickCount;

    // Seconds 1..crashSecond - 1 each gave away their own number of megabytes, and the two
    // reported remainders are everything not yet given away.
    private static long AllocatedPlusRemaining(int[] answer)
    {
        long crashSecond = answer[CrashSecondIndex];

        return (crashSecond * (crashSecond - 1) / TriangularNumberDivisor) + answer[FirstStickIndex] + answer[SecondStickIndex];
    }
}
