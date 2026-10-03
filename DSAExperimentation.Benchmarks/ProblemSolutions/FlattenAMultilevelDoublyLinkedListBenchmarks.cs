using DSAExperimentation.LeetCode.FlattenAMultilevelDoublyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FlattenAMultilevelDoublyLinkedListSolution's, the same
// methods FlattenAMultilevelDoublyLinkedListSolutionTests proves correct. Each [Benchmark] rebuilds
// a fresh copy since flattening is destructive (Child pointers are cleared in place),
// matching RotateImageBenchmarks' per-invocation Clone convention - so list construction
// stays outside [GlobalSetup] deliberately, same as the pre-migration benchmark. Each arm
// returns the flattened list's head as object?, since a public [Benchmark] method cannot
// name the internal Node (CS0050). LC 430 caps the list, children included, at 1000
// nodes: Length chain nodes and Length - 1 children make 500 the largest Length.
public class FlattenAMultilevelDoublyLinkedListBenchmarks
{
    [Params(200, 500)]
    public int Length { get; set; }

    [Benchmark(Baseline = true)]
    public object? BruteForceRescanFromHead() =>
        FlattenAMultilevelDoublyLinkedListSolution.FlattenByBruteForceRescan(BuildList(Length));

    [Benchmark]
    public object? StackBasedOnePass() =>
        FlattenAMultilevelDoublyLinkedListSolution.FlattenByStack(BuildList(Length));

    // The chain holds 1..length and a child holds length plus its parent's value, so every
    // value sits inside LC 430's 1..10^5 and no two nodes share one.
    private static Node BuildList(int length)
    {
        var head = new Node(1);
        var current = head;

        for (var i = 1; i < length; i++)
        {
            var next = new Node(i + 1);
            current.Next = next;
            next.Previous = current;
            current = next;
        }

        // Every node but the last gets a single-node child, forcing one splice per node.
        for (var node = head; node.Next is not null; node = node.Next)
        {
            node.Child = new Node(length + node.Value);
        }

        return head;
    }
}
