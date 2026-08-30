using BenchmarkDotNet.Attributes;
using StackOfInt = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Flipping an Image (LC 832): in-place two-pointer reverse+invert (the textbook
// approach) vs. this repo's own LIFO Stack<T> reversing each row while inverting
// each value as it comes back off the stack - the same digit-reversal primitive
// ReverseIntegerBenchmarks/RotateImageBenchmarks already compose, applied to a
// row's bits instead of decimal digits or a transposed matrix's columns.
[MemoryDiagnoser]
public class FlippingAnImageBenchmarks
{
    [Params(50, 300)]
    public int Side;

    private int[][] _image = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(4);
        _image = new int[Side][];

        for (var r = 0; r < Side; r++)
        {
            _image[r] = new int[Side];

            for (var c = 0; c < Side; c++)
            {
                _image[r][c] = random.Next(0, 2);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[][] TwoPointerReverseAndInvert()
    {
        var image = Clone(_image);

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

    [Benchmark]
    public int[][] StackReverseAndInvert()
    {
        var image = Clone(_image);

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

    private static int[][] Clone(int[][] image)
    {
        var copy = new int[image.Length][];

        for (var r = 0; r < image.Length; r++)
        {
            copy[r] = (int[])image[r].Clone();
        }

        return copy;
    }
}
