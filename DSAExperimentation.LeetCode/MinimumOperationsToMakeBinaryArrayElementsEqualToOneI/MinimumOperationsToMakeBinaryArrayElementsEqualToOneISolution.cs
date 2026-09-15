using FlipStartQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.MinimumOperationsToMakeBinaryArrayElementsEqualToOneI;

// LeetCode 3191. Minimum Operations to Make Binary Array Elements Equal to One
// I: an operation flips 3 consecutive elements; return the fewest operations
// that make every element 1, or -1 if some element can never be reached by one.
//
// A zero can only ever be fixed by an operation that STARTS at or before it and
// covers it, and fixing it with the leftmost such start (its own index) never
// hurts a later position - so the greedy left-to-right walk is optimal, and both
// strategies below implement exactly that walk. They differ only in how they
// track "how many pending flips still cover the current index."
internal static class MinimumOperationsToMakeBinaryArrayElementsEqualToOneISolution
{
    private const int FlipLength = 3;

    // The textbook version: copy nums and actually flip the 3 elements in place
    // whenever the current one reads 0. Deliberately mutates a plain BCL array -
    // this is the arm the queue-based strategy below has to justify itself
    // against.
    public static int MinOperationsByArrayMutation(int[] nums)
    {
        var flipped = (int[])nums.Clone();
        var operations = 0;

        for (var i = 0; i < flipped.Length; i++)
        {
            if (flipped[i] == 1)
            {
                continue;
            }

            if (i + FlipLength > flipped.Length)
            {
                return LeetCodeAnswer.None;
            }

            flipped[i] ^= 1;
            flipped[i + 1] ^= 1;
            flipped[i + 2] ^= 1;
            operations++;
        }

        return operations;
    }

    // Never materializes a flipped copy: this repo's own Queue<int> holds the
    // start index of every operation still covering the current position (at
    // most 2 of them, since a flip only reaches 2 indices ahead), so the
    // element's EFFECTIVE value is just its original value XORed with the
    // parity of that queue's length. Once an index falls more than FlipLength-1
    // behind, its operation can no longer cover anything and is dequeued.
    public static int MinOperationsByFlipParityWindow(int[] nums)
    {
        var activeFlips = new FlipStartQueue();
        var operations = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            var (canFinish, updatedOperations) = ApplyFlipAt(activeFlips, nums, i, operations);
            operations = updatedOperations;

            if (!canFinish)
            {
                return LeetCodeAnswer.None;
            }
        }

        return operations;
    }

    // Retires the operations that can no longer cover `index`, then - if the
    // element's effective value is 0 - spends the one operation that has to
    // start here. Returns false when that operation would run off the end.
    private static (bool CanFinish, int Operations) ApplyFlipAt(
        FlipStartQueue activeFlips, int[] nums, int index, int operations)
    {
        while (activeFlips.TryPeek(out var start) && start <= index - FlipLength)
        {
            activeFlips.TryDequeue(out _);
        }

        var effectiveValue = nums[index] ^ (activeFlips.Count % 2);

        if (effectiveValue == 1)
        {
            return (true, operations);
        }

        if (index + FlipLength > nums.Length)
        {
            return (false, operations);
        }

        activeFlips.Enqueue(index);

        return (true, operations + 1);
    }
}
