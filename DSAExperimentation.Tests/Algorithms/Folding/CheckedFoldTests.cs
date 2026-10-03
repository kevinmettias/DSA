using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Tests.Algorithms.Folding.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding;

public sealed partial class CheckedFoldTests
{
    private struct NullRootMarker;
    private struct TreeMarker;
    private struct DiamondMarker;
    private struct DiamondEnterMarker;

    [Fact]
    public void TryFold_TrueCycle_ReturnsFalseInsteadOfThrowing()
    {
        var root = TestGraphs.CycleWithLeaf();

        var succeeded = CheckedFold.TryFold<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            CountNodesFoldAlgebra, int>(root, out _);

        Assert.False(succeeded);
    }

    [Fact]
    public void TryFold_NullRoot_SucceedsWithEmpty()
    {
        var succeeded = TryFold<NullRootMarker>(null, out var spelled);

        Assert.True(succeeded);
        Assert.Equal(RecordingNamesFoldAlgebra<NullRootMarker>.Empty, spelled);
    }

    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void TryFold_Tree_CombinesEachNodeWithItsChildrenInOrder()
    {
        var succeeded = TryFold<TreeMarker>(TestTrees.NArySample(), out var spelled);

        Assert.True(succeeded);
        Assert.Equal("ABEFCDG", spelled);
    }

    [Fact]
    public void TryFold_SharedDescendant_IsNotMistakenForACycle()
    {
        // D is reached a second time through C only after B's visit has finished with
        // it, so it is memoized rather than in progress: no cycle, and its result appears
        // under both parents.
        var succeeded = TryFold<DiamondMarker>(TestGraphs.Diamond(), out var spelled);

        Assert.True(succeeded);
        Assert.Equal("ABDCD", spelled);
    }

    [Fact]
    public void TryFold_SharedDescendant_IsEnteredOnceAtTheDepthFirstReached()
    {
        var succeeded = TryFold<DiamondEnterMarker>(TestGraphs.Diamond(), out _);

        Assert.True(succeeded);
        Assert.Equal(
            new[] { ("A", 0), ("B", 1), ("D", 2), ("C", 1) },
            RecordingNamesFoldAlgebra<DiamondEnterMarker>.Entered);
    }

    private static bool TryFold<TMarker>(TestNode? root, out string spelled)
        where TMarker : struct
        => CheckedFold.TryFold<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingNamesFoldAlgebra<TMarker>, string>(root, out spelled);

    // VisitOutcome is private to CheckedFold; its Failure is what every level of the
    // recursion hands back once a cycle is found, and TryFold's false is where it
    // surfaces.
    public sealed partial class VisitOutcomeTests
    {
        private struct CycleBelowRootMarker;
        private struct SelfLoopMarker;
        private struct ShortCircuitMarker;
        private struct ShortCircuitCombineMarker;

        [Fact]
        public void Failure_CycleBelowTheRoot_PropagatesUpToTryFold()
        {
            // A -> B -> C -> B: the root is not on the cycle, so the failure has to
            // travel up through A's own visit to reach TryFold.
            var a = new TestNode("A");
            var b = new TestNode("B");
            var c = new TestNode("C");
            a.Children.Add(b);
            b.Children.Add(c);
            c.Children.Add(b);

            var succeeded = TryFold<CycleBelowRootMarker>(a, out _);

            Assert.False(succeeded);
        }

        [Fact]
        public void Failure_SelfLoop_IsACycle()
        {
            var a = new TestNode("A");
            a.Children.Add(a);

            var succeeded = TryFold<SelfLoopMarker>(a, out _);

            Assert.False(succeeded);
        }

        [Fact]
        public void Failure_StopsTheWalk_SoALaterSiblingIsNeverEntered()
        {
            // A's children are B (which leads round the cycle) then D. Once B's branch
            // fails, A gives up instead of moving on to D.
            var root = TestGraphs.CycleWithLeaf();

            TryFold<ShortCircuitMarker>(root, out _);

            Assert.Equal(
                new[] { ("A", 0), ("B", 1), ("C", 2) },
                RecordingNamesFoldAlgebra<ShortCircuitMarker>.Entered);
        }

        [Fact]
        public void Failure_NothingOnTheCyclicPathIsCombined()
        {
            var root = TestGraphs.CycleWithLeaf();

            CheckedFold.TryFold<
                TestNode, TestTopology, ListChildren<TestNode>,
                NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
                CountCombineCallsFoldAlgebra<ShortCircuitCombineMarker>, int>(root, out _);

            Assert.Equal(0, CountCombineCallsFoldAlgebra<ShortCircuitCombineMarker>.CombineCalls);
        }
    }
}
