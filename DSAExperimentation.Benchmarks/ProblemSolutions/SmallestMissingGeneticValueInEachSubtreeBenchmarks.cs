using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Smallest Missing Genetic Value in Each Subtree (LC 2003): a chain-shaped family
// tree with genetic value 1 planted at the deepest leaf is the worst case for the
// textbook "rescan every node's subtree from scratch" approach (O(n^2) - each of
// the n nodes pays for its own independent DFS) and the best case for the
// ancestor-chain-with-skip approach (O(n) total - this repo's own
// DepthFirstSearch.Traverse walking only the newly-reached portion of the tree at
// each step up to the root, the same technique MaximumGeneticDifferenceQueryTests
// already uses for a parent-array family tree).
[MemoryDiagnoser]
public class SmallestMissingGeneticValueInEachSubtreeBenchmarks
{
    private const int BaseGeneValue = 2;

    [Params(200, 2000)]
    public int NodeCount;

    private int[] _parents = null!;
    private int[] _nums = null!;
    private List<int>[] _children = null!;

    [GlobalSetup]
    public void Setup()
    {
        _parents = new int[NodeCount];
        _nums = new int[NodeCount];
        _parents[0] = -1;
        _nums[0] = BaseGeneValue;

        for (var i = 1; i < NodeCount; i++)
        {
            _parents[i] = i - 1;
            _nums[i] = i + BaseGeneValue;
        }

        _nums[NodeCount - 1] = 1;
        _children = BuildChildren(_parents);
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce()
    {
        var answers = new int[NodeCount];

        for (var node = 0; node < NodeCount; node++)
        {
            var values = new HashSet<int>();
            CollectSubtreeValues(node, values);

            var mex = 1;
            while (values.Contains(mex))
            {
                mex++;
            }

            answers[node] = mex;
        }

        return answers;
    }

    [Benchmark]
    public int[] AncestorChainWithSkip()
    {
        var answers = new int[NodeCount];
        Array.Fill(answers, 1);

        var nodeWithValueOne = Array.IndexOf(_nums, 1);
        if (nodeWithValueOne == -1)
        {
            return answers;
        }

        var geneValues = new Set<int>();
        var state = new AncestorChainState(nodeWithValueOne, Skip: -1, Mex: 1);

        while (state.Node != -1)
        {
            state = AdvanceAncestorChain(state, geneValues, answers);
        }

        return answers;
    }

    private AncestorChainState AdvanceAncestorChain(AncestorChainState state, Set<int> geneValues, int[] answers)
    {
        var skipChild = state.Skip;
        var newlyVisited = DepthFirstSearch.Traverse(state.Node, current => _children[current].Where(c => c != skipChild));

        foreach (var visited in newlyVisited)
        {
            geneValues.TryAdd(_nums[visited]);
        }

        var mex = state.Mex;
        while (geneValues.Has(mex))
        {
            mex++;
        }

        answers[state.Node] = mex;

        return state with { Node = _parents[state.Node], Skip = state.Node, Mex = mex };
    }

    private readonly record struct AncestorChainState(int Node, int Skip, int Mex);

    private void CollectSubtreeValues(int node, HashSet<int> values)
    {
        values.Add(_nums[node]);

        foreach (var child in _children[node])
        {
            CollectSubtreeValues(child, values);
        }
    }

    private static List<int>[] BuildChildren(int[] parents)
    {
        var children = new List<int>[parents.Length];
        for (var i = 0; i < parents.Length; i++)
        {
            children[i] = [];
        }

        for (var i = 0; i < parents.Length; i++)
        {
            if (parents[i] != -1)
            {
                children[parents[i]].Add(i);
            }
        }

        return children;
    }
}
