using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Walking.Fixtures;

// Records the (name, depth) of every node the engine steps into, in the order it
// steps into them - the walk engines' entire observable behaviour.
internal readonly struct RecordingWalkStep : IReduceAlgebra<TestNode, List<(string Name, int Depth)>>
{
    public static List<(string Name, int Depth)> Seed => [];

    public static List<(string Name, int Depth)> Enter(
        List<(string Name, int Depth)> state, TestNode node, int depth)
    {
        state.Add((node.Name, depth));
        return state;
    }
}
