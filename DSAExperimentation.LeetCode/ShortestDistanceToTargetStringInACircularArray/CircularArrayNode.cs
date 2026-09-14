namespace DSAExperimentation.LeetCode.ShortestDistanceToTargetStringInACircularArray;

// One node per array index of LC 2515's words[]. Neighbors always holds exactly
// the two circular steps - (index + 1) % n and (index - 1 + n) % n - filled in
// once while the graph is built, so the wraparound lives in the construction and
// never in the search.
//
// Answers this problem alone - an index-and-payload node fixing no vertex set and
// no modulus - so it lives beside the solution rather than in Domain/ or
// DataStructures/ (ARCHITECTURE.md #17.3/#17.6), the same placement
// FunctionalGraphNode has for LC 2360. It previously existed as a Tests fixture
// and again in Benchmarks/Fixtures; this is the declaration this problem now uses
// for both.
internal sealed class CircularArrayNode(int index, string word)
{
    public int Index { get; } = index;

    public string Word { get; } = word;

    public List<CircularArrayNode> Neighbors { get; } = [];

    public override string ToString() => $"{Index}:{Word}";
}
