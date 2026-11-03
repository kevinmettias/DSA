using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.Algorithms.Reducing;

// The search tree 4 -> (2 -> 1, 3), (6 -> 5, 7): in-order reads its keys in ascending order, and the
// depths read off the same walk are 2 1 2 0 2 1 2.
public sealed partial class InOrderReduceOrderTests
{
    [Fact]
    public void Evaluate_SearchTree_EntersTheKeysInAscendingOrder()
        => Assert.Equal("1234567", Reduce.Tree<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            InOrderReduceOrder<int>, KeysInOrder, string>(SearchTree()));

    [Fact]
    public void Evaluate_SearchTree_ReportsEachNodesDepth()
        => Assert.Equal("2120212", Reduce.Tree<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            InOrderReduceOrder<int>, DepthsInOrder, string>(SearchTree()));

    // Done once three keys are in: the walk enters 1, 2 and 3 and never reaches 4's right subtree.
    [Fact]
    public void Evaluate_AlgebraDone_StopsAtTheThirdKey()
        => Assert.Equal("123", Reduce.Tree<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            InOrderReduceOrder<int>, FirstThreeKeys, string>(SearchTree()));

    [Fact]
    public void Evaluate_NullRoot_ReturnsTheSeed()
        => Assert.Equal("", Reduce.Tree<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            InOrderReduceOrder<int>, KeysInOrder, string>(null));

    // A one-sided chain 1 -> right 2 -> right 3 still reads in ascending order with no left children.
    [Fact]
    public void Evaluate_RightOnlyChain_EntersEachNodeBeforeItsRightSubtree()
        => Assert.Equal("123", Reduce.Tree<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            InOrderReduceOrder<int>, KeysInOrder, string>(new BinaryTreeNode<int>(1) { Right = new(2) { Right = new(3) } }));

    // Two roots walked in turn on the graph tier: 2 1 3 from the first, then 4 5 6 from the second.
    [Fact]
    public void EvaluateFrom_TwoRoots_WalksEachInOrderInTurn()
        => Assert.Equal("213456", Reduce.GraphFromAll<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            InOrderReduceOrder<int>, KeysInOrder, string>(
            [new BinaryTreeNode<int>(1) { Left = new(2), Right = new(3) }, new BinaryTreeNode<int>(5) { Left = new(4), Right = new(6) }], ""));

    private static BinaryTreeNode<int> SearchTree()
        => new(4)
        {
            Left = new(2) { Left = new(1), Right = new(3) },
            Right = new(6) { Left = new(5), Right = new(7) },
        };

    private readonly struct KeysInOrder : IReduceAlgebra<BinaryTreeNode<int>, string>
    {
        public static string Seed => "";

        public static string Enter(string state, BinaryTreeNode<int> node, int depth) => state + node.Value;
    }

    private readonly struct DepthsInOrder : IReduceAlgebra<BinaryTreeNode<int>, string>
    {
        public static string Seed => "";

        public static string Enter(string state, BinaryTreeNode<int> node, int depth) => state + depth;
    }

    private readonly struct FirstThreeKeys : IReduceAlgebra<BinaryTreeNode<int>, string>
    {
        private const int KeysWanted = 3;

        public static string Seed => "";

        public static bool StopsEarly => true;

        public static string Enter(string state, BinaryTreeNode<int> node, int depth) => state + node.Value;

        public static bool IsDone(string state) => state.Length >= KeysWanted;
    }
}
