using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNodeII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PopulatingNextRightPointersInEachNodeII;

// Harness only. Both strategies are
// PopulatingNextRightPointersInEachNodeIISolution's - this file pins them to
// LeetCode's published examples, given in LeetCode's own level-order-with-null
// array shape (BinaryTreeNode<int> is internal, so it cannot appear in a public
// TheoryData signature; BuildTree reconstructs it). Every value in an example tree
// is distinct, so a value can stand in for its node's identity when stating the
// expected next-pointer chain.
public sealed partial class PopulatingNextRightPointersInEachNodeIITests
{
    public static TheoryData<int?[], (int Value, int? NextValue)[]> Examples =>
        new()
        {
            // [1,2,3,4,5,null,7] -> 5's next must "reach across" 3's missing left
            // child to land on 7.
            {
                [1, 2, 3, 4, 5, null, 7],
                [(1, null), (2, 3), (3, null), (4, 5), (5, 7), (7, null)]
            },
            { [], [] },
            { [1], [(1, null)] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectByManualQueueBfs_LeetCodeExamples_LinksEachNodeToItsRightNeighbor(
        int?[] values, (int Value, int? NextValue)[] expectedNext)
    {
        var tree = BuildTree(values);
        var chain = new DictionaryNextPointers(
            PopulatingNextRightPointersInEachNodeIISolution.ConnectByManualQueueBfs(tree.Root));

        AssertLinks(expectedNext, tree, chain);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConnectByLevelGroupedTraversal_LeetCodeExamples_LinksEachNodeToItsRightNeighbor(
        int?[] values, (int Value, int? NextValue)[] expectedNext)
    {
        var tree = BuildTree(values);
        var chain = new HashMapNextPointers(
            PopulatingNextRightPointersInEachNodeIISolution.ConnectByLevelGroupedTraversal(tree.Root));

        AssertLinks(expectedNext, tree, chain);
    }

    private static void AssertLinks(
        (int Value, int? NextValue)[] expectedNext,
        ExampleTree tree,
        NextPointerChain chain)
    {
        foreach (var (value, nextValue) in expectedNext)
        {
            var actual = chain.NextOf(tree.ByValue[value]);
            var expected = ExpectedNextNode(tree, nextValue);

            Assert.Equal(expected, actual);
        }
    }

    // The node an example's next value names, or null where it names none - the end of
    // a level, which is exactly what a null next pointer means.
    private static BinaryTreeNode<int>? ExpectedNextNode(ExampleTree tree, int? nextValue)
    {
        if (nextValue is not int expectedValue)
        {
            return null;
        }

        return tree.ByValue[expectedValue];
    }

    // LeetCode's level-order array shape, turned into the root a strategy is handed and
    // its nodes indexed by value.
    private static ExampleTree BuildTree(int?[] values) => new ExampleTreeBuilder(values).Build();

    // The next-pointer chain one strategy produced, read as a lookup from a node to the
    // node to its right, or null at the end of that node's level. A named strategy type
    // rather than a bare Func: the method says what is being asked for, and the contract
    // below has somewhere to live.
    private abstract class NextPointerChain
    {
        // Null is the end of a level, so a strategy that skipped a node must not be able
        // to read as one: a missing entry is the strategy breaking the contract every
        // implementation here is written against, which is what a guard reports.
        public BinaryTreeNode<int>? NextOf(BinaryTreeNode<int> node)
        {
            if (TryNextOf(node, out var nextNode))
            {
                return nextNode;
            }

            throw new InvalidOperationException(
                "the strategy returned no next pointer for a node of the tree it was given");
        }

        protected abstract bool TryNextOf(BinaryTreeNode<int> node, out BinaryTreeNode<int>? nextNode);
    }

    // ConnectByManualQueueBfs's result, keyed by the node each pointer belongs to.
    private sealed class DictionaryNextPointers(Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?> next)
        : NextPointerChain
    {
        protected override bool TryNextOf(BinaryTreeNode<int> node, out BinaryTreeNode<int>? nextNode)
            => next.TryGetValue(node, out nextNode);
    }

    // ConnectByLevelGroupedTraversal's result: the same try-get surface, on this
    // repository's own HashMap rather than a Dictionary.
    private sealed class HashMapNextPointers(HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?> next)
        : NextPointerChain
    {
        protected override bool TryNextOf(BinaryTreeNode<int> node, out BinaryTreeNode<int>? nextNode)
            => next.TryGetValue(node, out nextNode);
    }

    // LeetCode's level-order array shape: each existing node consumes exactly two
    // subsequent slots for its children, null marking a missing one. The array and the
    // two collections built from it are the builder's own state rather than parameters
    // threaded through every call.
    private sealed class ExampleTreeBuilder(int?[] values)
    {
        private readonly Dictionary<int, BinaryTreeNode<int>> _byValue = [];
        private readonly Queue<BinaryTreeNode<int>> _pending = new();

        public ExampleTree Build()
        {
            if (values.Length == 0 || values[0] is null)
            {
                return new ExampleTree(Root: null, ByValue: _byValue);
            }

            var root = SeedWithRoot(values[0].Value);
            ConsumeRemainingValues();

            return new ExampleTree(root, _byValue);
        }

        // The first present value is the root, and the queue of nodes still waiting for
        // children starts with it alone.
        private BinaryTreeNode<int> SeedWithRoot(int rootValue)
        {
            var root = new BinaryTreeNode<int>(rootValue);
            _byValue[root.Value] = root;
            _pending.Enqueue(root);

            return root;
        }

        private void ConsumeRemainingValues()
        {
            var index = 1;

            while (_pending.Count > 0 && index < values.Length)
            {
                index = AttachChildren(_pending.Dequeue(), index);
            }
        }

        // The two slots following a node: a present value becomes that child, and the
        // index returned is the first slot the next node has not yet consumed.
        private int AttachChildren(BinaryTreeNode<int> node, int index)
        {
            if (index < values.Length && values[index] is int leftValue)
            {
                node.Left = new BinaryTreeNode<int>(leftValue);
                _byValue[leftValue] = node.Left;
                _pending.Enqueue(node.Left);
            }

            index++;

            if (index < values.Length && values[index] is int rightValue)
            {
                node.Right = new BinaryTreeNode<int>(rightValue);
                _byValue[rightValue] = node.Right;
                _pending.Enqueue(node.Right);
            }

            return index + 1;
        }
    }

    // A parsed example tree: the root a strategy is handed, and its nodes indexed by the
    // value that stands in for each node's identity.
    private readonly record struct ExampleTree(
        BinaryTreeNode<int>? Root,
        Dictionary<int, BinaryTreeNode<int>> ByValue);
}
