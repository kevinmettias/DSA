using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Valid Anagram (LC 242): the O(n^2) brute force (for every character of s, scan t
// for an unmatched occurrence) vs. the O(n) one-pass frequency count using this
// repo's own HashMap<char,int>. t is a rotation of s (same multiset, different
// order) so both strategies are forced through their full comparison instead of an
// early mismatch cutting brute force short.
[MemoryDiagnoser]
public class ValidAnagramBenchmarks
{
    private const int AlphabetSize = 26;

    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;
    private string _t = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var letters = Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray();
        _s = new string(letters);
        _t = new string([.. letters[1..], letters[0]]);
    }

    [Benchmark(Baseline = true)]
    public bool BruteForce()
    {
        if (_s.Length != _t.Length)
        {
            return false;
        }

        var matched = new bool[_t.Length];
        foreach (var c in _s)
        {
            var found = false;
            for (var j = 0; j < _t.Length; j++)
            {
                if (!matched[j] && _t[j] == c)
                {
                    matched[j] = true;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public bool HashMapFrequencyCount()
    {
        if (_s.Length != _t.Length)
        {
            return false;
        }

        var counts = BuildFrequencyCounts(_s);

        return ConsumesAllCounts(counts, _t);
    }

    private static HashMap<char, int> BuildFrequencyCounts(string s)
    {
        var counts = new HashMap<char, int>();

        foreach (var c in s)
        {
            counts.TryGetValue(c, out var count);
            counts.Set(c, count + 1);
        }

        return counts;
    }

    private static bool ConsumesAllCounts(HashMap<char, int> counts, string t)
    {
        foreach (var c in t)
        {
            if (!counts.TryGetValue(c, out var count) || count == 0)
            {
                return false;
            }

            counts.Set(c, count - 1);
        }

        return true;
    }
}
