using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNodeII;

namespace DSAExperimentation.LeetCode.Tests.PopulatingNextRightPointersInEachNodeII;

// Harness only. Both strategies are PopulatingNextRightPointersInEachNodeIISolution's. Each row is a
// tree in LeetCode's level order and LeetCode's readout of it once connected - each level along its
// next pointers, '#' (null here) closing it. The first two rows are LeetCode's published examples,
// where 5's next pointer reaches across 3's missing left child to 7. The rest are read by hand:
// a single node; a tree whose second level starts its children at 3, not 2, so the readout must
// find the next level past a childless node; and a right-leaning chain, one node per level. The
// readout itself is asserted on its own, over trees whose next pointers are wired by hand.
public sealed partial class PopulatingNextRightPointersInEachNodeIISolutionTests
{
    public static TheoryData<int?[], int?[]> Examples =>
        new()
        {
            { [1, 2, 3, 4, 5, null, 7], [1, null, 2, 3, null, 4, 5, 7, null] },
            { [], [] },
            { [1], [1, null] },
            { [1, 2, 3, null, null, 4, 5], [1, null, 2, 3, null, 4, 5, null] },
            { [1, null, 2, null, 3], [1, null, 2, null, 3, null] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectByManualQueueBfs_LeetCodeExamples_ReadsEachLevelAlongItsNextPointers(
        int?[] levelOrder, int?[] expected) =>
        Assert.Equal(
            expected,
            PopulatingNextRightPointersInEachNodeIISolution.ConnectByManualQueueBfs(LeetCodeWireFormat.ToBinaryTree(levelOrder)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectByLevelGroupedTraversal_LeetCodeExamples_ReadsEachLevelAlongItsNextPointers(
        int?[] levelOrder, int?[] expected) =>
        Assert.Equal(
            expected,
            PopulatingNextRightPointersInEachNodeIISolution.ConnectByLevelGroupedTraversal(LeetCodeWireFormat.ToBinaryTree(levelOrder)));

    // LeetCode's first tree - 1 over 2 and 3, 2 over 4 and 5, 3 over 7 alone - with the
    // pointers its answer draws: 2 -> 3, 4 -> 5 and 5 -> 7, every other one null. Read
    // level by level, each level closed by null: 1 #, 2 3 #, 4 5 7 #.
    [Fact]
    public void ReadLevelsAlongNextPointers_LeetCodeFirstTreeWired_ReadsEachLevelThenNull()
    {
        var nodes = FirstExampleNodesByValue();
        var next = NextPointersOf(nodes, [(2, 3), (4, 5), (5, 7)]);

        var readout = PopulatingNextRightPointersInEachNodeIISolution.ReadLevelsAlongNextPointers(
            nodes[1], new PopulatingNextRightPointersInEachNodeIISolution.DictionaryNextPointers(next));

        Assert.Equal([1, null, 2, 3, null, 4, 5, 7, null], readout);
    }

    // The same tree with 2's pointer left null: the readout follows the pointers, not the
    // tree, so the second level stops at 2. The third still starts at 2's left child and
    // runs 4 -> 5 -> 7: 1 #, 2 #, 4 5 7 #.
    [Fact]
    public void ReadLevelsAlongNextPointers_ALevelsPointerStopsEarly_ReadsOnlyWhatThePointersReach()
    {
        var nodes = FirstExampleNodesByValue();
        var next = NextPointersOf(nodes, [(4, 5), (5, 7)]);

        var readout = PopulatingNextRightPointersInEachNodeIISolution.ReadLevelsAlongNextPointers(
            nodes[1], new PopulatingNextRightPointersInEachNodeIISolution.DictionaryNextPointers(next));

        Assert.Equal([1, null, 2, null, 4, 5, 7, null], readout);
    }

    // The fourth row's tree - 1 over 2 and 3, 2 childless, 3 over 4 and 5 - wired 2 -> 3
    // and 4 -> 5 in this repo's HashMap, where a node with no entry has no next node. The
    // third level is found past childless 2, at 3's left child: 1 #, 2 3 #, 4 5 #.
    [Fact]
    public void ReadLevelsAlongNextPointers_FirstNodeOfALevelChildless_StartsTheNextLevelFurtherRight()
    {
        var four = new BinaryTreeNode<int>(4);
        var five = new BinaryTreeNode<int>(5);
        var two = new BinaryTreeNode<int>(2);
        var three = new BinaryTreeNode<int>(3) { Left = four, Right = five };
        var next = new HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?>();
        next.Set(two, three);
        next.Set(four, five);

        var readout = PopulatingNextRightPointersInEachNodeIISolution.ReadLevelsAlongNextPointers(
            new BinaryTreeNode<int>(1) { Left = two, Right = three },
            new PopulatingNextRightPointersInEachNodeIISolution.HashMapNextPointers(next));

        Assert.Equal([1, null, 2, 3, null, 4, 5, null], readout);
    }

    // The fifth row's chain, 1 -> right 2 -> right 3, one node per level and so no next
    // pointer anywhere - an empty map. Each level is found at the right child, the only
    // child there is: 1 #, 2 #, 3 #.
    [Fact]
    public void ReadLevelsAlongNextPointers_RightLeaningChain_StartsEachLevelAtTheRightChild()
    {
        var two = new BinaryTreeNode<int>(2) { Right = new BinaryTreeNode<int>(3) };
        var noPointers = new HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?>();

        var readout = PopulatingNextRightPointersInEachNodeIISolution.ReadLevelsAlongNextPointers(
            new BinaryTreeNode<int>(1) { Right = two },
            new PopulatingNextRightPointersInEachNodeIISolution.HashMapNextPointers(noPointers));

        Assert.Equal([1, null, 2, null, 3, null], readout);
    }

    private static Dictionary<int, BinaryTreeNode<int>> FirstExampleNodesByValue()
    {
        var four = new BinaryTreeNode<int>(4);
        var five = new BinaryTreeNode<int>(5);
        var seven = new BinaryTreeNode<int>(7);
        var two = new BinaryTreeNode<int>(2) { Left = four, Right = five };
        var three = new BinaryTreeNode<int>(3) { Right = seven };
        var root = new BinaryTreeNode<int>(1) { Left = two, Right = three };

        return new() { [1] = root, [2] = two, [3] = three, [4] = four, [5] = five, [7] = seven };
    }

    // Every node starts with a null next pointer; then each (From, To) pair of values is
    // wired, so the map has an entry for every node, as DictionaryNextPointers expects.
    private static Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?> NextPointersOf(
        Dictionary<int, BinaryTreeNode<int>> nodes, (int From, int To)[] links)
    {
        var next = new Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?>();

        foreach (var node in nodes.Values)
        {
            next[node] = null;
        }

        foreach (var (from, to) in links)
        {
            next[nodes[from]] = nodes[to];
        }

        return next;
    }
}
