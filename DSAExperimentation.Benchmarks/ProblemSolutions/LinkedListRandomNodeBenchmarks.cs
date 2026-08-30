using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Linked List Random Node (LC 382): reservoir sampling directly over this repo's
// SinglyLinkedListNode<int> chain (O(1) extra space, but an O(n) walk on EVERY
// GetRandom call) vs. caching the list once into a DynamicArray<int> (O(n) space,
// O(1) per call afterward) - the same space/time tradeoff InsertDeleteGetRandomO1's
// array-backed GetRandom already commits to, made explicit here as two competing
// strategies. CallCount is large relative to Length so the array cache's one-time
// O(n) conversion is amortized across many O(1) lookups, instead of a small call
// count hiding ReservoirSampling's per-call O(n) cost.
[MemoryDiagnoser]
public class LinkedListRandomNodeBenchmarks
{
    private const int CallCount = 2_000;

    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup()
    {
        var head = new SinglyLinkedListNode<int>(Length - 1);
        for (var value = Length - 2; value >= 0; value--)
        {
            head = new SinglyLinkedListNode<int>(value) { Next = head };
        }

        _head = head;
    }

    [Benchmark(Baseline = true)]
    public long ReservoirSampling()
    {
        var random = new Random(1);
        long sum = 0;

        for (var call = 0; call < CallCount; call++)
        {
            var result = _head.Value;
            var index = 2;

            for (var node = _head.Next; node is not null; node = node.Next)
            {
                if (random.Next(index) == 0)
                {
                    result = node.Value;
                }

                index++;
            }

            sum += result;
        }

        return sum;
    }

    [Benchmark]
    public long DynamicArrayCache()
    {
        var random = new Random(1);
        var values = new DynamicArray<int>();

        for (var node = _head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        long sum = 0;
        for (var call = 0; call < CallCount; call++)
        {
            sum += values.Get(random.Next(values.Count));
        }

        return sum;
    }
}
