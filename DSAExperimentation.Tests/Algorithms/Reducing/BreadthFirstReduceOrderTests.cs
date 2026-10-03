using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Algorithms.Walking;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.Reducing.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Reducing;

// Evaluate is called directly here, with the guard handed in, rather than through
// Reduce.Tree/Graph: which guard to build is Reduce's decision, while starting the walk
// from the algebra's Seed at depth zero and honouring whatever guard it is given is the
// strategy's.
public sealed partial class BreadthFirstReduceOrderTests
{
    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Evaluate_StartsFromSeedAtDepthZeroAndEntersLevelByLevel()
    {
        var root = TestTrees.NArySample();

        var walk = Evaluate(root, default(UnguardedVisit<TestNode>));

        Assert.Equal("*(A0(B1(C1(D1(E2(F2(G2", walk);
    }

    [Fact]
    public void Evaluate_NeverCallsExit()
    {
        // A breadth-first walk has no moment at which a subtree is finished, so the
        // algebra's Exit - which would close a bracket - is never reached.
        var root = TestTrees.NArySample();

        var walk = Evaluate(root, default(UnguardedVisit<TestNode>));

        Assert.DoesNotContain(")", walk);
    }

    [Fact]
    public void Evaluate_DoesNotExpandAChildTheGuardRejects()
    {
        // B is already in the set, so B is never entered and its children E and F never
        // join the next level.
        var root = TestTrees.NArySample();
        var b = root.Children[0];

        var walk = Evaluate(root, new TrackedVisitGuard<TestNode>([root, b]));

        Assert.Equal("*(A0(C1(D1(G2", walk);
    }

    private static string Evaluate<TGuard>(TestNode root, TGuard guard)
        where TGuard : struct, IVisitGuard<TestNode>
        => BreadthFirstReduceOrder<TestNode>.Evaluate<
            TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            TGuard, NestingReduceAlgebra, string>(root, NestingReduceAlgebra.Seed, guard);
}
