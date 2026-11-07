using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The hashed store over a node class of its own, the suffix tree's: a child is found by the
// letter it was added or put under, by reference, and putting a node under a letter replaces what
// was there.
public sealed partial class HashedNodeChildrenTests
{
    [Fact]
    public void Empty_HasNoChildren()
    {
        var children = HashedNodeChildren<SuffixTreeNode>.Empty();

        Assert.Empty(children.Letters);
        Assert.Empty(children.Nodes);
        Assert.Null(children.ChildFor('a'));
    }

    [Fact]
    public void GetOrAddChildFor_NewLetter_AddsAChildThatChildForFinds()
    {
        var node = new SuffixTreeNode();
        var added = node.Children.GetOrAddChildFor('a');

        Assert.Same(added, node.Children.ChildFor('a'));
    }

    [Fact]
    public void GetOrAddChildFor_LetterWithAChild_ReturnsTheSameChild()
    {
        var node = new SuffixTreeNode();
        var first = node.Children.GetOrAddChildFor('a');

        Assert.Same(first, node.Children.GetOrAddChildFor('a'));
    }

    [Fact]
    public void ChildFor_LetterWithNoChild_ReturnsNull()
    {
        var node = new SuffixTreeNode();
        node.Children.GetOrAddChildFor('a');

        Assert.Null(node.Children.ChildFor('A'));
    }

    [Fact]
    public void SetChildFor_LetterWithAChild_PutsTheGivenNodeInItsPlace()
    {
        var node = new SuffixTreeNode();
        node.Children.GetOrAddChildFor('a');
        var given = new SuffixTreeNode();

        node.Children.SetChildFor('a', given);

        Assert.Equal((given, 1), (node.Children.ChildFor('a'), node.Children.Nodes.Count()));
    }

    [Fact]
    public void LettersAndNodes_ListEveryChild()
    {
        var node = new SuffixTreeNode();
        var b = node.Children.GetOrAddChildFor('b');
        var a = node.Children.GetOrAddChildFor('a');

        Assert.Equal(new HashSet<char> { 'a', 'b' }, node.Children.Letters.ToHashSet());
        Assert.Equal(new HashSet<SuffixTreeNode> { a, b }, node.Children.Nodes.ToHashSet());
    }
}
