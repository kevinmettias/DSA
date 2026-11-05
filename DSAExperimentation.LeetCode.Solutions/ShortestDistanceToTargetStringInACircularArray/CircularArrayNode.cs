namespace DSAExperimentation.LeetCode.Solutions.ShortestDistanceToTargetStringInACircularArray;

// One node per array index of LC 2515's words[]. Neighbors always holds exactly
// the two circular steps - (index + 1) % n and (index - 1 + n) % n - filled in
// once while the graph is built, so the wraparound lives in the construction and
// never in the search.
//
// Answers this problem alone - DataStructures' AdjacencyNode plus the word at its
// index, which only this problem reads - so it lives beside the solution rather than
// in DataStructures/ (ARCHITECTURE.md #17.3/#17.6). It previously existed as a Tests
// fixture and again in Benchmarks/Fixtures; this is the declaration this problem now
// uses for both.
internal sealed class CircularArrayNode(int index, string word)
{
    public int Index { get; } = index;

    public string Word { get; } = word;

    public List<CircularArrayNode> Neighbors { get; } = [];

    public override string ToString() => $"{Index}:{Word}";
}
