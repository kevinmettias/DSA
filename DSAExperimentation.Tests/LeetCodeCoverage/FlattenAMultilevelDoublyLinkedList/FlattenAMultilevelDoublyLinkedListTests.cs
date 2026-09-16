using DSAExperimentation.LeetCode.FlattenAMultilevelDoublyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FlattenAMultilevelDoublyLinkedList;

// Harness only. Both strategies are FlattenAMultilevelDoublyLinkedListSolution's - this
// file just pins them to LeetCode's published examples, asserting the depth-first order,
// that every Child pointer is gone, and that Previous is rebuilt consistently throughout -
// the three things the pre-migration test checked across three separate Facts, now
// checked together for every example against every strategy so a failure names the
// strategy that broke.
public sealed class FlattenAMultilevelDoublyLinkedListTests
{
    // Each level is (ParentLevel, ParentIndex, Values): ParentLevel < 0 marks the top level
    // (its first node is the list head); otherwise Values' first node becomes the Child of
    // levels[ParentLevel]'s node at ParentIndex. Plain tuples/arrays only, so Examples stays
    // a public member without exposing the internal Node type through its signature - the
    // graph itself is built privately in BuildMultilevelList, same as MinStackTests keeps
    // MinStackSolution.MinStackOperations out of its own public signatures.
    public static TheoryData<(int ParentLevel, int ParentIndex, int[] Values)[], int[]> Examples =>
        new()
        {
            {
                [(-1, -1, [1, 2, 3]), (0, 1, [4, 5]), (1, 0, [6, 7])],
                [1, 2, 4, 6, 7, 5, 3]
            },
            {
                [(-1, -1, [1, 2])],
                [1, 2]
            },
            {
                [(-1, -1, [1, 2]), (0, 1, [3])],
                [1, 2, 3]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FlattenByStack_LeetCodeExamples_ProducesDepthFirstOrderWithPointersConsistent(
        (int ParentLevel, int ParentIndex, int[] Values)[] levels, int[] expected) =>
        AssertFlattened(FlattenAMultilevelDoublyLinkedListSolution.FlattenByStack(BuildMultilevelList(levels)), expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void FlattenByBruteForceRescan_LeetCodeExamples_ProducesDepthFirstOrderWithPointersConsistent(
        (int ParentLevel, int ParentIndex, int[] Values)[] levels, int[] expected) =>
        AssertFlattened(FlattenAMultilevelDoublyLinkedListSolution.FlattenByBruteForceRescan(BuildMultilevelList(levels)), expected);

    private static void AssertFlattened(Node? head, int[] expected)
    {
        var nodes = ToNodes(head);

        Assert.Equal(expected, nodes.Select(node => node.Val));
        Assert.All(nodes, node => Assert.Null(node.Child));

        for (var i = 1; i < nodes.Count; i++)
        {
            Assert.Same(nodes[i - 1], nodes[i].Previous);
        }
    }

    private static List<Node> ToNodes(Node? head)
    {
        var nodes = new List<Node>();

        for (var current = head; current is not null; current = current.Next)
        {
            nodes.Add(current);
        }

        return nodes;
    }

    private static Node BuildMultilevelList((int ParentLevel, int ParentIndex, int[] Values)[] levels)
    {
        var levelNodes = new List<Node>[levels.Length];
        Node? head = null;

        for (var levelIndex = 0; levelIndex < levels.Length; levelIndex++)
        {
            AttachLevel(levels[levelIndex], levelIndex, levelNodes, ref head);
        }

        // Every Examples row opens with a top level (ParentLevel < 0), and that is the
        // only branch in AttachLevel that assigns head - so one was always installed.
        return head
            ?? throw new InvalidOperationException(
                "every Examples row opens with a top level (ParentLevel < 0), which is the branch that assigns head");
    }

    // Consumes one Examples level: materializes its nodes, links them, and wires the level
    // into the graph - the top level (ParentLevel < 0) installs head, and every other level
    // hangs off its parent's node as that node's Child.
    private static void AttachLevel(
        (int ParentLevel, int ParentIndex, int[] Values) level,
        int levelIndex,
        List<Node>[] levelNodes,
        ref Node? head)
    {
        var (parentLevel, parentIndex, values) = level;
        var nodes = values.Select(value => new Node(value)).ToList();
        Link([.. nodes]);
        levelNodes[levelIndex] = nodes;

        if (parentLevel < 0)
        {
            head = nodes[0];
        }
        else
        {
            levelNodes[parentLevel][parentIndex].Child = nodes[0];
        }
    }

    private static void Link(params Node[] nodes)
    {
        for (var i = 0; i < nodes.Length - 1; i++)
        {
            nodes[i].Next = nodes[i + 1];
            nodes[i + 1].Previous = nodes[i];
        }
    }
}
