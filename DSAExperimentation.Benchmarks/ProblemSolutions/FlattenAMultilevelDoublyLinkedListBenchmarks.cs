using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FlattenAMultilevelDoublyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FlattenAMultilevelDoublyLinkedListSolution's, the same
// methods FlattenAMultilevelDoublyLinkedListTests proves correct. Each [Benchmark] rebuilds
// a fresh copy since flattening is destructive (Child pointers are cleared in place),
// matching RotateImageBenchmarks' per-invocation Clone convention - so list construction
// stays outside [GlobalSetup] deliberately, same as the pre-migration benchmark.
[MemoryDiagnoser]
public class FlattenAMultilevelDoublyLinkedListBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    [Benchmark(Baseline = true)]
    public int BruteForceRescanFromHead() =>
        CountNodes(FlattenAMultilevelDoublyLinkedListSolution.FlattenByBruteForceRescan(BuildList(Length)));

    [Benchmark]
    public int StackBasedOnePass() =>
        CountNodes(FlattenAMultilevelDoublyLinkedListSolution.FlattenByStack(BuildList(Length)));

    private static Node BuildList(int length)
    {
        var head = new Node(0);
        var current = head;

        for (var i = 1; i < length; i++)
        {
            var next = new Node(i);
            current.Next = next;
            next.Previous = current;
            current = next;
        }

        // Every node but the last gets a single-node child, forcing one splice per node.
        for (var node = head; node.Next is not null; node = node.Next)
        {
            node.Child = new Node(-1);
        }

        return head;
    }

    private static int CountNodes(Node? head)
    {
        var count = 0;
        for (var node = head; node is not null; node = node.Next)
        {
            count++;
        }

        return count;
    }
}
