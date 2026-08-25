using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Reducing.Fixtures;

internal readonly struct CountNodesReduceAlgebra : IReduceAlgebra<TestNode, int>
{
    public static int Seed => 0;

    public static int Enter(int state, TestNode node, int depth) => state + 1;
}
