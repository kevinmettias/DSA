using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountIncreasingQuadrupletsBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a rebuilt workload, so this asserts only a bound that comes
// from the workload's shape rather than from either arm. Setup builds a permutation of 1..Length,
// and one quadruplet is one choice of four distinct indices, so no permutation can admit more than
// Length choose 4 of them.
public sealed partial class CountIncreasingQuadrupletsBenchmarksTests
{
    private const int SmallestLength = 8;

    // The divisor in Length choose 4: four indices, taken 4! ways.
    private const int QuadrupletIndexCount = 24;

    // Length choose 4 multiplies four descending factors - Length, Length - 1, Length - 2 and
    // Length - 3 - so these are the decrements that produce its third and fourth.
    private const int ThirdFactorDecrement = 2;
    private const int FourthFactorDecrement = 3;

    // A permutation need not admit a single increasing quadruplet.
    private const long FewestQuadruplets = 0L;

    [Fact]
    public void FenwickTreeSweep_EightElementPermutation_StaysWithinTheIndexBound() =>
        Assert.InRange(
            BuildHarness().FenwickTreeSweep(SmallestLength),
            FewestQuadruplets,
            FourIndexSelections(SmallestLength));

    private static CountIncreasingQuadrupletsBenchmarks BuildHarness()
    {
        var harness = new CountIncreasingQuadrupletsBenchmarks();
        harness.Setup();

        return harness;
    }

    // Length choose 4: i < j < k < l is exactly a choice of four distinct indices.
    private static long FourIndexSelections(int length) =>
        (long)length * (length - 1) * (length - ThirdFactorDecrement) * (length - FourthFactorDecrement)
            / QuadrupletIndexCount;
}
