using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using BitWalk = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieWalk<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BitTrieNode,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BinarySlots<
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BitTrieNode>,
    int>;
using CountedWalk = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieWalk<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.CountedBitTrieNode,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BinarySlots<
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.CountedBitTrieNode>,
    int>;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The one maximum-XOR walk over both bit trie payloads: it takes the opposite digit's child
// wherever that child holds a value, and which children hold one is the payload's answer, a present
// child for BitTrieNode and a positive count for CountedBitTrieNode.
public sealed partial class MaxXorWalkTests
{
    private const int NearNeighbor = 0b1110;
    private const int AllOnes = 0b1111;
    private const int Query = 6;
    private const int Removed = 8;
    private const int Kept = 14;

    // 0 differs from 15 in every one of its four bits, so it beats 14, nearer in value.
    [Fact]
    public void Pattern_BitNodes_TakesTheOppositeChildWhereItExists()
    {
        var root = BitNodesOver([0, NearNeighbor]);

        var xor = MaxXorOf(root, AllOnes);

        Assert.Equal((uint)AllOnes, xor);
    }

    // 6 ^ 8 = 14 would win, but 8's path is left with a count of zero; 6 ^ 14 = 8 is the live best.
    [Fact]
    public void Pattern_CountedNodes_SkipsAChildWhoseCountIsZero()
    {
        var root = CountedNodesOver([Removed, Kept], Removed);

        var xor = MaxXorOf(root, Query);

        Assert.Equal((uint)(Query ^ Kept), xor);
    }

    // The walk compiled for a node class, given the query's complement as its callers give it.
    private static uint MaxXorOf<TNode>(TNode root, int query)
        where TNode : class, IBitTrieNode<TNode> =>
        MaxXorWalk<TNode>.Pattern(root, query, ~query);

    private static BitTrieNode BitNodesOver(int[] values)
    {
        var root = new BitTrieNode();

        foreach (var value in values)
        {
            BitWalk.GetOrAddPath(root, new Int32BitsKey(value));
        }

        return root;
    }

    // Every value goes in with a count of one along its path, then one copy of removed comes out.
    private static CountedBitTrieNode CountedNodesOver(int[] values, int removed)
    {
        var root = new CountedBitTrieNode();

        foreach (var value in values)
        {
            CountedWalk.GetOrAddPath<Int32BitsKey, CountUp>(root, new Int32BitsKey(value));
        }

        CountedWalk.GetOrAddPath<Int32BitsKey, CountDown>(root, new Int32BitsKey(removed));
        return root;
    }

    // A test step: one more value passes through each node it reaches.
    private readonly struct CountUp : ITrieStep<CountedBitTrieNode>
    {
        public static void Apply(CountedBitTrieNode node) => node.Count++;
    }

    // A test step: one fewer value passes through each node it reaches.
    private readonly struct CountDown : ITrieStep<CountedBitTrieNode>
    {
        public static void Apply(CountedBitTrieNode node) => node.Count--;
    }
}
