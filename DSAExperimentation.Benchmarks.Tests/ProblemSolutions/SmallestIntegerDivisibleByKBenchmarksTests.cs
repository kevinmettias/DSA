using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestIntegerDivisibleByKBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a modular walk that appends one more "1"
// digit per step against a shortest-path search over the prepared remainder graph - so a
// harness whose arms disagree is walking two different remainders. Divisor is the only input
// either arm takes, and both tuned values are coprime to ten, so no arm may short-circuit on
// the problem's "no such number" answer.
//
// Divisor is also the whole of the oracle: the smallest number made only of ones that is
// divisible by it is the multiplicative order of ten modulo Divisor times nine, which at the
// smallest tuning is thirty-three ones, so the arms are checked against that length and not
// only against each other.
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
    public void Setup_SameDivisor_RebuildsTheSameRemainderGraph() =>
        Assert.Equal(BuildHarness().ModularWalk(), BuildHarness().ModularWalk());

    [Fact]
    public void ModularWalk_TwoHundredOneDivisor_AgreesWithReduceGraphBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraphBfs(), harness.ModularWalk());
        Assert.Equal(ExpectedRepunitLength, harness.ModularWalk());
    }

    [Fact]
    public void ReduceGraphBfs_TwoHundredOneDivisor_AgreesWithModularWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ModularWalk(), harness.ReduceGraphBfs());
        Assert.Equal(ExpectedRepunitLength, harness.ReduceGraphBfs());
    }

    private static SmallestIntegerDivisibleByKBenchmarks BuildHarness()
    {
        var harness = new SmallestIntegerDivisibleByKBenchmarks { Divisor = SmallestDivisor };
        harness.Setup();

        return harness;
    }
}
