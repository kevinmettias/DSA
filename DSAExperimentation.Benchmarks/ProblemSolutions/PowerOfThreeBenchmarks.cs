using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Power of Three (LC 326): the textbook O(log3 n) repeated-division loop vs. this
// repo's BinarySearch.Find over an ArraySequence<int> of the 20 powers of three that
// fit in a 32-bit int (3^0..3^19) - reframing "is n a power of three" as a bounded
// lookup instead of a division loop.
[MemoryDiagnoser]
public class PowerOfThreeBenchmarks
{
    [Params(1162261467, 1162261466)] // 3^19 (a true power of three) vs. one less (not)
    public int Value;

    private int[] _powersOfThree = null!;

    [GlobalSetup]
    public void Setup() => _powersOfThree = BuildPowersOfThree();

    [Benchmark(Baseline = true)]
    public bool DivisionLoop()
    {
        var remaining = Value;

        if (remaining < 1)
        {
            return false;
        }

        while (remaining % 3 == 0)
        {
            remaining /= 3;
        }

        return remaining == 1;
    }

    [Benchmark]
    public bool BinarySearchOverPowers()
    {
        var sequence = new ArraySequence<int>(_powersOfThree);

        return BinarySearch.Find<int, ArraySequence<int>>(sequence, Value) is not null;
    }

    private static int[] BuildPowersOfThree()
    {
        var powers = new List<int>();
        long power = 1;

        while (power <= int.MaxValue)
        {
            powers.Add((int)power);
            power *= 3;
        }

        return [.. powers];
    }
}
