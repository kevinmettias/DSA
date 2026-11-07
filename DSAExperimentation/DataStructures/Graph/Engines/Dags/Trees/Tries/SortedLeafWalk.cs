namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// What SuffixTree.ToSuffixArray's walk has taken so far: the suffixes in the order met, each
// neighbour pair's common prefix length, and the shallowest depth passed since the last suffix
// taken. Its own type so the tree's public surface stays the tree's.
internal sealed class SortedLeafWalk(int textLength)
{
    private int _taken;
    private int _shallowestSinceLast;

    public int[] Suffixes { get; } = new int[textLength];

    public int[] NeighbourPrefixes { get; } = new int[Math.Max(textLength - 1, 0)];

    // The walk enters a node from its parent, at the parent's depth.
    public void Pass(int parentDepth) =>
        _shallowestSinceLast = Math.Min(_shallowestSinceLast, parentDepth);

    public void Take(int suffixStart, int depth)
    {
        if (_taken > 0)
        {
            NeighbourPrefixes[_taken - 1] = _shallowestSinceLast;
        }

        Suffixes[_taken] = suffixStart;
        _taken++;
        _shallowestSinceLast = depth;
    }
}
