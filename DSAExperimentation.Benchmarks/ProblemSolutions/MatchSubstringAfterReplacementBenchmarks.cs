using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Match Substring After Replacement (LC 2301): checking whether an allowed
// (old, new) replacement pair exists for a mismatched character pair, done by
// linearly scanning the raw mappings list (baseline) vs. an O(1) two-step lookup
// through this repo's own HashMap<char, Set<char>> (primitive). _sub's characters
// deliberately have no entry in _mappings, so every single start position fails on
// its very first character comparison after a full scan of every mapping - forcing
// both strategies through their true per-comparison worst case, the same
// "unreachable target" convention TwoSumBenchmarks uses, just against the mapping
// lookup instead of the sum check.
[MemoryDiagnoser]
public class MatchSubstringAfterReplacementBenchmarks
{
    private const int SubLength = 20;
    private const int MappingCount = 200;

    [Params(500, 5_000)]
    public int SLength;

    private string _s = null!;
    private string _sub = null!;
    private (char Old, char New)[] _mappings = null!;

    [GlobalSetup]
    public void Setup()
    {
        _s = new string('a', SLength);
        _sub = new string('b', SubLength);
        _mappings = Enumerable.Range(0, MappingCount)
            .Select(i => ((char)('c' + i % 20), 'a'))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool LinearScanMappings()
    {
        for (var start = 0; start + _sub.Length <= _s.Length; start++)
        {
            if (MatchesAtLinearScan(start))
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool HashMapSetLookup()
    {
        var allowed = BuildAllowedMap();

        for (var start = 0; start + _sub.Length <= _s.Length; start++)
        {
            if (MatchesAtHashMap(start, allowed))
            {
                return true;
            }
        }

        return false;
    }

    private bool MatchesAtLinearScan(int start)
    {
        for (var j = 0; j < _sub.Length; j++)
        {
            var subChar = _sub[j];
            var sChar = _s[start + j];

            if (subChar == sChar)
            {
                continue;
            }

            var mapped = false;

            foreach (var (oldChar, newChar) in _mappings)
            {
                if (oldChar == subChar && newChar == sChar)
                {
                    mapped = true;
                    break;
                }
            }

            if (!mapped)
            {
                return false;
            }
        }

        return true;
    }

    private HashMap<char, Set<char>> BuildAllowedMap()
    {
        var allowed = new HashMap<char, Set<char>>();

        foreach (var (oldChar, newChar) in _mappings)
        {
            if (!allowed.TryGetValue(oldChar, out var targets))
            {
                targets = new Set<char>();
                allowed.Set(oldChar, targets);
            }

            targets.TryAdd(newChar);
        }

        return allowed;
    }

    private bool MatchesAtHashMap(int start, HashMap<char, Set<char>> allowed)
    {
        for (var j = 0; j < _sub.Length; j++)
        {
            var subChar = _sub[j];
            var sChar = _s[start + j];

            if (subChar == sChar)
            {
                continue;
            }

            if (!allowed.TryGetValue(subChar, out var targets) || !targets.Has(sChar))
            {
                return false;
            }
        }

        return true;
    }
}
