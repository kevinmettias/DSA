using DSAExperimentation.LeetCode.DesignLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignLinkedListSolution's, the same factories
// DesignLinkedListSolutionTests proves correct. Design Linked List (LC 707): an array-backed
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
//
// The script ends with two gets, at the head and at the tail, and their outputs are
// the arm's answer: AddAtHead returns nothing, so a script of inserts alone has no
// output for the arms to agree on. The head must be the last value inserted and the
// tail the first; reading the head costs both arms O(1), the tail one pointer walk.
public class DesignLinkedListBenchmarks
{
    [Params(5_000, 50_000)]
    public int Calls { get; set; }

    [Benchmark(Baseline = true)]
    public int[] ArrayListAddAtHead() => Replay(DesignLinkedListSolution.CreateByArrayList());

    [Benchmark]
    public int[] SinglyLinkedListChainAddAtHead() => Replay(DesignLinkedListSolution.CreateBySinglyLinkedListChain());

    private int[] Replay(DesignLinkedListSolution.IMyLinkedList list)
    {
        for (var i = 0; i < Calls; i++)
        {
            list.AddAtHead(i);
        }

        return [list.Get(0), list.Get(Calls - 1)];
    }
}
