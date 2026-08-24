using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

public readonly struct CountWeightedNodesReduceAlgebra : IReduceAlgebra<WeightedNode, int>
{
    public static int Seed => 0;

    public static int Enter(int state, WeightedNode node, int depth) => state + 1;
}
