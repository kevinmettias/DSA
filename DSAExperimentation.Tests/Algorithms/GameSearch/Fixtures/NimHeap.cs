using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

// One heap of Nim: a turn takes any positive number of stones. Its Grundy number is its size, the fact
// Sprague-Grundy builds every impartial game's analysis on.
internal readonly struct NimHeap : IImpartialGame<int>
{
    public int MoveCount(int state) => state;

    // Move i takes i + 1 stones.
    public int Next(int state, int move) => state - (move + 1);
}
