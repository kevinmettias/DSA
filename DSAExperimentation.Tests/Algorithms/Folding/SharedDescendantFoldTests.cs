using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags;
using DSAExperimentation.Tests.Algorithms.Folding.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Folding;

public sealed partial class SharedDescendantFoldTests
{
    // A -> B, A -> C, B -> D, C -> D: D is a shared descendant, not a cycle -
    // shared by DagFold and CheckedFold's tests below.
    private static TestNode DiamondSample()
    {
        var d = new TestNode("D");
        var b = new TestNode("B");
        b.Children.Add(d);
        var c = new TestNode("C");
        c.Children.Add(d);
        var a = new TestNode("A");
        a.Children.Add(b);
        a.Children.Add(c);
        return a;
    }

    [Fact]
    public void CheckedFold_SharedDescendant_IsCombinedOnce()
    {
        var a = DiamondSample();
        var counter = new CombineCounter();

        var succeeded = CheckedFold.TryFold<
            TestNode, TestTopology, ListChildren<TestNode>,
            CountCombineCallsFoldAlgebra, int>(a, new CountCombineCallsFoldAlgebra(counter), out var count);

        // The *value* still double-counts D (5: A=1 + B's count-of-2 + C's
        // count-of-2, since D is genuinely reachable via both paths) -
        // memoization is a performance guarantee, not a semantic one; it doesn't
        // change what a "count nodes" formula means when a node has two parents.
        // What it guarantees is that Combine only ever *executes* once per
        // distinct node - D's second use reads the cached result instead of
        // recomputing, so the call count is 4, not 5.
        Assert.True(succeeded);
        Assert.Equal(5, count);
        Assert.Equal(4, counter.Calls);
    }

    [Fact]
    public void DagFold_SharedDescendant_IsCombinedOnce()
    {
        // TestTopology promises ITreeTopology, which - now that ITreeTopology
        // extends IDagTopology - is automatically usable wherever IDagTopology is
        // expected. Same diamond, same result as CheckedFold above, just via the
        // trusted (unchecked) path that IDagTopology unlocks.
        var a = DiamondSample();
        var counter = new CombineCounter();

        var count = DagFold.Fold<
            TestNode, TestTopology, ListChildren<TestNode>,
            CountCombineCallsFoldAlgebra, int>(a, new CountCombineCallsFoldAlgebra(counter));

        Assert.Equal(5, count);
        Assert.Equal(4, counter.Calls);
    }
}
