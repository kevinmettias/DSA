using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignLinkedListSolution's, the same factories
// DesignLinkedListTests proves correct. Design Linked List (LC 707): an array-backed
// list's front insert (List<int>.Insert(0, _), the naive baseline an array-backed
// "just use a List" implementation reaches for - O(n) per call, since every existing
// element has to shift right) vs. this repo's SinglyLinkedListNode<TValue> chain's
// AddAtHead, a single O(1) pointer reassignment regardless of how many nodes already
// exist. Both run the same Calls-length sequence of head insertions. Params start at
// 5,000, not TwoSumBenchmarks' usual 200: below a few thousand calls, List<int>.Insert's
// vectorized memmove is fast enough per call to beat the linked-list chain's per-node
// heap allocation despite doing asymptotically more work - the real O(n^2) vs O(n) split
// only dominates once Calls is large enough that the shifting cost outweighs the
// constant allocation overhead (confirmed with a standalone Stopwatch check up to
// Calls=50,000, see this problem's manifest notes).
[MemoryDiagnoser]
public class DesignLinkedListBenchmarks
{
    [Params(5_000, 50_000)]
    public int Calls { get; set; }

    [Benchmark(Baseline = true)]
    public int ArrayListAddAtHead() => Replay(DesignLinkedListSolution.CreateByArrayList());

    [Benchmark]
    public int SinglyLinkedListChainAddAtHead() => Replay(DesignLinkedListSolution.CreateBySinglyLinkedListChain());

    private int Replay(DesignLinkedListSolution.IMyLinkedList list)
    {
        for (var i = 0; i < Calls; i++)
        {
            list.AddAtHead(i);
        }

        return Calls;
    }
}
