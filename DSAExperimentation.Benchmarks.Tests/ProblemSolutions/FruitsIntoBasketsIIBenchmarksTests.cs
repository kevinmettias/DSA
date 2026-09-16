using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FruitsIntoBasketsIIBenchmarks (ARCHITECTURE 17.9): the class has a single
// arm, so there is no second strategy to reconcile it against and the assertion has to come from
// the workload itself. FruitsIntoBasketsIISolution documents the rule the arm applies - each fruit
// type, left to right, is placed into the leftmost still-empty basket whose capacity can hold it,
// and the answer counts the fruit types left with no basket - and [GlobalSetup] documents its draw
// sequence: one Random seeded with Seed, Length fruits drawn out of [1, MaxCapacityExclusive) first,
// then Length baskets. The expected count is therefore re-derived here from that rule applied to a
// fixture rebuilt from that sequence, with the oracle walking a shrinking list of the remaining
// capacities in index order rather than the arm's bool[] used scan, so the test states the rule
// rather than replaying the arm. Setup is a pure function of Length off that seed, so the same
// Length must rebuild the same two arrays.
public sealed partial class FruitsIntoBasketsIIBenchmarksTests
{
    private const int SmallestLength = 20;
    private const int Seed = 3477;
    private const int MaxCapacityExclusive = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SeededFruitsAndBaskets_LeavesTheCountThePlacementRulePredicts()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedUnplacedCount(SmallestLength), harness.BruteForce());
    }

    // The documented rule, applied to a fixture rebuilt from the documented draw sequence: every
    // fruit takes the leftmost basket still free whose capacity reaches it, and each fruit that
    // finds none is counted as unplaced.
    private static int ExpectedUnplacedCount(int length)
    {
        var (fruits, baskets) = RebuildFixture(length);
        var remaining = new List<int>(baskets);
        var unplaced = 0;

        foreach (var quantity in fruits)
        {
            var landing = remaining.FindIndex(capacity => capacity >= quantity);

            if (landing < 0)
            {
                unplaced++;
            }
            else
            {
                remaining.RemoveAt(landing);
            }
        }

        return unplaced;
    }

    // [GlobalSetup]'s own draw sequence, restated: one seeded generator, every fruit drawn before
    // any basket.
    private static (int[] Fruits, int[] Baskets) RebuildFixture(int length)
    {
        var random = new Random(Seed);
        var fruits = Enumerable.Range(0, length).Select(_ => random.Next(1, MaxCapacityExclusive)).ToArray();
        var baskets = Enumerable.Range(0, length).Select(_ => random.Next(1, MaxCapacityExclusive)).ToArray();

        return (fruits, baskets);
    }

    private static FruitsIntoBasketsIIBenchmarks BuildHarness()
    {
        var harness = new FruitsIntoBasketsIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
