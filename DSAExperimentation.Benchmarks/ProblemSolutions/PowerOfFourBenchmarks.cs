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
    // Arbitrary fixed seed for reproducible benchmark input.
    private const int RandomSeed = 3;

    // Coin-flip range: half the values are real powers of four, half are not.
    private const int CoinFlipRange = 2;

    // 4^k == 2^(2k) - the exponent doubling step from a power-of-four index to its
    // equivalent binary left-shift amount.
    private const int PowerOfFourExponentStep = 2;

    // Number of powers of four representable in a non-negative int (4^0..4^15).
    private const int PowersOfFourCount = 16;

    // The base whose powers are being tested for.
    private const int PowerOfFourBase = 4;

    [Params(1_000, 50_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        // Mostly non-powers-of-four, with the occasional real one mixed in - the
        // same "realistic, not artificially easy" spirit TwoSumBenchmarks' target
        // choice documents.
        _values = Enumerable.Range(0, Length)
            .Select(_ => random.Next(0, CoinFlipRange) == 0
                ? 1 << (PowerOfFourExponentStep * random.Next(0, PowersOfFourCount))
                : random.Next(1, int.MaxValue))
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

        while (n % PowerOfFourBase == 0)
        {
            n /= PowerOfFourBase;
        }

        return n == 1;
    }

    private readonly struct PowersOfFourSequence : IRandomAccessSequence<int>
    {
        public int Length => PowersOfFourCount;

        public int Get(int index) => 1 << (PowerOfFourExponentStep * index);
    }
}
