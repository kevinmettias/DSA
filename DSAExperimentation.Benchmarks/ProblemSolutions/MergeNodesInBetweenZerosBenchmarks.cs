using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Merge Nodes in Between Zeros (LC 2181): a two-pass approach that first
// materializes every node's value into an intermediate List<int> and buckets that
// into a second List<int> of group sums before returning, vs. a single pass directly
// over this repo's own SinglyLinkedListNode<int>.Next that writes each merged sum
// straight into the output list as it walks - the same "extra pass, extra
// allocation" vs. "single primitive-native pass" contrast MiddleOfTheLinkedListBenchmarks
// already draws for LC 876. Both are O(n) time; MemoryDiagnoser is what separates them.
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

    [Benchmark(Baseline = true)]
    public int[] TwoPassWithIntermediateLists()
    {
        var values = new List<int>();
        for (var node = _head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        var groupSums = new List<int>();
        var sum = 0;
        for (var i = 1; i < values.Count; i++)
        {
            if (values[i] == 0)
            {
                groupSums.Add(sum);
                sum = 0;
            }
            else
            {
                sum += values[i];
            }
        }

        return groupSums.ToArray();
    }

    [Benchmark]
    public int[] SinglePassInPlaceSum() => ToArray(MergeNodes(_head));

    private static SinglyLinkedListNode<int>? MergeNodes(SinglyLinkedListNode<int> head)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        var sum = 0;

        for (var node = head.Next; node is not null; node = node.Next)
        {
            if (node.Value == 0)
            {
                tail.Next = new SinglyLinkedListNode<int>(sum);
                tail = tail.Next;
                sum = 0;
            }
            else
            {
                sum += node.Value;
            }
        }

        return dummy.Next;
    }

    private static int[] ToArray(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();
        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values.ToArray();
    }

    private static SinglyLinkedListNode<int> BuildRandomDelimitedList(Random random, int groupCount)
    {
        var head = new SinglyLinkedListNode<int>(0);
        var tail = head;

        for (var g = 0; g < groupCount; g++)
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
