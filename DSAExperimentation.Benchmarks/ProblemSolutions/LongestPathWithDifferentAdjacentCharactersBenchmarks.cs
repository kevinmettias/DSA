using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.LongestPathWithDifferentAdjacentCharacters;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestPathWithDifferentAdjacentCharactersSolution's,
// the same methods LongestPathWithDifferentAdjacentCharactersTests proves agree -
// the O(n^2) recomputing walk against this repo's single-pass TreeFold.
//
// A skewed chain with alternating labels, not a bushy random tree: every edge is
// valid, so this is the recomputing walk's worst case (a fresh O(depth) Height
// walk repeated at every one of n nodes) and the case where the fold's single O(n)
// pass wins most decisively - the same reasoning DiameterOfBinaryTreeBenchmarks'
// Setup comment gives for its own left-skewed chain.
[MemoryDiagnoser]
public class LongestPathWithDifferentAdjacentCharactersBenchmarks
{
    // Setup alternates each node's label between exactly two characters ('a'/'b')
    // so that every parent-child edge in the chain has different adjacent labels.
    private const int LabelAlternationPeriod = 2;

    [Params(200, 2_000)]
    public int NodeCount;

    private RootedTreeNode _root = null!;
    private string _labels = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        _root = ParentArrayTree.Chain(NodeCount);
        _labels = AlternatingLabels(NodeCount);
    }

    [Benchmark(Baseline = true)]
    public int RecomputedChainPerNode() =>
        LongestPathWithDifferentAdjacentCharactersSolution.LongestPathByRecomputedSubtreeWalk(_root, _labels);

    [Benchmark]
    public int TreeFoldLongestPath() =>
        LongestPathWithDifferentAdjacentCharactersSolution.LongestPathByTreeFold(_root, _labels);

    private static string AlternatingLabels(int nodeCount)
    {
        var labels = new char[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            labels[i] = i % LabelAlternationPeriod == 0 ? 'a' : 'b';
        }

        return new string(labels);
    }
}
