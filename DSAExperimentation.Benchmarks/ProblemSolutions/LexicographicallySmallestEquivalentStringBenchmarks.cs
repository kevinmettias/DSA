using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LexicographicallySmallestEquivalentString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LexicographicallySmallestEquivalentStringSolution's, the
// same methods LexicographicallySmallestEquivalentStringTests proves correct. The
// comparison is adjacency-list BFS over the 26 letters (a component list allocated per
// group) against this repo's own DisjointSet(26) - O(1) Union per pair, O(a(26)) Find
// per baseStr character, no per-component allocation. The three strings are LeetCode's
// own input shape, so [GlobalSetup] only sizes and seeds them.
[MemoryDiagnoser]
public class LexicographicallySmallestEquivalentStringBenchmarks
{
    private const int RandomSeed = 1061; // LC problem number
    private const int AlphabetSize = 26;

    [Params(200, 5_000)]
    public int Length;

    private string _s1 = null!;
    private string _s2 = null!;
    private string _baseStr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var s1 = new char[Length];
        var s2 = new char[Length];
        var baseChars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            s1[i] = (char)('a' + random.Next(AlphabetSize));
            s2[i] = (char)('a' + random.Next(AlphabetSize));
            baseChars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        _s1 = new string(s1);
        _s2 = new string(s2);
        _baseStr = new string(baseChars);
    }

    [Benchmark(Baseline = true)]
    public string AdjacencyListBfs() =>
        LexicographicallySmallestEquivalentStringSolution.SmallestEquivalentStringByAdjacencyListBfs(_s1, _s2, _baseStr);

    [Benchmark]
    public string DisjointSetUnionFind() =>
        LexicographicallySmallestEquivalentStringSolution.SmallestEquivalentStringByDisjointSet(_s1, _s2, _baseStr);
}
