using DSAExperimentation.LeetCode.ImplementRand10UsingRand7;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ImplementRand10UsingRand7;

// Harness only: both strategies live in ImplementRand10UsingRand7Solution. A seeded
// System.Random stands in for the black-box Rand7() the real LeetCode judge
// supplies (this suite's established randomness-problem convention - see
// ShuffleAnArrayTests, RandomPickIndexTests, LinkedListRandomNodeTests).
//
// Rand10ByRejectionSampling is the only strategy asserted for uniformity, because
// it is the only one that is supposed to be uniform. Rand10ByNaiveModuloFold is
// asserted only to stay in range 1..10 - it is a deliberately biased "fast but
// wrong" contrast (see the solution's own doc comment), so a uniformity assertion
// against it would be asserting a property the strategy does not have.
public sealed partial class ImplementRand10UsingRand7Tests
{
    // The sample size the uniformity test draws. A constant rather than a local: its
    // scope is a claim about where the value is authoritative, and the tolerance band
    // the test asserts against is chosen from this magnitude, not from the loop.
    private const int Trials = 20_000;

    private static IRand7 SeededRand7(int seed) => new SeededRandomRand7(new Random(seed));

    [Fact]
    public void Rand10ByRejectionSampling_ManyCalls_AlwaysStaysInRange()
    {
        var rand7 = SeededRand7(seed: 1);

        for (var i = 0; i < 2_000; i++)
        {
            var value = ImplementRand10UsingRand7Solution.Rand10ByRejectionSampling(rand7);
            Assert.InRange(value, 1, 10);
        }
    }

    [Fact]
    public void Rand10ByRejectionSampling_ManyCalls_EventuallyReturnsEveryValueRoughlyUniformly()
    {
        var rand7 = SeededRand7(seed: 1);
        var counts = new int[11];

        for (var i = 0; i < Trials; i++)
        {
            counts[ImplementRand10UsingRand7Solution.Rand10ByRejectionSampling(rand7)]++;
        }

        for (var value = 1; value <= 10; value++)
        {
            // Expected count per value is trials/10 = 2000; a wide tolerance keeps
            // this deterministic-seed test robust to the rejection sampler's own
            // variance.
            Assert.InRange(counts[value], 1_500, 2_500);
        }
    }

    [Fact]
    public void Rand10ByNaiveModuloFold_ManyCalls_AlwaysStaysInRange()
    {
        var rand7 = SeededRand7(seed: 1);

        for (var i = 0; i < 2_000; i++)
        {
            var value = ImplementRand10UsingRand7Solution.Rand10ByNaiveModuloFold(rand7);
            Assert.InRange(value, 1, 10);
        }
    }

    // The stand-in for LeetCode's black-box Rand7(): a seeded System.Random drawn
    // from per call. One type serves every test here, so the seed stays the only
    // thing a test picks.
    private sealed class SeededRandomRand7(Random random) : IRand7
    {
        public int Draw() => random.Next(1, 8);
    }
}
