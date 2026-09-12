using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.MapSumPairs.MapSumPairsSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MapSumPairsSolution's, the same classes
// MapSumPairsTests proves correct. [GlobalSetup] builds one fixed key/value/prefix
// workload so key generation is charged to setup rather than to the
// insert-then-sum replay each [Benchmark] arm measures. Each query prefix is a
// real leading substring of one of the inserted keys, guaranteeing at least one
// match instead of letting either strategy bail out early on "no keys share this
// prefix."
[MemoryDiagnoser]
public class MapSumPairsBenchmarks
{
    private const int KeyLength = 8;
    private const int PrefixLength = 3;
    private const int RandomSeed = 677; // LC problem number
    private const int MaxValueExclusive = 100;
    private const int AlphabetSize = 26;

    [Params(5_000, 20_000)]
    public int KeyCount;

    private string[] _keys = null!;
    private int[] _values = null!;
    private string[] _prefixes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _keys = Enumerable.Range(0, KeyCount).Select(_ => RandomWord(random)).Distinct().ToArray();
        _values = _keys.Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _prefixes = _keys.Select(key => key[..PrefixLength]).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long DictionaryScan() => Replay(new MapSumByDictionaryScan());

    [Benchmark]
    public long TrieFoldSum() => Replay(new MapSumByTrieFold());

    // Counts the total of every query rather than discarding each Sum result, so
    // the JIT can't eliminate the replay as dead code - the same "return the real
    // answer, not a weaker proxy" shape DesignSpreadsheetBenchmarks follows.
    private long Replay(IMapSumStrategy mapSum)
    {
        for (var i = 0; i < _keys.Length; i++)
        {
            mapSum.Insert(_keys[i], _values[i]);
        }

        var total = 0L;

        foreach (var prefix in _prefixes)
        {
            total += mapSum.Sum(prefix);
        }

        return total;
    }

    private static string RandomWord(Random random)
        => new(Enumerable.Range(0, KeyLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
}
