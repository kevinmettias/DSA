namespace DSAExperimentation.Benchmarks.Fixtures;

// One node per array index. Neighbors always holds exactly the two circular steps
// - (index-1+n)%n and (index+1)%n - filled in once while the graph is built
// (RemainderNode precedent, with a fixed 2-way branching factor instead of 1).
internal sealed class CircularArrayNode(int index, string word)
{
    public int Index { get; } = index;

    public string Word { get; } = word;

    public List<CircularArrayNode> Neighbors { get; } = [];

    public override string ToString() => $"{Index}:{Word}";
}
