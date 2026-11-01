using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountKReducibleNumbersLessThanNBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a fresh harness, so this asserts what it
// cannot: a range for the answer that comes from the workload's shape rather than from either arm,
// and that a harness which has already answered once answers as a rebuild does, since
// BenchmarkDotNet calls an arm many times on one harness while the generic check builds a fresh
// harness for every call. Setup seeds a fixed random binary string, so the same Length must rebuild
// the same n.
public sealed partial class CountKReducibleNumbersLessThanNBenchmarksTests
{
    private const int SmallestLength = 16;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // Setup always sets the leading bit, so n has exactly SmallestLength bits and the count
        // ranges over [1, n): the answer can never leave [1, 2^SmallestLength - 1], because x = 1
        // needs no popcount steps at all and so is always k-reducible.
        Assert.InRange(first.BruteForce(), 1, (1 << SmallestLength) - 1);
        Assert.Equal(first.BruteForce(), second.BruteForce());
    }

    private static CountKReducibleNumbersLessThanNBenchmarks BuildHarness()
    {
        var harness = new CountKReducibleNumbersLessThanNBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
