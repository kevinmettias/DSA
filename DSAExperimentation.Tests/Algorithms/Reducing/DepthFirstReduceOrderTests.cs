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
public sealed partial class DepthFirstReduceOrderTests
{
    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Evaluate_StartsFromSeedAtDepthZeroAndClosesEachSubtreeBeforeTheNext()
    {
        var root = TestTrees.NArySample();

        var walk = Evaluate(root, default(UnguardedVisit<TestNode>));

        Assert.Equal("*(A0(B1(E2)(F2))(C1)(D1(G2)))", walk);
    }

    [Fact]
    public void Evaluate_DoesNotDescendIntoAChildTheGuardRejects()
    {
        // B is already in the set, so neither B nor anything below it is walked.
        var root = TestTrees.NArySample();
        var b = root.Children[0];

        var walk = Evaluate(root, new TrackedVisitGuard<TestNode>([root, b]));

        Assert.Equal("*(A0(C1)(D1(G2)))", walk);
    }

    private static string Evaluate<TGuard>(TestNode root, TGuard guard)
        where TGuard : struct, IVisitGuard<TestNode>
        => DepthFirstReduceOrder<TestNode>.Evaluate<
            TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            TGuard, NestingReduceAlgebra, string>(root, NestingReduceAlgebra.Seed, guard);
}
