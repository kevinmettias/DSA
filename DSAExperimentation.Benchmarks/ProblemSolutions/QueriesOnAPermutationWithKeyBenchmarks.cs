using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.QueriesOnAPermutationWithKey;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are QueriesOnAPermutationWithKeySolution's, the same
// methods QueriesOnAPermutationWithKeyTests proves correct - the move-to-front
// simulation over a BCL List<int> vs. over this repo's own DynamicArray<int>,
// both O(Queries * M). The query stream is a fixed-seed random draw over
// [1..M], built in [GlobalSetup] so only the simulation is measured.
[MemoryDiagnoser]
public class QueriesOnAPermutationWithKeyBenchmarks
{
    private const int Seed = 1;
    private const int FirstPermutationValue = 1;

    private int[] _queries = [];

    [Params(200, 1_000)]
    public int M { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _queries = Enumerable.Range(0, M).Select(_ => random.Next(FirstPermutationValue, M + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public List<int> ListMoveToFront() =>
        QueriesOnAPermutationWithKeySolution.ProcessQueriesByListMoveToFront(_queries, M);

    [Benchmark]
    public List<int> DynamicArrayMoveToFront() =>
        QueriesOnAPermutationWithKeySolution.ProcessQueriesByDynamicArrayMoveToFront(_queries, M);
}
