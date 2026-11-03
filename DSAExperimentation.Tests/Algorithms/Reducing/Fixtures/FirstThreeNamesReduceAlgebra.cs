using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Reducing.Fixtures;

// Records each Enter as the node's name and each Exit as ")", and is done once three names are in:
// a walk that honours IsDone leaves exactly three names and no ")" from any node still open.
internal readonly struct FirstThreeNamesReduceAlgebra : IReduceAlgebra<TestNode, string>
{
    private const int NamesWanted = 3;

    public static string Seed => "";

    public static bool StopsEarly => true;

    public static string Enter(string state, TestNode node, int depth) => $"{state}{node.Name}";

    public static string Exit(string state, TestNode node, int depth) => $"{state})";

    public static bool IsDone(string state) => state.Count(char.IsLetter) >= NamesWanted;
}
