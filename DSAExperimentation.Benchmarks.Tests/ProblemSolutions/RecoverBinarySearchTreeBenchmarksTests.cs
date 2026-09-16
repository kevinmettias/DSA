using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.RecoverBinarySearchTree;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RecoverBinarySearchTreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - LeetCode's recoverTree, which rewrites the two
// swapped values in place - so a harness whose arms disagree is timing two different problems.
//
// This is the stateful bucket, and here arm order genuinely matters: both arms return void and
// mutate the single tree the harness holds in a FIELD, so a second arm call on the same instance
// would run against a tree the first call had already sorted. Every arm is therefore replayed on
// its own freshly built tree rather than two arms being compared on one harness.
//
// The oracle is independent of the replay: RecoverBinarySearchTreeWorkloads documents a balanced
// tree over 0..Size-1 with its min and max values swapped, so the recovered in-order traversal has
// to be exactly 0..Size-1, and the corrupted one has to be that sequence with its two ends
// exchanged. Both are derived from the layout here, not read back out of the fixture.
//
// The harness's own tree is private, so the two Setup tests state the workload against the
// expression each method assigns - which is what makes the arm replays below meaningful - rather
// than against a value the harness exposes.
public sealed partial class RecoverBinarySearchTreeBenchmarksTests
{
    private const int SmallestSize = 100;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameCorruptedTree()
    {
        // The arm call proves the harness is constructible; the workload it was handed is stated
        // against the expression [GlobalSetup] builds it from.
        BuildHarness().ManualRecursiveScan();

        Assert.Equal(
            AnswerText.Of(CorruptedInOrder(SmallestSize)),
            AnswerText.Of(InOrderValues(RecoverBinarySearchTreeWorkloads.BuildCorruptedBst(SmallestSize))));
    }

    [Fact]
    public void IterationSetup_SameSize_RebuildsAFreshCorruptedTree()
    {
        // [IterationSetup] exists because one successful recovery leaves the tree sorted, so it
        // reassigns precisely the corrupted workload [GlobalSetup] builds - a full 0..Size-1 tree
        // with its two extremal values swapped - before every iteration. That assignment is what
        // this asserts; the harness's tree itself is private.
        var harness = BuildHarness();
        harness.IterationSetup();

        Assert.Equal(
            AnswerText.Of(CorruptedInOrder(SmallestSize)),
            AnswerText.Of(InOrderValues(RecoverBinarySearchTreeWorkloads.BuildCorruptedBst(SmallestSize))));
    }

    [Fact]
    public void ManualRecursiveScan_SmallestSize_SortsTheCorruptedTree()
    {
        BuildHarness().ManualRecursiveScan();

        var tree = RecoverBinarySearchTreeWorkloads.BuildCorruptedBst(SmallestSize);

        RecoverBinarySearchTreeSolution.RecoverByManualRecursiveScan(tree);

        Assert.Equal(AnswerText.Of(SortedValues(SmallestSize)), AnswerText.Of(InOrderValues(tree)));
    }

    [Fact]
    public void InOrderTraversalHooks_SmallestSize_SortsTheCorruptedTree()
    {
        BuildHarness().InOrderTraversalHooks();

        var tree = RecoverBinarySearchTreeWorkloads.BuildCorruptedBst(SmallestSize);

        RecoverBinarySearchTreeSolution.RecoverByInOrderHooks(tree);

        Assert.Equal(AnswerText.Of(SortedValues(SmallestSize)), AnswerText.Of(InOrderValues(tree)));
    }

    private static int[] SortedValues(int size) => [.. Enumerable.Range(0, size)];

    // The fixture's corruption read off its own documentation - the sorted sequence with its first
    // and last values exchanged - so this oracle is derived rather than read back out of the tree.
    private static int[] CorruptedInOrder(int size)
    {
        var values = SortedValues(size);
        (values[0], values[^1]) = (values[^1], values[0]);

        return values;
    }

    private static int[] InOrderValues(BinaryTreeNode<int>? root)
    {
        var values = new List<int>();
        AppendInOrder(values, root);

        return [.. values];
    }

    private static void AppendInOrder(List<int> values, BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return;
        }

        AppendInOrder(values, node.Left);
        values.Add(node.Value);
        AppendInOrder(values, node.Right);
    }

    private static RecoverBinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new RecoverBinarySearchTreeBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
