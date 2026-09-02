using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Validate Binary Tree Nodes (LC 1361): the textbook approach that tries every node
// as a candidate root and re-traverses from scratch until one traversal covers every
// node (O(n) per candidate, O(n^2) overall in the worst case) against this repo's own
// DisjointSet-based approach, which finds the sole indegree-0 root and validates the
// whole structure in one O(n * alpha(n)) union pass - the same "naive re-validate
// from scratch vs. one DisjointSet pass" shape RedundantConnectionIIBenchmarks uses
// for LC 685.
[MemoryDiagnoser]
public class ValidateBinaryTreeNodesBenchmarks
{
    [Params(200, 5_000)]
    public int NodeCount;

    private int[] _leftChild = null!;
    private int[] _rightChild = null!;

    [GlobalSetup]
    public void Setup()
    {
        // A left-leaning chain n-1 -> n-2 -> ... -> 0, rooted at the LAST index -
        // every candidate root the naive scan tries before reaching it still walks a
        // real (shrinking) prefix of the chain instead of failing in O(1), which is
        // what forces the naive strategy through its full O(n^2), not just O(n).
        var leftChild = new int[NodeCount];
        var rightChild = new int[NodeCount];
        Array.Fill(leftChild, -1);
        Array.Fill(rightChild, -1);

        for (var i = 1; i < NodeCount; i++)
        {
            leftChild[i] = i - 1;
        }

        _leftChild = leftChild;
        _rightChild = rightChild;
    }

    [Benchmark(Baseline = true)]
    public bool NaiveRootScan() => ValidateByRootScan(NodeCount, _leftChild, _rightChild);

    [Benchmark]
    public bool DisjointSetOnePass() => ValidateByDisjointSet(NodeCount, _leftChild, _rightChild);

    private static bool ValidateByRootScan(int n, int[] leftChild, int[] rightChild)
    {
        for (var root = 0; root < n; root++)
        {
            if (CoversAllNodesFrom(root, n, leftChild, rightChild))
            {
                return true;
            }
        }

        return false;
    }

    private static bool CoversAllNodesFrom(int root, int n, int[] leftChild, int[] rightChild)
    {
        var visited = new bool[n];
        var stack = new Stack<int>();
        stack.Push(root);
        visited[root] = true;
        var visitedCount = 1;

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            foreach (var child in new[] { leftChild[node], rightChild[node] })
            {
                if (!TryVisitChild(child, visited, stack, ref visitedCount))
                {
                    return false;
                }
            }
        }

        return visitedCount == n;
    }

    private static bool TryVisitChild(int child, bool[] visited, Stack<int> stack, ref int visitedCount)
    {
        if (child == -1)
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

    private static bool ValidateByDisjointSet(int n, int[] leftChild, int[] rightChild)
    {
        var hasParent = new bool[n];
        var components = new DisjointSet(n);

        for (var node = 0; node < n; node++)
        {
            foreach (var child in new[] { leftChild[node], rightChild[node] })
            {
                if (!TryAttachChild(node, child, hasParent, components))
                {
                    return false;
                }
            }
        }

        return hasParent.Count(parented => !parented) == 1;
    }

    private static bool TryAttachChild(int node, int child, bool[] hasParent, DisjointSet components)
    {
        if (child == -1)
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
}
