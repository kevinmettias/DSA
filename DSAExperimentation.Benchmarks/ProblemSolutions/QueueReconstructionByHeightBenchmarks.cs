using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.QueueReconstructionByHeight;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are QueueReconstructionByHeightSolution's, the same
// methods QueueReconstructionByHeightTests proves correct. Each arm takes the
// (Height, K) pairs [GlobalSetup] already prepared, so decoding LeetCode's
// int[][] shape is not charged to the measured method - the hoisted overload
// QueueReconstructionByHeightSolution exposes for exactly that. Both arms
// return the reconstructed queue itself (LeetCode's actual answer), not just
// its count.
[MemoryDiagnoser]
public class QueueReconstructionByHeightBenchmarks
{
    private const int RandomSeed = 406;

    private (int Height, int K)[] _people = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _people = Enumerable.Range(0, Length)
            .Select(_ =>
            {
                var height = random.Next(1, Length);
                return (Height: height, K: random.Next(0, height));
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[][] ArraySortListInsert() =>
        QueueReconstructionByHeightSolution.ReconstructQueueByArraySortListInsert(_people);

    [Benchmark]
    public int[][] MergeSortDynamicArrayInsert() =>
        QueueReconstructionByHeightSolution.ReconstructQueueByMergeSortDynamicArrayInsert(_people);
}
