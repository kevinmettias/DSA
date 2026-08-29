using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Common Prefix (LC 14): linear shrink-and-compare vs. BinarySearch over
// the monotone predicate "prefix length n is shared by every string".
[MemoryDiagnoser]
public class LongestCommonPrefixBenchmarks
{
    private string[] _values = null!;

    [Params(64, 512)]
    public int PrefixLength;

    [GlobalSetup]
    public void Setup()
    {
        var prefix = new string('x', PrefixLength);
        _values =
        [
            prefix + "a",
            prefix + "b",
            prefix + "c",
            prefix + "d",
        ];
    }

    [Benchmark(Baseline = true)]
    public string LinearScan()
    {
        var prefix = _values[0];

        foreach (var value in _values.Skip(1))
        {
            while (!value.StartsWith(prefix, StringComparison.Ordinal))
            {
                prefix = prefix[..^1];
            }
        }

        return prefix;
    }

    [Benchmark]
    public string BinarySearchPredicate()
    {
        var shortest = _values.Min(value => value.Length);
        var sequence = new PrefixFeasibilitySequence(_values, shortest);
        var firstFailingLength = BinarySearch.LowerBound<int, PrefixFeasibilitySequence>(sequence, 1);

        return _values[0][..(firstFailingLength - 1)];
    }

    private readonly struct PrefixFeasibilitySequence(string[] values, int maxLength) : IRandomAccessSequence<int>
    {
        public int Length => maxLength + 1;

        public int Get(int length) => AllSharePrefix(length) ? 0 : 1;

        private bool AllSharePrefix(int length)
        {
            for (var i = 1; i < values.Length; i++)
            {
                if (!values[0].AsSpan(0, length).SequenceEqual(values[i].AsSpan(0, length)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
