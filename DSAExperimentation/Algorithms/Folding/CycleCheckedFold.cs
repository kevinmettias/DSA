using System.Diagnostics.CodeAnalysis;

namespace DSAExperimentation.Algorithms.Folding;

// The graph tier's policy: nothing is promised, so it memoizes as MemoizedFold does and also tracks
// the nodes still on the recursion path. Meeting one of those again is a true cycle - a node whose
// result would depend on itself - and the fold is aborted rather than handed a wrong answer.
//
// The policy is copied as FoldRecursion threads it down the recursion, so the cycle flag cannot be
// a field of the struct itself: all three pieces of state live in one small ledger the struct points
// at, and every copy sees a cycle the moment any copy finds one. The constructor builds that ledger;
// a default value has none and throws on first use.
internal readonly struct CycleCheckedFold<TNode, TResult> : IFoldMemo<TNode, TResult>
    where TNode : class
{
    private readonly Ledger _ledger;

    public bool Aborted => _ledger.CycleFound;

    public CycleCheckedFold() => _ledger = new Ledger();

    public bool TryRecall(TNode node, [MaybeNullWhen(false)] out TResult result) => _ledger.Completed.TryGetValue(node, out result);

    // A node already on the path is a cycle; recording it here is what makes every copy abort.
    public bool TryOpen(TNode node)
    {
        if (_ledger.InProgress.Add(node))
        {
            return true;
        }

        _ledger.CycleFound = true;

        return false;
    }

    public void Close(TNode node, TResult result)
    {
        _ledger.InProgress.Remove(node);
        _ledger.Completed[node] = result;
    }

    private sealed class Ledger
    {
        public Dictionary<TNode, TResult> Completed { get; } = [];

        public HashSet<TNode> InProgress { get; } = [];

        public bool CycleFound { get; set; }
    }
}
