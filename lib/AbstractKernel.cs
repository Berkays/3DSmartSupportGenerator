public abstract class AbstractKernel<T, K> : StaticAbstractKernel<T, K>
{
    public T[] Kernel { get; init; }
    public Span<T> Span => this.Kernel.AsSpan();
    public AbstractKernel(T[] kernel, int size) : base(size)
    {
        if (kernel.Length != this.length)
            throw new ArgumentException("Kernel size does not match");

        this.Kernel = new T[this.length];

        kernel.CopyTo<T>(this.Span);
    }

    public AbstractKernel(T[,] kernel, int size) : base(size)
    {
        if (kernel.GetLength(0) != this.size || kernel.GetLength(1) != this.size)
            throw new ArgumentException("Kernel size does not match");

        this.Kernel = new T[this.length];

        kernel.Cast<T[]>().SelectMany(n => n).ToArray().CopyTo<T>(this.Span);
    }
    public T this[int i]
    {
        get
        {
            return this.Kernel[i];
        }
    }

    public T this[int i, int k]
    {
        get
        {
            // 0,0 0,1 0,2
            // 1,0 1,1 1,2
            // 2,0 2,1 2,2
            return this.Kernel[(i * this.size) + k];
        }
    }
}