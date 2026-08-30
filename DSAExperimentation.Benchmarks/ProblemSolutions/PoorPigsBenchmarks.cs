using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Poor Pigs (LC 458): a brute-force scan that recomputes basis^pigs from scratch by
// repeated multiplication for every candidate pig count (O(pigs^2) total) vs. this
// repo's own BinarySearch.LowerBound over an ArraySequence<long> of precomputed powers
// (O(pigs) to build the sequence once, O(log pigs) to search it). Basis is fixed at 2
// (minutesToDie == minutesToTest) so Buckets alone drives how many pigs are needed.
[MemoryDiagnoser]
public class PoorPigsBenchmarks
{
    private const long Basis = 2;

    [Params(100, 1_000)]
    public int Buckets;

    [Benchmark(Baseline = true)]
    public int LinearRecompute()
    {
        var pigs = 0;

        while (Power(Basis, pigs) < Buckets)
        {
            pigs++;
        }

        return pigs;
    }

    private static long Power(long basis, int exponent)
    {
        var result = 1L;

        for (var i = 0; i < exponent; i++)
        {
            result *= basis;
        }

        return result;
    }

    [Benchmark]
    public int BinarySearchOverPowers()
    {
        var powers = new List<long> { 1L };

        while (powers[^1] < Buckets)
        {
            powers.Add(powers[^1] * Basis);
        }

        var sequence = new ArraySequence<long>(powers.ToArray());

        return BinarySearch.LowerBound<long, ArraySequence<long>>(sequence, Buckets);
    }
}
