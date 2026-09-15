using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.LinkedListRandomNode;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LinkedListRandomNodeSolution's, the same methods
// LinkedListRandomNodeTests proves correct. [GlobalSetup] builds the chain
// (workload sizing); each [Benchmark] method drives its own construct-once-then
// -call-many loop - a real Solution instance's own lifetime - so DynamicArrayCache's
// one-time O(n) DynamicArray conversion is charged to the measured method, same as
// ReservoirSampling's O(1) setup, rather than hoisting construction into
// [GlobalSetup] and hiding it from the comparison. CallCount is large relative to
// Length so that one-time conversion is amortized across many O(1) lookups, instead
// of a small call count hiding ReservoirSampling's per-call O(n) cost.
[MemoryDiagnoser]
public class LinkedListRandomNodeBenchmarks
{
    private const int CallCount = 2_000;

    private SinglyLinkedListNode<int> _head = null!;

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _head = LinkedListRandomNodeWorkloads.Build(Length);

    [Benchmark(Baseline = true)]
    public long ReservoirSampling()
    {
        var random = new Random(1);
        long sum = 0;

        for (var call = 0; call < CallCount; call++)
        {
            sum += LinkedListRandomNodeSolution.GetRandomByReservoirSampling(_head, random);
        }

        return sum;
    }

    [Benchmark]
    public long DynamicArrayCache()
    {
        var random = new Random(1);
        var cache = LinkedListRandomNodeSolution.CacheValues(_head);
        long sum = 0;

        for (var call = 0; call < CallCount; call++)
        {
            sum += LinkedListRandomNodeSolution.GetRandomByDynamicArrayCache(cache, random);
        }

        return sum;
    }
}
