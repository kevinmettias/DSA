using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNumberOfBalancedPermutationsBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a fresh harness, so this asserts what it
// cannot: a ceiling on the answer that comes from the workload's shape rather than from either arm,
// and that a harness which has already answered once answers as a rebuild does, since
// BenchmarkDotNet calls an arm many times on one harness while the generic check builds a fresh
// harness for every call. Setup seeds num from a fixed random draw over a small digit alphabet, so
// the same Length must rebuild the same num.
public sealed partial class CountNumberOfBalancedPermutationsBenchmarksTests
{
    private const int SmallestLength = 6;

    // Distinct permutations of a six-digit num, the ceiling on how many of them can be balanced.
    private const long MostSixDigitPermutations = 720;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: num holds SmallestLength digits, so the answer can never exceed
        // the SmallestLength! distinct permutations they can be arranged into.
        Assert.InRange(first.BruteForce(SmallestLength), 0L, MostSixDigitPermutations);
        Assert.Equal(first.BruteForce(SmallestLength), second.BruteForce(SmallestLength));
    }

    private static CountNumberOfBalancedPermutationsBenchmarks BuildHarness()
    {
        var harness = new CountNumberOfBalancedPermutationsBenchmarks();
        harness.Setup();

        return harness;
    }
}
