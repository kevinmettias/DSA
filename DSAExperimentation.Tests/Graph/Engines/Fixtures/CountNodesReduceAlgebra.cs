using DSAExperimentation.Algorithms.Graph.Engines.Reducing;
using DSAExperimentation.Tests.Graph.Fixtures;

namespace DSAExperimentation.Tests.Graph.Engines.Fixtures;

internal readonly struct CountNodesReduceAlgebra : IReduceAlgebra<TestNode, int>
{
    public static int Seed => 0;

    public static int Enter(int state, TestNode node, int depth) => state + 1;
}
