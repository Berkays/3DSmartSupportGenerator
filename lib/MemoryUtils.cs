public static class MemoryUtils
{
    public static byte[] GetRow(this byte[,] array, int row)
    {
        if (array == null)
            throw new ArgumentNullException("array");

        int cols = array.GetUpperBound(1) + 1;
        byte[] result = new byte[cols];

        Buffer.BlockCopy(array, row * cols, result, 0, cols);

        return result;
    }
    public static void GetRowSpan<T>(this T[,] array, int row, int bound, Span<T> buffer) where T : unmanaged
    {
        unsafe
        {
            fixed (T* srcPtr = array, destPtr = buffer)
            {
                Buffer.MemoryCopy(srcPtr + (row * bound), destPtr, bound, bound);
            }
        }
    }

    public static void GetCol(this byte[,] array, int column, int bound, Span<byte> buffer)
    {
        for (int i = 0; i < bound; i++)
            buffer[i] = array[i, column];
    }
}