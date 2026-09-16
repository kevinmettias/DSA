using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SmallestStringWithSwaps;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SmallestStringWithSwapsSolution's, the same methods
// SmallestStringWithSwapsTests proves correct. Adjacency-list BFS (build a graph over
// the string's own indices from every swap pair, then BFS from each unvisited index to
// find its component) vs. this repo's own DisjointSet(n) - O(1) Union per pair and
// O(a(n)) Find per index, with no per-component allocation for the walk itself. Same
// LexicographicallySmallestEquivalentStringBenchmarks shape, over index positions
// instead of the 26 letters.
//
// LeetCode's own input shape - a string and a jagged pair array - is already what both
// strategies take, so [GlobalSetup] only decides how large the workload is and hands
// the finished input straight over; there is no construction left for a hoisted
// overload to lift out of the measured methods.
[MemoryDiagnoser]
public class SmallestStringWithSwapsBenchmarks
{
    // LC problem number, used as the RNG seed.
    private const int RandomSeed = 1202;
    private const int AlphabetSize = 26;

    private string _source = "";

    private int[][] _pairs = [];
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];
        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        _source = new string(chars);

        _pairs = new int[Length][];
        for (var i = 0; i < Length; i++)
        {
            _pairs[i] = [random.Next(Length), random.Next(Length)];
        }
    }

    [Benchmark(Baseline = true)]
    public string AdjacencyListBfs() =>
        SmallestStringWithSwapsSolution.SmallestStringByAdjacencyListBfs(_source, _pairs);

    [Benchmark]
    public string DisjointSetUnionFind() =>
        SmallestStringWithSwapsSolution.SmallestStringByDisjointSet(_source, _pairs);
}
