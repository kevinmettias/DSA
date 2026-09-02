using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Guess Number Higher or Lower (LC 374): an O(n) linear guess() scan vs. this repo's
// own BinarySearch.Find over a virtual sequence of the candidates 1..n, routed through
// guess() via a custom IComparer (FirstBadVersionBenchmarks' shape) - O(log n) probes
// instead of walking every candidate below Pick. Pick sits at 70% of NumberCount so the
// linear scan pays close to its full O(n) worst case every call.
[MemoryDiagnoser]
public class GuessNumberHigherOrLowerBenchmarks
{
    private const double PickFraction = 0.7;

    [Params(1_000, 1_000_000)]
    public int NumberCount;

    private int _pick;

    [GlobalSetup]
    public void Setup() => _pick = (int)(NumberCount * PickFraction);

    [Benchmark(Baseline = true)]
    public int LinearScan()
    {
        for (var candidate = 1; candidate <= NumberCount; candidate++)
        {
            if (Guess(candidate) == 0)
            {
                return candidate;
            }
        }

        return -1;
    }

    [Benchmark]
    public int BinarySearchFind()
    {
        var sequence = new NumberLineSequence(NumberCount);
        var comparer = new GuessComparer(Guess);

        return BinarySearch.Find<int, NumberLineSequence>(sequence, target: 0, comparer)!.Value + 1;
    }

    private int Guess(int num) => _pick.CompareTo(num);

    private readonly struct NumberLineSequence(int length) : IRandomAccessSequence<int>
    {
        public int Length => length;

        public int Get(int index) => index + 1;
    }

    private sealed class GuessComparer(Func<int, int> guess) : IComparer<int>
    {
        public int Compare(int candidate, int target) => -guess(candidate);
    }
}
