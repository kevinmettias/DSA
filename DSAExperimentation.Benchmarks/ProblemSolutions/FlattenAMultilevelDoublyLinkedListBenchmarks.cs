using DSAExperimentation.LeetCode.FlattenAMultilevelDoublyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FlattenAMultilevelDoublyLinkedListSolution's, the same
// methods FlattenAMultilevelDoublyLinkedListSolutionTests proves correct. Each [Benchmark] rebuilds
// a fresh copy since flattening is destructive (Child pointers are cleared in place),
// matching RotateImageBenchmarks' per-invocation Clone convention - so list construction
// stays outside [GlobalSetup] deliberately, same as the pre-migration benchmark. Each arm
// returns the flattened list's head as object?, since a public [Benchmark] method cannot
// name the internal Node (CS0050).
public class FlattenAMultilevelDoublyLinkedListBenchmarks
{
    [Params(200, 5_000)]
    public int Length { get; set; }

    [Benchmark(Baseline = true)]
    public object? BruteForceRescanFromHead() =>
        FlattenAMultilevelDoublyLinkedListSolution.FlattenByBruteForceRescan(BuildList(Length));

    [Benchmark]
    public object? StackBasedOnePass() =>
        FlattenAMultilevelDoublyLinkedListSolution.FlattenByStack(BuildList(Length));

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
}
