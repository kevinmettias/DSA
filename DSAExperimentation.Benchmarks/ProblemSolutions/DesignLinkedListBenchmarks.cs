using DSAExperimentation.LeetCode.DesignLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignLinkedListSolution's, the same factories
// DesignLinkedListSolutionTests proves correct. Design Linked List (LC 707): an array-backed
// list's front insert (List<int>.Insert(0, _), the naive baseline an array-backed
// "just use a List" implementation reaches for - O(n) per call, since every existing
// element has to shift right) vs. this repo's SinglyLinkedListNode<TValue> chain's
// AddAtHead, a single O(1) pointer reassignment regardless of how many nodes already
// exist. Both run the same Calls-length sequence of head insertions.
//
// LC 707 caps every index and value at 1000, and the tail read's index is Calls - 1,
// so Calls stops at 1001. That is below the few thousand calls where the O(n^2) vs
// O(n) split starts to dominate - until then List<int>.Insert's vectorized memmove
// beats the chain's per-node heap allocation despite doing asymptotically more work
// (a standalone Stopwatch check up to 50,000 calls showed the crossover, see this
// problem's manifest notes) - so the chain cannot show its advantage inside the bound.
//
// The script ends with two gets, at the head and at the tail, and their outputs are
// the arm's answer: AddAtHead returns nothing, so a script of inserts alone has no
// output for the arms to agree on. The head must be the last value inserted and the
// tail the first; reading the head costs both arms O(1), the tail one pointer walk.
public class DesignLinkedListBenchmarks
{
    [Params(100, 1_001)]
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
