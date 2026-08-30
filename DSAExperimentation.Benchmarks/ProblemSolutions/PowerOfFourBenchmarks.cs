using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Power of Four (LC 342): the textbook repeated-division loop (up to 15 divides
// worst case) vs. this repo's own BinarySearch.Find over a 16-element virtual
// sequence of every power of four an int can hold (up to 4 probes worst case) -
// the same monotone-virtual-sequence idiom SqrtXBenchmarks already exercises for
// LeetCode 69, applied here to a membership check instead of a floor-root search.
[MemoryDiagnoser]
public class PowerOfFourBenchmarks
{
    [Params(1_000, 50_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(3);

        // Mostly non-powers-of-four, with the occasional real one mixed in - the
        // same "realistic, not artificially easy" spirit TwoSumBenchmarks' target
        // choice documents.
        _values = Enumerable.Range(0, Length)
            .Select(_ => random.Next(0, 2) == 0 ? 1 << (2 * random.Next(0, 16)) : random.Next(1, int.MaxValue))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RepeatedDivisionLoop()
    {
        var count = 0;

        foreach (var value in _values)
        {
            if (IsPowerOfFourByDivision(value))
            {
                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public int BinarySearchOverPowersOfFour()
    {
        var count = 0;
        var sequence = new PowersOfFourSequence();

        foreach (var value in _values)
        {
            if (value > 0 && BinarySearch.Find<int, PowersOfFourSequence>(sequence, value) is not null)
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsPowerOfFourByDivision(int n)
    {
        if (n <= 0)
        {
            return false;
        }

        while (n % 4 == 0)
        {
            n /= 4;
        }

        return n == 1;
    }

    private readonly struct PowersOfFourSequence : IRandomAccessSequence<int>
    {
        public int Length => 16;

        public int Get(int index) => 1 << (2 * index);
    }
}
