using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MatchSubstringAfterReplacement;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MatchSubstringAfterReplacementSolution's, the same
// methods MatchSubstringAfterReplacementTests proves correct - linearly scanning
// the raw mappings list for an allowed (old, new) pair (baseline) vs. an O(1)
// two-step lookup through this repo's own HashMap<char, Set<char>> (primitive).
// _sub's characters deliberately have no entry in _mappings, so every single start
// position fails on its very first character comparison after a full scan of every
// mapping - forcing both strategies through their true per-comparison worst case,
// the same "unreachable target" convention TwoSumBenchmarks uses, just against the
// mapping lookup instead of the sum check.
[MemoryDiagnoser]
public class MatchSubstringAfterReplacementBenchmarks
{
    private const int SubLength = 20;
    private const int MappingCount = 200;
    private const int DistinctOldCharacters = 20;

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
            .Select(i => ((char)('c' + i % DistinctOldCharacters), 'a'))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool LinearScanMappings() =>
        MatchSubstringAfterReplacementSolution.IsMatchByLinearScan(_s, _sub, _mappings);

    [Benchmark]
    public bool HashMapSetLookup() =>
        MatchSubstringAfterReplacementSolution.IsMatchByHashMapLookup(_s, _sub, _mappings);
}
