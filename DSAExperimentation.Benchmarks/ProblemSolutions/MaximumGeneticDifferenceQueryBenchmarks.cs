using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Genetic Difference Query (LC 1938): the tree is a straight chain
// (parents[i] = i-1), the adversarial shape that forces the naive "walk the parent
// pointers, XOR against every ancestor" baseline to pay its full O(depth) worst
// case on every query - the same "force the worst case instead of letting an early
// exit make the baseline look artificially competitive" convention TwoSumBenchmarks'
// own doc comment states. The alternative is one offline DFS that inserts/removes
// each node's value into this repo's own BitTrie exactly once, answering every
// query in O(32) while its ancestors are still live - the same subtree-count-based
// Remove technique MaximumGeneticDifferenceQueryTests composes from
// CountPairsWithXorInARangeTests.
[MemoryDiagnoser]
public class MaximumGeneticDifferenceQueryBenchmarks
{
    // Bit 31 is the most significant bit of a 32-bit int; every bit walk below
    // processes all 32 bits, MSB first.
    private const int MostSignificantBitIndex = 31;

    private readonly record struct BitTrieState(BitTrie Trie, HashMap<BitTrieNode, int> SubtreeCount);

    [Params(200, 2_000)]
    public int NodeCount;

    private int[] _parents = null!;
    private List<int>[] _children = null!;
    private (int Node, int Value)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);

        _parents = BuildParentChain(NodeCount);
        _children = BuildChildrenLists(_parents, NodeCount);
        _queries = GenerateQueries(random, NodeCount);
    }

    private static int[] BuildParentChain(int nodeCount)
    {
        var parents = new int[nodeCount];
        parents[0] = -1;

        for (var i = 1; i < nodeCount; i++)
        {
            parents[i] = i - 1;
        }

        return parents;
    }

    private static List<int>[] BuildChildrenLists(int[] parents, int nodeCount)
    {
        var children = new List<int>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            children[i] = [];
        }

        for (var i = 1; i < nodeCount; i++)
        {
            children[parents[i]].Add(i);
        }

        return children;
    }

    private static (int Node, int Value)[] GenerateQueries(Random random, int nodeCount)
    {
        return Enumerable.Range(0, nodeCount)
            .Select(_ => (Node: random.Next(nodeCount), Value: random.Next(nodeCount)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public long NaiveAncestorWalk()
    {
        var total = 0L;

        foreach (var (node, value) in _queries)
        {
            var best = 0;
            var current = node;

            while (current != -1)
            {
                best = Math.Max(best, current ^ value);
                current = _parents[current];
            }

            total += best;
        }

        return total;
    }

    [Benchmark]
    public long BitTrieOfflineDfs()
    {
        var queriesByNode = GroupQueriesByNode(_queries, NodeCount);
        var trie = new BitTrie();
        var subtreeCount = new HashMap<BitTrieNode, int>();
        var answers = new int[_queries.Length];

        Visit(0, queriesByNode, new BitTrieState(trie, subtreeCount), answers);

        return SumAnswers(answers);
    }

    private static List<(int QueryIndex, int Value)>[] GroupQueriesByNode((int Node, int Value)[] queries, int nodeCount)
    {
        var queriesByNode = new List<(int QueryIndex, int Value)>[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            queriesByNode[i] = [];
        }

        for (var i = 0; i < queries.Length; i++)
        {
            queriesByNode[queries[i].Node].Add((i, queries[i].Value));
        }

        return queriesByNode;
    }

    private static long SumAnswers(int[] answers)
    {
        var total = 0L;

        foreach (var answer in answers)
        {
            total += answer;
        }

        return total;
    }

    private void Visit(
        int node,
        List<(int QueryIndex, int Value)>[] queriesByNode,
        BitTrieState state,
        int[] answers)
    {
        Insert(state.Trie, state.SubtreeCount, node);

        foreach (var (queryIndex, value) in queriesByNode[node])
        {
            answers[queryIndex] = MaxXorAmongLive(state.Trie.Root, state.SubtreeCount, value);
        }

        foreach (var child in _children[node])
        {
            Visit(child, queriesByNode, state, answers);
        }

        Remove(state.Trie, state.SubtreeCount, node);
    }

    private static void AdjustSubtreeCountsAlongPath(
        BitTrieNode root, HashMap<BitTrieNode, int> subtreeCount, int value, int delta)
    {
        var current = root;
        var bits = unchecked((uint)value);

        for (var i = MostSignificantBitIndex; i >= 0; i--)
        {
            var bit = (bits >> i) & 1u;
            current = bit == 0 ? current.Zero! : current.One!;
            subtreeCount.TryGetValue(current, out var existing);
            subtreeCount.Set(current, existing + delta);
        }
    }

    private static void Insert(BitTrie trie, HashMap<BitTrieNode, int> subtreeCount, int value)
    {
        trie.Insert(value);
        AdjustSubtreeCountsAlongPath(trie.Root, subtreeCount, value, 1);
    }

    private static void Remove(BitTrie trie, HashMap<BitTrieNode, int> subtreeCount, int value)
    {
        AdjustSubtreeCountsAlongPath(trie.Root, subtreeCount, value, -1);
    }

    private static int MaxXorAmongLive(BitTrieNode root, HashMap<BitTrieNode, int> subtreeCount, int value)
    {
        var current = root;
        var bits = unchecked((uint)value);
        var xor = 0u;

        for (var i = MostSignificantBitIndex; i >= 0; i--)
        {
            var bit = (bits >> i) & 1u;
            var opposite = bit == 0 ? current.One : current.Zero;
            var same = bit == 0 ? current.Zero : current.One;

            if (opposite is not null && subtreeCount.TryGetValue(opposite, out var count) && count > 0)
            {
                xor |= 1u << i;
                current = opposite;
            }
            else
            {
                current = same!;
            }
        }

        return unchecked((int)xor);
    }
}
