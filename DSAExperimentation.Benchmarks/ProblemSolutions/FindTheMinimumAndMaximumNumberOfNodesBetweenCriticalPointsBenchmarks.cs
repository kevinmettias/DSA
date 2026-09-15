using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPoints;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsSolution's, the same
// methods FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsTests proves
// correct. [GlobalSetup] builds the zigzag chain (workload sizing), so each
// measured call is only the walk - LeetCode's own input shape is already the
// prepared repo object here, so neither strategy needs a hoisted overload.
[MemoryDiagnoser]
public class FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsBenchmarks
{
    // The deterministic list seed this benchmark has always used.
    private const int ListSeed = 7;

    private SinglyLinkedListNode<int> _head = null!;

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _head = CriticalPointsWorkloads.BuildZigzagList(Length, seed: ListSeed);

    [Benchmark(Baseline = true)]
    public int[] MaterializeIndicesThenScan() =>
        FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsSolution
            .NodesBetweenCriticalPointsByMaterializedIndices(_head);

    [Benchmark]
    public int[] SinglePassConstantSpace() =>
        FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsSolution
            .NodesBetweenCriticalPointsBySinglePassScan(_head);
}
