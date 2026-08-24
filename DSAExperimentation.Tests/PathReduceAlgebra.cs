using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

// Concatenates node names in visit order - deliberately non-commutative, so it can
// demonstrate that reduce (unlike fold) actually depends on traversal order.
public readonly struct PathReduceAlgebra : IReduceAlgebra<TestNode, string>
{
    public static string Seed => "";

    public static string Enter(string state, TestNode node, int depth) => state + node.Name;
}
