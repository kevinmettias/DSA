using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.MiddleOfTheLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MiddleOfTheLinkedListSolution's, the same methods
// MiddleOfTheLinkedListSolutionTests proves correct. CountThenWalk is the naive two-pass
// approach (count the list, then walk length/2 steps from the head);
// SlowFastTwoPointer lands on the same node in a single pass. Both are O(n), but
// the baseline touches every node twice. Length stops at LC 876's own bound of 100
// nodes, with values drawn from its [1, 100].
public class MiddleOfTheLinkedListBenchmarks
{
    private const int RandomSeed = 876; // LC problem number
    private const int MinNodeValue = 1;
    private const int MaxNodeValueExclusive = 101;

    private SinglyLinkedListNode<int> _head = null!;

    [Params(10, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _head = BuildRandomList(random, Length);
    }

    private static SinglyLinkedListNode<int> BuildRandomList(Random random, int length)
    {
        var head = new SinglyLinkedListNode<int>(random.Next(MinNodeValue, MaxNodeValueExclusive));
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(random.Next(MinNodeValue, MaxNodeValueExclusive));
            tail = tail.Next;
        }

        return head;
    }

    // Returns object, not SinglyLinkedListNode<int> - the node type is internal,
    // so a public [Benchmark] method cannot name it as a return type (CS0050).
    // Returning the middle node itself keeps both arms on LeetCode's real answer
    // shape rather than the weaker "read a value off it" measurement the pre-§17
    // benchmark took.
    [Benchmark(Baseline = true)]
    public object? CountThenWalk() => MiddleOfTheLinkedListSolution.MiddleNodeByCountThenWalk(_head);

    [Benchmark]
    public object? SlowFastTwoPointer() => MiddleOfTheLinkedListSolution.MiddleNodeBySlowFastTwoPointer(_head);
}
