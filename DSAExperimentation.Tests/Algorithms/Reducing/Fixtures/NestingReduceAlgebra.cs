using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Reducing.Fixtures;

// Writes the walk out as text: Enter opens "(" plus the node's name and depth, Exit
// closes ")". A non-empty Seed shows where the walk started from, the brackets show
// where every subtree opened and closed, and a breadth-first walk - which never calls
// Exit - leaves every bracket open. One string pins the seed, the visit order, the
// depths and the Enter/Exit pairing at once.
internal readonly struct NestingReduceAlgebra : IReduceAlgebra<TestNode, string>
{
    public static string Seed => "*";

    public static string Enter(string state, TestNode node, int depth) => $"{state}({node.Name}{depth}";

    public static string Exit(string state, TestNode node, int depth) => $"{state})";
}
