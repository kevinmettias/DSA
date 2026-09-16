using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.ValidateBinaryTreeNodes;

// LeetCode 1361. Validate Binary Tree Nodes: given leftChild[i]/rightChild[i]
// (-1 for "no child"), decide whether the nodeCount nodes form exactly one binary
// tree.
//
// The baseline is the textbook "try every node as the root and re-traverse from
// scratch" scan; the composed strategy reuses this repo's own DisjointSet the same
// way RedundantConnectionII does - every edge must land on a node with no parent
// yet (an indegree check) and must not already share a component with its parent (a
// cycle check), and the structure is a single rooted tree only if exactly one node
// ends up parentless and every node lands in one shared component.
internal static class ValidateBinaryTreeNodesSolution
{
    // LeetCode encodes "this node has no child on that side" as -1.
    private const int NoChild = -1;

    // The textbook answer: no union-find, just a depth-first sweep from each
    // candidate root until one covers every node without revisiting any - O(n) per
    // candidate, O(n^2) overall. Deliberately plain BCL (bool[], Stack<int>); it is
    // the arm the DisjointSet strategy below has to justify itself against.
    public static bool IsValidByRootScan(int nodeCount, int[] leftChild, int[] rightChild)
    {
        for (var root = 0; root < nodeCount; root++)
        {
            if (HasFullCoverageFrom(root, nodeCount, leftChild, rightChild))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasFullCoverageFrom(int root, int nodeCount, int[] leftChild, int[] rightChild)
    {
        var visited = new bool[nodeCount];
        var stack = new Stack<int>();
        stack.Push(root);
        visited[root] = true;
        var visitedCount = 1;

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (!TryVisitChild(leftChild[node], visited, stack, ref visitedCount) ||
                !TryVisitChild(rightChild[node], visited, stack, ref visitedCount))
            {
                return false;
            }
        }

        return visitedCount == nodeCount;
    }

    private static bool TryVisitChild(int child, bool[] visited, Stack<int> stack, ref int visitedCount)
    {
        if (child == NoChild)
        {
            return true;
        }

        if (visited[child])
        {
            return false;
        }

        visited[child] = true;
        visitedCount++;
        stack.Push(child);

        return true;
    }

    // One O(n * alpha(n)) pass over the declared edges: DisjointSet answers "would
    // this edge close a cycle?", a parent flag answers "does this child already have
    // a parent?", and the two end conditions answer "is what is left a single tree?".
    public static bool IsValidByDisjointSet(int nodeCount, int[] leftChild, int[] rightChild)
    {
        var hasParent = new bool[nodeCount];
        var components = new DisjointSet(nodeCount);

        for (var node = 0; node < nodeCount; node++)
        {
            if (!TryLinkChild(node, leftChild[node], hasParent, components) ||
                !TryLinkChild(node, rightChild[node], hasParent, components))
            {
                return false;
            }
        }

        return HasExactlyOneRoot(hasParent) && HasAllNodesInOneComponent(components, nodeCount);
    }

    private static bool TryLinkChild(int node, int child, bool[] hasParent, DisjointSet components)
    {
        if (child == NoChild)
        {
            return true;
        }

        if (hasParent[child] || components.IsConnected(node, child))
        {
            return false;
        }

        hasParent[child] = true;
        components.Union(node, child);

        return true;
    }

    private static bool HasExactlyOneRoot(bool[] hasParent) => hasParent.Count(parented => !parented) == 1;

    private static bool HasAllNodesInOneComponent(DisjointSet components, int nodeCount)
    {
        for (var node = 1; node < nodeCount; node++)
        {
            if (!components.IsConnected(0, node))
            {
                return false;
            }
        }

        return true;
    }
}
