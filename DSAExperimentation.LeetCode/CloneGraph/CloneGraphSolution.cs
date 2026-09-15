using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.CloneGraph;

// LeetCode 133. Clone Graph: given a reference to a node in a connected
// undirected graph, return a deep copy - every node and edge duplicated, none
// shared with the original.
//
// Both strategies are the same depth-first walk with a visited-to-clone memo so
// each node is copied exactly once and cycles terminate instead of recursing
// forever; they differ only in which map holds that memo. The pre-migration
// benchmark's two [Benchmark] arms (Baseline, PrimitiveComposed) were both
// unimplemented placeholders (`return 1;`) that never called either walk, so
// the naive-BCL-baseline arm below is new - the composed HashMap walk is the
// one that used to live as CloneGraphTests' own private helper.
internal static class CloneGraphSolution
{
    // The textbook DFS: a BCL Dictionary as the original-to-clone memo.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed strategy below has to justify itself against.
    public static Node? CloneByDictionaryDfs(Node? node)
    {
        if (node is null)
        {
            return null;
        }

        return CopyByDictionary(node, new Dictionary<Node, Node>());
    }

    // The same DFS composed over this repo's own HashMap as the memo.
    public static Node? CloneByHashMapDfs(Node? node)
    {
        if (node is null)
        {
            return null;
        }

        return CopyByHashMap(node, new HashMap<Node, Node>());
    }

    private static Node CopyByDictionary(Node node, Dictionary<Node, Node> clones)
    {
        if (clones.TryGetValue(node, out var existing))
        {
            return existing;
        }

        var clone = new Node(node.Value);
        clones[node] = clone;

        foreach (var neighbor in node.Neighbors)
        {
            var neighborClone = CopyByDictionary(neighbor, clones);
            clone.Neighbors.Add(neighborClone);
        }

        return clone;
    }

    private static Node CopyByHashMap(Node node, HashMap<Node, Node> clones)
    {
        if (clones.TryGetValue(node, out var existing))
        {
            return existing;
        }

        var clone = new Node(node.Value);
        clones.Set(node, clone);

        foreach (var neighbor in node.Neighbors)
        {
            var neighborClone = CopyByHashMap(neighbor, clones);
            clone.Neighbors.Add(neighborClone);
        }

        return clone;
    }
}
