namespace DSAExperimentation.Tests.LeetCodeCoverage.FlattenAMultilevelDoublyLinkedList.Fixtures;

// Mirrors LeetCode's own multilevel doubly linked list Node: Previous/Next walk the
// current level, Child optionally begins an independent, deeper-nested sublist.
internal sealed class Node(int val)
{
    public int Val { get; } = val;

    public Node? Previous { get; set; }

    public Node? Next { get; set; }

    public Node? Child { get; set; }
}
