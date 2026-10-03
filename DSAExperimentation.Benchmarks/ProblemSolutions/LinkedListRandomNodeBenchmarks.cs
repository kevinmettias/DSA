using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.LinkedListRandomNode;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LinkedListRandomNodeSolution's, the same methods
// LinkedListRandomNodeSolutionTests proves correct. [GlobalSetup] builds the chain
// (workload sizing); each [Benchmark] method drives its own construct-once-then
// -call-many loop - a real Solution instance's own lifetime - so DynamicArrayCache's
// one-time O(n) DynamicArray conversion is charged to the measured method, same as
// ReservoirSampling's O(1) setup, rather than hoisting construction into
// [GlobalSetup] and hiding it from the comparison. CallCount is large relative to
// Length so that one-time conversion is amortized across many O(1) lookups, instead
// of a small call count hiding ReservoirSampling's per-call O(n) cost.
public class LinkedListRandomNodeBenchmarks
{
    private const int CallCount = 2_000;

    private SinglyLinkedListNode<int> _head = null!;

    // Every value a call drew, in call order - what each arm returns.
    private int[] _draws = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _head = LinkedListRandomNodeWorkloads.Build(Length);
        _draws = new int[CallCount];
    }

    [Benchmark(Baseline = true)]
    public int[] ReservoirSampling()
    {
        var random = new Random(1);

        for (var call = 0; call < CallCount; call++)
        {
            _draws[call] = LinkedListRandomNodeSolution.GetRandomByReservoirSampling(_head, random);
        }

        return _draws;
    }

    [Benchmark]
    public int[] DynamicArrayCache()
    {
        var random = new Random(1);
        var cache = LinkedListRandomNodeSolution.CacheValues(_head);

        for (var call = 0; call < CallCount; call++)
        {
            _draws[call] = LinkedListRandomNodeSolution.GetRandomByDynamicArrayCache(cache, random);
        }

        return _draws;
    }
}
