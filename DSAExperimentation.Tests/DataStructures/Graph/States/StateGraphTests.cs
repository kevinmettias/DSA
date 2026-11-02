using DSAExperimentation.DataStructures.Graph.States;

namespace DSAExperimentation.Tests.DataStructures.Graph.States;

// The state graph over a six-state ring, where a move steps one place either way, with and without
// a filter that leaves state 3 out.
public sealed partial class StateGraphTests
{
    private const int RingSize = 6;
    private const int LeftOut = 3;

    [Fact]
    public void Count_IsEveryStateTheFilterKeeps()
        => Assert.Equal(RingSize - 1, StateGraph<int>.Build(new Ring(), new AllBut(LeftOut)).Count);

    [Fact]
    public void Count_WithEveryState_IsTheWholeSpace()
        => Assert.Equal(RingSize, StateGraph<int>.Build(new Ring(), new EveryState<int>()).Count);

    [Fact]
    public void TryGetNode_StateTheFilterLeftOut_FindsNothing()
        => Assert.False(StateGraph<int>.Build(new Ring(), new AllBut(LeftOut)).TryGetNode(LeftOut, out _));

    [Fact]
    public void TryGetNode_StateOutsideTheSpace_FindsNothing()
        => Assert.False(StateGraph<int>.Build(new Ring(), new EveryState<int>()).TryGetNode(RingSize, out _));

    // State 0's moves are 1 and 5, in the order the ring writes them.
    [Fact]
    public void Build_JoinsAStateToItsSuccessorsInTheSpacesOrder()
    {
        var graph = StateGraph<int>.Build(new Ring(), new EveryState<int>());

        Assert.True(graph.TryGetNode(0, out var zero));
        Assert.Equal([1, 5], zero.Successors.Select(successor => successor.State));
    }

    // State 2's moves are 3, which the filter left out, and 1.
    [Fact]
    public void Build_LeavesOutASuccessorTheFilterLeftOut()
    {
        var graph = StateGraph<int>.Build(new Ring(), new AllBut(LeftOut));

        Assert.True(graph.TryGetNode(2, out var two));
        Assert.Equal([1], two.Successors.Select(successor => successor.State));
    }

    // Successors are nodes of the same graph, so a walk reaches the state it left from.
    [Fact]
    public void Build_JoinsNodesToEachOtherNotToCopies()
    {
        var graph = StateGraph<int>.Build(new Ring(), new EveryState<int>());

        Assert.True(graph.TryGetNode(0, out var zero));
        Assert.Same(zero, zero.Successors[0].Successors.Single(successor => successor.State == 0));
    }

    // States 0 to RingSize - 1 around a ring, each one move from its two neighbours.
    private readonly struct Ring : IStateSpace<int>
    {
        public int StateCount => RingSize;

        public int MaxSuccessors => 2;

        public int StateAt(int index) => index;

        public int WriteSuccessors(int state, Span<int> successors)
        {
            successors[0] = (state + 1) % RingSize;
            successors[1] = (state + RingSize - 1) % RingSize;

            return 2;
        }
    }

    private readonly struct AllBut(int excluded) : IStateFilter<int>
    {
        public bool IsKept(int state) => state != excluded;
    }
}
