using DSAExperimentation.Graph.Engines.Reducing;
using DSAExperimentation.Tests.Fixtures;

namespace DSAExperimentation.Tests.Engines.Fixtures;

// Concatenates node names in visit order - deliberately non-commutative, so it can
// demonstrate that reduce (unlike fold) actually depends on traversal order.
internal readonly struct PathReduceAlgebra : IReduceAlgebra<TestNode, string>
{
    public static string Seed => "";

    public static string Enter(string state, TestNode node, int depth) => state + node.Name;
}
