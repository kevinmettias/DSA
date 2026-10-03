using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.QueriesOnAPermutationWithKey;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are QueriesOnAPermutationWithKeySolution's, the same
// methods QueriesOnAPermutationWithKeySolutionTests proves correct - the move-to-front
// simulation over a BCL List<int> vs. over this repo's own DynamicArray<int>,
// both O(Queries * M). The query stream is a fixed-seed random draw over
// [1..PermutationSize], built in [GlobalSetup] so only the simulation is measured.
public class QueriesOnAPermutationWithKeyBenchmarks
{
    private const int Seed = 1;
    private const int FirstPermutationValue = 1;

    private int[] _queries = [];

    [Params(200, 1_000)]
    public int PermutationSize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _queries = SeededDraws.Values(PermutationSize, FirstPermutationValue, PermutationSize + 1, random);
    }

    [Benchmark(Baseline = true)]
    public List<int> ListMoveToFront() =>
        QueriesOnAPermutationWithKeySolution.AnswerQueriesByListMoveToFront(_queries, PermutationSize);

    [Benchmark]
    public List<int> DynamicArrayMoveToFront() =>
        QueriesOnAPermutationWithKeySolution.AnswerQueriesByDynamicArrayMoveToFront(_queries, PermutationSize);
}
