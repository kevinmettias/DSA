using StackOfInt = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.FlippingAnImage;

// LeetCode 832. Flipping an Image: reverse each row of a binary matrix, then
// invert every value. Both strategies rewrite the rows they are handed and return
// the same image LeetCode's signature returns, so a caller that needs its input
// preserved clones first - the same in-place shape RotateImageSolution uses for
// LC 48.
//
// The two strategies differ only in how the row gets reversed: the textbook
// two-pointer swap, or this repo's own LIFO Stack<int>, whose pop order undoes
// the row so flip-and-invert happen in a single pass.
internal static class FlippingAnImageSolution
{
    // The textbook answer, BCL only: walk each row from both ends, swapping the
    // two values and inverting them as they cross over.
    public static int[][] FlipAndInvertImageByTwoPointerReverse(int[][] image)
    {
        foreach (var row in image)
        {
            var left = 0;
            var right = row.Length - 1;

            while (left <= right)
            {
                (row[left], row[right]) = (1 - row[right], 1 - row[left]);
                left++;
                right--;
            }
        }

        return image;
    }

    // This repo's own Stack<int>: pushing a row and popping it back yields the
    // values in reverse, so inverting each value as it comes off the stack
    // performs the flip and the inversion in the single pop pass - the same
    // digit-reversal primitive ReverseIntegerSolution/RotateImageSolution compose,
    // applied to a row's bits.
    public static int[][] FlipAndInvertImageByStackReverse(int[][] image)
    {
        foreach (var row in image)
        {
            var pending = new StackOfInt();

            foreach (var value in row)
            {
                pending.Push(value);
            }

            var index = 0;

            while (pending.TryPop(out var value))
            {
                row[index++] = 1 - value;
            }
        }

        return image;
    }
}
