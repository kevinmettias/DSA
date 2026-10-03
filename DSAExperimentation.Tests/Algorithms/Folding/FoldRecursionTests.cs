using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.Folding.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding;

// The one recursion every recursive fold tier runs, driven directly with each memo policy so the
// axis that distinguishes the tiers is visible on its own: over the diamond A -> [B, C], B -> [D],
// C -> [D], only the memoizing policies combine the shared D once.
public sealed partial class FoldRecursionTests
{
    // Nothing is remembered, so D is combined under B and again under C - correct for a tree,
    // where no node has two parents, and the reason this policy is reserved for the tree tier.
    [Fact]
    public void Visit_Unmemoized_CombinesASharedDescendantOncePerPath()
    {
        var combined = new List<string>();

        var spelled = Visit<UnmemoizedFold<TestNode, string>>(TestGraphs.Diamond(), default, combined);

        Assert.Equal("ABDCD", spelled);
        Assert.Equal(["D", "B", "D", "C", "A"], combined);
    }

    [Fact]
    public void Visit_Memoized_CombinesASharedDescendantOnce()
    {
        var combined = new List<string>();

        var spelled = Visit(TestGraphs.Diamond(), new MemoizedFold<TestNode, string>([]), combined);

        Assert.Equal("ABDCD", spelled);
        Assert.Equal(["D", "B", "C", "A"], combined);
    }

    // A shared descendant is finished before its second parent reaches it, so the cycle check
    // recalls it instead of mistaking it for a node still on the path.
    [Fact]
    public void Visit_CycleChecked_SharedDescendantIsNotACycle()
    {
        var combined = new List<string>();
        var memo = new CycleCheckedFold<TestNode, string>();

        var spelled = Visit(TestGraphs.Diamond(), memo, combined);

        Assert.False(memo.Aborted);
        Assert.Equal("ABDCD", spelled);
        Assert.Equal(["D", "B", "C", "A"], combined);
    }

    [Fact]
    public void Visit_CycleChecked_TrueCycleAbortsBeforeCombiningAnything()
    {
        var combined = new List<string>();
        var memo = new CycleCheckedFold<TestNode, string>();

        Visit(TestGraphs.CycleWithLeaf(), memo, combined);

        Assert.True(memo.Aborted);
        Assert.Empty(combined);
    }

    private static string Visit<TMemo>(TestNode root, TMemo memo, List<string> combined)
        where TMemo : struct, IFoldMemo<TestNode, string>
        => FoldRecursion.Visit<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            TMemo, RecordingNamesFoldAlgebra, string>(root, memo, new RecordingNamesFoldAlgebra(combined));
}
