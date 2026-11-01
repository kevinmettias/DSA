using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestIntegerDivisibleByKBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the answer the workload is built to have. Divisor is the only
// input either arm takes, and both tuned values are coprime to ten, so no arm may short-circuit on
// the problem's "no such number" answer.
//
// Divisor is also the whole of the oracle: the smallest number made only of ones that is
// divisible by it is the multiplicative order of ten modulo Divisor times nine, which at the
// smallest tuning is thirty-three ones, so the baseline is checked against that length.
public sealed partial class SmallestIntegerDivisibleByKBenchmarksTests
{
    private const int SmallestDivisor = 201;

    // The length of the shortest run of ones divisible by SmallestDivisor, computed
    // independently as the multiplicative order of ten modulo SmallestDivisor * 9 - the last
    // step being that a repunit of length n is (ten to the n minus one) ninths. Two hundred and
    // one is odd and not a multiple of five, so four digits of "1" would already answer it if
    // divisibility by three were the only constraint; thirty-three is the real order, which is
    // why this is worth pinning rather than leaving to the two arms to agree on blindly.
    private const int ExpectedRepunitLength = 33;

    [Fact]
    public void ModularWalk_TwoHundredOneDivisor_ReturnsThirtyThreeOnes() =>
        Assert.Equal(ExpectedRepunitLength, BuildHarness().ModularWalk());

    private static SmallestIntegerDivisibleByKBenchmarks BuildHarness()
    {
        var harness = new SmallestIntegerDivisibleByKBenchmarks { Divisor = SmallestDivisor };
        harness.Setup();

        return harness;
    }
}
