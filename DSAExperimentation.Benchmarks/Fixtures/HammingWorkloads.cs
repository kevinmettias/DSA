using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for the one-character-mutation family (LC 126/127/433)
// - everything about the graph itself now lives in DataStructures.Graph.Hamming, so what stays
// here is only how the input set is generated.
internal static class HammingWorkloads
{
    // A chain of `count` single-character mutations from a fixed first value, so a
    // real shortest transformation sequence to the chain's last value always
    // exists and every strategy does genuine BFS work instead of failing fast.
    // The pairwise scan HammingGraph.Build runs afterward then finds every edge,
    // not just the ones the chain happened to walk - the result is a random
    // cluster in the Hamming graph over alphabet^length, not one bare path.
    public static (string[] Values, string First, string Last) BuildChain(
        int count, int length, Alphabet alphabet, int seed)
    {
        var random = new Random(seed);
        var characters = alphabet.Characters;
        var current = new string(characters[0], length).ToCharArray();
        var values = new List<string> { new(current) };

        for (var i = 1; i < count; i++)
        {
            current[random.Next(length)] = characters[random.Next(characters.Length)];
            values.Add(new string(current));
        }

        return ([.. values], values[0], values[^1]);
    }
}
