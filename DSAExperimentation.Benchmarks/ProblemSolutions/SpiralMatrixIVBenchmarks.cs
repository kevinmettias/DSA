using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.SpiralMatrixIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SpiralMatrixIVSolution's, the same methods
// SpiralMatrixIVTests proves correct - the direction-vector walk that carries a
// second m x n visited matrix against the boundary-shrinking walk that decides a
// turn once per side. Both are O(m * n) time; MemoryDiagnoser is what separates
// them. [GlobalSetup] builds a list exactly as long as the grid (workload sizing),
// and neither strategy mutates the list it is handed, so one prepared chain serves
// every invocation and neither arm needs a hoisted overload.
[MemoryDiagnoser]
public class SpiralMatrixIVBenchmarks
{
    private const int RandomSeed = 2326; // LC problem number
    private const int NodeValueExclusiveUpperBound = 1_000;

    private SinglyLinkedListNode<int> _head = null!;

    [Params(50, 200)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup() => _head = BuildRandomList(Size * Size);

    private static SinglyLinkedListNode<int> BuildRandomList(int length)
    {
        var random = new Random(RandomSeed);
        var head = new SinglyLinkedListNode<int>(random.Next(0, NodeValueExclusiveUpperBound));
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(random.Next(0, NodeValueExclusiveUpperBound));
            tail = tail.Next;
        }

        return head;
    }

    [Benchmark(Baseline = true)]
    public int[][] DirectionArrayWithVisitedTracking() =>
        SpiralMatrixIVSolution.SpiralMatrixByDirectionArray(Size, Size, _head);

    [Benchmark]
    public int[][] BoundaryShrinkingSpiralWalk() =>
        SpiralMatrixIVSolution.SpiralMatrixByBoundaryShrink(Size, Size, _head);
}
