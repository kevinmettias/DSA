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
public sealed class ImplementRand10UsingRand7Tests
{
    private static Func<int> SeededRand7(int seed)
    {
        var random = new Random(seed);
        return () => random.Next(1, 8);
    }

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

        const int trials = 20_000;
        for (var i = 0; i < trials; i++)
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
}
