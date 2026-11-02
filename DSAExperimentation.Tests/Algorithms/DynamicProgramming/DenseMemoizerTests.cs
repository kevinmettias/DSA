using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Tests.Algorithms.DynamicProgramming.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.DynamicProgramming;

// The recurrences are MemoizerTests' own fixtures, so each claim here is the one Memoizer is
// held to, made again over the array cache.
public sealed partial class DenseMemoizerTests
{
    private const int FibonacciArgument = 30;

    [Fact]
    public void Memoize_Fibonacci_ReturnsCorrectValue()
    {
        var result = DenseMemoizer.Memoize(10, 11, new WaysFromPreviousTwoSteps());

        Assert.Equal(55, result);
    }

    // Naive fib(30) makes about 2.7 million calls; memoized, it makes one per state 0..30.
    [Fact]
    public void Memoize_Fibonacci_CallsRecurrenceOncePerDistinctState()
    {
        var recurrence = new CountedRecurrence<int, long>(new WaysFromPreviousTwoSteps());

        DenseMemoizer.Memoize(FibonacciArgument, FibonacciArgument + 1, recurrence);

        Assert.Equal(FibonacciArgument + 1, recurrence.Calls);
    }

    // A state whose result is the type's default is still a computed state, not an empty slot:
    // state 1 asks for state 0 twice, and only the first request reaches the recurrence.
    [Fact]
    public void Memoize_StateWhoseResultIsZero_IsNotAskedForAgain()
    {
        var recurrence = new CountedRecurrence<int, long>(new ZeroReachedTwice());

        var result = DenseMemoizer.Memoize(1, 2, recurrence);

        Assert.Equal(1, result);
        Assert.Equal(2, recurrence.Calls);
    }

    [Fact]
    public void Memoize_StateBeyondTheRange_ThrowsIndexOutOfRangeException() =>
        Assert.Throws<IndexOutOfRangeException>(() => DenseMemoizer.Memoize(10, 10, new WaysFromPreviousTwoSteps()));

    // DenseMemoRun, the recursion DenseMemoizer hands a recurrence, is private, so its Replay is
    // driven through one Memoize call whose recurrence branches twice onto the same sub-state:
    // the second request is answered with the very result the run stored.
    public sealed partial class DenseMemoRunTests
    {
        [Fact]
        public void Replay_ASecondRequestForTheSameState_IsAnsweredWithTheResultItStored()
        {
            var recurrence = new TwoBranchesOntoOneState();

            var result = DenseMemoizer.Memoize(1, 2, recurrence);

            Assert.Same(recurrence.FirstObservation, recurrence.SecondObservation);
            Assert.Same(recurrence.FirstObservation, result);
            Assert.Equal(1, recurrence.Computations);
        }
    }
}
