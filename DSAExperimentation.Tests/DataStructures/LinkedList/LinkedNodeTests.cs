using DSAExperimentation.Tests.DataStructures.LinkedList.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.LinkedList;

// The node template's contract, run over its three configurations: a value and a forward link that
// read back what was written, and a node no bigger than a hand-written one with its value and its
// links. That the singly linked node keeps no back link is read off what it allocates: exactly
// what a class with an int and one reference allocates, a reference less than the doubly linked
// node.
public sealed partial class LinkedNodeTests
{
    private const int ChainLength = 6;
    private const int SampleValue = 7;
    private const int RewrittenValue = 11;

    public static TheoryData<string> Configurations =>
    [
        LinkedNodeConfigurations.Singly,
        LinkedNodeConfigurations.Doubly,
        LinkedNodeConfigurations.Random,
    ];

    public static TheoryData<string, int> LinksPerConfiguration => new()
    {
        { LinkedNodeConfigurations.Singly, 1 },
        { LinkedNodeConfigurations.Doubly, 2 },
        { LinkedNodeConfigurations.Random, 2 },
    };

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Value_ConstructedWithValue_ReturnsThatValue(string configuration)
    {
        var view = LinkedNodeConfigurations.Of(configuration);

        Assert.Equal(SampleValue, view.ValueOf(view.Node(SampleValue)));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Value_SetAfterConstruction_ReturnsTheNewValue(string configuration)
    {
        var view = LinkedNodeConfigurations.Of(configuration);
        var node = view.Node(SampleValue);

        view.SetValue(node, RewrittenValue);

        Assert.Equal(RewrittenValue, view.ValueOf(node));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Next_NewNode_IsNull(string configuration)
    {
        var view = LinkedNodeConfigurations.Of(configuration);

        Assert.Null(view.NextOf(view.Node(SampleValue)));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Next_SetToItself_ReturnsItself(string configuration)
    {
        var view = LinkedNodeConfigurations.Of(configuration);
        var node = view.Node(SampleValue);

        view.Link(node, node);

        Assert.Same(node, view.NextOf(node));
    }

    [Theory]
    [MemberData(nameof(Configurations))]
    public void Next_FollowedFromTheHead_VisitsEveryValueInOrderThenEnds(string configuration)
    {
        var view = LinkedNodeConfigurations.Of(configuration);
        var visited = new List<int>();
        var head = view.Chain(ChainLength, cycleEntry: null)[0];

        for (var node = head; node is not null; node = view.NextOf(node))
        {
            visited.Add(view.ValueOf(node));
        }

        Assert.Equal(Enumerable.Range(0, ChainLength), visited);
    }

    [Theory]
    [MemberData(nameof(LinksPerConfiguration))]
    public void Allocation_EachConfiguration_IsAnIntAndItsLinksOnly(
        string configuration, int links) =>
        Assert.Equal(LinkShapes.BytesOfShapeWith(links), BytesPerNodeOf(configuration));

    [Fact]
    public void Allocation_SinglyLinkedNode_IsOneReferenceSmallerThanTheDoublyLinkedNode() =>
        Assert.Equal(
            BytesPerNodeOf(LinkedNodeConfigurations.Doubly) - IntPtr.Size,
            BytesPerNodeOf(LinkedNodeConfigurations.Singly));

    private static long BytesPerNodeOf(string configuration) =>
        LinkedNodeConfigurations.Of(configuration).BytesPerNode();
}
