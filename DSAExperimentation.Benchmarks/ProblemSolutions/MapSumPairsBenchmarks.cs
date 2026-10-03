using DSAExperimentation.LeetCode.MapSumPairs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MapSumPairsSolution's, the same classes
// MapSumPairsSolutionTests proves correct. [GlobalSetup] builds one fixed key/value/prefix
// workload so key generation is charged to setup rather than to the
// insert-then-sum replay each [Benchmark] arm measures. Each query prefix is a
// real leading substring of one of the inserted keys, guaranteeing at least one
// match instead of letting either strategy bail out early on "no keys share this
// prefix."
public class MapSumPairsBenchmarks
{
    private const int KeyLength = 8;
    private const int PrefixLength = 3;
    private const int RandomSeed = 677; // LC problem number
    private const int MaxValueExclusive = 100;
    private const int AlphabetSize = 26;

    private string[] _keys = [];

    private int[] _values = [];
    private string[] _prefixes = [];

    // What every Sum returned, in query order - what each arm returns.
    private int[] _sums = [];
    [Params(5_000, 20_000)]
    public int KeyCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _keys = Enumerable.Range(0, KeyCount).Select(_ => RandomWord(random)).Distinct().ToArray();
        _values = _keys.Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _prefixes = _keys.Select(key => key[..PrefixLength]).ToArray();
        _sums = new int[_prefixes.Length];
    }

    private static string RandomWord(Random random)
        => new(Enumerable.Range(0, KeyLength).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());

    [Benchmark(Baseline = true)]
    public int[] DictionaryScan() => Replay(new MapSumPairsSolution.MapSumByDictionaryScan());

    [Benchmark]
    public int[] TrieFoldSum() => Replay(new MapSumPairsSolution.MapSumByTrieFold());

    // Inserts every key, then returns every query's Sum in order; Insert returns nothing.
    private int[] Replay(MapSumPairsSolution.IMapSumStrategy mapSum)
    {
        for (var i = 0; i < _keys.Length; i++)
        {
            mapSum.Insert(_keys[i], _values[i]);
        }

        for (var i = 0; i < _prefixes.Length; i++)
        {
            _sums[i] = mapSum.Sum(_prefixes[i]);
        }

        return _sums;
    }
}
