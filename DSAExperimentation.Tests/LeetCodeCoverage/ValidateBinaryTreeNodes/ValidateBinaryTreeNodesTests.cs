using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidateBinaryTreeNodes;

// LeetCode 1361. Validate Binary Tree Nodes: reuses this repo's own DisjointSet the
// same way RedundantConnectionIITests does - every leftChild[i]/rightChild[i] edge
// must land on a node with no parent yet (indegree check) and must not already share
// a component with i (RedundantConnectionTests' own cycle check), and the structure
// is a single rooted tree only if exactly one node ends up parentless and every node
// lands in one shared component.
public sealed partial class ValidateBinaryTreeNodesTests
{
    [Fact]
    public void ValidateBinaryTreeNodes_ClassicValidTree_ReturnsTrue()
    {
        var isValid = ValidateBinaryTreeNodes(4, [1, -1, 3, -1], [2, -1, -1, -1]);

        Assert.True(isValid);
    }

    [Fact]
    public void ValidateBinaryTreeNodes_TwoRoots_ReturnsFalse()
    {
        var isValid = ValidateBinaryTreeNodes(4, [1, -1, 3, -1], [-1, -1, -1, -1]);

        Assert.False(isValid);
    }

    [Fact]
    public void ValidateBinaryTreeNodes_NodeHasTwoParents_ReturnsFalse()
    {
        var isValid = ValidateBinaryTreeNodes(3, [2, 2, -1], [-1, -1, -1]);

        Assert.False(isValid);
    }

    [Fact]
    public void ValidateBinaryTreeNodes_CycleAmongNonRootNodes_ReturnsFalse()
    {
        var isValid = ValidateBinaryTreeNodes(3, [1, 2, 0], [-1, -1, -1]);

        Assert.False(isValid);
    }

    private static bool ValidateBinaryTreeNodes(int n, int[] leftChild, int[] rightChild)
    {
        var hasParent = new bool[n];
        var components = new DisjointSet(n);

        for (var node = 0; node < n; node++)
        {
            foreach (var child in new[] { leftChild[node], rightChild[node] })
            {
                if (child == -1)
                {
                    continue;
                }

                if (hasParent[child] || components.IsConnected(node, child))
                {
                    return false;
                }

                hasParent[child] = true;
                components.Union(node, child);
            }
        }

        return HasExactlyOneRoot(hasParent) && AllNodesShareOneComponent(components, n);
    }

    private static bool HasExactlyOneRoot(bool[] hasParent) => hasParent.Count(parented => !parented) == 1;

    private static bool AllNodesShareOneComponent(DisjointSet components, int n)
    {
        for (var node = 1; node < n; node++)
        {
            if (!components.IsConnected(0, node))
            {
                return false;
            }
        }

        return true;
    }
}
