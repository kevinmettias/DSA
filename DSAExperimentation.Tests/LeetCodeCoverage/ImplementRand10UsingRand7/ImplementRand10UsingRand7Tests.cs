namespace DSAExperimentation.Tests.LeetCodeCoverage.ImplementRand10UsingRand7;

// LeetCode 470. Implement Rand10() Using Rand7(): the classic 7x7 rejection-sampling
// grid - two Rand7() calls address a uniform 1..49 cell, the 9 cells beyond 40 are
// discarded (uniform draws stay uniform under rejection) and the remaining 1..40 folds
// down to 1..10. No repo DataStructures/Algorithms primitive fits this problem's actual
// challenge (rejection-sampling arithmetic over a black-box RNG, not storage/traversal/
// search), so - matching the "lighter repo-primitive fit" precedent already used for
// LC 190/191/231 - this composes a seeded System.Random the same way every other
// randomness problem in this suite does (ShuffleAnArrayTests.cs, RandomPickIndexTests.cs,
// LinkedListRandomNodeTests.cs), standing in for the black-box Rand7() the real LeetCode
// judge supplies.
public sealed class ImplementRand10UsingRand7Tests
{
    [Fact]
    public void Rand10_ManyCalls_AlwaysStaysInRange()
    {
        var solution = new Solution(seed: 1);

        for (var i = 0; i < 2_000; i++)
        {
            var value = solution.Rand10();
            Assert.InRange(value, 1, 10);
        }
    }

    [Fact]
    public void Rand10_ManyCalls_EventuallyReturnsEveryValueRoughlyUniformly()
    {
        var solution = new Solution(seed: 1);
        var counts = new int[11];

        const int trials = 20_000;
        for (var i = 0; i < trials; i++)
        {
            counts[solution.Rand10()]++;
        }

        for (var value = 1; value <= 10; value++)
        {
            // Expected count per value is trials/10 = 2000; a wide tolerance keeps this
            // deterministic-seed test robust to the rejection sampler's own variance.
            Assert.InRange(counts[value], 1_500, 2_500);
        }
    }

    private sealed class Solution
    {
        private readonly Random _random;

        public Solution(int seed) => _random = new Random(seed);

        public int Rand10()
        {
            int index;
            do
            {
                var row = Rand7();
                var col = Rand7();
                index = (row - 1) * 7 + col;
            } while (index > 40);

            return 1 + (index - 1) % 10;
        }

        private int Rand7() => _random.Next(1, 8);
    }
}
