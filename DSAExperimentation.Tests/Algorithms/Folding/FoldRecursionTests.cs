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
    private struct UnmemoizedDiamondMarker;
    private struct MemoizedDiamondMarker;
    private struct CycleCheckedDiamondMarker;
    private struct CycleCheckedCycleMarker;

    // Nothing is remembered, so D is combined under B and again under C - correct for a tree,
    // where no node has two parents, and the reason this policy is reserved for the tree tier.
    [Fact]
    public void Visit_Unmemoized_CombinesASharedDescendantOncePerPath()
    {
        var spelled = Visit<UnmemoizedFold<TestNode, string>, UnmemoizedDiamondMarker>(TestGraphs.Diamond(), default);

        Assert.Equal("ABDCD", spelled);
        Assert.Equal(["D", "B", "D", "C", "A"], RecordingNamesFoldAlgebra<UnmemoizedDiamondMarker>.Combined);
    }

    [Fact]
    public void Visit_Memoized_CombinesASharedDescendantOnce()
    {
        var spelled = Visit<MemoizedFold<TestNode, string>, MemoizedDiamondMarker>(
            TestGraphs.Diamond(), new MemoizedFold<TestNode, string>([]));

        Assert.Equal("ABDCD", spelled);
        Assert.Equal(["D", "B", "C", "A"], RecordingNamesFoldAlgebra<MemoizedDiamondMarker>.Combined);
    }

    // A shared descendant is finished before its second parent reaches it, so the cycle check
    // recalls it instead of mistaking it for a node still on the path.
    [Fact]
    public void Visit_CycleChecked_SharedDescendantIsNotACycle()
    {
        var memo = new CycleCheckedFold<TestNode, string>();

        var spelled = Visit<CycleCheckedFold<TestNode, string>, CycleCheckedDiamondMarker>(TestGraphs.Diamond(), memo);

        Assert.False(memo.Aborted);
        Assert.Equal("ABDCD", spelled);
        Assert.Equal(["D", "B", "C", "A"], RecordingNamesFoldAlgebra<CycleCheckedDiamondMarker>.Combined);
    }

    [Fact]
    public void Visit_CycleChecked_TrueCycleAbortsBeforeCombiningAnything()
    {
        var memo = new CycleCheckedFold<TestNode, string>();

        Visit<CycleCheckedFold<TestNode, string>, CycleCheckedCycleMarker>(TestGraphs.CycleWithLeaf(), memo);

        Assert.True(memo.Aborted);
        Assert.Empty(RecordingNamesFoldAlgebra<CycleCheckedCycleMarker>.Combined);
    }

    private static string Visit<TMemo, TMarker>(TestNode root, TMemo memo)
        where TMemo : struct, IFoldMemo<TestNode, string>
        where TMarker : struct
        => FoldRecursion.Visit<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            TMemo, RecordingNamesFoldAlgebra<TMarker>, string>(root, memo);
}
