using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MostStonesRemovedWithSameRowOrColumn;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MostStonesRemovedWithSameRowOrColumnSolution's, the same
// methods MostStonesRemovedWithSameRowOrColumnTests proves correct. The stone field is
// generated once in [GlobalSetup] so only edge discovery and unioning are measured -
// PairwiseScan's O(n^2) pair sweep against RowColumnKeyedUnion's O(n) axis unions.
[MemoryDiagnoser]
public class MostStonesRemovedWithSameRowOrColumnBenchmarks
{
    private const int RandomSeed = 947; // LC problem number
    private const int CoordinateRangeDivisor = 4;

    private int[][] _stones = [];

    [Params(200, 2_000)]
    public int StoneCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var range = Math.Max(1, StoneCount / CoordinateRangeDivisor);
        var seen = new HashSet<(int Row, int Col)>();
        var stones = new List<int[]>();

        while (stones.Count < StoneCount)
        {
            var row = random.Next(range);
            var col = random.Next(range);

            if (seen.Add((row, col)))
            {
                stones.Add([row, col]);
            }
        }

        _stones = stones.ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseScanThenUnion() =>
        MostStonesRemovedWithSameRowOrColumnSolution.RemoveStonesByPairwiseScan(_stones);

    [Benchmark]
    public int RowColumnKeyedUnion() =>
        MostStonesRemovedWithSameRowOrColumnSolution.RemoveStonesByRowColumnKeyedUnion(_stones);
}
