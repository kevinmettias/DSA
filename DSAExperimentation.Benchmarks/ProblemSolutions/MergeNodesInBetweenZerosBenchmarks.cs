using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.MergeNodesInBetweenZeros;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MergeNodesInBetweenZerosSolution's, the same methods
// MergeNodesInBetweenZerosTests proves correct - the two-pass value buffer against
// the single pass that appends each group sum as it walks. Both are O(n) time;
// MemoryDiagnoser is what separates them. [GlobalSetup] builds the delimited list
// (workload sizing), and neither strategy mutates the list it is handed, so one
// prepared chain serves every invocation and neither arm needs a hoisted overload.
[MemoryDiagnoser]
public class MergeNodesInBetweenZerosBenchmarks
{
    private const int GroupSizeExclusiveUpperBound = 5;
    private const int NodeValueExclusiveUpperBound = 100;
    private const int RandomSeed = 2181; // LC problem number

    [Params(200, 5_000)]
    public int GroupCount;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _head = BuildRandomDelimitedList(random, GroupCount);
    }

    // Returns object, not SinglyLinkedListNode<int> - the node type is internal,
    // so a public [Benchmark] method cannot name it as a return type (CS0050).
    [Benchmark(Baseline = true)]
    public object? TwoPassValueBuffer() =>
        MergeNodesInBetweenZerosSolution.MergeNodesByTwoPassValueBuffer(_head);

    [Benchmark]
    public object? SinglePassSum() =>
        MergeNodesInBetweenZerosSolution.MergeNodesBySinglePassSum(_head);

    private static SinglyLinkedListNode<int> BuildRandomDelimitedList(Random random, int groupCount)
    {
        var head = new SinglyLinkedListNode<int>(0);
        var tail = head;

        for (var group = 0; group < groupCount; group++)
        {
            var groupSize = random.Next(1, GroupSizeExclusiveUpperBound);

            for (var i = 0; i < groupSize; i++)
            {
                tail.Next = new SinglyLinkedListNode<int>(random.Next(1, NodeValueExclusiveUpperBound));
                tail = tail.Next;
            }

            tail.Next = new SinglyLinkedListNode<int>(0);
            tail = tail.Next;
        }

        return head;
    }
}
