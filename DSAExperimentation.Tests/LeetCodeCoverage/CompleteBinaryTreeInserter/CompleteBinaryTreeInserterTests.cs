using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.BinaryTreeNode<int>>;
using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CompleteBinaryTreeInserter;

// LeetCode 919. Complete Binary Tree Inserter: seed a candidate queue of "nodes
// with room for one more child" by walking the existing tree level order via this
// repo's own LevelGroupedBreadthFirstTraversal (PopulatingNextRightPointersInEachNode's
// own precedent for that walk), filtering to nodes missing a Left or Right child;
// each Insert attaches the new node into the front candidate's next open slot and
// enqueues the new (childless) node itself - amortized O(1), no re-walk - using
// this repo's own Queue<T> (ImplementStackUsingQueues' own precedent for the alias
// this needs, since a bare "Queue" reference resolves to the enclosing namespace
// segment before any using directive, per ARCHITECTURE.md §10.3) to hold the
// candidates.
public sealed partial class CompleteBinaryTreeInserterTests
{
    [Fact]
    public void Insert_LeetCodeExampleOne_FillsLeftToRightAndReturnsParentValues()
    {
        var root = new BinaryTreeNode<int>(1) { Left = new(2) };
        var inserter = new CBTInserter(root);

        Assert.Equal(1, inserter.Insert(3));
        Assert.Equal(2, inserter.Insert(4));

        Assert.Same(root, inserter.Root);
        Assert.Equal(3, root.Right!.Value);
        Assert.Equal(4, root.Left!.Left!.Value);
    }

    [Fact]
    public void Insert_LeetCodeExampleTwo_FillsRemainingLevelBeforeStartingANewOne()
    {
        var root = new BinaryTreeNode<int>(1)
        {
            Left = new(2) { Left = new(4), Right = new(5) },
            Right = new(3) { Left = new(6) },
        };
        var inserter = new CBTInserter(root);

        Assert.Equal(3, inserter.Insert(7));
        Assert.Equal(4, inserter.Insert(8));
        Assert.Equal(4, inserter.Insert(9));

        Assert.Equal(7, root.Right!.Right!.Value);
        Assert.Equal(8, root.Left!.Left!.Left!.Value);
        Assert.Equal(9, root.Left!.Left!.Right!.Value);
    }

    private sealed class CBTInserter
    {
        private readonly RepoQueue _incomplete = new();

        public BinaryTreeNode<int> Root { get; }

        public CBTInserter(BinaryTreeNode<int> root)
        {
            Root = root;

            LevelHooks.Output.Value = [];
            LevelGroupedBreadthFirstTraversal.Walk<
                BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
                NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>, LevelHooks>(root);

            foreach (var level in LevelHooks.Output.Value!)
            {
                foreach (var node in level)
                {
                    if (node.Left is null || node.Right is null)
                    {
                        _incomplete.Enqueue(node);
                    }
                }
            }
        }

        public int Insert(int value)
        {
            var node = new BinaryTreeNode<int>(value);
            _incomplete.TryPeek(out var parent);

            if (parent.Left is null)
            {
                parent.Left = node;
            }
            else
            {
                parent.Right = node;
                _incomplete.TryDequeue(out _);
            }

            _incomplete.Enqueue(node);
            return parent.Value;
        }
    }

    private readonly struct LevelHooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public static readonly AsyncLocal<List<List<BinaryTreeNode<int>>>> Output = new();

        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) => Output.Value!.Add(level.ToList());
    }
}
